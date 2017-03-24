using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyTraversalTest
    {
        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_no_value_on_Traverse(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            Assert.False(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantWithDefaultValue))]
        public void IHierarchy_root_node_has_default_value_on_traversal(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            Assert.True(hierarchy.Traverse(HierarchyPath.Create<string>()).HasValue);
            Assert.Equal(AllHierarchyVariantWithDefaultValue.DefaultValue, hierarchy.Traverse(HierarchyPath.Create<string>()).Value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_node_knows_its_path(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>());

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(HierarchyPath.Create<string>(), result.Path);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_hierarchy_has_no_children_if_root_has_no_children(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).HasChildNodes;

            // ASSERT

            Assert.False(result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_get_a_nodes_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create<string>(), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).Value;

            // ASSERT

            Assert.Equal("v1", result);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_Get_children_of_root_node(IHierarchy<string, string> hierarchy)
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
        public void IHierarchy_child_node_knows_its_path(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            // ACT

            var result = hierarchy.Traverse(HierarchyPath.Create<string>()).ChildNodes.Single().Path;

            // ASSERT

            Assert.Equal(HierarchyPath.Create("a"), result);
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
        public void IHierarchy_children_of_root_have_root_as_parent(IHierarchy<string, string> hierarchy)
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
        public void IHierarchy_Start_traversal_at_child_of_root(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create("a"), "v1");

            var node_a = hierarchy.Traverse(HierarchyPath.Create<string>()).Children().First();

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
            Assert.Equal(new[] {
                    HierarchyPath.Create("a","b"),
                    HierarchyPath.Create("a"),
                    HierarchyPath.Create<string>(),
                },
                result.Ancestors().Select(n => n.Path));
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