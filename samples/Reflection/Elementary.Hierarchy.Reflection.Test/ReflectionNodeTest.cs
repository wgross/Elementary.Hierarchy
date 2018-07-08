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

        #region Try get nodes values

        [Fact]
        public void Create_root_from_scalar_value_type()

        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create(1);

            // ACT

            var (success, value) = hierarchyNode.TryGetValue<int>();

            // ASSERT

            Assert.True(success);
            Assert.Equal(1, value);
            Assert.False(hierarchyNode.HasChildNodes);
        }

        [Fact]
        public void ReflectionNode_sees_int_property_as_node() 
        {
            // ARRANGE

            var obj = new { property = (int)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_string_property_as_node()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
            Assert.Equal("1", result.Single().TryGetValue<string>().Item2);
        }

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
        public void Retrieve_property_as_child_fails_on_wrong_name()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, _) = hierarchyNode.TryGetChildNode("wrong");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Retrieve_property_value_from_child()
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
        public void Retrieve_property_value_from_child_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetValue<int>();

            // ASSERT

            Assert.False(success);
        }

        #endregion Try get nodes values

        #region Try set node values

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