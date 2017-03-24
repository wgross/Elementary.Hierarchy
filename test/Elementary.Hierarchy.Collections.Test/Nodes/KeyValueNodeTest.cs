using Elementary.Hierarchy.Collections.Nodes;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Nodes
{
    public class KeyValueNodeTest
    {
        [Fact]
        public void KeyValueNode_empty_node_has_no_value_and_no_key()
        {
            // ARRANGE

            var node = new KeyValueNode<string, int>();

            // ACT & ASSERT

            Assert.False(node.TryGetKey(out var key));
            Assert.False(node.TryGetValue(out var value));
        }

        [Fact]
        public void KeyValueNode_holds_key_and_value()
        {
            // ARRANGE

            var node = new KeyValueNode<string, int>("key", 1);

            // ACT & ASSERT

            Assert.False(node.TryGetKey(out var key));
            Assert.Equal("key", key);
            Assert.False(node.TryGetValue(out var value));
            Assert.Equal(1, value);
        }

        [Fact]
        public void KeyValueNode_stores_value()
        {
            // ARRANGE

            var node = new KeyValueNode<string, int>();

            // ACT

            node.SetValue(1);

            // ASSERT

            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);
        }

        [Fact]
        public void KeyValueNode_clears_set_value()
        {
            // ARRANGE

            var node = new KeyValueNode<string, int>();
            node.SetValue(1);

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.True(result);
            Assert.False(node.TryGetValue(out var value));
        }

        [Fact]
        public void KeyValueNode_clears_unset_value()
        {
            // ARRANGE

            var node = new KeyValueNode<string, int>();

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.False(result);
            Assert.False(node.TryGetValue(out var value));
        }
    }
}