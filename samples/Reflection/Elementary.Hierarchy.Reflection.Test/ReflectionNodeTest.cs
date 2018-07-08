using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class ReflectionNodeTest
    {
        private class ReadWritePropertyParent
        {
            public ReadWriteProperty ReadWriteProperty { get; set; }
        }

        private class ReadWriteProperty
        {
            public string Property { get; set; }
        }

        #region Map objects and properties to hierarchy nodes

        [Fact]
        public void Create_root_from_scalar_value_type()

        {
            // ACT

            var hierarchyNode = ReflectedHierarchy.Create(1);

            // ASSERT

            Assert.False(hierarchyNode.HasChildNodes);
        }

        [Fact]
        public void Create_node_from_scalar_value_type_property()
        {
            // ARRANGE

            var obj = new { property = (int)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
            Assert.False(result.Single().HasChildNodes);
        }

        [Fact]
        public void Create_node_from_scalar_ref_type_property()
        {
            // ARRANGE

            var obj = new { property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT
            // string has additionalproperties which qualify sub nodes.

            Assert.Single(result);
            Assert.True(result.Single().HasChildNodes);
        }

        #endregion Map objects and properties to hierarchy nodes

        #region TryGet node by name

        [Fact]
        public void Retrieve_property_as_child()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("property");

            // ASSERT

            Assert.True(success);
            Assert.NotNull(result);
        }

        [Fact]
        public void Retrieve_property_as_child_fails_on_unkown_name()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("wrong");

            // ASSERT

            Assert.False(success);
        }

        #endregion TryGet node by name

        #region TryGet value from node

        [Fact]
        public void Get_value_from_root_as_scalar_value_type()

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
        public void Get_value_from_root_as_scalar_ref_type()

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
        public void Get_value_from_root_as_scalar_array_type()

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
        public void Retrieve_property_ref_value_from_child()
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
        public void Retrieve_property_struct_value_from_child()
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
        public void Retrieve_property_array_value_from_child()
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
        public void Retrieve_property_value_type_as_object_from_child()
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
        public void Retrieve_property_ref_type_as_object_from_child()
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
        public void Retrieve_property_array_type_as_object_from_child()
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
        public void Retrieve_property_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new { property = 1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, value) = hierarchyNode.TryGetChildNode("property").Item2.TryGetValue<string>();

            // ASSERT

            Assert.False(success);
        }

        #endregion TryGet value from node

        #region Try Set node values

        [Fact]
        public void Set_root_value_fails()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent { ReadWriteProperty = new ReadWriteProperty { Property = "1" } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.TrySetValue(new ReadWritePropertyParent());

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Set_node_value()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent { ReadWriteProperty = new ReadWriteProperty { Property = "1" } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("ReadWriteProperty")).TrySetValue(new ReadWriteProperty { Property = "2" });

            // ASSERT

            Assert.True(success);
            Assert.Equal("2", obj.ReadWriteProperty.Property);
        }

        [Fact]
        public void Set_node_value_fails_on_wrong_name()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent { ReadWriteProperty = new ReadWriteProperty { Property = "1" } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = Assert.Throws<KeyNotFoundException>(() => hierarchyNode.DescendantAt(HierarchyPath.Create("Wrong")).TrySetValue(new ReadWriteProperty { Property = "2" }));

            // ASSERT

            Assert.Equal("Key not found:'Wrong'", result.Message);
        }

        [Fact]
        public void Set_node_value_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new ReadWritePropertyParent { ReadWriteProperty = new ReadWriteProperty { Property = "1" } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("ReadWriteProperty")).TrySetValue("2");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Set_node_value_fails_on_read_only_property()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.DescendantAt(HierarchyPath.Create("property")).TrySetValue("2");

            // ASSERT

            Assert.False(success);
        }

        #endregion Try set node values

        [Fact]
        public void Indexed_properties_are_skipped()
        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create("test");

            // ACT

            var result = hierarchyNode.Descendants().ToArray();

            // ASSERT
            // 'Chars' property is skipped

            Assert.Empty(result);
        }

        [Fact]
        public void Array_properties_are_converted_to_nodes()
        {
            // ARRANGE

            var h = new
            {
                data = new[] { 1, 2 }
            };

            //Array value = (Array)h.GetType().GetProperty("data").GetValue(h);
            //var ints = value.OfType<int>();
            //var ints2 = (int[])value;
            var hierarchyNode = ReflectedHierarchy.Create(h);

            // ACT

            var result = hierarchyNode.Descendants().ToArray();

            // ASSERT
            // 'Chars' property is skipped

            Assert.Single(result);
            Assert.Equal("data", result.Single().Id);
            Assert.Equal(new[] { 1, 2 }, result.Single().TryGetValue<int[]>().Item2);
        }
    }
}