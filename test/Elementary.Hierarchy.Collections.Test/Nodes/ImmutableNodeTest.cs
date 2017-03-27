using Elementary.Hierarchy.Collections.Nodes;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Nodes
{
    public class ImmutableNodeTest
    {
        [Fact]
        public void ImmutableNode_adds_child_to_clone_of_current_instance()
        {
            // ARRANGE

            var child = new ImmutableNode<string, int>("a");
            var node = new ImmutableNode<string, int>();
            node.SetValue(1);

            // ACT

            var result = node.AddChild(child);

            // ASSERT
            // node creates a clone with the new child.

            Assert.NotSame(node, result);
            Assert.False(node.HasChildNodes);
            Assert.True(result.HasChildNodes);
            Assert.True(result.HasValue);
            Assert.Equal(1, result.Value);
            Assert.Same(child, result.ChildNodes.Single());
            Assert.False(node.TryGetChildNode("a", out var addedChild1));
            Assert.True(result.TryGetChildNode("a", out var addedChild2));
            Assert.Same(child, addedChild2);
        }

        [Fact]
        public void ImmutableNode_removes_child_from_clone_of_current_instance()
        {
            // ARRANGE

            var child = new ImmutableNode<string, int>("a");
            var node = new ImmutableNode<string, int>(null, 1).AddChild(child);

            // ACT

            var result = node.RemoveChild(child);

            // ASSERT

            Assert.NotSame(node, result);
            Assert.True(node.HasChildNodes);
            Assert.False(result.HasChildNodes);
            Assert.Equal(1, result.Value);
            Assert.True(node.TryGetChildNode("a", out var readChild1));
            Assert.Same(child, readChild1);
            Assert.False(result.TryGetChildNode("a", out var readChild2));
        }

        [Fact]
        public void ImmutableNode_replaces_child_in_clone_current_instance()
        {
            // ARRANGE

            var child = new ImmutableNode<string, int>("a");
            var node = new ImmutableNode<string, int>().AddChild(child);
            node.SetValue(1);
            var secondChild = new ImmutableNode<string, int>("a");

            // ACT

            var result = node.ReplaceChild(child, secondChild);

            // ASSERT

            Assert.NotSame(node, result);
            Assert.True(node.HasChildNodes);
            Assert.True(result.HasChildNodes);
            Assert.Equal(1, result.Value);
            Assert.True(node.TryGetChildNode("a", out var readChildNode1));
            Assert.Same(child, readChildNode1);
            Assert.True(result.TryGetChildNode("a", out var readChildNode2));
            Assert.Same(secondChild, readChildNode2);
        }

        [Fact]
        public void ImmutableNode_fails_on_Replace_if_keys_are_different()
        {
            // ARRANGE

            var child = new ImmutableNode<string, int>("a");
            var node = new ImmutableNode<string, int>().AddChild(child);
            var secondChild = new ImmutableNode<string, int>("b");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal("Key of child to replace (key='a') and new child (key='b') must be equal", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, node.ChildNodes.Single());
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Same(child, addedChild);
        }

        [Fact]
        public void ImmutableNode_fails_on_replacing_unknown_child()
        {
            // ARRANGE

            var child = new ImmutableNode<string, int>("a");
            var node = new ImmutableNode<string, int>(null).AddChild(child);
            var secondChild = new ImmutableNode<string, int>("b");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal("Key of child to replace (key='a') and new child (key='b') must be equal", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, node.ChildNodes.Single());
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Same(child, addedChild);
        }
    }
}