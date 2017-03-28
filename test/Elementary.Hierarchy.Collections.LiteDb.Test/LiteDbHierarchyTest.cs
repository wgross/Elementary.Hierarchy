using LiteDB;
using System;
using System.IO;
using Xunit;

namespace Elementary.Hierarchy.Collections.LiteDb.Test
{
    public class LiteDbHierarchyTest
    {
        private LiteDatabase database;
        private MemoryStream databaseStream;
        private LiteDbHierarchy<Guid> hierarchy;

        public LiteDbHierarchyTest()
        {
            this.databaseStream = new MemoryStream();
            this.database = new LiteDatabase(this.databaseStream);
            this.hierarchy = new LiteDbHierarchy<Guid>(this.database);
        }

        [Fact]
        public void IHierarchy_root_node_has_no_value_on_TryGetValue()
        {
            // ACT & ASSERT

            Assert.False(this.hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
        }

        [Fact]
        public void IHierarchy_adds_value_to_root_node()
        {
            // ARRANGE

            Guid value = Guid.NewGuid();

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), value);

            // ASSERT
            // new hierarchy contains all values

            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var readValue));
            Assert.Same(value, readValue);
        }
    }
}