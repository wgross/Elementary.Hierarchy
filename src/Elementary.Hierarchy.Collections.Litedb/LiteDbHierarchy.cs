using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.LiteDb
{
    public class LiteDbHierarchy<TValue> : IHierarchy<string, TValue>
    {
        private class Node : IHierarchyNode<string, TValue>, IHasIdentifiableChildNodes<string, IHierarchyNode<string, TValue>>
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

            public IEnumerable<IHierarchyNode<string, TValue>> ChildNodes
                => Wrap(this.nodes.Find(Query.EQ("parent", this.currentNode.Get("_id"))));

            public bool HasChildNodes => this.nodes.Exists(Query.EQ("parent", this.currentNode.Get("_id")));

            #endregion IHasChildNodes Members

            public HierarchyPath<string> Path
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

            public IHierarchyNode<string, TValue> ParentNode => Wrap(this.nodes.FindOne(Query.EQ("_id", this.ParentId)));

            #endregion IHasParentNode Members

            #region IHasIdentifiableChildNodes Members

            public BsonValue Key => this.currentNode.Get("key");

            public bool TryGetChildNode(string key, out IHierarchyNode<string, TValue> childNode)
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
        private LiteDbMutableNode<TValue> rootNode;

        public LiteDbHierarchy(LiteDatabase database)
        {
            this.database = database;
            this.nodes = this.database.GetCollection("nodes_collection");
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
            value = default(TValue);
            return false;
        }

        #endregion IHierarchy Members

        #region LiteDb access implementations

        private LiteDbMutableNode<TValue> GetOrCreateRootNode()
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

            this.rootNode = new LiteDbMutableNode<TValue>(this.nodes, rootDocument);
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