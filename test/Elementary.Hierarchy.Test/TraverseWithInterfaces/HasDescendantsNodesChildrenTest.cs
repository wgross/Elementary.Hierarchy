using System.Linq;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HasDescendantsNodesChildrenTest
    {
        private readonly Mock<MockableNodeType> rootNode;

        private readonly Mock<MockableNodeType> leftNode;

        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        { }

        public HasDescendantsNodesChildrenTest()
        {

            this.leftNode = new Mock<MockableNodeType>();
            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode
                .Setup(n => n.GetDescendants(false, 1))
                .Returns(new[] { this.leftNode.Object });
        }

        [Fact]
        public void IHasDescendantNodes_leaf_returns_no_children_on_Children()
        {
            // ACT

            var result = this.leftNode.Object.Children().ToArray();

            // ASSERT

            this.leftNode.Verify(n => n.GetDescendants(false, 1), Times.Once());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            Assert.False(result.Any());
        }

        [Fact]
        public void IHasDescendantNodes_root_returns_children_on_Children()
        {
            // ACT

            var result = this.rootNode.Object.Children().ToArray();

            // ASSERT

            this.rootNode.Verify(n => n.GetDescendants(false, 1), Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            Assert.Equal(new[] { this.leftNode.Object }, result);
        }
    }
}