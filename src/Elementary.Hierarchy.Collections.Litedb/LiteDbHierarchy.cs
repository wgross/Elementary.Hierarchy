using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
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
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool RemoveNode(HierarchyPath<string> hierarchyPath, bool recurse)
        {
            throw new NotImplementedException();
        }

        public IHierarchyNode<string, TValue> Traverse(HierarchyPath<string> startAt)
        {
            return null;
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