using System;
using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNodeTest
    {
        [Fact]
        public void KeyValueNode_stores_Key_and_Value()
        {
            // ACT
            var result = new KeyValueNode<int, string>(key: 1, value: "value");

            // ASSERT

            Assert.Equal(1, result.Key);
            Assert.True(result.HasKey);
            Assert.Equal("value", result.Value);
        }

        [Fact]
        public void KeyValueNode_stores_Key_value_and_ChildNodes()
        {
            // ARRANGE
            var a = KeyValueNode.Create(key: 1, value: "nodeA");
            var b = KeyValueNode.Create(key: 2, value: "nodeB");

            // ACT
            // create third node, add a and b as childnodes

            var result = new KeyValueNode<int, string>(3, "nodeC", a, b);

            // ASSERT
            // check the parent node
            Assert.Equal(3, result.Key);
            Assert.Equal("nodeC", result.Value);
            Assert.True(result.HasChildNodes);
            Assert.Equal(new[] { a, b }, result.Children());
        }

        [Fact]
        public void KeyValueNode_root_has_no_key()
        {
            // ARRANGE

            var a = KeyValueNode.Create<string, string>(value: "nodeA");

            // ACT & ASSERT
            // throws on key retrieval

            var result = Assert.Throws<InvalidOperationException>(() => a.Key);

            Assert.Equal("node has no key", result.Message);
            Assert.Equal("nodeA", a.Value);
            Assert.False(a.HasKey);
        }
    }
}