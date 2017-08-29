using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Moq;
using System;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class GetOrCreateNodeWriterTest
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

            var result = writer.GetOrCreate(startNode.Object, HierarchyPath.Create<string>(), out var descendantAt);

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(startNode.Object, descendantAt);
        }

        [Fact]
        public void GetOrCreateNode_returns_child_of_startNode_as_destination()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>().Object;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a"))
                .Returns((true,childNode));
            startNode
                .Setup(n => n.ReplaceChild(childNode, childNode))
                .Returns(startNode.Object);

            var writer = new GetOrCreateNodeWriter<string, NodeType>(id => null);

            // ACT

            var result = writer.GetOrCreate(startNode.Object, HierarchyPath.Create("a"), out var descendentAt);

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(childNode, descendentAt);

            startNode.Verify(n => n.TryGetChildNode("a"), Times.Once());
            startNode.Verify(n => n.ReplaceChild(childNode, childNode), Times.Once());
            startNode.VerifyAll();
        }

        [Fact]
        public void GetOrCreateNode_creates_child_of_startNode_as_destination()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>().Object;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a"))
                .Returns((false,null));
            startNode
                .Setup(n => n.AddChild(childNode))
                .Returns(startNode.Object);

            Func<string, NodeType> createChildCallback = id =>
            {
                Assert.Equal("a", id);
                return childNode;
            };

            var writer = new GetOrCreateNodeWriter<string, NodeType>(createChildCallback);

            // ACT

            var result = writer.GetOrCreate(startNode.Object, HierarchyPath.Create("a"), out var descendantAt);

            // ASSERT

            Assert.Same(result, startNode.Object);
            Assert.Same(childNode, descendantAt);

            startNode.Verify(n => n.AddChild(It.IsAny<NodeType>()), Times.Once());
        }
    }
}