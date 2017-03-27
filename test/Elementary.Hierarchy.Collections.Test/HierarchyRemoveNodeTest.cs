using System.Collections.Generic;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyRemoveNodeTest
    {
        #region DataSources

        public static IEnumerable<object[]> One
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", "a", new MutableHierarchy<string, string>() }; // root with direct subnode
                yield return new object[] { "a", "a/b", new MutableHierarchy<string, string>() }; // sub node with direct subnode
                yield return new object[] { "a", "a/b/c", new MutableHierarchy<string, string>() }; // subnode with indirect subnode
                // mutable hierarchies
                yield return new object[] { "", "a", new MutableHierarchyEx<string, string>() }; // root with direct subnode
                yield return new object[] { "a", "a/b", new MutableHierarchyEx<string, string>() }; // sub node with direct subnode
                yield return new object[] { "a", "a/b/c", new MutableHierarchyEx<string, string>() }; // subnode with indirect subnode
                // immutable hierarchies
                yield return new object[] { "", "a", new ImmutableHierarchy<string, string>() }; // root with direct subnode
                yield return new object[] { "a", "a/b", new ImmutableHierarchy<string, string>() }; // root with direct subnode
                yield return new object[] { "a", "a/b/c", new ImmutableHierarchy<string, string>() }; // root with direct subnode
            }
        }

        public static IEnumerable<object[]> Two
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "a", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", false, new MutableHierarchy<string, string>() };
                // mutable hierarchies
                yield return new object[] { "a", true, new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a/b", true, new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a/b", false, new MutableHierarchyEx<string, string>() };
                // immutable hierarchies
                yield return new object[] { "a", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", false, new ImmutableHierarchy<string, string>() };
            }
        }

        public static IEnumerable<object[]> Three
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", new MutableHierarchy<string, string>() };
                yield return new object[] { "a", new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", new MutableHierarchy<string, string>() };
                // mutable hierarchies
                yield return new object[] { "", new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a", new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a/b", new MutableHierarchyEx<string, string>() };
                // immutable hierarchies
                yield return new object[] { "", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", new ImmutableHierarchy<string, string>() };
            }
        }

        public static IEnumerable<object[]> RemoveInnerNodeTwice
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "", false, new MutableHierarchy<string, string>() };
                yield return new object[] { "a", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchy<string, string>() };
                // mutable hierarchies
                yield return new object[] { "a", true, new MutableHierarchyEx<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchyEx<string, string>() };
                // immutable hierarchies
                yield return new object[] { "", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "", false, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new ImmutableHierarchy<string, string>() };
            }
        }

        public static IEnumerable<object[]> RemoveRootNodeTwice
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", true, new MutableHierarchyEx<string, string>() };
                yield return new object[] { "", false, new MutableHierarchyEx<string, string>() };
            }
        }

        public static IEnumerable<object[]> EmptyRootNodes
        {
            get
            {
                yield return new object[] { new MutableHierarchy<string, string>() };
                yield return new object[] { new MutableHierarchyEx<string, string>() };
                yield return new object[] { new ImmutableHierarchy<string, string>() };
            }
        }

        #endregion DataSources

        [Theory, MemberData(nameof(EmptyRootNodes))]
        public void IHierarchy_RemoveNode_root_removes_value_but_not_the_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create<string>(), false);

            // ASSERT

            Assert.True(result);

            // value is removed
            string value;
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));

            // node is still there
            Assert.NotNull(hierarchy.Traverse(HierarchyPath.Create<string>()));
            Assert.False(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
        }

        [Theory, MemberData(nameof(One))]
        public void IHierarchy_RemoveNode_non_recursive_fails_if_a_childnode_is_present(string nodePath, string subNodePath, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Parse(nodePath, "/"), test);
            hierarchy.Add(HierarchyPath.Parse(subNodePath, "/"), test1);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(nodePath, "/"), recurse: false);

            // ASSERT

            Assert.False(result);

            string value;

            // node has no value
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Parse(nodePath, "/"), out value));
            Assert.Same(test, value);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Parse(subNodePath, "/"), out value));
            Assert.Same(test1, value);
        }

        [Theory, MemberData(nameof(Two))]
        public void IHierarchy_RemoveNode_removes_leaf_from_hierarchy_completely(string pathToDelete, bool recurse, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            var node = HierarchyPath.Parse(pathToDelete, "/");

            hierarchy.Add(node, pathToDelete);

            // ACT

            var result = hierarchy.RemoveNode(node, recurse: recurse);

            // ASSERT

            Assert.True(result);

            // node has no value
            string value;
            Assert.False(hierarchy.TryGetValue(node, out value));

            // nodes are no longer present
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(node));
        }

        [Theory, MemberData(nameof(Three))]
        public void IHierarchy_RemoveNode_removes_inner_node_from_hierarchy_completely_and_all_descendants(string nodeToDelete, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            var node = HierarchyPath.Parse(nodeToDelete, "/");
            hierarchy.Add(node, node.ToString());

            // add subnode with value
            var subNode1 = node.Join("subNode");
            hierarchy.Add(subNode1, subNode1.ToString());

            // ACT

            var result = hierarchy.RemoveNode(node, recurse: true);

            // ASSERT

            Assert.True(result);

            // new node has no value
            string value;
            Assert.False(hierarchy.TryGetValue(node, out value));
            Assert.False(hierarchy.TryGetValue(subNode1, out value));

            // nodes are no longer present
            if (!node.IsRoot) Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(node));
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(subNode1));
        }

        [Theory, MemberData(nameof(RemoveInnerNodeTwice))]
        public void IHierarchy_RemoveNode_twice_returns_false(string path, bool recurse, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";

            hierarchy.Add(HierarchyPath.Parse(path, "/"), test);
            hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ASSERT

            Assert.False(result);
        }

        [Theory, MemberData(nameof(RemoveRootNodeTwice))]
        public void IHierarchy_RemoveNode_twice_returns_true_for_root_node(string path, bool recurse, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";

            hierarchy.Add(HierarchyPath.Parse(path, "/"), test);
            hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Parse(path, "/"), recurse: recurse);

            // ASSERT

            Assert.True(result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_RemoveNode_unknown_node_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create("a"), recurse: false);

            // ASSERT

            Assert.False(result);
        }
    }
}