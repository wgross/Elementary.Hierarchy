using Elementary.Hierarchy.Collections.Nodes;
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
            IHierarchyKeyReader<string>,
            IHasIdentifiableChildNodes<string, NodeType>
        { }

        #region IHasChildNodes and IHasParentNode interfaces are implemented

        [Fact]
        public void HierarchyTraverser_provides_IHasChildNodes_and_IHasParentNode()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => traverser.ParentNode);

            // ASSERT

            Assert.False(traverser.HasChildNodes);
            Assert.False(traverser.ChildNodes.Any());
            Assert.False(traverser.HasParentNode);
            Assert.Equal("node has no parent", result.Message);
        }

        [Fact]
        public void HierarchyTraverser_provides_child_nodes()
        {
            // ARRANGE

            string key = "a";
            var childNode = new Mock<NodeType>();
            childNode
                .Setup(n => n.TryGetKey(out key))
                .Returns(true);
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

            startNode.VerifyAll();
            childNode.VerifyAll();
        }

        [Fact]
        public void HierarchyTraverser_fails_child_node_without_key()
        {
            // ARRANGE

            string key = "a";
            var childNode = new Mock<NodeType>();
            childNode
                .Setup(n => n.TryGetKey(out key))
                .Returns(false);
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            startNode
                .Setup(n => n.ChildNodes)
                .Returns(new[] { childNode.Object });

            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ACT

            var result = Assert.Throws<ArgumentException>(() => traverser.ChildNodes.ToArray());

            // ASSERT

            Assert.Equal("node", result.ParamName);
            Assert.True(result.Message.Contains("child node must have a key"));
            Assert.True(traverser.HasChildNodes);

            startNode.VerifyAll();
            childNode.VerifyAll();
        }

        [Fact]
        public void HierarchyTraverser_provides_parent_for_child_traverser()
        {
            // ARRANGE

            string key = "a";
            var childNode = new Mock<NodeType>();
            childNode
                .Setup(n => n.TryGetKey(out key))
                .Returns(true);

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
        public void HierarchyTraverser_for_start_node_returns_root_path()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            var traverser = new HierarchyTraverser<string, int, NodeType>(startNode.Object);

            // ACT

            var result = traverser.Path;

            // ASSERT

            Assert.Equal(HierarchyPath.Create<string>(), result);
        }

        [Fact]
        public void HierarchyTraverser_for_inner_node_returns_inner_nodes_key()
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

            var result = traverser.Path;

            // ASSERT

            Assert.Equal(HierarchyPath.Create<string>(), result);
        }

        #endregion IHasChildNodes and IHasParentNode interfaces are implemented

        #region Equals and GetHashCode delegate behavior to inner node

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

        #endregion Equals and GetHashCode delegate behavior to inner node

        [Fact]
        public void HierarchyTravrser_HasValue_returns_false_if_value_missing()
        {
            // ARRANGE

            int value = 1;
            var node = new Mock<NodeType>();
            node
                .Setup(n => n.TryGetValue(out value))
                .Returns(false);

            var traverser = new HierarchyTraverser<string, int, NodeType>(node.Object);

            // ACT

            var result = traverser.TryGetValue(out value);

            // ASSERT

            Assert.False(result);

            node.Verify(n => n.TryGetValue(out value), Times.Exactly(1));
            node.VerifyAll();
        }

        [Fact]
        public void HierarchyTravrser_Value_returns_value()
        {
            // ARRANGE

            int value = 1;
            var node = new Mock<NodeType>();
            node
                .Setup(n => n.TryGetValue(out value))
                .Returns(true);

            var traverser = new HierarchyTraverser<string, int, NodeType>(node.Object);

            // ACT

            var result = traverser.TryGetValue(out value);

            // ASSERT

            Assert.True(result);
            Assert.Equal(1, value);

            node.Verify(n => n.TryGetValue(out value), Times.Exactly(1));
            node.VerifyAll();
        }
    }
}