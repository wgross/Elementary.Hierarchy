using System;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class FlattedObjectHierarchyNodeFactoryTest
    {
        [Fact]
        public void FlattedObjectNodeFactory_doesnt_create_child_nodes_for_string()
        {
            // ARRANGE

            var factory = new FlattedObjectHierarchyNodeFactory();
            var str = "string";

            // ACT

            var result = factory.Create(str, typeof(string).GetProperty(nameof(string.Length)));

            // ASSERT

            Assert.Null(result);
        }

        [Fact]
        public void FlattedObjectNodeFactory_doesnt_create_child_nodes_for_value_types()
        {
            // ARRANGE

            var factory = new FlattedObjectHierarchyNodeFactory();
            var date = DateTime.Now;

            // ACT

            var result = factory.Create(date, typeof(DateTime).GetProperty(nameof(DateTime.Second)));

            // ASSERT

            Assert.Null(result);
        }
    }
}