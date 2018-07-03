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

        public bool RemoveNode(bool recurse)
        {
            if (!recurse && this.HasChildNodes)
                return false; // removal of child nodes isn't allowed

            // removal of child nodes is allowed.

            if (this.RemoveAllChildNodesRecursivly() || !this.HasChildNodes) // child node have been removed or they have never existed
                if (this.TryGetId(out var id))
                    return this.nodes.Delete(id);

            return false;
        }

        public bool RemoveAllChildNodes(bool recurse)
        {
            if (!recurse && this.HasChildNodes)
                return false; // removal of child nodes isn't allowed

            return RemoveAllChildNodesRecursivly();
        }

        public bool RemoveAllChildNodesRecursivly()
        {
            var childNodeInfos = this.BsonDocumentChildNodes.Aggregate(new List<KeyValuePair<string, BsonValue>>(), (l, kv) => { l.Add(kv); return l; });

            bool? removeAllChildNodesFinished = null;
            foreach (var childNodeInfo in childNodeInfos)
            {
                var (found, childNode) = this.TryGetChildNode(childNodeInfo.Key);
                
                if (found)
                {
                    // descend int the tree further to clean up.
                    // this part is successful of deletions wen well and not childnode are left.

                    removeAllChildNodesFinished = (childNode.RemoveAllChildNodesRecursivly() && (!childNode.HasChildNodes)) && removeAllChildNodesFinished.GetValueOrDefault(true);

                    // then delete this child node.
                    // this parts is successful if deletion is successuf and the references coud be removed from this node

                    removeAllChildNodesFinished = (this.nodes.Delete(childNodeInfo.Value) && this.BsonDocumentChildNodes.Remove(childNodeInfo.Key)) || removeAllChildNodesFinished.GetValueOrDefault(false);
                }
            }

            // this node requires an update if the strcuture has changed.

            if (removeAllChildNodesFinished.GetValueOrDefault(false))
                return this.nodes.Update(this.BsonDocument);

            // desn't seem to have worked
            return false;
        }

        public bool HasChildNodes => this.BsonDocumentChildNodes.Any();

        public bool HasValue => this.TryGetValue(out var _);

        public IEnumerable<LiteDbMutableNode<TValue>> ChildNodes => this.BsonDocumentChildNodes.Select(kv =>
        {
            var (found, node) = this.TryGetChildNode(kv.Key);

            return found
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
                    if (childToRemove.RemoveNode(recurse: true))
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

        public (bool, LiteDbMutableNode<TValue>) TryGetChildNode(string key)
        {
            
            if (!this.BsonDocumentChildNodes.TryGetValue(key, out var childNodeId))
                return (false,null);

            var childNode = new LiteDbMutableNode<TValue>(this.nodes, this.nodes.FindById(childNodeId), key);
            return (true,childNode);
        }
    }
}