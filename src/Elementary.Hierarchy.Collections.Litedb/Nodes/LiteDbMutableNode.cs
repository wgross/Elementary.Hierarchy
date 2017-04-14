using Elementary.Hierarchy.Collections.Nodes;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.LiteDb.Nodes
{
    public class LiteDbMutableNode<TValue> : BsonKeyValueNode<TValue>,
        IHierarchyNodeWriter<LiteDbMutableNode<TValue>>,
        IHasIdentifiableChildNodes<string, LiteDbMutableNode<TValue>>,
        IHasChildNodes<LiteDbMutableNode<TValue>>
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

        public bool RemoveAllChildNodes(bool recurse)
        {
            if (!this.HasChildNodes)
                return false; // obviously removal of childnodes is not necessary

            if (this.HasChildNodes && !recurse)
                return false; // removal of child nodes isn't allowed

            return EnsureChildNodesAreDeleted();
        }

        public bool EnsureChildNodesAreDeleted()
        {
            var childNodeInfos = this.BsonDocumentChildNodes
                .Aggregate(new List<KeyValuePair<string, BsonValue>>(), (l, kv) => { l.Add(kv); return l; });

            bool? childNodsHaveBeenDeleted = null;
            foreach (var childNodeInfo in childNodeInfos)
            {
                if (this.TryGetChildNode(childNodeInfo.Key, out var childNode))
                {
                    if (childNode.EnsureChildNodesAreDeleted()) // descend int the tree further to clean up.
                        if (this.nodes.Delete(childNodeInfo.Value)) // the child node committed the removal of its child nodes->delete the child document
                            childNodsHaveBeenDeleted = this.BsonDocumentChildNodes.Remove(childNodeInfo.Key) || childNodsHaveBeenDeleted.GetValueOrDefault(false); // remove the child id from ths document
                }
            }

            // this node requires an update if the strcuture has changed.

            if (childNodsHaveBeenDeleted.GetValueOrDefault(false))
                return this.nodes.Update(this.BsonDocument);

            // at the end the question is: are there child nodes left or not.

            return !this.HasChildNodes;
        }

        public bool HasChildNodes => this.BsonDocumentChildNodes.Any();

        public bool HasValue => this.TryGetValue(out var _);

        public IEnumerable<LiteDbMutableNode<TValue>> ChildNodes => this.BsonDocumentChildNodes.Select(kv =>
        {
            return this.TryGetChildNode(kv.Key, out var node)
                ? node
                : throw new InvalidOperationException($"Db is inconsistent: a node(key='{kv.Key}',id='{kv.Value}') is missing");
        });

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