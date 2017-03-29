using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using LiteDB;
using System;
using Xunit;

namespace Elementary.Hierarchy.Collections.LiteDb.Test.Nodes
{
    public class BsonKeyValueNodeTest
    {
        [Fact]
        public void BsonKeyValueNode_is_empty_by_default()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<Guid, int>(new BsonDocument());

            // ACT & ASSERT

            Assert.False(node.TryGetValue(out var value));
            Assert.False(node.TryGetKey(out var key));
        }

        [Fact]
        public void BsonMutableNode_holds_key_and_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<string, int>(new BsonDocument(), "key", 1);

            // ACT & ASSERT

            Assert.True(node.TryGetKey(out var key));
            Assert.Equal("key", key);
            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);
        }

        [Fact]
        public void BsonMutableNode_sees_Null_key_as_unset()
        {
            // ARRANGE

            var bsonDocument = new BsonDocument();
            bsonDocument.Set("key", BsonValue.Null);

            var node = new BsonKeyValueNode<string, int>(bsonDocument);

            // ACT & ASSERT

            Assert.False(node.TryGetKey(out var key));
        }

        [Fact]
        public void BsonMutableNode_stores_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<string, int>(new BsonDocument());

            // ACT

            node.SetValue(1);

            // ASSERT

            Assert.True(node.TryGetValue(out var value));
            Assert.Equal(1, value);
        }

        [Fact]
        public void BsonMutableNode_clears_stored_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<string, int>(new BsonDocument());
            node.SetValue(1);

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.True(result);
            Assert.False(node.TryGetValue(out var value));
        }

        [Fact]
        public void BsonMutableNode_clears_unset_value()
        {
            // ARRANGE

            var node = new BsonKeyValueNode<string, int>(new BsonDocument());

            // ACT

            var result = node.RemoveValue();

            // ASSERT

            Assert.False(result);
            Assert.False(node.TryGetValue(out var value));
        }

        
    }
}