using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Traversal
{
    public class HierarchyTraverser<TKey, TValue, TNode> :
        IHierarchyNode<TKey, TValue>
        where TNode :
            IHierarchyValueReader<TValue>,
            IHierarchyKeyReader<TKey>,
            IHasIdentifiableChildNodes<TKey, TNode>, IHasChildNodes<TNode>
    {
        private readonly ParentNodeDecorator<TNode> decorator;

        #region Construction and initialization of this instance

        public HierarchyTraverser(TNode node)
        {
            this.decorator = new ParentNodeDecorator<TNode>(node, hasParentNode: () => false, getParentNode: null);
            this.Path = HierarchyPath.Create<TKey>();
        }

        public HierarchyTraverser(HierarchyTraverser<TKey, TValue, TNode> parentTraverser, TNode node)
        {
            if (parentTraverser == null)
                throw new ArgumentNullException(nameof(parentTraverser));

            this.decorator = new ParentNodeDecorator<TNode>(node, hasParentNode: () => true, getParentNode: () => parentTraverser.decorator);
            this.decorator.DecoratedNode.TryGetKey(out var key);
            this.Path = this.ParentNode.Path.Join(key);
        }

        #endregion Construction and initialization of this instance

        #region Override object behavior

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var objAsTraverser = obj as HierarchyTraverser<TKey, TValue, TNode>;
            if (objAsTraverser == null)
                return false;

            return this.decorator.Equals(objAsTraverser.decorator);
        }

        public override int GetHashCode()
        {
            return this.decorator.GetHashCode();
        }

        #endregion Override object behavior

        public bool TryGetChildNode(TKey id, out IHierarchyNode<TKey, TValue> childNode)
        {
            childNode = null;
            if (!this.decorator.DecoratedNode.TryGetChildNode(id, out var innerChildNode))
                return false;
            childNode = new HierarchyTraverser<TKey, TValue, TNode>(this, innerChildNode);
            return true;
        }

        public HierarchyPath<TKey> Path
        {
            get;

            private set;
        }

        public bool HasValue => this.decorator.DecoratedNode.TryGetValue(out var _);

        public TValue Value
        {
            get
            {
                if (!this.decorator.DecoratedNode.TryGetValue(out var value))
                    throw new InvalidOperationException("node has no value");

                return value;
            }
        }

        public bool HasChildNodes => this.decorator.HasChildNodes;

        public IEnumerable<IHierarchyNode<TKey, TValue>> ChildNodes => this.decorator.ChildNodes.Select(n => new HierarchyTraverser<TKey, TValue, TNode>(this, n.DecoratedNode));

        public bool HasParentNode => this.decorator.HasParentNode;

        public IHierarchyNode<TKey, TValue> ParentNode => new HierarchyTraverser<TKey, TValue, TNode>(this.decorator.ParentNode.DecoratedNode);
    }
}