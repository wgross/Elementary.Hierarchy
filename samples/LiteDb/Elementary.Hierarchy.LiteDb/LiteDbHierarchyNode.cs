using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNode : IHasChildNodes<LiteDbHierarchyNode>, IHasIdentifiableChildNodes<string, LiteDbHierarchyNode>
    {
        private readonly ILiteDbHierarchyNodeRepository repository;
        private Lazy<IEnumerable<LiteDbHierarchyNode>> childNodes = null;

        public LiteDbHierarchyNode(ILiteDbHierarchyNodeRepository repository, LiteDbHierarchyNodeEntity innerNode)
        {
            this.repository = repository;
            this.InnerNode = innerNode;
            this.childNodes = this.CreateLazyChildNodes();
        }

        public LiteDbHierarchyNode(ILiteDbHierarchyNodeRepository repository, LiteDbHierarchyNodeEntity innerNode, LiteDbHierarchyValueEntity innerValue)
            : this(repository, innerNode)
        {
            this.InnerValue = innerValue;
        }

        #region child nodes are read lazy

        private Lazy<IEnumerable<LiteDbHierarchyNode>> CreateLazyChildNodes() => new Lazy<IEnumerable<LiteDbHierarchyNode>>(valueFactory: () => ReadChildNodes(this.InnerNode.ChildNodes).ToList());

        private IEnumerable<LiteDbHierarchyNode> ReadChildNodes(IEnumerable<KeyValuePair<string, BsonValue>> ids) => ids
            .Select(kv => this.repository.Read(kv.Value))
            .Select(e => new LiteDbHierarchyNode(this.repository, e));

        #endregion child nodes are read lazy

        public string Key => this.InnerNode.Key;

        public LiteDbHierarchyNodeEntity InnerNode { get; }

        public LiteDbHierarchyValueEntity InnerValue { get; private set; }

        public void SetValue<T>(T value)
        {
            var valueEntity = this.InnerValue ?? new LiteDbHierarchyValueEntity();
            if (valueEntity.SetValue(value))
            {
                // value has changed
                this.repository.Upsert(valueEntity);
                this.InnerValue = valueEntity;
            }
            if (this.InnerNode.ValueRef is null || this.InnerNode.ValueRef.CompareTo(valueEntity._Id) != 0)
            {
                // value is new
                this.InnerNode.ValueRef = this.InnerValue._Id;
                this.repository.Update(this.InnerNode);
            }
        }

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
                this.repository.DeleteNode(childId, false);
                // restore the child node list
                this.InnerNode.ChildNodeIds = innerNodeCheckpoint;

                return null;
            }

            this.childNodes = this.CreateLazyChildNodes();
            return this.ChildNodes.Single(n => n.Key.Equals(key));
        }

        public bool RemoveChildNode(string key)
        {
            var (exists, childNode) = this.TryGetChildNode(key);
            if (!exists)
                return false;

            if (childNode.Delete())
                if (this.InnerNode.ChildNodeIds.Remove(key))
                    if (this.repository.Update(this.InnerNode))
                    {
                        this.childNodes = this.CreateLazyChildNodes();
                        return true;
                    }

            return false;
        }

        public (bool, object) TryGetValue()
        {
            if (this.InnerNode.ValueRef != null)
                this.InnerValue = this.repository.ReadValue(this.InnerNode.ValueRef);

            return (this.InnerValue != null, this.InnerValue?.Value.RawValue);
        }

        public bool Delete()
        {
            var valueRemoved = false;
            if (this.InnerNode.ValueRef != null && this.InnerNode.ValueRef.AsObjectId != ObjectId.Empty)
                valueRemoved = this.repository.DeleteValue(this.InnerNode.ValueRef);
            return this.repository.DeleteNode(this.InnerNode._Id, this.InnerNode.HasChildNodes) || valueRemoved;
        }
    }
}