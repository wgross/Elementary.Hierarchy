using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchySetValueTest
    {
        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_value_at_root_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";

            // ACT

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ASSERT
            // hierarchy contains the value
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Same(test, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_value_at_root_node_twice_overwrites_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test2 = "test2";

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ACT & ASSERT

            hierarchy[HierarchyPath.Create<string>()] = test2;

            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Same(test2, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_child_sets_value_at_child_node(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy[HierarchyPath.Create<string>()] = test;

            // ACT

            hierarchy[HierarchyPath.Create("a")] = test1;

            // ASSERT
            // hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
            Assert.Same(test, value);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.Same(test1, value);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_child_twice_throws_ArgumentException(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;

            // ACT

            hierarchy[HierarchyPath.Create("a")] = test1;

            // ASSERT
            // hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Same(test1, value2);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_value_at_child_sibling(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;

            // ACT

            hierarchy.Add(HierarchyPath.Create("b"), test2);

            // ASSERT
            // new hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Same(test1, value2);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("b"), out var value3));
            Assert.Same(test2, value3);
        }

        [Theory, ClassData(typeof(AllHierarchyVariantsWithoutDefaultValue))]
        public void IHierarchy_set_value_at_grandchild(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";
            string test3 = "test3";

            hierarchy[HierarchyPath.Create<string>()] = test;
            hierarchy[HierarchyPath.Create("a")] = test1;
            hierarchy[HierarchyPath.Create("b")] = test2;

            // ACT

            hierarchy[HierarchyPath.Create("a", "c")] = test3;

            // ASSERT
            // hierarchy contains the root date and the new node.
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value1));
            Assert.Same(test, value1);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value2));
            Assert.Same(test1, value2);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("b"), out var value3));
            Assert.Same(test2, value3);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a", "c"), out var value4));
            Assert.Same(test3, value4);
        }
    }
}