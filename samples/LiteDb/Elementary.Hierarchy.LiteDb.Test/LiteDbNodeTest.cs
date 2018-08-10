using LiteDB;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class BsonDocumentNodeAdpaterTest
    {
        [Fact]
        public void BsonDocumentNodeAdapter_has_no_child_nodes_when_empty()
        {
            // ACT

            var result = new BsonDocumentNodeAdpater(string.Empty, new BsonDocument());

            // ASSERT

            Assert.Empty(result.Id);
            Assert.False(result.HasChildNodes);
            Assert.False(result.ChildNodes.Any());
            Assert.NotNull(result.BsonDocument["cn"].AsDocument);
            Assert.False(result.BsonDocument["cn"].AsDocument.Any());
        }

        [Fact]
        public void BsonDocumentNodeAdapter_add_new_node_as_child()
        {
            // ARRANGE

            var child = new BsonDocumentNodeAdpater("child", new BsonDocument());
            var result = new BsonDocumentNodeAdpater(string.Empty, new BsonDocument());

            // ACT

            result.AddChildNode(child);

            // ASSERT

            Assert.True(result.HasChildNodes);
            Assert.Same(child, result.ChildNodes.Single());
            Assert.NotNull(result.BsonDocument["cn"].AsDocument);
            Assert.True(result.BsonDocument["cn"].AsDocument.Any());
            Assert.Equal(BsonValue.Null, result.BsonDocument["cn"].AsDocument["child"]);
        }

        [Fact]
        public void BsonDocumentNodeAdapter_adding_new_node_as_child_throws_on_duplicate_id()
        {
            // ARRANGE

            var child1 = new BsonDocumentNodeAdpater("child", new BsonDocument());
            var node = new BsonDocumentNodeAdpater(string.Empty, new BsonDocument());
            node.AddChildNode(child1);

            var child2 = new BsonDocumentNodeAdpater("child", new BsonDocument());

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.AddChildNode(child2));

            // ASSERT

            Assert.Equal("duplicate key: 'child'", result.Message);
            Assert.Same(child1, node.ChildNodes.Single());
            Assert.Equal(BsonValue.Null, node.BsonDocument["cn"].AsDocument["child"]);
        }

        [Fact]
        public void BsonDocumentNodeAdapter_removes_child_node()
        {
            // ARRANGE

            var child = new BsonDocumentNodeAdpater("child", new BsonDocument());
            var parent = new BsonDocumentNodeAdpater(string.Empty, new BsonDocument());
            parent.AddChildNode(child);

            // ACT

            var result = parent.RemoveChildNode(child);

            // ASSERT

            Assert.True(result);
            Assert.False(parent.HasChildNodes);
            Assert.NotNull(parent.BsonDocument["cn"].AsDocument);
            Assert.False(parent.BsonDocument["cn"].AsDocument.Any());
        }

        [Fact]
        public void BsonDocumentNodeAdapter_removing_child_node_fails_silently_for_unknown_child()
        {
            // ARRANGE

            var child = new BsonDocumentNodeAdpater("child", new BsonDocument());
            var parent = new BsonDocumentNodeAdpater(string.Empty, new BsonDocument());

            // ACT

            var result = parent.RemoveChildNode(child);

            // ASSERT

            Assert.False(result);
            Assert.False(parent.HasChildNodes);
            Assert.NotNull(parent.BsonDocument["cn"].AsDocument);
            Assert.False(parent.BsonDocument["cn"].AsDocument.Any());
        }
    }
}