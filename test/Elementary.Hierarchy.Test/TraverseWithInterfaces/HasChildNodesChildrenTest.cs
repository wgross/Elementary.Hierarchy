using System.Linq;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HasChildNodesChildrenTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>
        { }

        private readonly Mock<MockableNodeType> rootNode;
        private readonly Mock<MockableNodeType> leftNode;
        private readonly Mock<MockableNodeType> rightNode;

        public HasChildNodesChildrenTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode // has single child
                .Setup(n => n.HasChildNodes).Returns(false);

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode // has two children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode // has a tw children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rootNode // returns the left node and right node as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
        }

        [Fact]
        public void IHasChildNodes_root_returns_children_on_Children()
        {
            // ACT

            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            this.rootNode.Verify(r => r.HasChildNodes, Times.Once);
            this.rootNode.Verify(r => r.ChildNodes, Times.Once());

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object }, children);
        }

        [Fact]
        public void IHasChildNodes_leaf_doesnt_retrieve_childnodes_if_it_hasnt_some()
        {
            // ACT

            var children = this.leftNode.Object.Children().ToArray();

            // ASSERT

            this.leftNode.Verify(r => r.HasChildNodes, Times.Once());
            this.leftNode.Verify(r => r.ChildNodes, Times.Never());

            Assert.False(children.Any());
        }
    }
}