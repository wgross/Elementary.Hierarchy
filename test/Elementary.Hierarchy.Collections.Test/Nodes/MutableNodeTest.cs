using System;
using System.Linq;
using Elementary.Hierarchy.Collections.Nodes;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Nodes
{
    public class MutableNodeTest
    {
        [Fact]
        public void MutableNode_adds_child_to_current_instance()
        {
            // ARRANGE

            var child = new MutableNode<string, int>("a");
            var node = MutableNode<string, int>.CreateRoot();

            // ACT

            var result = node.AddChild(child);

            // ASSERT

            Assert.Same(node, result);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, result.ChildNodes.Single());

            var (found, addedChild) = node.TryGetChildNode("a");

            Assert.True(found);
            Assert.Same(child, addedChild);
        }

        [Fact]
        public void MutableNode_removes_child_from_current_instance()
        {
            // ARRANGE

            var child = new MutableNode<string, int>("a");
            var node = MutableNode<string, int>.CreateRoot().AddChild(child);

            // ACT

            var result = node.RemoveChild(child);

            // ASSERT

            Assert.Same(node, result);
            Assert.False(node.HasChildNodes);
            Assert.False(result.ChildNodes.Any());

            var (found, addedChild) = node.TryGetChildNode("a");

            Assert.False(found);
        }

        [Fact]
        public void MutableNode_replaces_child_in_current_instance()
        {
            // ARRANGE

            var child = new MutableNode<string, int>("a");
            var node = MutableNode<string, int>.CreateRoot().AddChild(child);
            var secondChild = new MutableNode<string, int>("a");

            // ACT

            var result = node.ReplaceChild(child, secondChild);

            // ASSERT

            Assert.Same(node, result);
            Assert.True(node.HasChildNodes);
            Assert.Same(secondChild, result.ChildNodes.Single());

            var (found, addedChild) = node.TryGetChildNode("a");

            Assert.True(found);
            Assert.Same(secondChild, addedChild);
        }

        [Fact]
        public void MutableNode_fails_on_Replace_if_keys_are_different()
        {
            // ARRANGE

            var child = new MutableNode<string, int>("a");
            var node = MutableNode<string, int>.CreateRoot().AddChild(child);
            var secondChild = new MutableNode<string, int>("b");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal("Key of child to replace (key='a') and new child (key='b') must be equal", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, node.ChildNodes.Single());

            var (found, addedChild) = node.TryGetChildNode("a");

            Assert.True(found);
            Assert.Same(child, addedChild);
        }

        [Fact]
        public void MutableNode_fails_on_replacing_unknown_child()
        {
            // ARRANGE
            // none of the child nodes are added to the root node.

            var child = new MutableNode<string, int>("a");
            var node = MutableNode<string, int>.CreateRoot();
            var secondChild = new MutableNode<string, int>("a");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal("The node (id=a) doesn't substutite any of the existing child nodes", result.Message);
            Assert.False(node.HasChildNodes);
        }
    }
}