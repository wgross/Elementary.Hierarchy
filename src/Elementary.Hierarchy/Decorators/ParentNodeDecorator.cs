using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Decorators
{
    /// <summary>
    /// A parent node decorator extends a class implementing <see cref="IHasChildNodes{TNode}"/> interface
    /// an implementation of <see cref="IHasParentNode{TNode}"/>.
    /// The decorator delegates travesal to childnodes to the wrapped <typeparamref name="TNode"/> instance.
    /// Traversal to the parent nodes is possible because the decorator remombers the traversed nodes during
    /// child node traversal.
    /// </summary>
    /// <typeparam name="TNode">decorated node type</typeparam>
    public class ParentNodeDecorator<TNode> : IHasChildNodes<ParentNodeDecorator<TNode>>, IHasParentNode<ParentNodeDecorator<TNode>>
        where TNode : IHasChildNodes<TNode>
    {
        private readonly TNode decoratedNode;
        private readonly Func<bool> hasParentNode;
        private readonly Func<ParentNodeDecorator<TNode>> getParentNode;

        /// <summary>
        /// Creates a decorator of the given node.
        /// </summary>
        /// <param name="decoratedNode"></param>
        public ParentNodeDecorator(TNode decoratedNode)
            : this(decoratedNode, hasParentNode: null, getParentNode: null)
        { }

        /// <summary>
        /// Creates a decorator of the given node. The delegates <paramref name="getParentNode"/> and <paramref name="hasParentNode"/>
        /// are used for paranet node traversal.
        /// </summary>
        /// <param name="decoratedNode"></param>
        /// <param name="hasParentNode">returns true, if a parent ode is known. If set null, a parent node is unkown</param>
        /// <param name="getParentNode">returns a decorator of the parent node. If null, <see cref="InvalidOperationException"/> is thrown</param>
        public ParentNodeDecorator(TNode decoratedNode, Func<bool> hasParentNode, Func<ParentNodeDecorator<TNode>> getParentNode)
        {
            this.decoratedNode = decoratedNode;
            this.hasParentNode = hasParentNode ?? (() => false);
            this.getParentNode = getParentNode ?? (() => throw new InvalidOperationException("no parent"));
        }

        /// <summary>
        /// Allows access to the decorated node instance.
        /// </summary>
        public TNode DecoratedNode => this.decoratedNode;

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
        public ParentNodeDecorator<TNode> ParentNode => this.getParentNode();

        /// <summary>
        /// Allows access to the child nodes of the decorated node instance.
        /// </summary>
        public IEnumerable<ParentNodeDecorator<TNode>> ChildNodes => this.decoratedNode.ChildNodes.Select(c => new ParentNodeDecorator<TNode>(c, () => true, () => this));

        /// <summary>
        /// The test of equality is delegated to the decoratied node instance.
        /// Two instances of <see cref="ParentNodeDecorator{TNode}"/> are equal if the decorated TNode instances are equal.
        /// </summary>
        /// <param name="obj">obect to compare this instance with</param>
        /// <returns>true if the decoerated node instances </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var objAsTraverser = obj as ParentNodeDecorator<TNode>;
            if (objAsTraverser == null)
                return false;

            // Traversers are equals if the point to the same node
            return this.decoratedNode.Equals(objAsTraverser.DecoratedNode);
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