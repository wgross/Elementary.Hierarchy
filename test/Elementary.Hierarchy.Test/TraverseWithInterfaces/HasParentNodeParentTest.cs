namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System;
    using Xunit;

    public class HasParentNodeParentTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasParentNodeParentTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void IHasParentNode_root_throws_InvalidOperationException_on_Parent()
        {
            // ARRANGE
            // a parent isn't available

            startNode.Setup(m => m.HasParentNode).Returns(false);

            // ACT
            // ask for parent

            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => startNode.Object.Parent());

            // ASSERT

            startNode.Verify(m => m.HasParentNode, Times.Once());
            startNode.Verify(m => m.ParentNode, Times.Never());

            Assert.Contains("has no parent", result.Message);
        }

        [Fact]
        public void IHasParentNode_inner_node_returns_parent_on_Parent()
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

            startNode.Verify(m => m.HasParentNode, Times.Once());
            startNode.Verify(m => m.ParentNode, Times.Once());

            Assert.Same(parentOfStartNode.Object, result);
        }
    }
}