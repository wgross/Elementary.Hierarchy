using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNode : IHasChildNodes<LiteDbHierarchyNode>, IHasIdentifiableChildNodes<string, LiteDbHierarchyNode>
    {
        private readonly ILiteDbHierarchyNodeRepository repository;
        private readonly LiteDbHierarchyNodeEntity innerNode;

        public LiteDbHierarchyNode(ILiteDbHierarchyNodeRepository repository, LiteDbHierarchyNodeEntity innerNode)
        {
            this.repository = repository;
            this.innerNode = innerNode;
            this.childNodes = this.CreateLazyChildNodes();
        }

        #region child nodes are read lazy

        private Lazy<IEnumerable<LiteDbHierarchyNode>> childNodes = null;

        private Lazy<IEnumerable<LiteDbHierarchyNode>> CreateLazyChildNodes() => new Lazy<IEnumerable<LiteDbHierarchyNode>>(valueFactory: () => ReadChildNodes(this.InnerNode.ChildNodes).ToList());

        private IEnumerable<LiteDbHierarchyNode> ReadChildNodes(IEnumerable<KeyValuePair<string, BsonValue>> ids) => ids
            .Select(kv => this.repository.Read(kv.Value))
            .Select(e => new LiteDbHierarchyNode(this.repository, e));

        #endregion child nodes are read lazy

        public string Key => this.InnerNode.Key;

        public LiteDbHierarchyNodeEntity InnerNode => this.innerNode;

        #region IHasChildNodes members

        public bool HasChildNodes => this.InnerNode.HasChildNodes;

        public IEnumerable<LiteDbHierarchyNode> ChildNodes => this.childNodes.Value;

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildNodes members

        public (bool, LiteDbHierarchyNode) TryGetChildNode(string key)
        {
            var child = this.ChildNodes.SingleOrDefault(n => n.Key.Equals(key));
            return (child != null, child);
        }

        #endregion IHasIdentifiableChildNodes members

        public LiteDbHierarchyNode AddChildNode(string key)
        {
            if (this.InnerNode.ChildNodeIds.ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate child node(key='{key}') under parent node(id='{this.InnerNode._Id}') was rejected.");
            }

            var (inserted, childId) = this.repository.TryInsert(new LiteDbHierarchyNodeEntity { Key = key });
            if (!inserted)
                return null;

            // checkpoint the inner node children in case update failes
            var innerNodeCheckpoint = new Dictionary<string, BsonValue>(this.InnerNode.ChildNodeIds);

            this.InnerNode.ChildNodeIds[key] = childId;
            if (!this.repository.Update(this.InnerNode))
            {
                // delete the orphaned node
                this.repository.Remove(childId);
                // restore the child node list
                this.InnerNode.ChildNodeIds = innerNodeCheckpoint;

                return null;
            }

            this.childNodes = this.CreateLazyChildNodes();
            return this.ChildNodes.Single(n => n.Key.Equals(key));
        }

        public bool RemoveChildNode(string key)
        {
            // remove child node from db
            if (this.InnerNode.ChildNodeIds.TryGetValue(key, out var childNodeId) && this.repository.Remove(childNodeId))
                // update parent node
                if (this.InnerNode.ChildNodeIds.Remove(key) && this.repository.Update(this.InnerNode))
                {
                    // cleanup cached transient data
                    var childNode = this.ChildNodes.SingleOrDefault(c => c.Key.Equals(key));
                    if (childNode != null)
                        this.childNodes = this.CreateLazyChildNodes();
                    return true;
                }

            return false;
        }
    }
}