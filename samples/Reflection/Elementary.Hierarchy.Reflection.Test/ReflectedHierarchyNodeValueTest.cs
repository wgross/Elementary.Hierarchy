using System.Collections.Generic;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class ReflectedHierarchyNodeTrySetValueTest
    {
        #region Set Value

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

        #endregion Set Value

        #region TryGet Value

        [Fact]
        public void Get_struct_value_from_root()

        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create(1);

            // ACT

            var (success, value) = hierarchyNode.TryGetValue<int>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(1, value);
        }

        [Fact]
        public void Get_ref_value_from_root()

        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create("1");

            // ACT

            var (success, value) = hierarchyNode.TryGetValue<string>();

            // ASSERT

            Assert.True(success);
            Assert.Equal("1", value);
        }

        [Fact]
        public void Get_array_value_from_root()

        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create(new[] { 1, 2 });

            // ACT

            var (success, value) = hierarchyNode.TryGetValue<int[]>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(new[] { 1, 2 }, value);
        }

        [Fact]
        public void Get_ref_value_from_property_node()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<string>();

            // ASSERT

            Assert.True(success);
            Assert.Equal("1", value);
        }

        [Fact]
        public void Get_struct_value_from_property_node()
        {
            // ARRANGE

            var obj = new { property = 1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<int>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(1, value);
        }

        [Fact]
        public void Get_array_value_from_array_node()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<int[]>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(new[] { 1, 2 }, value);
        }

        [Fact]
        public void Get_list_value_from_enumerable_node()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<List<int>>();

            // ASSERT

            Assert.True(success);
            Assert.Same(obj.property, value);
        }

        [Fact]
        public void Get_array_item_value_from_array_node()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetChildNode("1").Item2.TryGetValue<int>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(2, value);
        }

        [Fact]
        public void Get_enumerable_item_value_from_enumerable_node()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetChildNode("1").Item2.TryGetValue<int>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(2, value);
        }

        [Fact]
        public void Get_struct_value_as_object_from_property_node()
        {
            // ARRANGE

            var obj = new { property = 1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<object>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(1, value);
        }

        [Fact]
        public void Get_ref_value_as_object_from_property_node()
        {
            // ARRANGE

            var obj = new { property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<object>();

            // ASSERT

            Assert.True(success);
            Assert.Equal("1", value);
        }

        [Fact]
        public void Get_array_value_as_object_from_array_node()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<object>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(new[] { 1, 2 }, value);
        }

        [Fact]
        public void Get_array_item_value_as_object_from_array_node()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetChildNode("1").Item2.TryGetValue<object>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(2, value);
        }

        [Fact]
        public void Get_value_from_property_node_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new { property = 1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<string>();

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Get_value_from_root_fails_on_wrong_type()
        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create(1);

            // ACT

            var (success, value) = hierarchyNode.TryGetValue<string>();

            // ASSERT

            Assert.False(success);
        }

        #endregion TryGet Value
    }
}