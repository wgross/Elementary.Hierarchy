using LiteDB;
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
            var child = new LiteDbHierarchyNodeAdapter(this.repository, new LiteDbHierarchyNode { Key = key });
            var (inserted, childId) = this.repository.TryInsert(child.InnerNode);
            this.InnerNode._ChildNodeIds[key] = childId;
            this.repository.Update(this.InnerNode);
            this.childNodes.Add(child);
            return child;
        }

        public bool RemoveChildNode(LiteDbHierarchyNodeAdapter child)
        {
            if (this.InnerNodeChildNodes.Remove(child.Key))
                if (this.childNodes.Remove(child))
                    return true;
            return false;
        }
    }
}