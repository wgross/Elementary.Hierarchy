namespace Elementary.Hierarchy.Test
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static NUnit.Framework.Assert;
    using static NUnit.Framework.CollectionAssert;

    [TestFixture]
    public class HierarchyPathTest
    {
        #region Create

        [Test]
        public void Create_empty_hierarchy_path()
        {
            // ACT

            var result = HierarchyPath.Create<string>();

            // ASSERT

            IsNotNull(result);
            IsNotNull(result);
            IsFalse(result.Items.Any());
            IsTrue(!result.HasParentNode);
            //questionable//AreEqual(HierarchyPath.Create<string>().GetHashCode(), result.GetHashCode());
        }

        [Test]
        public void Create_path_with_single_item()
        {
            // ACT

            var result = HierarchyPath.Create("a");

            // ASSERT

            IsNotNull(result);
            IsTrue(result.Items.Any());
            AreEqual(new[] { "a" }, result.Items.ToArray());
        }

        [Test]
        public void Create_path_from_item_array()
        {
            // ACT

            var result = HierarchyPath.Create(new[] { "a", "B" });

            // ASSERT

            IsNotNull(result);
            IsNotNull(result);
            AreEqual(new[] { "a", "B" }, result.Items.ToArray());
        }

        [Test]
        public void Create_immutable_path_from_item_array()
        {
            // ARRANGE

            var sourceArray = new[] { "a", "B" };
            var result = HierarchyPath.Create(sourceArray);

            // ACT

            sourceArray[1] = "c";

            // ASSERT

            AreEqual(new[] { "a", "B" }, result.Items.ToArray());
        }

        [Test]
        public void Create_immutable_path_from_item_enumerable()
        {
            // ARRANGE

            var sourceList = new List<string> { "a", "B" };
            var result = HierarchyPath.Create<string>(sourceList);

            // ACT

            sourceList[1] = "c";

            // ASSERT

            AreEqual(new[] { "a", "B" }, result.Items.ToArray());
        }

        #endregion Create

        #region Parent

        [Test]
        public void Split_parent_from_path_as_prefix()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Parent();

            // ASSERT

            AreEqual(new[] { "1", "2" }, result.Items);
        }

        [Test]
        public void Split_empty_parent_path_from_empty_path()
        {
            // ARRANGE

            var path = HierarchyPath.Create(string.Empty);

            // ACT

            var result = path.Parent();

            // ASSERT

            IsFalse(result.Items.Any());
        }

        #endregion Parent

        #region Leaf

        [Test]
        public void Split_leaf_from_path()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Leaf();

            // ASSERT

            AreEqual(new[] { "3" }, result.Items);
        }

        #endregion Leaf

        #region SplitDescendants

        [Test]
        public void Split_descendants_under_root_item()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.SplitDescendants();

            // ASSERT

            AreEqual(new[] { "2", "3" }, result.Items);
        }

        [Test]
        public void Split_empty_descendants_from_empty_path()
        {
            // ARRANGE

            var path = HierarchyPath.Create<string>();

            // ACT & ASSERT

            AreSame(path, path.Leaf());
            AreEqual(Enumerable.Empty<string>().ToArray(), path.SplitDescendants().Items);
        }

        #endregion SplitDescendants

        #region Parse

        [Test]
        public void Parse_string_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<string> result = HierarchyPath.Parse(path: "/test/test2", separator: "/");

            // ASSERT

            AreEqual(2, result.Items.Count());
            AreEqual(new[] { "test", "test2" }, result.Items.ToArray());
        }

        [Test]
        public void Parse_int_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<int> result = HierarchyPath.Parse<int>(path: "1/2", separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            AreEqual(2, result.Items.Count());
            AreEqual(new[] { 1, 2 }, result.Items.ToArray());
        }

        #endregion Parse

        #region TryParse

        [Test]
        public void TryParse_string_path_items_from_string_representation()
        {
            // ACT
            HierarchyPath<string> resultPath = null;
            bool result = HierarchyPath.TryParse(path: "/test/test2", hierarchyPath: out resultPath, convertPathItem: i => i, separator: "/");

            // ASSERT

            IsTrue(result);
            AreEqual(2, resultPath.Items.Count());
            AreEqual(new[] { "test", "test2" }, resultPath.Items.ToArray());
        }

        [Test]
        public void TryParse_int_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<int> resultPath = null;
            bool result = HierarchyPath.TryParse<int>(path: "1/2", hierarchyPath: out resultPath, separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            IsTrue(result);
            AreEqual(2, resultPath.Items.Count());
            AreEqual(new[] { 1, 2 }, resultPath.Items.ToArray());
        }

        [Test]
        public void TryParse_returns_false_for_null_string_representation()
        {
            // ACT

            HierarchyPath<int> resultPath = null;
            bool result = HierarchyPath.TryParse<int>(path: null, hierarchyPath: out resultPath, separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            IsFalse(result);
            IsNull(resultPath);
        }

        #endregion TryParse

        #region Equals

        [Test]
        public void Pathes_are_equal_if_all_items_are_equal()
        {
            // ARRANGE

            var right = HierarchyPath.Create("a", "b", "c");
            var left = HierarchyPath.Create("a", "b", "c");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            IsTrue(result1);
            IsTrue(result2);
            AreEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void Pathes_arent_equal_if_item_number_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create("a", "b", "c");
            var left = HierarchyPath.Create("a", "b");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            IsFalse(result1);
            IsFalse(result2);
            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void Pathes_are_different_if_item_type_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create<short>(1, 2, 3);
            var left = HierarchyPath.Create<int>(1, 2, 3);

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            IsFalse(result1);
            IsFalse(result2);
            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void Pathes_are_different_if_item_casing_is_different()
        {
            // ARRANGE

            var right = HierarchyPath.Create("A");
            var left = HierarchyPath.Create("a");

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            IsFalse(result1);
            IsFalse(result2);
            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void Pathes_are_equal_if_equality_comparer_says_so()
        {
            // ARRANGE

            var right = HierarchyPath.Create("A");
            var left = HierarchyPath.Create("a");

            // ACT

            bool result1 = left.Equals(right, StringComparer.OrdinalIgnoreCase);
            bool result2 = right.Equals(left, StringComparer.OrdinalIgnoreCase);

            // ASSERT

            IsTrue(result1);
            IsTrue(result2);
            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void Path_is_equal_compered_with_itself()
        {
            // ARRANGE

            var left = HierarchyPath.Create("a", "b", "c");

            // ACT

            bool result = left.Equals(left);

            // ASSERT

            IsTrue(result);
        }

        #endregion Equals

        #region ToString

        [Test]
        public void Path_creates_string_representation_with_default_separator()
        {
            // ACT

            var result = HierarchyPath.Create("a", "b").ToString();

            // ASSERT

            AreEqual("a/b", result);
        }

        [Test]
        public void Path_creates_empty_string_represention_for_empty_path()
        {
            // ACT

            var result = HierarchyPath.Create(string.Empty).ToString();

            // ASSERT

            AreEqual(string.Empty, result);
        }

        #endregion ToString

        #region GetHashCode

        [Test]
        public void Path_hashcode_is_different_for_permutations()
        {
            // ARRANGE

            var left = HierarchyPath.Create("a", "b");
            var right = HierarchyPath.Create("b", "a");

            // ACT & ASSERT

            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        #endregion GetHashCode

        #region Join

        [Test]
        public void Join_empty_path_with_leaf_item()
        {
            // ARRAGE

            var root = HierarchyPath.Create<int>();

            // ACT

            var result = root.Join(2);

            // ASSERT

            AreEqual(0, root.Items.Count());
            AreEqual(1, result.Items.Count());
            AreEqual(2, result.Items.Single());
        }

        [Test]
        public void Join_path_with_leaf()
        {
            // ARRAGE

            var key = HierarchyPath.Create<int>().Join(2);

            // ACT

            var result = key.Join(3);

            // ASSERT

            AreEqual(1, key.Items.Count());
            AreEqual(2, result.Items.Count());
            AreEqual(2, result.Items.ElementAt(0));
            AreEqual(3, result.Items.ElementAt(1));
        }

        [Test]
        public void Join_path_with_leaf_path()
        {
            // ARRAGE

            var key = HierarchyPath.Create<int>(1, 2);
            var other = HierarchyPath.Create(3, 4);

            // ACT

            var result = key.Join(other);

            // ASSERT

            AreEqual(4, result.Items.Count());
            AreEqual(1, result.Items.ElementAt(0));
            AreEqual(2, result.Items.ElementAt(1));
            AreEqual(3, result.Items.ElementAt(2));
            AreEqual(4, result.Items.ElementAt(3));
        }

        #endregion Join

        #region RelativeToAncestor

        [Test]
        public void Calculate_relative_path_from_ancestor_to_descendant()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("a", "b");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(ancestorKey);

            // ASSERT

            AreEqual(1, result.Items.Count());
            AreEqual("b", result.Items.First());
        }

        [Test]
        public void Relative_path_to_itself_is_empty()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("a");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(ancestorKey);

            // ASSERT

            AreEqual(0, result.Items.Count());
        }

        [Test]
        public void Calculate_relative_path_to_different_branch_throws()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("b");

            // ACT

            var ex = Throws<InvalidOperationException>(() => descendantKey.RelativeToAncestor(ancestorKey));
            IsTrue(ex.Message.Contains("No common key parts"));
        }

        [Test]
        public void Relative_path_to_root_is_itself_path()
        {
            // ASSERT
            var descendantKey = HierarchyPath.Create("a");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(HierarchyPath.Create<string>());

            // ASSERT

            AreEqual(1, result.Items.Count());
            AreEqual(descendantKey, result);
            AreSame(descendantKey, result);
        }

        #endregion RelativeToAncestor

        #region IsDescendant

        [Test]
        public void Parent_path_is_ancestor_of_descendant_path()
        {
            // ARRANGE

            var parent = HierarchyPath.Create("a");
            var descendant = HierarchyPath.Create("a", "b");

            // ACT

            bool result1 = parent.IsAncestorOf(descendant: descendant);
            bool result2 = descendant.IsAncestorOf(descendant: parent);

            // ASSERT

            IsTrue(result1);
            IsFalse(result2);
        }

        [Test]
        public void Sibling_isnt_descendant()
        {
            // ARRANGE

            var sibling1 = HierarchyPath.Create("a");
            var sibling2 = HierarchyPath.Create("b");

            // ACT

            bool result1 = sibling1.IsAncestorOf(sibling2);
            bool result2 = sibling2.IsAncestorOf(sibling1);

            // ASSERT

            IsFalse(result1);
            IsFalse(result2);
        }

        #endregion IsDescendant
    }
}