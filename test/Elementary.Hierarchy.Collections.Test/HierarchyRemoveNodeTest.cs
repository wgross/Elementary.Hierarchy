using Elementary.Hierarchy.Collections.LiteDb;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyRemoveNodeTest
    {
        #region DataSources

        public static IEnumerable<object[]> DontRemoveNodeWithChildNodes
        {
            get
            {
                // mutable hierarchies
                yield return new object[]
                {
                    "",                                         // node wichih will be removed
                    "a",                                        // child node which make removal fails
                    new MutableHierarchy<string, string>()    // hierarchy instance to test at
                };
                yield return new object[] { "a", "a/b", new MutableHierarchy<string, string>() };
                yield return new object[] { "a", "a/b/c", new MutableHierarchy<string, string>() };
                // immutable hierarchies
                yield return new object[] { "", "a", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", "a/b", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", "a/b/c", new ImmutableHierarchy<string, string>() };
                // liteDB
                yield return new object[] { "", "a", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a", "a/b", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a", "a/b/c", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
            }
        }

        public static IEnumerable<object[]> RemoveNodeWithoutChildNodes
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "a", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", false, new MutableHierarchy<string, string>() };
                // immutable hierarchies
                yield return new object[] { "a", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", false, new ImmutableHierarchy<string, string>() };
                // liteDb
                yield return new object[] { "a", true, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a", false, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a/b", true, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a/b", false, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
            }
        }

        public static IEnumerable<object[]> RemoveNodeRecursively
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", new MutableHierarchy<string, string>() };
                yield return new object[] { "a", new MutableHierarchy<string, string>() };
                yield return new object[] { "a/b", new MutableHierarchy<string, string>() };
                // immutable hierarchies
                yield return new object[] { "", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a/b", new ImmutableHierarchy<string, string>() };
                // liteDb
                yield return new object[] { "", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a/b", new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
            }
        }

        public static IEnumerable<object[]> RemoveInnerNodeTwice
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "a", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new MutableHierarchy<string, string>() };
                // immutable hierarchies
                yield return new object[] { "a", true, new ImmutableHierarchy<string, string>() };
                yield return new object[] { "a", false, new ImmutableHierarchy<string, string>() };
                // liteDb
                yield return new object[] { "a", true, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
                yield return new object[] { "a", false, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
            }
        }

        public static IEnumerable<object[]> RemoveRootNodeTwice
        {
            get
            {
                // mutable hierarchies
                yield return new object[] { "", true, new MutableHierarchy<string, string>() };
                yield return new object[] { "", false, new MutableHierarchy<string, string>() };
                yield return new object[] { "", false, new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
            }
        }

        #endregion DataSources

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_removes_root_by_removing_its_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create<string>(), "test");

            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create<string>(), recurse: false);

            // ASSERT

            Assert.True(result);

            // value is removed
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
        }

        [Theory, MemberData(nameof(DontRemoveNodeWithChildNodes))]
        public void IHierarchy_RemoveNode_non_recursive_fails_if_childnode_is_present(string nodePath, string subNodePath, IHierarchy<string, string> hierarchy)
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
            Assert.Equal(test, value);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Parse(subNodePath, "/"), out value));
            Assert.Equal(test1, value);
        }

        [Theory, MemberData(nameof(RemoveNodeWithoutChildNodes))]
        public void IHierarchy_RemoveNode_removes_leaf_from_hierarchy(string pathToDelete, bool recurse, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            var path = HierarchyPath.Parse(pathToDelete, "/");

            hierarchy.Add(path, pathToDelete);

            // ACT
            // the value of recurse doen't make difference

            var result = hierarchy.RemoveNode(path, recurse: recurse);

            // ASSERT
            // result must be true always

            Assert.True(result);

            // node has no value
            Assert.False(hierarchy.TryGetValue(path, out var _));

            // nodes are no longer present
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(path));
        }

        [Theory, MemberData(nameof(RemoveNodeRecursively))]
        public void IHierarchy_RemoveNode_removes_inner_node_from_hierarchy_and_descendants(string nodeToDelete, IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            // node
            //    \
            //     subNode

            var nodePath = HierarchyPath.Parse(nodeToDelete, "/");
            hierarchy.Add(nodePath, nodePath.ToString());

            // add subnode with value
            var subNode = nodePath.Join("subNode");
            hierarchy.Add(subNode, subNode.ToString());

            // ACT
            // remove node and subNode

            var result = hierarchy.RemoveNode(nodePath, recurse: true);

            // ASSERT
            // node could be removed

            Assert.True(result);

            // values of node and subNode can't be read anymore
            Assert.False(hierarchy.TryGetValue(nodePath, out var valueNode));
            Assert.False(hierarchy.TryGetValue(subNode, out var valueSubNode));

            // nodes are no longer present
            if (!nodePath.IsRoot) Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(nodePath));
            Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(subNode));
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

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_RemoveNode_unknown_node_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.RemoveNode(HierarchyPath.Create("a"), recurse: false);

            // ASSERT

            Assert.False(result);
        }
    }
}