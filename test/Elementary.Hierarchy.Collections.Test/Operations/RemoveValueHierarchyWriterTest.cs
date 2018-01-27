using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class RemoveValueHierarchyWriterTest
    {
        public interface NodeType :
            IHierarchyNodeWriter<NodeType>,
            IHierarchyValueReader<int>,
            IHierarchyValueWriter<int>,
            IHasIdentifiableChildNodes<string, NodeType>,
            IHasChildNodes<NodeType>
        {
        }

        [Fact]
        public void RemoveValueHierarchyWriter_removes_value_from_startNode()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>();

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(writer.ValueWasCleared);

            startNode.Verify(n => n.RemoveValue(), Times.Once());
        }

        [Fact]
        public void RemoveValueHierarchyWriter_removes_value_from_childNode()
        {
            // ARRANGE

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a"))
                .Returns((true,childNode));

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>();

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
        public void RemoveValueHierarchyWriter_removes_value_from_grandChildNode()
        {
            // ARRANGE

            var grandChildMock = new Mock<NodeType>();
            grandChildMock
                .Setup(n => n.RemoveValue())
                .Returns(true);

            var grandChild = grandChildMock.Object;

            var childNodeMock = new Mock<NodeType>();
            childNodeMock
                .Setup(n => n.TryGetChildNode("b"))
                .Returns((true,grandChild));

            var childNode = childNodeMock.Object;

            var startNodeMock = new Mock<NodeType>();
            startNodeMock
                .Setup(n => n.TryGetChildNode("a"))
                .Returns((true, childNode));

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>();

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
        public void RemoveValueHierarchyWriter_doesnt_removes_value_from_startNode_if_no_value_was_set()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.RemoveValue())
                .Returns(false);

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>();

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.RemoveValue(), Times.Once());
        }

        [Fact]
        public void RemoveValueHierarchyWriter_doesnt_remove_value_from_startNode_child_if_no_value_was_set()
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
                .Setup(n => n.TryGetChildNode("a"))
                .Returns((true, childNode));

            var writer = new RemoveValueHierarchyWriter<string, int, NodeType>();

            // ACT

            writer.ClearValue(startNode.Object, HierarchyPath.Create<string>("a"));

            // ASSERT

            Assert.False(writer.ValueWasCleared);

            startNode.Verify(n => n.TryGetChildNode("a"), Times.Once());
            childNodeMock.Verify(n => n.RemoveValue());
        }
    }
}