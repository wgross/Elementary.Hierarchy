using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Moq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class GetOrCreateHierarchyWriterTest
    {
        public interface NodeType : IHierarchyNodeWriter<NodeType>, IHasIdentifiableChildNodes<string, NodeType>
        {
        }

        [Fact]
        public void GetOrCreateNode_returns_startNode_as_destination()
        {
            // ARRANGE

            var startNode = new Mock<NodeType>();
            var writer = new GetOrCreateNodeWriter<string, NodeType>(id => null);

            // ACT

            var result = writer.Visit(startNode.Object, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(startNode.Object, writer.DescandantAt);
        }

        [Fact]
        public void GetOrCreateNode_returns_child_of_startNode_as_destination()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>().Object;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNode))
                .Returns(true);
            startNode
                .Setup(n => n.ReplaceChild(childNode, childNode))
                .Returns(startNode.Object);

            var writer = new GetOrCreateNodeWriter<string, NodeType>(id => null);

            // ACT

            var result = writer.Visit(startNode.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(childNode, writer.DescandantAt);

            startNode.Verify(n => n.TryGetChildNode("a", out childNode), Times.Once());
            startNode.Verify(n => n.ReplaceChild(childNode, childNode), Times.Once());
            startNode.VerifyAll();
        }

        [Fact]
        public void GetOrCreateNode_creates_child_of_startNode_as_destination()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>().Object;

            NodeType nullNode = null;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out nullNode))
                .Returns(false);
            startNode
                .Setup(n => n.AddChild(childNode))
                .Returns(startNode.Object);

            var writer = new GetOrCreateNodeWriter<string, NodeType>(id => childNode);

            // ACT

            var result = writer.Visit(startNode.Object, HierarchyPath.Create("a"));

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(childNode, writer.DescandantAt);

            startNode.Verify(n => n.AddChild(It.IsAny<NodeType>()), Times.Once());
        }
    }
}