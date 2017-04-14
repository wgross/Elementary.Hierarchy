using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyTraversalTest
    {
        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_no_parent_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var traverser = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ACT & ASSERT

            Assert.False(traverser.HasParentNode);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_no_value_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var traverser = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ACT & ASSERT

            Assert.False(traverser.TryGetValue(out var value));
        }

        [Theory, ClassData(typeof(AllHierarchyVariantWithDefaultValue))]
        public void IHierarchy_root_node_has_default_value_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var traverser = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(traverser.TryGetValue(out var value));
            Assert.Equal(AllHierarchyVariantWithDefaultValue.DefaultValue, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_empty_path_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal(HierarchyPath.Create<string>(), result.Path);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_no_children_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasChildNodes;

            // ASSERT

            Assert.False(result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_get_root_nodes_value_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create<string>(), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).TryGetValue(out var result_value);

            // ASSERT

            Assert.Equal("v1", result_value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_get_children_of_root_node_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("b"), "v2");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.ToArray();

            // ASSERT

            Assert.Equal(2, result.Length);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_child_node_knows_its_path_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.Single().Path;

            // ASSERT

            Assert.Equal(HierarchyPath.Create("a"), result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_grandchild_node_knows_its_path_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a", "b"), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().Single().Children().Single().Path;

            // ASSERT

            Assert.Equal(HierarchyPath.Create("a", "b"), result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_has_no_parent(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasParentNode;

            // ASSERT

            Assert.False(result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_child_of_root_has_root_as_parent_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var root = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().Single().Parent();

            // ASSERT

            Assert.Equal(root, result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_start_at_child_of_root_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().Single();

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create("a"));

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(HierarchyPath.Create("a"), result.Path);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_start_at_inner_node_stil_allows_to_ascend(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");
            hierarchy.Add(HierarchyPath.Create("a", "b", "c"), "v2");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create("a", "b", "c"));

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(new[]
            {
                HierarchyPath.Create("a", "b", "c"),
                HierarchyPath.Create("a","b"),
                HierarchyPath.Create("a"),
                HierarchyPath.Create<string>(),
            },
            result.AncestorsAndSelf().Select(n => n.Path).ToArray());
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_throw_if_start_path_doesnt_exist(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy.Traverse(HierarchyPath.Create("b")));

            // ASSERT

            Assert.True(result.Message.Contains("'b'"));
        }
    }
}