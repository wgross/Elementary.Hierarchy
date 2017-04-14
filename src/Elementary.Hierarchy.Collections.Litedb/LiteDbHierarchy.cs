using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using LiteDB;
using System;

namespace Elementary.Hierarchy.Collections.LiteDb
{
    public class LiteDbHierarchy<TValue> : IHierarchy<string, TValue>
    {
        private readonly LiteCollection<BsonDocument> nodes;
        private LiteDbMutableNode<TValue> rootNode;

        public LiteDbHierarchy(LiteCollection<BsonDocument> nodes)
        {
            this.nodes = nodes;
        }

        #region IHierarchy Members

        public TValue this[HierarchyPath<string> hierarchyPath]
        {
            set
            {
                this.GetOrCreateNode(hierarchyPath).SetValue(value);
            }
        }

        public void Add(HierarchyPath<string> path, TValue value)
        {
            var nodeToSetValueAt = this.GetOrCreateNode(path);

            if (nodeToSetValueAt.HasValue)
                throw new ArgumentException($"{nameof(LiteDbHierarchy<TValue>)} at '{path}' already has a value", nameof(path));

            nodeToSetValueAt.SetValue(value);
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
            return ((IHierarchyNode<string, TValue>)new HierarchyTraverser<string, TValue, LiteDbMutableNode<TValue>>(this.rootNode)).DescendantAt(startAt);
        }

        public bool TryGetValue(HierarchyPath<string> hierarchyPath, out TValue value)
        {
            LiteDbMutableNode<TValue> descendantNode;
            if (this.GetOrCreateRootNode().TryGetDescendantAt(hierarchyPath, out descendantNode))
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

        private LiteDbMutableNode<TValue> GetOrCreateNode(HierarchyPath<string> hierarchyPath)
        {
            var writer = new GetOrCreateNodeHierarchyWriter<string, LiteDbMutableNode<TValue>>(createNode: key => new LiteDbMutableNode<TValue>(this.nodes, new BsonDocument(), key));

            writer.Visit(this.GetOrCreateRootNode(), hierarchyPath);

            return writer.DescandantAt;
        }
    }

    #endregion LiteDb access implementations
}