using System;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyAddValueTest
    {
        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_root_node_has_no_value_on_TryGetValue(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
        }

        [Theory, ClassData(typeof(AllHierarchyVariantWithDefaultValue))]
        public void IHierarchy_root_node_has_default_value_on_TryGetValue(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Equal(AllHierarchyVariantWithDefaultValue.DefaultValue, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_value_to_root_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ASSERT
            // new hierarchy contains all values

            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Same(test, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_same_value_to_root_twice_throws_ArgumentException(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create<string>(), test1));

            // ASSERT

            Assert.True(result.Message.Contains("already has a value"));
            Assert.True(result.Message.Contains("''"));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Same(test, value);
            Assert.Equal("path", result.ParamName);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_child_sets_value_at_child_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);

            // ACT

            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ASSERT
            // new hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Equal(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Equal(test1, value2);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_child_value_twice_throws_ArgumentException(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create("a"), test2));

            // ASSERT

            Assert.True(result.Message.Contains("already has a value"));
            Assert.True(result.Message.Contains("'a'"));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value));
            Assert.Same(test1, value);
            Assert.Equal("path", result.ParamName);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_child_sibling(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ASSERT
            // new hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Equal(test1, value2);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("b"), out var value3));
            Assert.Equal(test2, value3);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_child_sibling_value_twice_throws_ArgumentException(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentException>(() => hierarchy.Add(HierarchyPath.Create("b"), test3));

            // ASSERT

            Assert.True(result.Message.Contains("already has a value"));
            Assert.True(result.Message.Contains("'b'"));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("b"), out var value));
            Assert.Equal(test2, value);
            Assert.Equal("path", result.ParamName);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_adds_grandchild_under_existing_nodes_returns_hierachy_with_same_values(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ACT

            hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT
            // new hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Equal(test1, value2);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("b"), out var value3));
            Assert.Equal(test2, value3);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out var value4));
            Assert.Equal(test3, value4);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantWithDefaultValue))]
        public void IHierarchy_adds_grandchild_as_first_node_returns_new_hierachy_with_same_values(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test3 = "test3";

            // ACT

            hierarchy.Add(HierarchyPath.Create("a", "c"), test3);

            // ASSERT
            // new hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(AllHierarchyVariantWithDefaultValue.DefaultValue, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Same(AllHierarchyVariantWithDefaultValue.DefaultValue, value2);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out var value3));
            Assert.Same(test3, value3);
        }
    }
}