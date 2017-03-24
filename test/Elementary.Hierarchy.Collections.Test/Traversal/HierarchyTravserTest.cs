using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Traversal
{
    public class HierarchyTravserTest
    {
        public interface NodeType :
            IHasChildNodes<NodeType>,
            IHierarchyValueReader<int>,
            IHasIdentifiableChildNodes<string, NodeType>
        { }

        [Fact]
        public void HierarchyTraverser_provides_IHasChildNodes_and_IHasParentNode()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ASSERT

            Assert.False(traverser.HasChildNodes);
            Assert.False(traverser.ChildNodes.Any());
            Assert.False(traverser.HasParentNode);
            Assert.Throws<InvalidOperationException>(() => traverser.ParentNode);
        }

        [Fact]
        public void HierarchyTraverser_provides_child_nodes()
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

            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ACT

            var result = traverser.ChildNodes.ToArray();

            // ASSERT

            Assert.Equal(1, result.Length);
            Assert.True(traverser.HasChildNodes);
        }

        [Fact]
        public void HierarchyTraverser_provides_parent_for_child_traverser()
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

            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ACT

            var result = traverser.ChildNodes.ToArray();

            // ASSERT

            Assert.True(result.ElementAt(0).HasParentNode);
            Assert.Equal(traverser, result.ElementAt(0).ParentNode);
        }

        [Fact]
        public void HierarchyTraverser_are_equal_if_same()
        {
            // ARRANGE

            var traverser = new HierarchyTraverser<string, int, NodeType>(new Mock<NodeType>().Object);

            // ACT

            var result = traverser.Equals(traverser);

            // ASSERT

            Assert.True(result);
            Assert.Equal(traverser.GetHashCode(), traverser.GetHashCode());
        }

        [Fact]
        public void HierarchyTraverser_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new Mock<NodeType>().Object;
            var a = new HierarchyTraverser<string, int, NodeType>(node);
            var b = new HierarchyTraverser<string, int, NodeType>(node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.True(result);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
    }
}