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

            var rootDoc = this.nodes.FindAll().Single();
            Assert.False(rootDoc.TryGetValue("key", out var rootKey));
            Assert.True(rootDoc.TryGetValue("value", out var rootValue));
            Assert.Equal(value, rootValue.RawValue);
        }

        [Fact]
        public void LiteDbHierarchy_removes_value_from_root()
        {
            // ARRANGE

            var setValue_root = Guid.NewGuid();
            var setValue_a = Guid.NewGuid();

            hierarchy.Add(HierarchyPath.Create<string>(), setValue_root);
            hierarchy.Add(HierarchyPath.Create("a"), setValue_a);

            // ACT
            // remove value from root

            var result = hierarchy.Remove(HierarchyPath.Create<string>());

            // ASSERT
            // root has no value, /a still has a value

            Assert.True(result);
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value_root));
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value_a));
            Assert.Equal(setValue_a, value_a);

            // check db

            var rootDoc = this.nodes.FindOne(Query.EQ("key", null));

            Assert.False(rootDoc.TryGetValue("value", out var rootDocValue));
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesValue));
            Assert.True(childNodesValue.AsDocument.TryGetValue("a", out var aDocId));

            var aDoc = this.nodes.FindById(aDocId);

            Assert.NotNull(aDoc);
            Assert.True(aDoc.TryGetValue("value", out var aDocValue));
            Assert.Equal(setValue_a, aDocValue.AsGuid);
        }

        [Fact]
        public void LiteDbHierarchy_removes_value_from_child()
        {
            // ARRANGE

            var setValue_root = Guid.NewGuid();
            var setValue_a = Guid.NewGuid();

            hierarchy.Add(HierarchyPath.Create<string>(), setValue_root);
            hierarchy.Add(HierarchyPath.Create("a"), setValue_a);

            // ACT
            // remove value from root

            var result = hierarchy.Remove(HierarchyPath.Create("a"));

            // ASSERT
            // root has no value, /a still has a value

            Assert.True(result);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value_root));
            Assert.Equal(setValue_root, value_root);
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create("a"), out var value_a));

            // check db

            var rootDoc = this.nodes.FindOne(Query.EQ("key", null));

            Assert.True(rootDoc.TryGetValue("value", out var rootDocValue));
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesValue));
            Assert.True(childNodesValue.AsDocument.TryGetValue("a", out var aDocId));

            var aDoc = this.nodes.FindById(aDocId);

            Assert.NotNull(aDoc);
            Assert.False(aDoc.TryGetValue("value", out var aDocValue));
        }

        [Fact]
        public void LiteDbHierarchy_removes_root_by_removing_its_value()
        {
            // ARRANGE

            hierarchy.Add(HierarchyPath.Create<string>(), Guid.NewGuid());

            var originalRootDoc = this.nodes.FindOne(Query.EQ("key", null));
            originalRootDoc.TryGetValue("_id", out var originalRootDocId);

            // ACT
            // remove value from root

            var result = hierarchy.RemoveNode(HierarchyPath.Create<string>(), recurse: false);

            // ASSERT
            // root has no value, /a still has a value

            Assert.True(result);
            Assert.False(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value_root));

            // check db

            var rootDoc = this.nodes.FindOne(Query.EQ("key", null));

            Assert.True(rootDoc.TryGetValue("_id", out var rootDocId));
            Assert.Equal(originalRootDocId, rootDocId);
            Assert.False(rootDoc.TryGetValue("value", out var rootDocValue));
        }

        [Fact]
        public void LiteDbHierarchy_removing_root_non_recursive_fails_if_child_node_is_present()
        {
            // ARRANGE

            var setValue_root = Guid.NewGuid();

            hierarchy.Add(HierarchyPath.Create<string>(), setValue_root);
            hierarchy.Add(HierarchyPath.Create("a"), Guid.NewGuid());

            var originalRootDoc = this.nodes.FindOne(Query.EQ("key", null));
            originalRootDoc.TryGetValue("_id", out var originalRootDocId);

            // ACT
            // remove value from root

            var result = hierarchy.RemoveNode(HierarchyPath.Create<string>(), recurse: false);

            // ASSERT
            // root has no value, /a still has a value

            Assert.False(result);
            Assert.True(hierarchy.TryGetValue(HierarchyPath.Create<string>(), out var value_root));
            Assert.Equal(setValue_root, value_root);

            // check db

            var rootDoc = this.nodes.FindOne(Query.EQ("key", null));

            Assert.True(rootDoc.TryGetValue("_id", out var rootDocId));
            Assert.Equal(originalRootDocId, rootDocId);
            Assert.True(rootDoc.TryGetValue("value", out var rootDocValue));
            Assert.Equal(setValue_root, rootDocValue.RawValue);
        }
    }
}