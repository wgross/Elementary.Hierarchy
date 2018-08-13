using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNode : IHasChildNodes<LiteDbHierarchyNode>
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

        private Lazy<IEnumerable<LiteDbHierarchyNode>> CreateLazyChildNodes() => new Lazy<IEnumerable<LiteDbHierarchyNode>>(valueFactory: () => ReadChildNodes(this.InnerNode.ChildNodeIds).ToList());

        private IEnumerable<LiteDbHierarchyNode> ReadChildNodes(IDictionary<string, BsonValue> ids) => ids
            .Select(kv => this.repository.Read(kv.Value))
            .Select(e => new LiteDbHierarchyNode(this.repository, e));

        #endregion child nodes are read lazy

        public string Key => this.InnerNode.Key;

        public LiteDbHierarchyNodeEntity InnerNode => this.innerNode;

        public bool HasChildNodes => this.InnerNode.ChildNodeIds.Any();

        public IEnumerable<LiteDbHierarchyNode> ChildNodes => this.childNodes.Value;

        public LiteDbHierarchyNode AddChildNode(string key)
        {
            if (this.InnerNode.ChildNodeIds.ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate child node(key='{key}') under parent node(id='{this.InnerNode._Id}') was rejected.");
            }

            var child = new LiteDbHierarchyNode(this.repository, new LiteDbHierarchyNodeEntity { Key = key });
            var (inserted, childId) = this.repository.TryInsert(child.InnerNode);

            this.InnerNode.ChildNodeIds[key] = childId;
            this.repository.Update(this.InnerNode);
            this.childNodes = this.CreateLazyChildNodes();
            return child;
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