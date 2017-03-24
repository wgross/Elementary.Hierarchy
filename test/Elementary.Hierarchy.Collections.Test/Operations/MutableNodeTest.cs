using Elementary.Hierarchy.Collections.Nodes;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test.Operations
{
    public class MutableNodeTest
    {
        [Fact]
        public void MutableNode_Adds_child_to_current_instance()
        {
            // ARRANGE

            var node = new MutableNode<string, int>(null);
            var child = new MutableNode<string, int>("a");

            // ACT

            var result = node.AddChild(child);

            // ASSERT

            Assert.Same(node, result);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, result.ChildNodes.Single());
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Same(child, addedChild);
        }

        [Fact]
        public void MutableNode_Removes_child_from_current_instance()
        {
            // ARRANGE

            var node = new MutableNode<string, int>(null);
            var child = new MutableNode<string, int>("a");

            node.AddChild(child);

            // ACT

            var result = node.RemoveChild(child);

            // ASSERT

            Assert.Same(node, result);
            Assert.False(node.HasChildNodes);
            Assert.False(result.ChildNodes.Any());
            Assert.False(node.TryGetChildNode("a", out var addedChild));
        }

        [Fact]
        public void MutableNode_Replaces_child_in_current_instance()
        {
            // ARRANGE

            var node = new MutableNode<string, int>(null);
            var child = new MutableNode<string, int>("a");
            var secondChild = new MutableNode<string, int>("a");

            node.AddChild(child);

            // ACT

            var result = node.ReplaceChild(child, secondChild);

            // ASSERT

            Assert.Same(node, result);
            Assert.True(node.HasChildNodes);
            Assert.Same(secondChild, result.ChildNodes.Single());
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Same(secondChild, addedChild);
        }

        [Fact]
        public void MutableNode_fails_on_Replace_of_unkown_child()
        {
            // ARRANGE

            var node = new MutableNode<string, int>(null);
            var child = new MutableNode<string, int>("a");
            var secondChild = new MutableNode<string, int>("b");

            node.AddChild(child);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal($"The node (id={secondChild.Key}) doesn't substutite any of the existing child nodes in (id={node.Key})", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.Same(child, node.ChildNodes.Single());
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Same(child, addedChild);
        }
    }
}