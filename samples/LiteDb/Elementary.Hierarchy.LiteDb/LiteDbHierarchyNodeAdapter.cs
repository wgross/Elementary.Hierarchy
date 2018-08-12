using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNodeAdapter : IHasChildNodes<LiteDbHierarchyNodeAdapter>
    {
        private readonly ICollection<LiteDbHierarchyNodeAdapter> childNodes = new List<LiteDbHierarchyNodeAdapter>();
        private readonly ILiteDbHierarchyNodeRepository repository;
        private readonly LiteDbHierarchyNode innerNode;

        public LiteDbHierarchyNodeAdapter(ILiteDbHierarchyNodeRepository repository, LiteDbHierarchyNode innerNode)
        {
            this.repository = repository;
            this.innerNode = innerNode;
        }

        public string Key => this.InnerNode.Key;

        public LiteDbHierarchyNode InnerNode => this.innerNode;

        private IDictionary<string, BsonValue> InnerNodeChildNodes => this.InnerNode._ChildNodeIds;

        public bool HasChildNodes => this.ChildNodes.Any();

        public IEnumerable<LiteDbHierarchyNodeAdapter> ChildNodes => this.childNodes;

        public LiteDbHierarchyNodeAdapter AddChildNode(string key)
        {
            if (this.InnerNodeChildNodes.ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate child node(key='{key}') under parent node(id='{this.InnerNode._Id}') was rejected.");
            }

            var child = new LiteDbHierarchyNodeAdapter(this.repository, new LiteDbHierarchyNode { Key = key });
            var (inserted, childId) = this.repository.TryInsert(child.InnerNode);

            this.InnerNode._ChildNodeIds[key] = childId;
            this.repository.Update(this.InnerNode);
            this.childNodes.Add(child);
            return child;
        }

        public bool RemoveChildNode(string key)
        {
            // remove child node from db
            if (this.InnerNodeChildNodes.TryGetValue(key, out var childNodeId) && this.repository.Remove(childNodeId))
                // update parent node
                if (this.InnerNodeChildNodes.Remove(key) && this.repository.Update(this.InnerNode))
                {
                    // cleanup cached transient data
                    var childNode = this.childNodes.SingleOrDefault(c => c.Key.Equals(key));
                    if (childNode != null)
                        this.childNodes.Remove(childNode);
                    return true;
                }

            return false;
        }
    }
}