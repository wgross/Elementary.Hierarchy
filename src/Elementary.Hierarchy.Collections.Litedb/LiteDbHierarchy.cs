using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections.LiteDb
{
    public class LiteDbHierarchy<TValue> : IHierarchy<string, TValue>
    {
        #region Construction and initialization of this instance

        private readonly LiteCollection<BsonDocument> nodes;
        private LiteDbMutableNode<TValue> rootNode;

        public LiteDbHierarchy(LiteCollection<BsonDocument> nodes)
        {
            this.nodes = nodes;
        }

        #endregion Construction and initialization of this instance

        #region IHierarchy Members

        public TValue this[HierarchyPath<string> path]
        {
            get
            {
                if (this.TryGetValue(path, out var value))
                    return value;

                throw new KeyNotFoundException($"path '{path}' doesn't exist or has no value");
            }
            set
            {
                new SetOrAddNodeValueWriter<string, TValue, LiteDbMutableNode<TValue>>(
                    createNode: key => new LiteDbMutableNode<TValue>(this.nodes, new BsonDocument(), key))
                    .SetValue(this.GetOrCreateRootNode(), path, value);
            }
        }

        public void Add(HierarchyPath<string> path, TValue value)
        {
            new SetOrAddNodeValueWriter<string, TValue, LiteDbMutableNode<TValue>>(
                createNode: key => new LiteDbMutableNode<TValue>(this.nodes, new BsonDocument(), key))
                .AddValue(this.GetOrCreateRootNode(), path, value);
        }

        public bool Remove(HierarchyPath<string> path)
        {
            var writer = new RemoveValueHierarchyWriter<string, TValue, LiteDbMutableNode<TValue>>();
            writer.ClearValue(this.GetOrCreateRootNode(), path);

            return writer.ValueWasCleared;
        }

        public bool RemoveNode(HierarchyPath<string> path, bool recurse)
        {
            // this isn't a special case.
            // use the hierachy writer for inner nodes

            var writer = new RemoveNodeRecursivlyWriter<TValue, LiteDbMutableNode<TValue>>(recurse);
            if (null == writer.RemoveNode(this.GetOrCreateRootNode(), path, out var nodeWasRemoved))
            {
                // getting null as the result of the deletions measns to delete the root node.
                // this is not done be the visitor

                if (nodeWasRemoved) // deletion is allowed
                    if (nodeWasRemoved = this.rootNode.RemoveNode(recurse))
                        this.rootNode = null;
            }

            return nodeWasRemoved;
        }

        /// <summary>
        /// Starts a traversal of the hierarchy at the specified hierachy node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<string, TValue> Traverse(HierarchyPath<string> startAt)
        {
            return ((IHierarchyNode<string, TValue>)new HierarchyTraverser<string, TValue, LiteDbMutableNode<TValue>>(this.GetOrCreateRootNode())).DescendantAt(startAt);
        }

        public bool TryGetValue(HierarchyPath<string> path, out TValue value)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (this.GetOrCreateRootNode().TryGetDescendantAt(path, out var descendantNode))
                return descendantNode.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        #endregion IHierarchy Members

        #region LiteDb access implementations

        private LiteDbMutableNode<TValue> GetOrCreateRootNode()
        {
            if (this.rootNode == null)
            {
                // recover from existing root document or create a new root

                var tmp = new LiteDbMutableNode<TValue>(this.nodes, this.nodes.FindOne(Query.EQ("key", null)) ?? new BsonDocument());
                this.nodes.Upsert(tmp.BsonDocument);
                this.rootNode = tmp;
            }

            return this.rootNode;
        }

        #endregion LiteDb access implementations
    }
}