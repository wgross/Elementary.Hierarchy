using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyRemoveValueTest
    {
        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_removes_value_from_root(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(result);

            string value;

            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.Equal(test1, value);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_remove_value_twice_from_root_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Remove(HierarchyPath.Create<string>());

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.False(result);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_remove_value_from_child_returns_true(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.True(result);

            string value;

            // new node has no value
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out value));
            Assert.Equal(test, value);
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create("a"), out value));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.Equal(test2, value);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_removes_false_if_no_value_was_removed(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE

            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT

            Assert.False(result);

            string value;

            // new node has no value
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a", "b"), out value));
            Assert.Equal(test2, value);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_removes_value_from_child_twice_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            string test = "test";
            string test1 = "test1";
            string test2 = "test2";

            hierarchy.Add(HierarchyPath.Create<string>(), test);
            hierarchy.Add(HierarchyPath.Create("a"), test1);
            hierarchy.Add(HierarchyPath.Create("a", "b"), test2);
            hierarchy.Remove(HierarchyPath.Create("a"));

            // ACT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.False(result);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_removes_value_from_unknown_node_returns_false(IHierarchy<string, string> hierarchy)
        {
            // ACT & ASSERT

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT

            Assert.False(result);
        }
    }
}