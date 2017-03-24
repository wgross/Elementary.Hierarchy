using Elementary.Hierarchy.Collections.Operations;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class RemoveNodeHierarchyWriterTest
    {
        public interface NodeType : IHierarchyNodeWriter<NodeType>, IHasIdentifiableChildNodes<string, NodeType>
        {
        }

        [Fact]
        public void RemoveNodeHierarchyWriter_startNode_cant_be_removed_because_parnet_is_unkown()
        {
            // ARRANGE

            var startNodeMock = new Mock<NodeType>();
            var writer = new RemoveNodeHierarchyWriter<string, NodeType>();

            // ACT

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Null(result);
            Assert.False(writer.HasRemovedNode);
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

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(writer.HasRemovedNode);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Once());
            startNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_does_nothinh_for_wrong_path()
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

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.False(writer.HasRemovedNode);

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

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create("a", "b"));

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(writer.HasRemovedNode);

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

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal(startNodeMock.Object, result);
            Assert.False(writer.HasRemovedNode);

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

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.False(writer.HasRemovedNode);

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

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>(recurse: true);

            // ACT

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNodeMock.Object);
            Assert.True(writer.HasRemovedNode);

            startNodeMock.Verify(n => n.RemoveChild(childNode), Times.Once());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveChild(grandChild), Times.Never());
            childNodeMock.VerifyAll();
        }

        [Fact]
        public void RemoveNodeHierachyWriter_removes_startNode_with_children_if_recursion_is_enabled()
        {
            // ARRANGE

            var startNodeMock = new Mock<NodeType>();

            var writer = new RemoveNodeHierarchyWriter<string, NodeType>(recurse: true);

            // ACT

            var result = writer.Visit(startNodeMock.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Null(result);
            Assert.False(writer.HasRemovedNode);

            startNodeMock.VerifyAll();
        }
    }
}