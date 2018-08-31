using LiteDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyNodeRepositoryTest : IDisposable
    {
        private readonly LiteDatabase database;
        private readonly LiteDbHierarchyNodeRepository repository;
        private readonly LiteCollection<BsonDocument> nodes;
        private readonly LiteCollection<BsonDocument> values;

        public LiteDbHierarchyNodeRepositoryTest()
        {
            this.database = new LiteDatabase(new MemoryStream());
            this.repository = new LiteDbHierarchyNodeRepository(this.database, "nodes", "values");
            this.nodes = this.database.GetCollection("nodes");
            this.values = this.database.GetCollection("values");
        }

        public void Dispose()
        {
            this.database.Dispose();
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_has_root_node()
        {
            // ACT

            var result = this.repository.Root;

            // ASSERT

            Assert.NotNull(result);
            Assert.Null(result.Key);
            Assert.False(result.HasChildNodes);
            Assert.False(result.ChildNodes.Any());
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_creates_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNodeEntity { Key = "node_key" };

            // ACT

            var (result, resultId) = this.repository.TryInsert(node);

            // ASSERT
            // node is in db

            Assert.True(result);

            var nodeFromDb = this.nodes.FindById(resultId);

            Assert.NotNull(nodeFromDb);
            Assert.Equal("node_key", nodeFromDb.AsDocument[nameof(LiteDbHierarchyNodeEntity.Key)]);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_reads_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNodeEntity { Key = "key" };
            var (_, nodeId) = this.repository.TryInsert(node);

            // ACT

            var result = this.repository.Read(nodeId);

            // ASSERT

            Assert.Equal(node._Id, result._Id);
            Assert.NotSame(node, result);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_reading_node_fails_gracefully_on_missing_id()
        {
            // ACT

            var result = this.repository.Read(ObjectId.NewObjectId());

            // ASSERT

            Assert.Null(result);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_updates_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNodeEntity { Key = "key" };
            var (_, nodeId) = this.repository.TryInsert(node);

            node.Key = "new_key";
            node.ChildNodeIds["child_key"] = "child_id";

            // ACT

            this.repository.Update(node);

            // ASSERT
            // node is modified in db
            var nodeFromDb = this.nodes.FindById(nodeId);
            Assert.NotNull(nodeFromDb);
            Assert.Equal("new_key", nodeFromDb.AsDocument[nameof(LiteDbHierarchyNodeEntity.Key)]);
            Assert.Equal("child_id", nodeFromDb.AsDocument["_cn"].AsDocument["child_key"].AsString);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deletes_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNodeEntity { Key = "key" };
            var (_, nodeId) = this.repository.TryInsert(node);

            // ACT

            this.repository.DeleteNode(nodeId, false);

            // ASSERT
            // node is removed from db
            Assert.Null(this.nodes.FindById(nodeId));
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deleting_node_fails_gracefully_on_missing_id()
        {
            // ACT

            var result = this.repository.DeleteNode(ObjectId.NewObjectId(), false);

            // ASSERT

            Assert.False(result);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_inserts_a_value()
        {
            // ARRANGE

            var valueEntity = new LiteDbHierarchyValueEntity();
            valueEntity.SetValue(1);

            // ACT

            this.repository.Upsert(valueEntity);

            // ASSERT

            Assert.NotNull(valueEntity._Id);

            // check db

            var fromRead = this.values.FindById(valueEntity._Id);

            Assert.NotNull(fromRead);
            Assert.Equal(1, fromRead[nameof(LiteDbHierarchyValueEntity.Value)].AsInt32);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_updates_value()
        {
            // ARRANGE

            var valueEntity = new LiteDbHierarchyValueEntity();
            valueEntity.SetValue(1);

            this.repository.Upsert(valueEntity);

            // ACT

            valueEntity.SetValue(2);

            this.repository.Upsert(valueEntity);

            // ASSERT

            //Assert.True(result);

            // check db

            var fromRead = this.values.FindById(valueEntity._Id);

            Assert.NotNull(fromRead);
            Assert.Equal(2, fromRead[nameof(LiteDbHierarchyValueEntity.Value)].AsInt32);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deletes_value()
        {
            // ARRANGE

            var valueEntity = new LiteDbHierarchyValueEntity();
            valueEntity.SetValue(1);
            this.repository.Upsert(valueEntity);

            // ACT

            var result = this.repository.DeleteValue(valueEntity._Id);

            // ASSERT

            Assert.True(result);
            Assert.NotNull(valueEntity._Id);

            // check db

            Assert.Empty(this.values.FindAll());
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deleting_value_fails_gracefully_on_missing_id()
        {
            // ACT

            var result = this.repository.DeleteValue(ObjectId.NewObjectId());

            // ASSERT

            Assert.False(result);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deletes_nodes_and_values()
        {
            // ARRANGE

            var childValue = new LiteDbHierarchyValueEntity();
            childValue.SetValue(1);
            this.repository.Upsert(childValue);

            var child = new LiteDbHierarchyNodeEntity { Key = "key2", ValueRef = childValue._Id };
            var (_, childId) = this.repository.TryInsert(child);

            var nodeValue = new LiteDbHierarchyValueEntity();
            nodeValue.SetValue(1);
            this.repository.Upsert(nodeValue);

            var node = new LiteDbHierarchyNodeEntity { Key = "key1", ValueRef = nodeValue._Id };
            node.ChildNodeIds.Add("key2", childId);
            var (_, nodeId) = this.repository.TryInsert(node);

            // ACT

            var result = this.repository.Delete(new[] { node, child });

            // ASSERT
            // nodes are removed from db

            Assert.True(result);
            Assert.Null(this.nodes.FindById(nodeId));
            Assert.Null(this.nodes.FindById(childId));
            Assert.Null(this.values.FindById(nodeValue._Id));
            Assert.Null(this.values.FindById(childValue._Id));
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_deleting_nodes_and_values_skip_null_value()
        {
            // ARRANGE

            var child = new LiteDbHierarchyNodeEntity { Key = "key2" };
            var (_, childId) = this.repository.TryInsert(child);

            var nodeValue = new LiteDbHierarchyValueEntity();
            nodeValue.SetValue(1);
            this.repository.Upsert(nodeValue);

            var node = new LiteDbHierarchyNodeEntity { Key = "key1", ValueRef = nodeValue._Id };
            node.ChildNodeIds.Add("key2", childId);
            var (_, nodeId) = this.repository.TryInsert(node);

            // ACT

            var result = this.repository.Delete(new[] { node, child });

            // ASSERT
            // nodes are removed from db

            Assert.True(result);
            Assert.Null(this.nodes.FindById(nodeId));
            Assert.Null(this.nodes.FindById(childId));
            Assert.Null(this.values.FindById(nodeValue._Id));
        }
    }
};