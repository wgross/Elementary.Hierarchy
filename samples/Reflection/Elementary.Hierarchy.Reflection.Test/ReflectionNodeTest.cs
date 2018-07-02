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
        public void Retrieve_the_leafs_value()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);
            var (_, node) = hierarchyNode.TryGetChildNode("property");

            // ACT

            var (success, result) = node.TryGetValue<string>();

            // ASSERT

            Assert.True(success);
            Assert.Equal("1", result);
        }

        [Fact]
        public void Retrieve_nodes_value()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetValue<object>();

            // ASSERT

            Assert.True(success);
            Assert.Same(obj, result);
        }

        [Fact]
        public void Retrieve_nodes_value_fails_on_wrong_type()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetValue<int>();

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Retrieve_the_descandants_leafs_inner_value()
        {
            // ARRANGE

            var obj = new
            {
                child = new
                {
                    property = (string)"1"
                }
            };

            var hierarchyNode = ReflectedHierarchy.Create(obj);
            var node = hierarchyNode.DescendantAt(HierarchyPath.Create("child", "property"));

            // ACT

            var (success, result) = node.TryGetValue<string>();

            // ASSERT

            Assert.True(success);
            Assert.Equal("1", result);
        }

        [Fact]
        public void Set_inner_value_of_leaf_node()
        {
            // ARRANGE

            var obj = new ReadWriteProperty { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.TryGetChildNode("Property").Item2.TrySetValue("2");

            // ASSERT

            Assert.True(success);
            Assert.Equal("2", hierarchyNode.TryGetChildNode("Property").Item2.TryGetValue<string>().Item2);
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
    }
}