using Elementary.Hierarchy.Collections.Nodes;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.LiteDb.Nodes
{
    public class LiteDbMutableNode<TValue> : BsonKeyValueNode<TValue>,
        IHierarchyNodeWriter<LiteDbMutableNode<TValue>>,
        IHasIdentifiableChildNodes<string, LiteDbMutableNode<TValue>>
    {
        private readonly LiteCollection<BsonDocument> nodes;

        #region Construction and initialization of this instance

        public LiteDbMutableNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument)
            : base(nodes, bsonDocument)
        {
            this.nodes = nodes;
            if (!this.TryGetId(out var id))
                this.BsonDocument.Set("_id", ObjectId.NewObjectId());
        }

        public LiteDbMutableNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument, string key)
            : base(nodes, bsonDocument, key)
        {
            this.nodes = nodes;

            if (!this.TryGetId(out var id))
                this.BsonDocument.Set("_id", ObjectId.NewObjectId());
        }

        public LiteDbMutableNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument, string key, TValue value)
            : base(nodes, bsonDocument, key, value)
        {
            this.nodes = nodes;

            if (!this.TryGetId(out var id))
                this.BsonDocument.Set("_id", ObjectId.NewObjectId());
        }

        #endregion Construction and initialization of this instance

        private BsonDocument BsonDocumentChildNodes
        {
            get
            {
                if (!this.BsonDocument.TryGetValue("cn", out var childNodes))
                {
                    this.BsonDocument.Add("cn", childNodes = new BsonDocument());
                }
                return childNodes.AsDocument;
            }
        }

        public bool HasChildNodes => this.BsonDocumentChildNodes.Any();

        public bool HasValue => this.TryGetValue(out var _);

        public LiteDbMutableNode<TValue> AddChild(LiteDbMutableNode<TValue> newChild)
        {
            if (!newChild.TryGetKey(out var newChildKey))
                throw new InvalidOperationException("Child node must have a key");

            if (this.BsonDocumentChildNodes.TryGetValue(newChildKey, out var newChildId))
                throw new InvalidOperationException($"Node contains child node(id='{newChildId}') with same key='{newChildKey}'");

            this.BsonDocumentChildNodes.Set(newChildKey, this.nodes.Insert(newChild.BsonDocument));

            this.nodes.Update(this.BsonDocument);
            return this;
        }

        public LiteDbMutableNode<TValue> RemoveChild(LiteDbMutableNode<TValue> childToRemove)
        {
            if (childToRemove.TryGetKey(out var childKey))
                if (this.BsonDocumentChildNodes.TryGetValue(childKey, out var childId))
                    if (this.nodes.Delete(childId))
                        if (this.BsonDocumentChildNodes.Remove(childKey))
                            this.nodes.Update(this.BsonDocument);

            return this;
        }

        public LiteDbMutableNode<TValue> ReplaceChild(LiteDbMutableNode<TValue> childToReplace, LiteDbMutableNode<TValue> newChild)
        {
            if (object.ReferenceEquals(childToReplace, newChild))
                return this; // nothing to do

            newChild.TryGetKey(out var newChildKey);
            childToReplace.TryGetKey(out var childToReplaceKey);

            if (!EqualityComparer<string>.Default.Equals(childToReplaceKey, newChildKey))
                throw new InvalidOperationException($"Key of child to replace (key='{childToReplaceKey}') and new child (key='{newChildKey}') must be equal");

            if (this.BsonDocumentChildNodes.TryGetValue(newChildKey, out var childId))
            {
                if (!childToReplace.TryGetId(out var childToReplaceId) || !childId.Equals(childToReplaceId))
                    throw new InvalidOperationException($"The node (id='{newChildKey}') doesn't replace any of the existing child nodes");

                this.BsonDocumentChildNodes.Set(newChildKey, this.nodes.Insert(newChild.BsonDocument));
                this.nodes.Update(this.BsonDocument);
            }
            return this;
        }

        public bool TryGetChildNode(string key, out LiteDbMutableNode<TValue> childNode)
        {
            childNode = null;
            if (!this.BsonDocumentChildNodes.TryGetValue(key, out var childNodeId))
                return false;

            childNode = new LiteDbMutableNode<TValue>(this.nodes, this.nodes.FindById(childNodeId), key);
            return true;
        }
    }
}