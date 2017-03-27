using Elementary.Hierarchy.Collections.Nodes;
using System;

namespace Elementary.Hierarchy.Collections.Traversal
{
    public class HierarchyTraverser<TKey, TValue, TNode> : ParentNodeDecorator<IHierarchyNode<TKey, TValue>, TNode>,
        IHierarchyNode<TKey, TValue>
        where TNode :
            IHierarchyValueReader<TValue>,
            IHierarchyKeyReader<TKey>,
            IHasIdentifiableChildNodes<TKey, TNode>, IHasChildNodes<TNode>
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

            this.InnerNode.TryGetKey(out var key);
            this.Path = this.ParentNode.Path.Join(key);
        }

        protected override IHierarchyNode<TKey, TValue> DecorateChildNode(TNode node)
        {
            return new HierarchyTraverser<TKey, TValue, TNode>(this, node);
        }

        #endregion Construction and initialization of this instance

        public bool TryGetChildNode(TKey id, out IHierarchyNode<TKey, TValue> childNode)
        {
            childNode = null;
            if (!this.InnerNode.TryGetChildNode(id, out var innerChildNode))
                return false;
            childNode = new HierarchyTraverser<TKey, TValue, TNode>(this, innerChildNode);
            return true;
        }

        public HierarchyPath<TKey> Path
        {
            get;

            private set;
        }

        public bool HasValue => this.InnerNode.TryGetValue(out var _);

        public TValue Value => this.InnerNode.TryGetValue(out var value) ? value : throw new InvalidOperationException("node has no value");
    }
}