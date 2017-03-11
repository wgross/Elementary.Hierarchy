namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;

    using System;
    using Xunit;

    public class HasParentNodeParentTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = null;

        public HasParentNodeParentTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void Root_node_throws_InvalidOperationException_on_Parent()
        {
            // ARRANGE

            this.startNode.SetupGet(s => s.HasParentNode).Returns(false);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => this.startNode.Object.Parent());

            // ASSERT

            Assert.True(result.Message.Contains("startNode has no parent"));

            this.startNode.Verify(s => s.HasParentNode, Times.Once());
            this.startNode.Verify(s => s.ParentNode, Times.Never());
        }

        [Fact]
        public void Inner_node_returns_parent_for_Parent()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();
            rootNode.Setup(r => r.HasParentNode).Returns(false);

            var rootNodeObject = rootNode.Object;

            var parentOfStartNode = new Mock<MockableNodeType>();
            parentOfStartNode.SetupGet(p => p.HasParentNode).Returns(true);
            parentOfStartNode.SetupGet(p => p.ParentNode).Returns(rootNodeObject);

            var parentOfStartNodeObject = parentOfStartNode.Object;

            this.startNode.SetupGet(s => s.HasParentNode).Returns(true);
            this.startNode.SetupGet(s => s.ParentNode).Returns(parentOfStartNodeObject);

            // ACT

            MockableNodeType result = this.startNode.Object.Parent();

            // ASSERT

            Assert.Same(parentOfStartNode.Object, result);
        }
    }
}