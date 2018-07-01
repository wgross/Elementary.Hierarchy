using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class ReflectionNodeTest
    {
        private class ReadWriteProperty
        {
            public string Property { get; set; }
        }

        #region Stop descending into object graph by property type

        [Fact]
        public void ReflectionNode_sees_byte_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (byte)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_char_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (char)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_short_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (short)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_ushort_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (ushort)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_int_property_as_leaf()
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
        public void ReflectionNode_sees_uint_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (uint)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_long_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (long)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_ulong_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (ulong)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_double_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (double)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_float_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (float)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_decimal_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (float)1 };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void ReflectionNode_sees_string_property_as_leaf()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        #endregion Stop descending into object graph by property type

        [Fact]
        public void ReflectionNode_provides_access_to_child_from_property_name()
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

        [Fact(Skip = "just an idea")]
        public void ReflectionNode_provides_access_to_child_by_PropertyExpression()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            // var (success, result) = hierarchyNode.TryGetChildNode(n => n.property);

            // ASSERT

            //Assert.True(success);
            //Assert.NotNull(result);
        }

        [Fact]
        public void Retrieve_the_leafs_inner_value()
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
        public void Retrieve_inner_value_of_inner_node_fails()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetValue<string>();

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
        public void Set_inner_value_of_inner_node_fails()
        {
            // ARRANGE

            var obj = new ReadWriteProperty { Property = "1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var success = hierarchyNode.TrySetValue("2");

            // ASSERT

            Assert.False(success);
        }
    }
}