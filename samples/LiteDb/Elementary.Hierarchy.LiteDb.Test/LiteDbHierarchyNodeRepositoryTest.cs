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

        public LiteDbHierarchyNodeRepositoryTest()
        {
            this.database = new LiteDatabase(new MemoryStream());
            this.repository = new LiteDbHierarchyNodeRepository(this.database, "nodes");
            this.nodes = this.database.GetCollection("nodes");
        }

        public void Dispose()
        {
            this.database.Dispose();
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_has_a_root_node()
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
        public void LiteDbHierarchyNodeRepository_inserts_a_new_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNode { Key = "node_key" };

            // ACT

            var (result, resultId) = this.repository.TryInsert(node);

            // ASSERT
            // node is in db

            Assert.True(result);

            var nodeFromDb = this.nodes.FindById(resultId);

            Assert.NotNull(nodeFromDb);
            Assert.Equal("node_key", nodeFromDb.AsDocument[nameof(LiteDbHierarchyNode.Key)]);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_inserting_a_new_node_fails_with_duplicate_key()
        {
            // ARRANGE

            var node1 = new LiteDbHierarchyNode { Key = "node_key" };
            var (_, node1Id) = this.repository.TryInsert(node1);

            // ACT

            var node2 = new LiteDbHierarchyNode { Key = "node_key" };
            var (result, _) = this.repository.TryInsert(node1);

            // ASSERT
            // insert failed
            Assert.False(result);
            // node is still in db
            var nodeFromDb = this.nodes.FindById(node1Id);
            Assert.NotNull(nodeFromDb);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_updates_an_existing_node()
        {
            // ARRANGE

            var node = new LiteDbHierarchyNode { Key = "key" };
            var (_, nodeId) = this.repository.TryInsert(node);

            node.Key = "new_key";
            node._ChildNodeIds["child_key"] = "child_id";

            // ACT

            this.repository.Update(node);

            // ASSERT
            // node is modified in db
            var nodeFromDb = this.nodes.FindById(nodeId);
            Assert.NotNull(nodeFromDb);
            Assert.Equal("new_key", nodeFromDb.AsDocument[nameof(LiteDbHierarchyNode.Key)]);
            Assert.Equal("child_id", nodeFromDb.AsDocument["_cn"].AsDocument["child_key"].AsString);
        }

        [Fact]
        public void LiteDbHierarchyNodeRepository_updating_an_existing_node_fails_on_duplicate_key()
        {
            // ARRANGE

            var node1 = new LiteDbHierarchyNode { Key = "key1" };
            var (_, node1Id) = this.repository.TryInsert(node1);

            var node2 = new LiteDbHierarchyNode { Key = "key2" };
            var (_, node2Id) = this.repository.TryInsert(node2);

            // ACT

            node2.Key = "key1";
            var result = this.repository.Update(node2);

            // ASSERT
            // node isn't modified in db
            Assert.False(result);

            var node1FromDb = this.nodes.FindById(node1Id);
            Assert.Equal("key1", node1FromDb.AsDocument[nameof(LiteDbHierarchyNode.Key)]);

            var node2FromDb = this.nodes.FindById(node2Id);
            Assert.Equal("key2", node2FromDb.AsDocument[nameof(LiteDbHierarchyNode.Key)]);
        }
    }
}