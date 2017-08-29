using System;
using Elementary.Hierarchy.Collections.Nodes;

namespace Elementary.Hierarchy.Collections.Traversal
{
    public class HierarchyTraverser<TKey, TValue, TNode> : ParentNodeDecorator<IHierarchyNode<TKey, TValue>, TNode>,
        IHierarchyNode<TKey, TValue>
        where TNode :
            IHierarchyValueReader<TValue>,
            IHierarchyKeyReader<TKey>,
            IHasIdentifiableChildNodes<TKey, TNode>,
            IHasChildNodes<TNode>
    {
        #region Construction and initialization of this instance

        public HierarchyTraverser(TNode node)
            : base(decoratedNode: node)
        {
            this.Path = HierarchyPath.Create<TKey>();
        }

        public HierarchyTraverser(HierarchyTraverser<TKey, TValue, TNode> parentTraverser, TNode node)
            : base(decoratedNode: node, parentNode: parentTraverser)
        {
            if (parentTraverser == null)
                throw new ArgumentNullException(nameof(parentTraverser));

            if (!this.InnerNode.TryGetKey(out var key))
                throw new ArgumentException("child node must have a key", nameof(node));

            this.Path = this.ParentNode.Path.Join(key);
        }

        protected override IHierarchyNode<TKey, TValue> DecorateChildNode(TNode node)
        {
            return new HierarchyTraverser<TKey, TValue, TNode>(this, node);
        }

        #endregion Construction and initialization of this instance

        public (bool, IHierarchyNode<TKey, TValue>) TryGetChildNode(TKey id)
        {
            var (found, innerChildNode) = this.InnerNode.TryGetChildNode(id);
            if (!found)
                return (false, null);
            return (true, new HierarchyTraverser<TKey, TValue, TNode>(this, innerChildNode));
        }

        public bool TryGetValue(out TValue value)
        {
            return this.InnerNode.TryGetValue(out value);
        }

        public HierarchyPath<TKey> Path
        {
            get;

            private set;
        }
    }
}