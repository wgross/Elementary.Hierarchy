using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HierarchyPathCreationTest
    {
        #region Create

        [Fact]
        public void Create_empty_hierarchy_path()
        {
            // ACT

            var result = HierarchyPath.Create<string>();

            // ASSERT

            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.False(result.Items.Any());
            Assert.True(!result.HasParentNode);
            //questionable//Equal(HierarchyPath.Create<string>().GetHashCode(), result.GetHashCode());
        }

        [Fact]
        public void Create_path_with_single_item()
        {
            // ACT

            var result = HierarchyPath.Create("a");

            // ASSERT

            Assert.NotNull(result);
            Assert.True(result.Items.Any());
            Assert.Equal(new[] { "a" }, result.Items.ToArray());
        }

        [Fact]
        public void Create_path_from_item_array()
        {
            // ACT

            var result = HierarchyPath.Create(new[] { "a", "B" });

            // ASSERT

            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.Equal(new[] { "a", "B" }, result.Items.ToArray());
        }

        [Fact]
        public void Create_path_from_array_copy()
        {
            // ARRANGE

            var sourceArray = new[] { "a", "B" };
            var result = HierarchyPath.Create(sourceArray);

            // ACT

            sourceArray[1] = "c";

            // ASSERT

            Assert.Equal(new[] { "a", "B" }, result.Items.ToArray());
        }

        [Fact]
        public void Create_from_enumerable_copy()
        {
            // ARRANGE

            var sourceList = new List<string> { "a", "B" };
            var result = HierarchyPath.Create<string>(sourceList);

            // ACT

            sourceList[1] = "c";

            // ASSERT

            Assert.Equal(new[] { "a", "B" }, result.Items.ToArray());
        }

        #endregion Create
    }
}