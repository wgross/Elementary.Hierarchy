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
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundNodePath);

            // ASSERT

            Assert.Equal("childNode", result1);
            Assert.Equal("childNode", result2);
            Assert.Equal(HierarchyPath.Create("childNode"), foundNodePath);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAtOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                childNode = null;
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            HierarchyPath<string> foundNodePath;
            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create<string>(), out foundNodePath);

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal("startNode", result);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }

        [Fact]
        public void D_returns_grandchild_on_DescendentAtOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }
                else if (node == "childNode" && key == "grandChildNode")
                {
                    childNode = "grandChildNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"), out foundNodePath);

            // ASSERT

            Assert.Same("grandChildNode", result1);
            Assert.Same("grandChildNode", result2);
            Assert.Equal(HierarchyPath.Create("childNode", "grandChildNode"), foundNodePath);
        }

        [Fact]
        public void D_returns_null_on_invalid_childId_on_DescendantOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundNodePath);

            // ASSERT

            Assert.Null(result1);
            Assert.Null(result2);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }

        [Fact]
        public void D_returns_substitute_on_invalid_childId_on_DescendantOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), createDefault: () => "substitute");

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundNodePath, createDefault: () => "substitute");

            // ASSERT

            Assert.Equal("substitute", result1);
            Assert.Equal("substitute", result2);
            Assert.Equal(HierarchyPath.Create<string>(), foundNodePath);
        }
    }
}