using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNodeTest
    {
        [Fact]
        public void Factory_creates_root_node_with_value()
        {
            // ACT

            var result = KeyValueNode.RootNode<string, int>(value: 0);

            // ASSERT

            Assert.Equal(0, result.Value);
            Assert.False(result.HasKey);
            Assert.False(result.HasChildNodes);
            Assert.False(result.ChildNodes.Any());
        }

        [Fact]
        public void Factory_creates_innerNode_with_child_having_key_and_value()
        {
            // ACT

            var result = KeyValueNode.InnerNode(key: "a", value: 1);

            // ASSERT

            Assert.Equal(1, result.Value);
            Assert.True(result.HasKey);
            Assert.Equal("a", result.Key);
            Assert.False(result.HasChildNodes);
            Assert.False(result.ChildNodes.Any());
            Assert.Equal((false, null), result.TryGetChildNode("xx"));
        }

        [Fact]
        public void Factory_creates_root_node_with_childNodes_and_grandchildren()
        {
            // ACT

            var result = KeyValueNode.RootNode<string, int>(0,
                KeyValueNode.InnerNode("a", 1,
                    KeyValueNode.InnerNode("b", 2)));

            // ASSERT

            Assert.True(result.HasChildNodes);
            Assert.Equal("a", result.ChildNodes.Single().Key);
            Assert.True(result.ChildNodes.Single().HasChildNodes);
            Assert.Equal("b", result.ChildNodes.Single().ChildNodes.Single().Key);

            (var found, var child) = result.TryGetChildNode("a");

            Assert.True(found);
            Assert.Equal("a", child.Key);
        }
    }
}