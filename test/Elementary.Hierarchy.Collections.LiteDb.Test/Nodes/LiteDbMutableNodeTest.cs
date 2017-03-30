using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using LiteDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.LiteDb.Test.Nodes
{
    public class LiteDbLiteDbMutableNodeTest
    {
        private readonly LiteDatabase database;
        private readonly LiteCollection<BsonDocument> nodes;
        private readonly BsonDocument rootDocument;

        public LiteDbLiteDbMutableNodeTest()
        {
            this.database = new LiteDatabase(new MemoryStream());
            this.rootDocument = new BsonDocument();
            this.nodes = this.database.GetCollection("nodes");
            this.nodes.Insert(this.rootDocument);
        }

        #region Add child node

        [Fact]
        public void LiteDbMutableNode_adds_child_to_current_instance()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument);

            // ACT

            var result = node.AddChild(child);

            // ASSERT
            // node has a child now

            Assert.Same(node, result);
            Assert.True(node.HasChildNodes);
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Equal(child, addedChild);

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(1, childNodesDoc.AsDocument.Count);
            Assert.True(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));

            var childDoc = this.nodes.FindById(childDocId);

            Assert.NotNull(childDoc);
            Assert.True(childDoc.TryGetValue("key", out var childKey));
            Assert.Equal("a", childKey.AsString);
        }

        [Fact]
        public void LiteDbMutableNode_fails_on_adding_same_key_twice()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            child.TryGetId(out var childId);
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child);
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.AddChild(secondChild));

            // ASSERT

            Assert.Equal($"Node contains child node(id='{childId}') with same key='a'", result.Message);

            // node is unchanged

            Assert.True(node.HasChildNodes);
            Assert.True(node.TryGetChildNode("a", out var addedChild));
            Assert.Equal(child, addedChild);

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(1, childNodesDoc.AsDocument.Count);
            Assert.True(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));

            var childDoc = this.nodes.FindById(childDocId);

            Assert.NotNull(childDoc);
            Assert.True(childDoc.TryGetValue("key", out var childKey));
            Assert.Equal("a", childKey.AsString);
        }

        [Fact]
        public void LiteDbMutableNode_fails_on_adding_child_without_key()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument());
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.AddChild(child));

            // ASSERT

            Assert.Equal("Child node must have a key", result.Message);

            // node is unchanged

            Assert.False(node.HasChildNodes);
            Assert.False(node.TryGetChildNode("a", out var addedChild));

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.False(rootDoc.TryGetValue("cn", out var childNodesDoc));
        }

        #endregion Add child node

        #region Remove child

        [Fact]
        public void LiteDbMutableNode_removes_child_from_current_instance()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child);

            // ACT

            var result = node.RemoveChild(child);

            // ASSERT

            Assert.NotNull(result);
            Assert.False(result.HasChildNodes);
            Assert.False(result.TryGetChildNode("a", out var addedChild));

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(0, childNodesDoc.AsDocument.Count);
            Assert.False(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));
            Assert.False(this.nodes.Find(Query.EQ("key", "a")).Any());
        }

        #endregion Remove child

        #region Replace Child

        [Fact]
        public void LiteDbMutableNode_replaces_child_from_current_instance()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            child.SetValue(1);
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child);
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            secondChild.SetValue(2);

            // ACT

            var result = node.ReplaceChild(child, secondChild);

            // ASSERT

            Assert.NotNull(result);
            Assert.True(result.HasChildNodes);
            Assert.True(result.TryGetChildNode("a", out var addedChild));

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(1, childNodesDoc.AsDocument.Count);
            Assert.True(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));

            var childDoc = this.nodes.FindById(childDocId);

            Assert.NotNull(childDoc);
            Assert.True(childDoc.TryGetValue("key", out var childKey));
            Assert.Equal("a", childKey.AsString);
            Assert.True(childDoc.TryGetValue("value", out var childValue));
            Assert.Equal(2, childValue.AsInt32);
            Assert.NotEqual(child.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
            Assert.Equal(secondChild.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
        }

        [Fact]
        public void LiteDbMutableNode_fails_on_Replace_if_keys_are_different()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            child.SetValue(1);
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child);
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "b");
            secondChild.SetValue(2);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(child, secondChild));

            // ASSERT

            Assert.Equal("Key of child to replace (key='a') and new child (key='b') must be equal", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.True(node.TryGetChildNode("a", out var addedChild));

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(1, childNodesDoc.AsDocument.Count);
            Assert.True(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));

            var childDoc = this.nodes.FindById(childDocId);

            Assert.NotNull(childDoc);
            Assert.True(childDoc.TryGetValue("key", out var childKey));
            Assert.Equal("a", childKey.AsString);
            Assert.True(childDoc.TryGetValue("value", out var childValue));
            Assert.Equal(1, childValue.AsInt32);
            Assert.Equal(child.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
            Assert.NotEqual(secondChild.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
        }

        [Fact]
        public void LiteDbMutableNode_fails_on_replacing_unknown_child()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            child.SetValue(1);
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child);
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            secondChild.SetValue(2);
            var thirdChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            thirdChild.SetValue(3);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => node.ReplaceChild(thirdChild, secondChild));

            // ASSERT

            Assert.Equal("The node (id='a') doesn't replace any of the existing child nodes", result.Message);
            Assert.True(node.HasChildNodes);
            Assert.True(node.TryGetChildNode("a", out var addedChild));

            // check db

            var rootDoc = this.nodes.Find(Query.EQ("key", null)).Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(1, childNodesDoc.AsDocument.Count);
            Assert.True(childNodesDoc.AsDocument.TryGetValue("a", out var childDocId));

            var childDoc = this.nodes.FindById(childDocId);

            Assert.NotNull(childDoc);
            Assert.True(childDoc.TryGetValue("key", out var childKey));
            Assert.Equal("a", childKey.AsString);
            Assert.True(childDoc.TryGetValue("value", out var childValue));
            Assert.Equal(1, childValue.AsInt32);
            Assert.Equal(child.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
            Assert.NotEqual(secondChild.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
            Assert.NotEqual(thirdChild.BsonDocument.Get("_id").AsObjectId, childDoc.Get("_id").AsObjectId);
        }

        #endregion Replace Child

        #region Remove all child nodes

        [Fact]
        public void LiteDbMutableNode_removes_all_childnodes_recursively()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "b");
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child).AddChild(secondChild);
            var grandChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "c");
            child.AddChild(grandChild);

            // ACT

            var result = node.RemoveAllChildNodes(recurse: true);

            // ASSERT
            // node has a child now

            Assert.True(result);
            Assert.False(node.HasChildNodes);
            Assert.False(node.TryGetChildNode("a", out var addedChild));

            // check db
            // only the node is there

            var rootDoc = this.nodes.FindAll().Single();

            Assert.NotNull(rootDoc);
            Assert.True(rootDoc.TryGetValue("cn", out var childNodesDoc));
            Assert.Equal(0, childNodesDoc.AsDocument.Count);
        }

        [Fact]
        public void LiteDbMutableNode_fails_on_removing_if_node_has_child_nodes()
        {
            // ARRANGE

            var child = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "a");
            var secondChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "b");
            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument).AddChild(child).AddChild(secondChild);
            var grandChild = new LiteDbMutableNode<int>(this.nodes, new BsonDocument(), "c");
            child.AddChild(grandChild);

            // ACT

            var result = node.RemoveAllChildNodes(recurse: false);

            // ASSERT
            // node has a child now

            Assert.False(result);
            Assert.True(node.HasChildNodes);

            // check db
            // all nodes are still there

            Assert.Equal(4, this.nodes.FindAll().Count());
        }

        [Fact]
        public void LiteDbMutableNode_RemoveAllChildNodes_returns_false_if_no_nodes_have_been_deleted()
        {
            // ARRANGE

            var node = new LiteDbMutableNode<int>(this.nodes, this.rootDocument);

            // ACT

            var result = node.RemoveAllChildNodes(recurse: false);

            // ASSERT
            // node has a child now

            Assert.False(result);
            Assert.False(node.HasChildNodes);
        }

        #endregion Remove all child nodes
    }
}