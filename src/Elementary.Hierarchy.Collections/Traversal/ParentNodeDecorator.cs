using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Traversal
{
    /// <summary>
    /// A parent node decorator extends a class implementing <see cref="IHasChildNodes{TInnerNode}"/> interface
    /// an implementation of <see cref="IHasParentNode{TNode}"/>.
    /// The decorator delegates travesal to childnodes to the wrapped <typeparamref name="TInnerNode"/> instance.
    /// Traversal to the parent nodes is possible because the decorator remombers the traversed nodes during
    /// child node traversal.
    /// </summary>
    /// <typeparam name="TInnerNode">decorated node type</typeparam>
    public abstract class ParentNodeDecorator<TOuterNode, TInnerNode> :
        IHasChildNodes<TOuterNode>,
        IHasParentNode<TOuterNode>
        where TInnerNode : IHasChildNodes<TInnerNode>
        where TOuterNode : IHasChildNodes<TOuterNode>, IHasParentNode<TOuterNode>
    {
        private readonly TInnerNode decoratedNode;
        private readonly Func<bool> hasParentNode;
        private readonly Func<TOuterNode> getParentNode;

        /// <summary>
        /// Creates a decorator of the given node.
        /// </summary>
        /// <param name="decoratedNode"></param>
        public ParentNodeDecorator(TInnerNode decoratedNode)
            : this(decoratedNode, hasParentNode: null, getParentNode: null)
        { }

        /// <summary>
        /// Creates a decorator of the given node. The delegates <paramref name="getParentNode"/> and <paramref name="hasParentNode"/>
        /// are used for paranet node traversal.
        /// </summary>
        /// <param name="decoratedNode"></param>
        /// <param name="hasParentNode">returns true, if a parent ode is known. If set null, a parent node is unkown</param>
        /// <param name="getParentNode">returns a decorator of the parent node. If null, <see cref="InvalidOperationException"/> is thrown</param>
        public ParentNodeDecorator(TInnerNode decoratedNode, Func<bool> hasParentNode, Func<TOuterNode> getParentNode)
        {
            this.decoratedNode = decoratedNode;
            this.hasParentNode = hasParentNode ?? (() => false);
            this.getParentNode = getParentNode ?? (() => throw new InvalidOperationException("no parent"));
        }

        /// <summary>
        /// Allows access to the decorated node instance.
        /// </summary>
        public TInnerNode InnerNode => this.decoratedNode;

        protected abstract TOuterNode DecorateChildNode(TInnerNode node);

        /// <summary>
        /// Indecates if the node has child nodes
        /// </summary>
        public bool HasChildNodes => this.decoratedNode.HasChildNodes;

        /// <summary>
        /// Indicates of this node has a parent node.
        /// </summary>
        public bool HasParentNode => this.hasParentNode();

        /// <summary>
        /// Allows access to the parent node decorator if available
        /// </summary>
        public TOuterNode ParentNode => this.getParentNode();

        /// <summary>
        /// Allows access to the child nodes of the decorated node instance.
        /// </summary>
        public IEnumerable<TOuterNode> ChildNodes => this.decoratedNode.ChildNodes.Select(c => this.DecorateChildNode(c));

        /// <summary>
        /// The test of equality is delegated to the decorated node instance.
        /// Two instances of <see cref="ParentNodeDecorator{TOuterNode, TInnerNode}"/> are equal if the decorated TInnerNode instances are equal.
        /// </summary>
        /// <param name="obj">obect to compare this instance with</param>
        /// <returns>true if the decoerated node instances </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var objAsTraverser = obj as ParentNodeDecorator<TOuterNode, TInnerNode>;
            if (objAsTraverser == null)
                return false;

            // Traversers are equals if the point to the same node
            return this.decoratedNode.Equals(objAsTraverser.InnerNode);
        }

        /// <summary>
        /// Delegates the calculation of the hashcode to teh underlying node instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.decoratedNode.GetHashCode();
        }
    }
}