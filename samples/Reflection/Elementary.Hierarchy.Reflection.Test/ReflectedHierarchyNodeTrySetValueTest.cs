using System.Collections.Generic;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class ReflectedHierarchyNodeTrySetValueTest
    {
        private class ReadWritePropertyParent<T>
        {
            public T Property { get; set; }
        }

        [Fact]
        public void Set_root_value_fails()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<string> { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.TrySetValue(new ReadWritePropertyParent<string> { Property = "2" });

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Set_value_at_property_node()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<string> { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue("2");

            // ASSERT

            Assert.True(success);
            Assert.Equal("2", obj.Property);
        }

        [Fact]
        public void Set_value_at_property_node_delegate()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<string> { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue((string old) => "2");

            // ASSERT

            Assert.True(success);
            Assert.Equal("2", obj.Property);
        }

        [Fact]
        public void Set_array_value_at_array_node()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<int[]> { Property = new[] { 1 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue(new[] { 2 });

            // ASSERT

            Assert.True(success);
            Assert.Equal(new[] { 2 }, obj.Property);
        }

        [Fact(Skip = "Requires array item node")]
        public void Set_array_item_value_at_array_node()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<int[]> { Property = new[] { 1 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property", "0")).TrySetValue(2);

            // ASSERT

            Assert.True(success);
            Assert.Equal(2, obj.Property[0]);
        }

        [Fact]
        public void Set_list_value_at_enumerable_node()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<List<int>> { Property = new List<int> { 1 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue(new List<int> { 2 });

            // ASSERT

            Assert.True(success);
            Assert.Equal(new[] { 2 }, obj.Property);
        }

        [Fact]
        public void Set_node_value_fails_on_wrong_name()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<string> { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchyNode.DescendantAt(HierarchyPath.Create("Wrong")).TrySetValue("2"));

            // ASSERT

            Assert.Equal("Key not found:'Wrong'", result.Message);
        }

        [Fact]
        public void Set_node_value_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<string> { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue(2);

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Set_array_value_at_property_node_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent<int[]> { Property = new[] { 1 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("Property")).TrySetValue(new double[] { 2.0 });

            // ASSERT

            Assert.False(success);
            Assert.Equal(new[] { 1 }, obj.Property);
        }

        [Fact]
        public void Set_node_value_fails_on_readonly_property()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("property")).TrySetValue("2");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Set_array_value_at_property_node_fails_on_readonly_property()
        {
            // ARRANGE

            var obj = new { property = new[] { 1 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("property")).TrySetValue(new int[] { 2 });

            // ASSERT

            Assert.False(success);
            Assert.Equal(new[] { 1 }, obj.property);
        }
    }
}