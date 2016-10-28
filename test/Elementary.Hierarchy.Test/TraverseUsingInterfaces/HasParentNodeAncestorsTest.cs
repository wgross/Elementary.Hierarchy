namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces.TraverseUsingInterfaces
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

        private Mock<MockableNodeType> startNode = null;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Test]
        public void Root_returns_null_for_Ancestors()
        {
            // ARRANGE

            this.startNode.SetupGet(n =>n.HasParentNode).Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = this.startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());

            this.startNode.Verify(n => n.HasParentNode,Times.Once());
            this.startNode.Verify(n => n.ParentNode, Times.Never());
        }

        [Test]
        public void Inner_node_returns_path_to_root_node()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();
            rootNode.SetupGet(n =>n.HasParentNode).Returns(false);

            var parentOfStartNode = new Mock<MockableNodeType>();
            parentOfStartNode.SetupGet(n => n.HasParentNode).Returns(true);
            parentOfStartNode.SetupGet(n => n.ParentNode).Returns(rootNode.Object);

            this.startNode.SetupGet(n => n.HasParentNode).Returns(true);
            this.startNode.SetupGet(n => n.ParentNode).Returns(parentOfStartNode.Object);

            // ACT

            IEnumerable<MockableNodeType> result = this.startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreSame(parentOfStartNode.Object, result.ElementAt(0));
            Assert.AreSame(rootNode.Object, result.ElementAt(1));
        }
    }
}