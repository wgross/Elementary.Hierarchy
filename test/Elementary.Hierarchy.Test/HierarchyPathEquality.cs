using System;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HierarchyPathEquality
    {
        #region Equals

        [Fact]
        public void Paths_are_equal_if_all_items_are_equal()
        {
            // ARRANGE

            var right = HierarchyPath.Create("a", "b", "c");
            var left = HierarchyPath.Create("a", "b", "c");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            Assert.True(result1);
            Assert.True(result2);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Paths_arent_equal_if_item_number_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create("a", "b", "c");
            var left = HierarchyPath.Create("a", "b");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            Assert.False(result1);
            Assert.False(result2);
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Paths_are_different_if_item_type_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create<short>(1, 2, 3);
            var left = HierarchyPath.Create<int>(1, 2, 3);

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            Assert.False(result1);
            Assert.False(result2);
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Paths_are_different_if_item_casing_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create("A");
            var left = HierarchyPath.Create("a");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            Assert.False(result1);
            Assert.False(result2);
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Paths_are_equal_if_equality_comparer_says_so()
        {
            // ARRANGE

            var right = HierarchyPath.Create("A");
            var left = HierarchyPath.Create("a");

            // ACT

            bool result1 = left.Equals(right, StringComparer.OrdinalIgnoreCase);
            bool result2 = right.Equals(left, StringComparer.OrdinalIgnoreCase);

            // ASSERT

            Assert.True(result1);
            Assert.True(result2);
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Path_is_equal_compared_with_itself()
        {
            // ARRANGE

            var left = HierarchyPath.Create("a", "b", "c");

            // ACT

            bool result = left.Equals(left);

            // ASSERT

            Assert.True(result);
        }

        #endregion Equals

        #region GetHashCode

        [Fact]
        public void Path_hashcode_is_different_for_permutations()
        {
            // ARRANGE

            var left = HierarchyPath.Create("a", "b");
            var right = HierarchyPath.Create("b", "a");

            // ACT & ASSERT

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        #endregion GetHashCode
    }
}