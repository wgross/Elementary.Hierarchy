namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Linq;
    using Xunit;

    public class GenericNodeDescendAlongPathTest
    {
        [Fact]
        public void D_returns_itself_for_empty_path_on_DescendAlongPath()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string, (bool, string)>)(delegate (string node, string key)
            {
                if (node == "startNode" && key == "childNode")
                {
                    return (true, "childNode");
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create<string>()).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("startNode", result.ElementAt(0));
        }

        [Fact]
        public void D_returns_child_on_DescendAlongPath()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string, (bool, string)>)(delegate (string node, string key)
              {
                  if (node == "startNode" && key == "childNode")
                  {
                      return (true, "childNode");
                  }

                  throw new InvalidOperationException("unknown node");
              });

            // ACT

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create<string>("childNode")).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(new[] { "startNode", "childNode" }, result);
        }

        [Fact]
        public void D_returns_child_and_grandchild_on_DescendAlongPath()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string, (bool, string)>)(delegate (string node, string key)
            {
                if (node == "startNode" && key == "childNode")
                {
                    return (true, "childNode");
                }
                else if (node == "childNode" && key == "grandChildNode")
                {
                    return (true, "grandChildNode");
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode")).ToArray();

            // ASSERT

            Assert.Equal(new[] { "startNode", "childNode", "grandChildNode" }, result);
        }

        [Fact]
        public void D_return_incomplete_list_on_DescendAlongPath()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string, (bool, string)>)(delegate (string node, string key)
            {
                if (node == "startNode")
                {
                    return (false, null);
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create("childNode")).ToArray();

            // ASSERT

            Assert.True(result.Any());
            Assert.Equal(new[] { "startNode" }, result);
        }
    }
}