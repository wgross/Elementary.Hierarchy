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
        public void CreateEmptyPath()
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
        public void CreateNewHierarchyPathFromString()
        {
            // ACT

            var result = HierarchyPath.Create("a");

            // ASSERT

            IsNotNull(result);
            IsTrue(result.Items.Any());
            AreEqual(new[] { "a" }, result.Items.ToArray());
        }

        [Test]
        public void CreateNewHierarchyPathFromStringArray()
        {
            // ACT

            var result = HierarchyPath.Create(new[] { "a", "B" });

            // ASSERT

            IsNotNull(result);
            IsNotNull(result);
            AreEqual(new[] { "a", "B" }, result.Items.ToArray());
        }

        [Test]
        public void ChangeArrayAfterCreateDoesntBreakThePath()
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
        public void HierarchyPathRemainsSameIfEnumerableSourceIsChanged()
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
        public void SplitParentPathItems()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.Parent();

            // ASSERT

            AreEqual(new[] { "1", "2" }, result.Items);
        }

        [Test]
        public void ParentPathItemsFromEmptyPath()
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
        public void SplitLeafPathItem()
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
        public void SplitDescendantsUnderRootItem()
        {
            // ARRANGE

            var path = HierarchyPath.Create("1", "2", "3");

            // ACT

            var result = path.SplitDescendants();

            // ASSERT

            AreEqual(new[] { "2", "3" }, result.Items);
        }

        [Test]
        public void SplittingOfRootIsRoot()
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
        public void ParseHierarchyPathFromStringWithSeperator()
        {
            // ACT

            HierarchyPath<string> result = HierarchyPath.Parse(path: "/test/test2", separator: "/");

            // ASSERT

            AreEqual(2, result.Items.Count());
            AreEqual(new[] { "test", "test2" }, result.Items.ToArray());
        }

        [Test]
        public void ParseIntegerHierarchyPathFromStringWithSeperator()
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
        public void TryParseHierarchyPathFromStringWithSeperator()
        {
            // ACT
            HierarchyPath<string> resultPath = null;
            bool result = HierarchyPath.TryParse(path: "/test/test2", hierarchyPath: out resultPath, convertPathItem:i=>i, separator: "/");

            // ASSERT

            IsTrue(result);
            AreEqual(2, resultPath.Items.Count());
            AreEqual(new[] { "test", "test2" }, resultPath.Items.ToArray());
        }

        [Test]
        public void TryParseIntegerHierarchyPathFromStringWithSeperator()
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
        public void TryParseIntegerHierarchyPathFromStringWithSeperatorFails()
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
        public void HierarchyPathesAreEqualIfAllPartsAreEqual()
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
        public void HierarchyPathesAreNotEqualIfLengthIsNotEqual()
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
        public void HierarchyPathesAreNotEqualIfItemTypeIsDifferent()
        {
            // ARRANGE

            var right = HierarchyPath.Create("a", "b", "c");
            var left = HierarchyPath.Create(1, 2, 3);

            // ACT

            bool result1 = left.Equals(right);
            bool result2 = right.Equals(left);

            // ASSERT

            IsFalse(result1);
            IsFalse(result2);
            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void StringHierarchyPathesAreNotEqualIfCasingIsDifferent()
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
        public void HierarchyPathesAreEqualIfEualityComparerSaysSo()
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
        public void HierarchyPathesAreEqualForSameInstance()
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
        public void HierarchyPathReturnsStringRepresentationOfPath()
        {
            // ACT

            var result = HierarchyPath.Create("a", "b").ToString();

            // ASSERT

            AreEqual("a/b", result);
        }

        [Test]
        public void HierarchyPathReturnsStringRepresentationFromRootPath()
        {
            // ACT

            var result = HierarchyPath.Create(string.Empty).ToString();

            // ASSERT

            AreEqual(string.Empty, result);
        }

        #endregion ToString

        #region GetHashCode

        [Test]
        public void HashcodesAreDifferentForPermutations()
        {
            // ARRANGE

            var left = HierarchyPath.Create("a", "b");
            var right = HierarchyPath.Create("b", "a");

            // ACT & ASSERT

            AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Test]
        public void EqualKeysHaveSameHashCode()
        {
            // ARRAMGE

            var key1 = HierarchyPath.Create("a", "b");
            var key2 = HierarchyPath.Create("a", "b");

            // ACT

            int hash1 = key1.GetHashCode();
            int hash2 = key2.GetHashCode();

            // ASSERT

            AreEqual(hash1, hash2);
            AreNotSame(key1, key2);
            AreEqual(key1, key2);
        }

        #endregion GetHashCode

        #region Join

        [Test]
        public void JoinRootHierarchyPathWithChildPart()
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
        public void JoinHierarchyPathWithChildPart()
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
        public void JoinHierarchyPathWithOtherPath()
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
        public void CalculateRelativePathToAncestor()
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
        public void CalculateRelativePathToSelf()
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
        public void CalculateRelativePathToDifferentBranch()
        {
            // ARRANGE

            var ancestorKey = HierarchyPath.Create("a");
            var descendantKey = HierarchyPath.Create("b");

            // ACT

            var ex = Throws<InvalidOperationException>(() => descendantKey.RelativeToAncestor(ancestorKey));
            IsTrue(ex.Message.Contains("No common key parts"));
        }

        [Test]
        public void CalculateRelativePathToRoot()
        {
            // ASSERT
            var descendantKey = HierarchyPath.Create("a");

            // ACT

            HierarchyPath<string> result = descendantKey.RelativeToAncestor(HierarchyPath.Create<string>());

            // ASSERT

            AreEqual(1, result.Items.Count());
            AreEqual(descendantKey, result);
        }

        #endregion RelativeToAncestor

        #region IsDescendant

        [Test]
        public void ChildKeyIsDescandantKeyOfParent()
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
        public void SiblingsAreNoDescendants()
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