using Xunit;

namespace Elementary.Hierarchy.Nodes.Test
{
    public class KeyValueNode_IHasIdentifieableChildNodesTest
    {
        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_root_node(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.DescendantAt(HierarchyPath.Create<string>());

            // ASSERT

            Assert.Same(root, result);
        }

        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_childnodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.DescendantAt(HierarchyPath.Create("leftNode"));

            // ASSERT

            Assert.Same("leftNode", result.Key);
        }

        [Theory]
        [MemberData(nameof(KeyValueNodeTreeDefinitions.Default), MemberType = typeof(KeyValueNodeTreeDefinitions))]
        public void KeyValueNode_returns_grandchildnodes(KeyValueNode<string, int> root)
        {
            // ACT

            var result = root.DescendantAt(HierarchyPath.Create("leftNode", "leftLeaf"));

            // ASSERT

            Assert.Same("leftLeaf", result.Key);
        }
    }
}