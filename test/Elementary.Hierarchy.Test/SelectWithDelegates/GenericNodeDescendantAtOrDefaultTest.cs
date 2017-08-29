using Elementary.Hierarchy.Generic;
using System;
using Xunit;

namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    public class GenericNodeDescendantAtOrDefaultTest
    {
        [Fact]
        public void D_returns_child_on_DescendantAtOrDefault()
        {
            // ACT

            string result1 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode"), out foundNodePath);

            // ASSERT

            Assert.Equal("leftNode", result1);
            Assert.Equal("leftNode", result2);
            Assert.Equal(HierarchyPath.Create("leftNode"), foundNodePath);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAtOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string,(bool,string)>)(delegate (string node, string key)
            {
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            HierarchyPath<string> foundNodePath;
            string result = "rootNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create<string>(), out foundNodePath);

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal("rootNode", result);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }

        [Fact]
        public void D_returns_grandchild_on_DescendentAtOrDefault()
        {

            // ACT

            string result1 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode", "leftLeaf"));

            HierarchyPath<string> foundNodePath;
            string result2 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode", "leftLeaf"), out foundNodePath);

            // ASSERT

            Assert.Same("leftLeaf", result1);
            Assert.Same("leftLeaf", result2);
            Assert.Equal(HierarchyPath.Create("leftNode", "leftLeaf"), foundNodePath);
        }

        [Fact]
        public void D_returns_null_on_invalid_childId_on_DescendantOrDefault()
        {
            // ACT

            string result1 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unknownNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unknownNode"), out foundNodePath);

            // ASSERT

            Assert.Null(result1);
            Assert.Null(result2);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }

        [Fact]
        public void D_returns_substitute_on_invalid_childId_on_DescendantOrDefault()
        {
            // ACT

            string result1 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unknownNode"), createDefault: () => "substitute");

            HierarchyPath<string> foundNodePath;
            string result2 = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unknownNode"), out foundNodePath, createDefault: () => "substitute");

            // ASSERT

            Assert.Equal("substitute", result1);
            Assert.Equal("substitute", result2);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }
    }
}