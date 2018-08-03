using System;
using System.Collections.Generic;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyGetValueTest
    {
        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_get_value_fails_for_missing_node(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy[HierarchyPath.Create("a")]);

            // ASSERT

            Assert.Equal("path 'a' doesn't exist or has no value", result.Message);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_get_value_fails_for_missing_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            // child node has value -> parent node exists

            hierarchy[HierarchyPath.Create("a", "b")] = "value";

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchy[HierarchyPath.Create("a")]);

            // ASSERT

            Assert.Equal("path 'a' doesn't exist or has no value", result.Message);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_tryget_value_fails_on_null_path(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = Assert.Throws<ArgumentNullException>(() => hierarchy.TryGetValue(null, out var value));

            // ASSERT

            Assert.Equal("path", result.ParamName);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_get_value_fails_on_null_path(IHierarchy<string, string> hierarchy)
        {
            // ACT

            var result = Assert.Throws<ArgumentNullException>(() => hierarchy[null]);

            // ASSERT

            Assert.Equal("path", result.ParamName);
        }

        [Theory, ClassData(typeof(InstancesOfAllHierarchyVariants))]
        public void IHierarchy_get_value_returns_existing_value(IHierarchy<string, string> hierarchy)
        {
            // ARRANGE
            // child node has value -> parent node exists

            hierarchy[HierarchyPath.Create("a")] = "value";

            // ACT

            var result = hierarchy[HierarchyPath.Create("a")];

            // ASSERT

            Assert.Equal("value", result);
        }
    }
}