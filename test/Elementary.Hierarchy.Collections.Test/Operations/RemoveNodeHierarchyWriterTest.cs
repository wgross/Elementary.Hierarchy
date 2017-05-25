using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class RemoveNodeHierarchyWriterTest
    {
        public interface NodeType : IHierarchyNodeWriter<NodeType>, IHasIdentifiableChildNodes<string, NodeType>, IHasChildNodes<NodeType>
        {
        }

        #region RemoveNode

        [Fact]
        public void RemoveNodeHierarchyWriter_removes_startNode()
        {
            // ARRANGE

            var startNodeMock = new Mock<NodeType>();
            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create<string>(), false, out var nodeRemoved);

            // ASSERT

            Assert.Null(result);
            Assert.True(nodeRemoved);
        }

        [Fact]
        public void RemoveNodeHierachyWriter_removes_child_node()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.RemoveChild(childNode))
                .Returns(startNodeMock.Object);
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create("a"), false, out var nodeRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(nodeRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Once());
            startNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_does_nothing_for_wrong_path()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(false);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create("a"), false, out var nodeRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.False(nodeRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Never());
            startNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_removes_grandchild_node()
        {
            // ARRANGE

            var grandChildMock = new Mock<NodeType>();
            var grandChild = grandChildMock.Object;

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.TryGetChildNode("b", out grandChild))
                .Returns(true);
            childNodeMock
                .Setup(n => n.RemoveChild(grandChild))
                .Returns(childNodeMock.Object);

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create("a", "b"), false, out var nodeRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(nodeRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Never());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveChild(grandChild), Times.Once());
            childNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierarchyWriter_doesnt_remove_startNode_if_it_has_children()
        {
            // ARRANGE

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create<string>(), false, out var nodeRemoved);

            // ASSERT

            Assert.Equal(startNodeMock.Object, result);
            Assert.False(nodeRemoved);

            startNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_doesnt_removes_child_node_if_it_has_children()
        {
            // ARRANGE

            var grandChildMock = new Mock<NodeType>();
            var grandChild = grandChildMock.Object;

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create("a"), false, out var nodeRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.False(nodeRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Never());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveChild(grandChild), Times.Never());
            childNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_removes_child_node_with_children_if_recursion_is_enabled()
        {
            // ARRANGE

            var grandChildMock = new Mock<NodeType>();
            var grandChild = grandChildMock.Object;

            var childNodeMock = new Mock<NodeType>();

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);
            startNodeMock
                .Setup(n => n.RemoveChild(childNode))
                .Returns(startNodeMock.Object);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveNode(startNodeMock.Object, HierarchyPath.Create("a"), true, out var nodeRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(nodeRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Once());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveChild(grandChild), Times.Never());
            childNodeMock.VerifyAll();
        }

        #endregion RemoveNode

        #region RemoveChildNodes

        [Fact]
        public void RemoveNodeHierachyWriter_removes_childNodes_of_given_node()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.RemoveChild(childNode))
                .Returns(startNodeMock.Object);
            startNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            startNodeMock
                .Setup(n => n.ChildNodes)
                .Returns(new[] { childNodeMock.Object });

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveChildNodes(startNodeMock.Object, out var childNodesWereRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(childNodesWereRemoved);

            startNodeMock.Verify(n => n.RemoveChild(childNodeMock.Object), Times.Once());
            startNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_ignores_if_node_childNodes_exist()
        {
            // ARRANGE

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.RemoveChildNodes(startNodeMock.Object, out var childNodesWereRemoved);

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.False(childNodesWereRemoved);

            startNodeMock.Verify(n => n.RemoveChild(It.IsAny<NodeType>()), Times.Never());
            startNodeMock.VerifyAll();
        }

        #endregion RemoveChildNodes
    }
}