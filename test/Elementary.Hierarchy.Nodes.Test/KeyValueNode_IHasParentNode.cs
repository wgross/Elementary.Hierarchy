using Moq;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNode_IHasParentNode
    {
        public interface MockableNode : IHasChildNodes<MockableNode>
        {
        }

        [Fact]
        public void HasParentDecorator_root_has_no_parent()
        {
            // ACT

            var rootTraverser = new HasParentDecorator<MockableNode>(Mock.Of<MockableNode>());

            // ASSERT

            Assert.False(rootTraverser.HasParentNode);
        }

        [Fact]
        public void HasParentDecorator_root_returns_childnodes()
        {
            // ARRANGE
            // root has a child node

            var node = Mock.Of<MockableNode>();
            var child = Mock.Of<MockableNode>();

            Mock.Get(node).Setup(n => n.HasChildNodes).Returns(true);
            Mock.Get(node).Setup(n => n.ChildNodes).Returns(new[] { child });

            var traverser = new HasParentDecorator<MockableNode>(node);

            // ACT

            var result = traverser.Children().Single().Node;

            // ASSERT

            Assert.True(traverser.HasChildNodes);
            Assert.Same(child, result);
        }

        [Fact]
        public void HasParentDecorator_has_parent_for_childnode()
        {
            // ARRANGE
            // root has a child node

            var node = Mock.Of<MockableNode>();
            var child = Mock.Of<MockableNode>();

            Mock.Get(node).Setup(n => n.HasChildNodes).Returns(true);
            Mock.Get(node).Setup(n => n.ChildNodes).Returns(new[] { child });

            var traverser = new HasParentDecorator<MockableNode>(node);

            // ACT
            // fetch parent fro single child

            var result = traverser.Children().Single().Parent().Node;

            // ASSERT
            // parent is accessible from the decorated child

            Assert.True(traverser.Children().Single().HasParentNode);
            Assert.Same(node, result);
        }
    }
}