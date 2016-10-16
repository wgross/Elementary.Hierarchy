namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class HasParentNodeParentTest
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
        public void I_Root_throws_InvalidOperationException_on_Parent()
        {
            // ARRANGE

            startNode // returns false for 'HasParentNode'
                .Setup(m => m.HasParentNode).Returns(false);

            // ACT

            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => startNode.Object.Parent());

            // ASSERT

            Assert.IsTrue(result.Message.Contains("has no parent"));

            startNode.Verify(m => m.HasParentNode, Times.Once());
            startNode.Verify(m => m.ParentNode,Times.Never());
        }

        [Test]
        public void I_inner_node_returns_parent_on_Parent()
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

            MockableNodeType result = this.startNode.Object.Parent();

            // ASSERT

            Assert.AreSame(parentOfStartNode.Object, result);
        }
    }
}