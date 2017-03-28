using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.LiteDb
{
    public class LiteDbHierarchy<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        private class Node : IHierarchyNode<TKey, TValue>, IHasIdentifiableChildNodes<TKey, IHierarchyNode<TKey, TValue>>
        {
            private readonly LiteCollection<BsonDocument> nodes;
            private BsonDocument currentNode;

            private Node Wrap(BsonDocument document)
            {
                return new Node(this.nodes, document);
            }

            private IEnumerable<Node> Wrap(IEnumerable<BsonDocument> documents)
            {
                return documents.Select(b => Wrap(b));
            }

            public Node(LiteCollection<BsonDocument> nodes, BsonDocument currentNode)
            {
                this.currentNode = currentNode;
                this.nodes = nodes;
            }

            #region IHasChildNodes Members

            public IEnumerable<IHierarchyNode<TKey, TValue>> ChildNodes
                => Wrap(this.nodes.Find(Query.EQ("parent", this.currentNode.Get("_id"))));

            public bool HasChildNodes => this.nodes.Exists(Query.EQ("parent", this.currentNode.Get("_id")));

            #endregion IHasChildNodes Members

            public HierarchyPath<TKey> Path
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool HasValue => this.currentNode.ContainsKey("value");

            public TValue Value => (TValue)this.currentNode.Get("value").RawValue;

            #region IHasParentNode Members

            private BsonValue ParentId => this.currentNode.Get("parent");

            public bool HasParentNode => this.nodes.Exists(Query.EQ("_id", this.ParentId));

            public IHierarchyNode<TKey, TValue> ParentNode => Wrap(this.nodes.FindOne(Query.EQ("_id", this.ParentId)));

            #endregion IHasParentNode Members

            #region IHasIdentifiableChildNodes Members

            public BsonValue Key => this.currentNode.Get("key");

            public bool TryGetChildNode(TKey key, out IHierarchyNode<TKey, TValue> childNode)
            {
                childNode = null;
                var node = this.nodes
                    .Find(Query.And(
                        Query.EQ("parent", this.currentNode.Get("_id")),
                        Query.EQ("key", new BsonValue(key))))
                    .SingleOrDefault();

                if (node == null)
                    return false;

                childNode = Wrap(node);
                return true;
            }

            internal void AddChildNode(Node node)
            {
                throw new NotImplementedException();
            }

            #endregion IHasIdentifiableChildNodes Members
        }

        private LiteDatabase database;
        private readonly LiteCollection<BsonDocument> nodes;
        private LiteDbMutableNode<TKey, TValue> rootNode;

        public LiteDbHierarchy(LiteDatabase database)
        {
            this.database = database;
            this.nodes = this.database.GetCollection("nodes_collection");
        }

        #region IHierarchy Members

        public TValue this[HierarchyPath<TKey> hierarchyPath]
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(HierarchyPath<TKey> path, TValue value)
        {
            var nodeToSetValueAt = this.GetOrCreateNode(path);

            if (nodeToSetValueAt.HasValue)
                throw new ArgumentException($"{nameof(LiteDbHierarchy<TKey, TValue>)} at '{path}' already has a value", nameof(path));

            nodeToSetValueAt.SetValue(value);
        }

        public bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = default(int?))
        {
            throw new NotImplementedException();
        }

        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            throw new NotImplementedException();
        }

        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            return null;
        }

        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        #endregion IHierarchy Members

        #region LiteDb access implementations

        private LiteDbMutableNode<TKey, TValue> GetOrCreateRootNode()
        {
            if (this.rootNode != null)
                return this.rootNode;

            // find the underlying root document

            var rootDocument = this.nodes.FindOne(Query.EQ("key", null));
            if (rootDocument == null)
            {
                rootDocument = new BsonDocument();
                rootDocument.Set("key", null);
                this.nodes.Insert(rootDocument);
            }

            // create a node representing the document and return it

            this.rootNode = new LiteDbMutableNode<TKey, TValue>(OnDocumentChanged);
            return this.rootNode;
        }

        private LiteDbMutableNode<TKey, TValue> GetOrCreateNode(HierarchyPath<TKey> hierarchyPath)
        {
            var writer = new GetOrCreateNodeHierarchyWriter<TKey, LiteDbMutableNode<TKey, TValue>>(createNode: key => new LiteDbMutableNode<TKey, TValue>(OnDocumentChanged, key));

            writer.Visit(this.GetOrCreateRootNode(), hierarchyPath);

            return writer.DescandantAt;
        }

        private void OnDocumentChanged()
        {
        }
    }

    #endregion LiteDb access implementations
}