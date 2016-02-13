namespace Elementary.Hierarchy.Test
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasParentNodeAncestorsTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Test]
        public void RootNodeReturnsNoAncestors_IF()
        {
            // ARRANGE

            startNode // returns false for 'HasParentNode'
                .Setup(m => m.HasParentNode).Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());

            startNode.Verify(m => m.HasParentNode, Times.Once);
            startNode.Verify(m => m.ParentNode, Times.Never);
        }

        [Test]
        public void InnerNodeReturnsPathToRootNode_IF()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();

            rootNode // has no parent
                .Setup(r => r.HasParentNode).Returns(false);

            var parentOfStartNode = new Mock<MockableNodeType>();

            parentOfStartNode // has root node as parent
                .Setup(p => p.HasParentNode).Returns(true);
            parentOfStartNode
                .Setup(p => p.ParentNode).Returns(rootNode.Object);

            this.startNode // has a parant
                .Setup(m => m.HasParentNode).Returns(true);
            this.startNode // returns parent node
                .Setup(m => m.ParentNode).Returns(parentOfStartNode.Object);

            // ACT

            IEnumerable<MockableNodeType> result = this.startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreSame(parentOfStartNode.Object, result.ElementAt(0));
            Assert.AreSame(rootNode.Object, result.ElementAt(1));
        }
    }
}