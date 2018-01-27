using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNode_IHasChildNodeTest
    {
        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_child_nodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.Children();

            // ASSERT

            Assert.Equal(new[] { "leftNode", "rightNode" }, result.Select(n => n.Key));
        }

        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_descendant_nodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.Descendants();

            // ASSERT

            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(n => n.Key));
        }

        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_descendantAndSelf_nodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.False(result.ElementAt(0).HasKey);
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Skip(1).Select(n => n.Key));
        }

        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_leaves_nodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.Leaves();

            // ASSERT

            Assert.Equal(new[] { "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(n => n.Key));
        }
    }
}