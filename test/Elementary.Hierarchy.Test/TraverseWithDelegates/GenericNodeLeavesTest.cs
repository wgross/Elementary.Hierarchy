using Elementary.Hierarchy.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    
    public class GenericNodeLeavesTest
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
            Assert.Equal(1, result.Count());
            Assert.Equal("root", result.ElementAt(0));
        }

        [Fact]
        public void D_root_returns_its_descendant_leaves_on_Leaves()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = p =>
            {
                switch (p)
                {
                    case "root":
                        return new[] { "leftNode", "rightNode" };

                    case "leftNode":
                        return new[] { "leftLeaf" };

                    case "rightNode":
                        return new[] { "leftRightLeaf", "rightRightLeaf" };
                }
                return Enumerable.Empty<string>();
            };

            // ACT

            IEnumerable<string> result = "root".Leaves(getChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.ToArray());
        }
    }
}