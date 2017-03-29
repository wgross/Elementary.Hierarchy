using LiteDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.LiteDb.Test
{
    public class LiteDbHierarchyTest
    {
        private LiteDatabase database;
        private MemoryStream databaseStream;
        private LiteDbHierarchy<Guid> hierarchy;
        private readonly LiteCollection<BsonDocument> nodes;

        public LiteDbHierarchyTest()
        {
            this.databaseStream = new MemoryStream();
            this.database = new LiteDatabase(this.databaseStream);
            this.nodes = this.database.GetCollection("nodes");
            this.hierarchy = new LiteDbHierarchy<Guid>(this.nodes);
        }

        [Fact]
        public void LiteDbHierarchy_root_node_has_no_value_on_TryGetValue()
        {
            // ACT & ASSERT

            Assert.False(this.hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value));
        }

        [Fact]
        public void LiteDbHierarchy_adds_value_to_root_node()
        {
            // ARRANGE

            Guid value = Guid.NewGuid();

            // ACT

            hierarchy.Add(HierarchyPath.Create<string>(), value);

            // ASSERT
            // new hierarchy contains all values

            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var readValue));
            Assert.Equal(value, readValue);

            // check db

            var rootNode = this.nodes.FindAll().Single();
            Assert.False(rootNode.TryGetValue("key", out var rootKey));
            Assert.True(rootNode.TryGetValue("value", out var rootValue));
            Assert.Equal(value, rootValue.RawValue);
        }
    }
}