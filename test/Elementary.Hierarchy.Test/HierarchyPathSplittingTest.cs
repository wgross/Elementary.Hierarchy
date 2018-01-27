namespace Elementary.Hierarchy.Test
{
    using System;
    using System.Linq;
    using Xunit;
    using static Xunit.Assert;

    public class HierarchyPathTest
    {
        #region Parent

        [Fact]
        public void Split_parent_from_path_as_prefix()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Parent();

            // ASSERT

            Equal(new[] { "1", "2" }, result.Items);
        }

        [Fact]
        public void Split_parent_from_string_root_path_throws_InvalidOperationException()
        {
            // ARRANGE

            var path = HierarchyPath.Create<string>();

            // ACT

            var result = Throws<InvalidOperationException>(() => path.Parent());

            // ASSERT

            NotNull(result);
        }

        [Fact]
        public void Split_parent_from_int_root_path_throws_InvalidOperationException()
        {
            // ARRANGE

            var path = HierarchyPath.Create<int>();

            // ACT

            var result = Throws<InvalidOperationException>(() => path.Parent());

            // ASSERT

            NotNull(result);
        }

        #endregion Parent

        #region Leaf

        [Fact]
        public void Split_leaf_from_path()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Leaf();

            // ASSERT

            Equal(new[] { "3" }, result.Items);
        }

        #endregion Leaf

        #region Descendants

        [Fact]
        public void Split_descendants_under_root_item()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Descendants();

            // ASSERT

            Equal(new[] { "2", "3" }, result.Items);
        }

        [Fact]
        public void Split_empty_descendants_from_empty_path()
        {
            // ARRANGE

            var path = HierarchyPath.Create<string>();

            // ACT & ASSERT

            Same(path, path.Leaf());
            Equal(Enumerable.Empty<string>().ToArray(), path.Descendants().Items);
        }

        #endregion Descendants
        
        #region ToString

        [Fact]
        public void Path_creates_string_representation_with_default_separator()
        {
            // ACT

            var result = HierarchyPath.Create("a", "b").ToString();

            // ASSERT

            Equal("a/b", result);
        }

        [Fact]
        public void Path_creates_string_representation_with_custom_separator()
        {
            // ACT

            var result = HierarchyPath.Create("a", "b").ToString(".");

            // ASSERT

            Equal("a.b", result);
        }

        [Fact]
        public void Path_creates_string_representation_with_null_separator()
        {
            // ACT

            var result = HierarchyPath.Create("a", "b").ToString(null);

            // ASSERT

            Equal("ab", result);
        }

        [Fact]
        public void Path_creates_empty_string_represention_for_empty_path()
        {
            // ACT

            var result = HierarchyPath.Create(string.Empty).ToString();

            // ASSERT

            Equal(string.Empty, result);
        }

        #endregion ToString
        
        #region Join

        [Fact]
        public void Join_empty_path_with_leaf_item()
        {
            // ARRAGE

            var root = HierarchyPath.Create<int>();

            // ACT

            var result = root.Join(2);

            // ASSERT

            Equal(0, root.Items.Count());
            Equal(1, result.Items.Count());
            Equal(2, result.Items.Single());
        }

        [Fact]
        public void Join_path_with_leaf()
        {
            // ARRAGE

            var key = HierarchyPath.Create<int>().Join(2);

            // ACT

            var result = key.Join(3);

            // ASSERT

            Equal(1, key.Items.Count());
            Equal(2, result.Items.Count());
            Equal(2, result.Items.ElementAt(0));
            Equal(3, result.Items.ElementAt(1));
        }

        [Fact]
        public void Join_path_with_leaf_path()
        {
            // ARRAGE

            var key = HierarchyPath.Create<int>(1, 2);
            var other = HierarchyPath.Create(3, 4);

            // ACT

            var result = key.Join(other);

            // ASSERT

            Equal(4, result.Items.Count());
            Equal(1, result.Items.ElementAt(0));
            Equal(2, result.Items.ElementAt(1));
            Equal(3, result.Items.ElementAt(2));
            Equal(4, result.Items.ElementAt(3));
        }

        #endregion Join

        #region RelativeToAncestor

        [Fact]
        public void Calculate_relative_path_from_ancestor_to_descendant()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("a", "b");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(ancestorKey);

            // ASSERT

            Equal(1, result.Items.Count());
            Equal("b", result.Items.First());
        }

        [Fact]
        public void Relative_path_to_itself_is_empty()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("a");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(ancestorKey);

            // ASSERT

            Equal(0, result.Items.Count());
        }

        [Fact]
        public void Calculate_relative_path_to_different_branch_throws()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("b");

            // ACT

            var ex = Throws<InvalidOperationException>(() => descendantKey.RelativeToAncestor(ancestorKey));
            True(ex.Message.Contains("No common key parts"));
        }

        [Fact]
        public void Relative_path_to_root_is_itself_path()
        {
            // ASSERT
            var descendantKey = HierarchyPath.Create("a");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(HierarchyPath.Create<string>());

            // ASSERT

            Equal(1, result.Items.Count());
            Equal(descendantKey, result);
            Same(descendantKey, result);
        }

        #endregion RelativeToAncestor

        #region IsDescendant

        [Fact]
        public void Parent_path_is_ancestor_of_descendant_path()
        {
            // ARRANGE

            var parent = HierarchyPath.Create("a");
            var descendant = HierarchyPath.Create("a", "b");

            // ACT

            bool result1 = parent.IsAncestorOf(descendant: descendant);
            bool result2 = descendant.IsAncestorOf(descendant: parent);

            // ASSERT

            True(result1);
            False(result2);
        }

        [Fact]
        public void Sibling_isnt_descendant()
        {
            // ARRANGE

            var sibling1 = HierarchyPath.Create("a");
            var sibling2 = HierarchyPath.Create("b");

            // ACT

            bool result1 = sibling1.IsAncestorOf(sibling2);
            bool result2 = sibling2.IsAncestorOf(sibling1);

            // ASSERT

            False(result1);
            False(result2);
        }

        #endregion IsDescendant

        #region Root path disambiguisation

        [Fact]
        public void Root_pathes_are_equal()
        {
            // ACT & ASSERT

            Equal(HierarchyPath.Create<string>(), HierarchyPath.Parse("/", separator: "/"));
            Equal(HierarchyPath.Parse("/", separator: "/"), HierarchyPath.Parse("", separator: "/"));
            NotEqual(HierarchyPath.Create<string>(), HierarchyPath.Create<string>(""));
        }

        #endregion Root path disambiguisation
    }
}