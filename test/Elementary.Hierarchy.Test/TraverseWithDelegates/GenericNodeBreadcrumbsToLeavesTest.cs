﻿using Elementary.Hierarchy.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodeBreadcrumbsToLeavesTest
    {
        [Fact]
        public void D_empty_root_returns_itself_on_Leaves()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = p => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "root".Leaves(getChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("root", result.ElementAt(0));
        }

        [Fact]
        public void D_root_returns_its_descendant_leaves_on_Leaves()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Leaves(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.ToArray());
        }

        [Fact]
        public void D_Leaves_are_not_Leaves_if_maxDepth_makes_them_to_Leaves_on_Leaves()
        {
            // ACT & ASSERT
            // maxDepths doesn craete leaves in the sense of the Leaves algorithm

            Assert.Empty("rootNode".Leaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 0));
            Assert.Empty("rootNode".Leaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 1));
            Assert.Empty("rootNode".Leaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 2));
            Assert.Equal(3, "rootNode".Leaves(DelegateTreeDefinition.GetChildNodes, maxDepth: 3).Count());
        }
    }
}