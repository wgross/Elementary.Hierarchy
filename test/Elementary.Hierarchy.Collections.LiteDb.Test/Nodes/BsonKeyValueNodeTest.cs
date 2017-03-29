using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using LiteDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.LiteDb.Test.Nodes
{
    public class BsonKeyValueNodeTest
    {
        private LiteDatabase database;
        private MemoryStream databaseStream;
        private LiteDbHierarchy<Guid> hierarchy;
        private readonly LiteCollection<BsonDocument> nodes;

        public BsonKeyValueNodeTest()
        {
            this.databaseStream = new MemoryStream();
            this.database = new LiteDatabase(this.databaseStream);
            this.nodes = this.database.GetCollection("nodes");
        }

        [Fact]
        public void BsonKeyValueNode_enforces_id()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());

            // ACT & ASSERT

            Assert.True(node.TryGetId(out var id));

            // check db: nothing should have been written

            Assert.False(this.nodes.FindAll().Any());
        }

        [Fact]
        public void BsonKeyValueNode_is_empty_by_default()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());

            // ACT & ASSERT

            Assert.False(node.TryGetValue(out var value));
            Assert.False(node.TryGetKey(out var key));

            // check db: nothing should have been written

            Assert.False(this.nodes.FindAll().Any());
        }

        [Fact]
        public void BsonKeyValueNode_holds_key_and_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument(), "key", 1);

            // ACT & ASSERT

            Assert.True(node.TryGetKey(out var key));
            Assert.Equal("key", key);
            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);

            // check db: nothing should have been written

            Assert.False(this.nodes.FindAll().Any());
        }

        [Fact]
        public void BsonKeyValueNode_sees_Null_key_as_unset()
        {
            // ARRANGE

            var bsonDocument = new BsonDocument();
            bsonDocument.Set("key", BsonValue.Null);

            var node = new BsonKeyValueNode<int>(this.nodes, bsonDocument);

            // ACT & ASSERT

            Assert.False(node.TryGetKey(out var key));

            // check db: nothing should have been written

            Assert.False(this.nodes.FindAll().Any());
        }

        [Fact]
        public void BsonKeyValueNode_stores_value_as_an_update()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());
            this.nodes.Insert(node.BsonDocument);

            // ACT

            node.SetValue(1);

            // ASSERT

            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);

            // check db update writes the node

            var nodeDoc = this.nodes.FindAll().Single();

            Assert.True(nodeDoc.TryGetValue("_id", out var nodeDocId));
            Assert.False(nodeDoc.TryGetValue("key", out var nodeDocKey));
            Assert.True(nodeDoc.TryGetValue("value", out var nodeDocValue));
            Assert.Equal(1, nodeDocValue.RawValue);
        }

        [Fact]
        public void BsonKeyValueNode_fails_on_SetValue_for_unsaved_document()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());

            // ACT

            node.SetValue(1);

            // ASSERT

            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);

            // check db update writes the node

            Assert.False(this.nodes.FindAll().Any());
        }

        [Fact]
        public void BsonKeyValueNode_clears_stored_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());
            node.SetValue(1);

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.True(result);
            Assert.False(node.TryGetValue(out var value));
        }

        [Fact]
        public void BsonKeyValueNode_clears_unset_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<int>(this.nodes, new BsonDocument());

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.False(result);
            Assert.False(node.TryGetValue(out var value));
        }

        #region Equals and GetHashCode delegate behavior to _id of inner node

        [Fact]
        public void BsonKeyValueNode_are_equal_if_documents_are_same()
        {
            // ARRANGE
            // nodes share doucment but not the data

            var bsonDocument = new BsonDocument();
            var node1 = new BsonKeyValueNode<int>(this.nodes, bsonDocument, "key", 1);
            var node2 = new BsonKeyValueNode<int>(this.nodes, bsonDocument, "key2", 2);

            // ACT

            var result = node1.Equals(node2);

            // ACT & ASSERT

            Assert.True(result);
            Assert.Equal(node1.GetHashCode(), node2.GetHashCode());
        }

        [Fact]
        public void BsonKeyValueNode_are_equal_if_ids_are_same()
        {
            // ARRANGE
            // nodes share doucment but not the data

            var id = ObjectId.NewObjectId();
            var bsonDocument1 = new BsonDocument();
            bsonDocument1.Set("_id", id);
            var bsonDocument2 = new BsonDocument();
            bsonDocument2.Set("_id", id);

            var node1 = new BsonKeyValueNode<int>(this.nodes, bsonDocument1, "key", 1);
            var node2 = new BsonKeyValueNode<int>(this.nodes, bsonDocument2, "key2", 2);

            // ACT

            var result = node1.Equals(node2);

            // ACT & ASSERT

            Assert.True(result);
            Assert.Equal(node1.GetHashCode(), node2.GetHashCode());
        }

        #endregion Equals and GetHashCode delegate behavior to _id of inner node
    }
}