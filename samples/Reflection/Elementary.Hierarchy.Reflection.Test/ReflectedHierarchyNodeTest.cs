﻿using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class ReflectedHierarchyNodeTest
    {
        private class ReadWritePropertyParent<T>
        {
            public T Property { get; set; }
        }

        #region Map objects to hierarchy root nodes

        [Fact]
        public void Create_root_from_scalar_value_type()

        {
            // ACT

            var hierarchyNode = ReflectedHierarchy.Create(1);

            // ASSERT

            Assert.False(hierarchyNode.HasChildNodes);
            Assert.Empty(hierarchyNode.ChildNodes);
        }

        [Fact]
        public void Create_root_from_scalar_ref_type()

        {
            // ACT

            var hierarchyNode = ReflectedHierarchy.Create(new { });

            // ASSERT

            Assert.False(hierarchyNode.HasChildNodes);
            Assert.Empty(hierarchyNode.ChildNodes);
        }

        [Fact(Skip = "array root node required")]
        public void Create_root_from_empty_array_type()
        {
            // ACT

            var hierarchyNode = ReflectedHierarchy.Create(new int[0]);

            // ASSERT
            //the Length property is the single chidl of an array

            Assert.True(hierarchyNode.HasChildNodes);
            Assert.Empty(hierarchyNode.ChildNodes);
        }

        #endregion Map objects to hierarchy root nodes

        #region Node calls factory for child node creation

        [Fact]
        public void Create_property_child_node_with_factory()
        {
            // ARRANGE

            var nodeValue = new { a = 1 };
            var factory = new Mock<IReflectedHierarchyNodeFactory>(MockBehavior.Strict);
            factory
                .Setup(f => f.Create(nodeValue, It.Is<PropertyInfo>(pi => pi.Name.Equals("a"))))
                .Returns((IReflectedHierarchyNode)null);

            var hierarchyNode = ReflectedHierarchy.Create(nodeValue, factory.Object);

            // ACT

            var result = hierarchyNode.ChildNodes.ToArray();

            // ASSERT

            factory.VerifyAll();
        }

        #endregion Node calls factory for child node creation

        #region Map object properties to inner nodes

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
            Assert.Equal("property", result.Single().Id);
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
            // string has additional properties which qualify sub nodes.

            Assert.Single(result);
            Assert.True(result.Single().HasChildNodes);
            Assert.Equal("property", result.Single().Id);
        }

        [Fact]
        public void Create_node_from_scalar_array_type_property()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().ToArray();

            // ASSERT
            // An array has its items as child nodes

            Assert.Single(result);
            Assert.True(result.Single().HasChildNodes);
            Assert.Equal("property", result.Single().Id);
        }

        [Fact]
        public void Create_child_nodes_from_scalar_array_type_property()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().Single().Children().ToArray();

            // ASSERT
            // An array has its items as child nodes

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void Create_child_nodes_from_scalar_enumerable_type_property()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var result = hierarchyNode.Children().Single().Children().ToArray();

            // ASSERT
            // An array has its items as child nodes

            Assert.Equal(2, result.Length);
            Assert.Equal(new[] { "0", "1" }, result.Select(p => p.Id).ToArray());
        }

        [Fact]
        public void Skip_properties_with_indexer()
        {
            // ARRANGE

            var hierarchyNode = ReflectedHierarchy.Create("test");

            // ACT

            var result = hierarchyNode.ChildNodes.ToArray();

            // ASSERT
            // 'Chars' property is skipped, Length is contained

            Assert.Single(result);
            Assert.Equal("Length", result.Single().Id);
        }

        [Fact]
        public void Sort_property_names_ascending_at_object_node()
        {
            // ARRANGE

            var obj1 = new
            {
                b = "b",
                a = 1
            };

            var obj2 = new
            {
                a = 1,
                b = "b"
            };

            var hierarchyNode1 = ReflectedHierarchy.Create(obj1);
            var hierarchyNode2 = ReflectedHierarchy.Create(obj2);

            // ACT

            var result1 = hierarchyNode1.Children().ToArray();
            var result2 = hierarchyNode2.Children().ToArray();

            // ASSERT

            Assert.Equal(new[] { "a", "b" }, result1.Select(n => n.Id));
            Assert.Equal(new[] { "a", "b" }, result2.Select(n => n.Id));
        }

        [Fact]
        public void Sort_property_names_ascending_at_property_node()
        {
            // ARRANGE

            var obj1 = new
            {
                property = new
                {
                    b = "b",
                    a = 1
                }
            };

            var obj2 = new
            {
                property = new
                {
                    a = 1,
                    b = "b"
                }
            };

            var hierarchyNode1 = ReflectedHierarchy.Create(obj1);
            var hierarchyNode2 = ReflectedHierarchy.Create(obj2);

            // ACT

            var result1 = hierarchyNode1.DescendantAt(HierarchyPath.Create("property")).Children().ToArray();
            var result2 = hierarchyNode2.DescendantAt(HierarchyPath.Create("property")).Children().ToArray();

            // ASSERT

            Assert.Equal(new[] { "a", "b" }, result1.Select(n => n.Id));
            Assert.Equal(new[] { "a", "b" }, result2.Select(n => n.Id));
        }

        #endregion Map object properties to inner nodes

        #region TryGet node by name

        [Fact]
        public void Retrieve_property_as_child_from_object_node()
        {
            // ARRANGE

            var obj = new { property = (string)"1" };
            var hierarchyNode = ReflectedHierarchy.Create(obj);

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("property");

            // ASSERT

            Assert.True(success);
            Assert.Equal("property", result.Id);
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

        [Fact]
        public void Retrieve_array_item_by_index_from_array_node()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("0");

            // ASSERT

            Assert.True(success);
            Assert.Equal("0", result.Id);
        }

        [Fact]
        public void Retrieve_enumerable_item_by_index_from_enumerable_node()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("0");

            // ASSERT

            Assert.True(success);
            Assert.Equal("0", result.Id);
        }

        [Fact]
        public void Retrieve_array_item_by_index_from_array_node_fails_on_wrong_index()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("2");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Retrieve_array_item_by_index_from_array_node_fails_on_index_not_a_number()
        {
            // ARRANGE

            var obj = new { property = new[] { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("number");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Retrieve_array_item_by_index_from_enumerable_node_fails_on_wrong_index()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("2");

            // ASSERT

            Assert.False(success);
        }

        [Fact]
        public void Retrieve_array_item_by_index_from_enumerable_node_fails_on_index_not_a_number()
        {
            // ARRANGE

            var obj = new { property = new List<int> { 1, 2 } };
            var (_, hierarchyNode) = ReflectedHierarchy.Create(obj).TryGetChildNode("property");

            // ACT

            var (success, result) = hierarchyNode.TryGetChildNode("number");

            // ASSERT

            Assert.False(success);
        }

        #endregion TryGet node by name

        

        #region Taverse the hierachy

        [Fact]
        public void Object_without_properties_hasnt_descendants()
        {
            // ARRANGE

            var obj = new { };

            // ACT

            var result = ReflectedHierarchy.Create(obj).Descendants().ToArray();

            // ASSERT

            Assert.Empty(result);
        }

        [Fact]
        public void Object_with_int_property_has_single_descendant()
        {
            // ARRANGE

            var obj = new
            {
                a = 1,
            };

            // ACT

            var result = ReflectedHierarchy.Create(obj).Descendants().ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void Object_with_string_property_has_single_descendant()
        {
            // ARRANGE

            var obj = new
            {
                a = "string",
            };

            // ACT

            var result = ReflectedHierarchy.Create(obj).Descendants().ToArray();

            // ASSERT
            // retuirns property "a" and a's value property Length. Chars is ignored.

            Assert.Equal(2, result.Length);
        }

        [Fact]
        private void Object_with_two_properties_create_three_descendants()
        {
            // ARRANGE

            var obj = new
            {
                a = 1,
                b = "b"
            };

            // ACT

            var result = ReflectedHierarchy.Create(obj).Descendants().ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
        }

        #endregion Taverse the hierachy
    }
}