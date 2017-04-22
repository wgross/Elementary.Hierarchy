﻿using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using LiteDB;
using System;
using System.Linq;

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

        public bool Remove(HierarchyPath<string> hierarchyPath, int? maxDepth = default(int?))
        {
            if (maxDepth != null)
                throw new NotSupportedException(nameof(maxDepth));

            var writer = new RemoveValueHierarchyWriter<string, TValue, LiteDbMutableNode<TValue>>();
            writer.ClearValue(this.GetOrCreateRootNode(), hierarchyPath);

            return writer.ValueWasCleared;
        }

        public bool RemoveNode(HierarchyPath<string> hierarchyPath, bool recurse)
        {
            if (hierarchyPath.IsRoot)
            {
                if (this.GetOrCreateRootNode().HasChildNodes && !recurse)
                    return false;

                this.Remove(hierarchyPath);
                return true; // even if it has no value.
            }
            return this.GetOrCreateRootNode().RemoveAllChildNodes(recurse);
        }

        /// <summary>
        /// Starts a traversal of the hierarchy at the specified hierachy node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<string, TValue> Traverse(HierarchyPath<string> startAt)
        {
            return ((IHierarchyNode<string, TValue>)new HierarchyTraverser<string, TValue, LiteDbMutableNode<TValue>>(this.GetOrCreateRootNode())).DescendantAt(startAt);
        }

        public bool TryGetValue(HierarchyPath<string> hierarchyPath, out TValue value)
        {
            if (this.GetOrCreateRootNode().TryGetDescendantAt(hierarchyPath, out var descendantNode))
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