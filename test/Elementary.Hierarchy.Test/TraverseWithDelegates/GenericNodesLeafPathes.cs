using Elementary.Hierarchy.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodesBreadcrumbsToLeaves
    {
        [Fact]
        public void D_empty_root_returns_empty_path_to_itself_on_BreadcrumbsToLeaves()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = p => Enumerable.Empty<string>();

            // ACT

            IEnumerable<(IEnumerable<string> breadcrumb, string leaf)> result = "root".BreadcrumbsToLeaves(getChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Empty(result.ElementAt(0).breadcrumb);
            Assert.Equal("root", result.ElementAt(0).leaf);
        }

        [Fact]
        public void D_root_returns_its_descendant_leaves_on_BreadcrumbsToLeaves()
        {
            // ACT

            IEnumerable<(IEnumerable<string> breadcrumb, string leaf)> result = "rootNode".BreadcrumbsToLeaves(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(r => r.leaf));
        }

        [Fact]
        public void D_Leaves_are_not_Leaves_if_maxDepth_makes_them_to_Leaves_on_BreadcrumbsToLeaves()
        {
            // ACT & ASSERT
            // maxDepths doesn't create leaves in the sense of the Leaves algorithm

            Assert.Empty("rootNode".BreadcrumbsToLeaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 0));
            Assert.Empty("rootNode".BreadcrumbsToLeaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 1));
            Assert.Equal(3, "rootNode".BreadcrumbsToLeaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 2).Count());
            Assert.Equal(3, "rootNode".BreadcrumbsToLeaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 3).Count());
        }
    }
}