using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Moq;
using System;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class SetOrAddNodeVaueWriterTest
    {
        public interface NodeType :
            IHierarchyNodeWriter<NodeType>,
            IHasIdentifiableChildNodes<string, NodeType>,
            IHierarchyValueWriter<int>,
            IHierarchyValueReader<int>
        {
        }

        #region AddValue

        [Fact]
        public void SetOrAddValue_Adds_value_to_start_node()
        {
            // ARRANGE
            int value = 0;
            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetValue(out value))
                .Returns(false);

            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => null);

            // ACT

            var result = writer.AddValue(startNode.Object, HierarchyPath.Create<string>(), 1);

            // ASSERT

            startNode.Verify(n => n.TryGetValue(out value), Times.Once());
            startNode.Verify(n => n.SetValue(1), Times.Once());
            startNode.VerifyAll();

            Assert.Same(result, startNode.Object);
            Assert.Same(startNode.Object, writer.DescandantAt);
        }

        [Fact]
        public void SetOrAddValue_Adds_value_to_created_inner_node()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>();
            var startNode = new Mock<NodeType>();

            NodeType child;
            startNode
                .Setup(n => n.TryGetChildNode("a", out child))
                .Returns(false);

            startNode
                .Setup(n => n.AddChild(childNode.Object))
                .Returns(startNode.Object);

            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => childNode.Object);

            // ACT

            var result = writer.AddValue(startNode.Object, HierarchyPath.Create("a"), 1);

            // ASSERT

            startNode.Verify(n => n.AddChild(childNode.Object), Times.Once());
            startNode.VerifyAll();

            childNode.Verify(n => n.SetValue(1), Times.Once());
            childNode.VerifyAll();

            Assert.Same(startNode.Object, result);
            Assert.Same(childNode.Object, writer.DescandantAt);
        }

        [Fact]
        public void SetOrAddValue_Adds_value_fails_if_node_has_value()
        {
            // ARRANGE

            int value = 2;
            var childNode = new Mock<NodeType>();
            childNode
                .Setup(n => n.TryGetValue(out value))
                .Returns(true);

            var childNodeObject = childNode.Object;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNodeObject))
                .Returns(true);

            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => null);

            // ACT

            var result = Assert.Throws<ArgumentException>(() => writer.AddValue(startNode.Object, HierarchyPath.Create("a"), 1));

            // ASSERT

            startNode.Verify(n => n.TryGetChildNode("a", out childNodeObject), Times.Once());
            startNode.VerifyAll();

            childNode.Verify(n => n.TryGetValue(out value), Times.Once());
            childNode.Verify(n => n.SetValue(1), Times.Never());
            childNode.VerifyAll();

            Assert.True(result.Message.Contains("NodeType at 'a' already has a value"));
            Assert.Equal("path", result.ParamName);
            Assert.Same(childNode.Object, writer.DescandantAt);
        }

        #endregion AddValue

        #region SetValue

        [Fact]
        public void SetOrAddValue_Sets_value_at_start_node()
        {
            // ARRANGE
            int value = 0;
            var startNode = new Mock<NodeType>();
            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => null);

            // ACT

            var result = writer.SetValue(startNode.Object, HierarchyPath.Create<string>(), 1);

            // ASSERT

            startNode.Verify(n => n.TryGetValue(out value), Times.Never());
            startNode.Verify(n => n.SetValue(1), Times.Once());
            startNode.VerifyAll();

            Assert.Same(result, startNode.Object);
            Assert.Same(startNode.Object, writer.DescandantAt);
        }

        [Fact]
        public void SetOrAddValue_Sets_value_at_created_inner_node()
        {
            // ARRANGE

            var childNode = new Mock<NodeType>();
            var startNode = new Mock<NodeType>();

            NodeType child;
            startNode
                .Setup(n => n.TryGetChildNode("a", out child))
                .Returns(false);

            startNode
                .Setup(n => n.AddChild(childNode.Object))
                .Returns(startNode.Object);

            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => childNode.Object);

            // ACT

            var result = writer.SetValue(startNode.Object, HierarchyPath.Create("a"), 1);

            // ASSERT

            startNode.Verify(n => n.AddChild(childNode.Object), Times.Once());
            startNode.VerifyAll();

            childNode.Verify(n => n.SetValue(1), Times.Once());
            childNode.VerifyAll();

            Assert.Same(startNode.Object, result);
            Assert.Same(childNode.Object, writer.DescandantAt);
        }

        [Fact]
        public void SetOrAddValue_Set_value_overwrites_existing_value()
        {
            // ARRANGE

            int value = 2;
            var childNode = new Mock<NodeType>();

            var childNodeObject = childNode.Object;

            var startNode = new Mock<NodeType>();
            startNode
                .Setup(n => n.TryGetChildNode("a", out childNodeObject))
                .Returns(true);
            startNode
                .Setup(n => n.ReplaceChild(childNodeObject, childNodeObject))
                .Returns(startNode.Object);

            var writer = new SetOrAddNodeValueWriter<string, int, NodeType>(id => null);

            // ACT

            var result = writer.SetValue(startNode.Object, HierarchyPath.Create("a"), 1);

            // ASSERT

            startNode.Verify(n => n.TryGetChildNode("a", out childNodeObject), Times.Once());
            startNode.VerifyAll();

            childNode.Verify(n => n.TryGetValue(out value), Times.Never());
            childNode.Verify(n => n.SetValue(1), Times.Once());
            childNode.VerifyAll();

            Assert.Same(startNode.Object, result);
            Assert.Same(childNode.Object, writer.DescandantAt);
        }

        #endregion SetVAlue
    }
}