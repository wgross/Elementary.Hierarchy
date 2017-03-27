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
        where TOuterNode : class, IHasChildNodes<TOuterNode>, IHasParentNode<TOuterNode>
    {
        private readonly TOuterNode parentNode;
        private readonly TInnerNode innerNode;

        /// <summary>
        /// Creates a decorator of the given root node.
        /// </summary>
        /// <param name="decoratedNode"></param>
        public ParentNodeDecorator(TInnerNode decoratedNode)
            : this(decoratedNode, parentNode: null)
        { }

        /// <summary>
        /// Creates a decorator of the given inner node.
        /// The decorrator receoves in addition the parentnode
        /// are used for paranet node traversal.
        /// </summary>
        /// <param name="decoratedNode">node which is decorated with a parent</param>
        /// <param name="parentNode">parent node of this hierarchy node</param>
        public ParentNodeDecorator(TInnerNode decoratedNode, TOuterNode parentNode)
        {
            this.innerNode = decoratedNode;
            this.parentNode = parentNode;
        }

        /// <summary>
        /// Allows access to the decorated node instance.
        /// </summary>
        public TInnerNode InnerNode => this.innerNode;

        /// <summary>
        /// This mothod has tobe implemented to implement the decoration of a child node of this instances inner node
        /// to an instance of the outer node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected abstract TOuterNode DecorateChildNode(TInnerNode node);

        /// <summary>
        /// Indecates if the node has child nodes
        /// </summary>
        public bool HasChildNodes => this.innerNode.HasChildNodes;

        /// <summary>
        /// Indicates of this node has a parent node.
        /// </summary>
        public bool HasParentNode => this.parentNode != null;

        /// <summary>
        /// Allows access to the parent node decorator if available
        /// </summary>
        public TOuterNode ParentNode => this.parentNode;

        /// <summary>
        /// Allows access to the child nodes of the decorated node instance.
        /// </summary>
        public IEnumerable<TOuterNode> ChildNodes => this.innerNode.ChildNodes.Select(c => this.DecorateChildNode(c));

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
            return this.innerNode.Equals(objAsTraverser.InnerNode);
        }

        /// <summary>
        /// Delegates the calculation of the hashcode to teh underlying node instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.innerNode.GetHashCode();
        }
    }
}