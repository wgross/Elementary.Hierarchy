using Moq;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Decorators.Test
{
    public class ParentNodeDecoratorTest
    {
        public interface NodeType : IHasChildNodes<NodeType>
        { }

        [Fact]
        public void ParentNodeDecorator_provides_IHasChildNodes_and_IHasParentNode()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            var decorator = new ParentNodeDecorator<NodeType>(startNode.Object);

            // ASSERT

            Assert.False(decorator.HasChildNodes);
            Assert.False(decorator.ChildNodes.Any());
            Assert.False(decorator.HasParentNode);
            Assert.Throws<InvalidOperationException>(() => decorator.ParentNode);
        }

        [Fact]
        public void ParentNodeDecorator_provides_child_nodes()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>();
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            startNode
                .Setup(n => n.ChildNodes)
                .Returns(new[] { childNode.Object });

            var decorator = new ParentNodeDecorator<NodeType>(startNode.Object);

            // ACT

            var result = decorator.ChildNodes.ToArray();

            // ASSERT

            Assert.Equal(1, result.Length);
            Assert.Same(childNode.Object, result.ElementAt(0).DecoratedNode);
            Assert.True(decorator.HasChildNodes);
        }

        [Fact]
        public void ParentNodeDecorator_provides_parent_for_child_traveser()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>();
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            startNode
                .Setup(n => n.ChildNodes)
                .Returns(new[] { childNode.Object });

            var decorator = new ParentNodeDecorator<NodeType>(startNode.Object);

            // ACT

            var result = decorator.ChildNodes.ToArray();

            // ASSERT

            Assert.True(result.ElementAt(0).HasParentNode);
            Assert.Same(decorator, result.ElementAt(0).ParentNode);
        }

        [Fact]
        public void ParentNodeDecorator_are_equal_if_same()
        {
            // ARRANGE

            var decorator = new ParentNodeDecorator<NodeType>(new Mock<NodeType>().Object);

            // ACT

            var result = decorator.Equals(decorator);

            // ASSERT

            Assert.True(result);
            Assert.Equal(decorator.GetHashCode(), decorator.GetHashCode());
        }

        [Fact]
        public void ParentNodeDecorators_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new Mock<NodeType>().Object;
            var a = new ParentNodeDecorator<NodeType>(node);
            var b = new ParentNodeDecorator<NodeType>(node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.True(result);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.Equal(node.GetHashCode(), a.GetHashCode());
            Assert.Equal(node.GetHashCode(), b.GetHashCode());
        }
    }
}