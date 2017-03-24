using Elementary.Hierarchy.Collections.Operations;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class ClearValueHierarchyWriterTest
    {
        public interface NodeType :
            IHierarchyNodeWriter<NodeType>,
            IHierarchyValueReader<int>,
            IHierarchyValueWriter<int>,
            IHasIdentifiableChildNodes<string, NodeType>
        {
        }

        [Fact]
        public void ClearValueHierarchyWriter_removes_value_from_startNode()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: false);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNode.Verify(n => n.RemoveValue(), Times.Once());
        }

        [Fact]
        public void ClearValueHierarchyWriter_removes_value_from_childNode()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: false);

            // ACT

            writer.ClearValue(startNodeMock.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNodeMock.Verify(n => n.RemoveValue(), Times.Never());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveValue(), Times.Once());
            childNodeMock.VerifyAll();
        }

        [Fact]
        public void ClearValueHierarchyWriter_removes_value_from_grandChildNode()
        {
            // ARRANGE

            var grandChildMock = new Mock<NodeType>();
            grandChildMock
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var grandChild = grandChildMock.Object;

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.TryGetChildNode("b", out grandChild))
                .Returns(true);

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: false);

            // ACT

            writer.ClearValue(startNodeMock.Object, HierarchyPath.Create("a", "b"));

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNodeMock.Verify(n => n.RemoveValue(), Times.Never());
            startNodeMock.VerifyAll();
            childNodeMock.Verify(n => n.RemoveValue(), Times.Never());
            childNodeMock.VerifyAll();
            grandChildMock.Verify(n => n.RemoveValue(), Times.Once());
            grandChildMock.VerifyAll();
        }

        [Fact]
        public void ClearValueHierarchyWriter_doesnt_removes_value_from_startNode_if_no_value_was_set()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.RemoveValue())
                .Returns(false);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: false);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.RemoveValue(), Times.Once());
        }

        [Fact]
        public void ClearValueHierarchyWriter_doesnt_remove_value_from_startNode_child_if_no_value_was_set()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            int value = 0;
            childNodeMock
                .Setup(n => n.TryGetValue(out value))
                .Returns(false);
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(false);

            var childNode = childNodeMock.Object;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: false);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            childNodeMock.Verify(n => n.RemoveValue());
        }

        [Fact]
        public void ClearValueHierarchyWriter_prunes_hierarchy_if_no_descendant_has_value()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var childNode = childNodeMock.Object;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);
            startNode
                .Setup(n => n.RemoveChild(childNode))
                .Returns(startNode.Object); // node is changed in place

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: true);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            startNode.Verify(n => n.RemoveChild(childNode), Times.Once());
            childNodeMock.Verify(n => n.RemoveValue());
        }

        [Fact]
        public void ClearValueHierarchyWriter_prunes_hierarchy_if_no_descendant_has_value_and_no_value_was_removed()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(false);

            var childNode = childNodeMock.Object;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);
            startNode
                .Setup(n => n.RemoveChild(childNode))
                .Returns(startNode.Object); // node is changed in place

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: true);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            startNode.Verify(n => n.RemoveChild(childNode), Times.Once());
            childNodeMock.Verify(n => n.RemoveValue());
        }

        [Fact]
        public void ClearValueHierarchyWriter_doesnt_prune_hierarchy_if_descendant_has_value()
        {
            // ARRANGE

            int value = 0;

            var grandChildMock = new Mock<NodeType>();
            grandChildMock
                .Setup(n => n.TryGetValue(out value))
                .Returns(true);

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(true);
            childNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            childNodeMock
                .Setup(n => n.ChildNodes)
                .Returns(new[] { grandChildMock.Object });

            var childNode = childNodeMock.Object;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: true);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            startNode.Verify(n => n.RemoveChild(It.IsAny<NodeType>()), Times.Never());
            startNode.VerifyAll();
            childNodeMock.Verify(n => n.RemoveValue());
            childNodeMock.VerifyAll();
            grandChildMock.Verify(n => n.TryGetValue(out value), Times.Once());
            grandChildMock.VerifyAll();
        }

        [Fact]
        public void ClearValueHierarchyWriter_doesnt_prune_hierarchy_if_value_wasnt_removed_and_descendant_has_value()
        {
            // ARRANGE

            int value = 0;

            var grandChildMock = new Mock<NodeType>();
            grandChildMock
                .Setup(n => n.TryGetValue(out value))
                .Returns(true);

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(false);
            childNodeMock
                .Setup(n => n.HasChildNodes)
                .Returns(true);
            childNodeMock
                .Setup(n => n.ChildNodes)
                .Returns(new[] { grandChildMock.Object });

            var childNode = childNodeMock.Object;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>(pruneAfterClear: true);

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            startNode.Verify(n => n.RemoveChild(It.IsAny<NodeType>()), Times.Never());
            startNode.VerifyAll();
            childNodeMock.Verify(n => n.RemoveValue());
            childNodeMock.VerifyAll();
            grandChildMock.Verify(n => n.TryGetValue(out value), Times.Once());
            grandChildMock.VerifyAll();
        }
    }
}