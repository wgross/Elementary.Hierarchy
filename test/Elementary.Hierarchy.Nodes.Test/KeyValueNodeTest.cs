using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNodeTest
    {
        #region CREATE

        [Fact]
        public void KeyValueNode_Factory_creates_root_node_with_value()
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
        public void KeyValueNode_Factory_creates_innerNode_with_child_having_key_and_value()
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
        public void KeyValueNode_Factory_creates_root_node_with_childNodes_and_grandchildren()
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

        #endregion CREATE

        #region UPDATE child nodes

        [Fact]
        public void KeyValueNode_adds_childnode()
        {
            // ARRANGE

            var root = KeyValueNode.RootNode<string, int>(0);
            var child = KeyValueNode.InnerNode("a", 1);

            // ACT

            root.Add(child);

            // ASSERT

            Assert.True(root.HasChildNodes);
            Assert.Same(child, root.ChildNodes.Single());
        }

        [Fact]
        public void KeyValueNode_adds_childnode_fails_on_duplicate_key()
        {
            // ARRANGE

            var root = KeyValueNode.RootNode<string, int>(0, KeyValueNode.InnerNode("a", 0));
            var child = KeyValueNode.InnerNode("a", 1);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => root.Add(child));

            // ASSERT

            Assert.True(root.HasChildNodes);
            Assert.NotSame(child, root.ChildNodes.Single());
        }

        [Fact]
        public void KeyValueNode_substitutes_child_node_by_Key()
        {
            // ARRANGE

            var root = KeyValueNode.RootNode<string, int>(0, KeyValueNode.InnerNode("a", 0));
            var child = KeyValueNode.InnerNode("a", 1);

            // ACT

            root.Set(child);

            // ASSERT

            Assert.Same(child, root.ChildNodes.Single());
        }

        #endregion UPDATE child nodes

        #region DELETE child node

        [Fact]
        public void KeyValueNode_removes_childnode_by_key()
        {
            // ARRANGE

            var root = KeyValueNode.RootNode<string, int>(0, KeyValueNode.InnerNode("a", 0));

            // ACT

            var result = root.Remove("a");

            // ASSERT

            Assert.True(result);
            Assert.False(root.ChildNodes.Any());
        }

        [Fact]
        public void KeyValueNode_fails_gracefully_on_removing_childnode_by_unkown_key()
        {
            // ARRANGE

            var root = KeyValueNode.RootNode<string, int>(0, KeyValueNode.InnerNode("a", 0));

            // ACT

            var result = root.Remove("x");

            // ASSERT

            Assert.False(result);
            Assert.Single(root.ChildNodes);
        }

        #endregion DELETE child node
    }
}