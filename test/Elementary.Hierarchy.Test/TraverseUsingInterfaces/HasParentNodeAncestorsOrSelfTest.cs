namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasParentAncestorsAndSelfTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode;

        public HasParentAncestorsAndSelfTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void Root_returns_self_for_AncestorsOrSelf()
        {
            // ARRANGE

            startNode.SetupGet(n => n.HasParentNode).Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.Object.AncestorsOrSelf().ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same(startNode.Object, result.ElementAt(0));

            this.startNode.Verify(n => n.HasParentNode, Times.Once());
            this.startNode.Verify(n => n.ParentNode, Times.Never());
        }

        [Fact]
        public void Inner_node_returns_path_to_parent_and_self_for_AncestorsOrSelf()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();
            rootNode.SetupGet(n => n.HasParentNode).Returns(false);

            var parentOfStartNode = new Mock<MockableNodeType>();
            parentOfStartNode.SetupGet(n => n.HasParentNode).Returns(true);
            parentOfStartNode.SetupGet(n => n.ParentNode).Returns(rootNode.Object);

            this.startNode.SetupGet(n => n.HasParentNode).Returns(true);
            this.startNode.SetupGet(n => n.ParentNode).Returns(parentOfStartNode.Object);

            // ACT

            IEnumerable<MockableNodeType> result = this.startNode.Object.AncestorsOrSelf().ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Same(this.startNode.Object, result.ElementAt(0));
            Assert.Same(parentOfStartNode.Object, result.ElementAt(1));
            Assert.Same(rootNode.Object, result.ElementAt(2));
        }
    }
}