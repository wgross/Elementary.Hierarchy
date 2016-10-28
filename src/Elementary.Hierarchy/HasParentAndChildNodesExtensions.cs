namespace Elementary.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extensions to classes implementing <see cref="IHasChildNodes{TNode}"/> and <see cref="IHasParentNode{TNode}"/>.
    /// </summary>
    public static class HasParentAndChildNodesExtensions
    {
        /// <summary>
        /// Traverses the start nodes parent child nodes beginning with the first sibling following the start node.
        /// The siblings are returned inside a <see cref="IEnumerable{TNode}"/>.
        /// The start node is not returned.
        /// </summary>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes without the start node.
        /// </returns>
        public static IEnumerable<TNode> FollowingSiblings<TNode>(this TNode startNode)
            where TNode : class, IHasChildNodes<TNode>, IHasParentNode<TNode>
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (!startNode.HasParentNode)
                return Enumerable.Empty<TNode>();

            // no check for 'hasChildNodes' obviously because the startNode is already a child node.
            return startNode.ParentNode.ChildNodes.SkipWhile(n => !n.Equals(startNode)).Skip(1);
        }

        /// <summary>
        /// Traverses the start nodes parent child node beginning with the first child until the start node is reached.
        /// The siblings are returned inside a <see cref="IEnumerable{TNode}"/>/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        public static IEnumerable<TNode> PrecedingSiblings<TNode>(this TNode startNode)
            where TNode : class, IHasChildNodes<TNode>, IHasParentNode<TNode>
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (!startNode.HasParentNode)
                return Enumerable.Empty<TNode>();

            return startNode.ParentNode.ChildNodes.TakeWhile(n => !n.Equals(startNode));
        }
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extsnsions to any type which may implement the cocept of 'having a parent node and children' with delegates.
    /// </summary>
    public static class HasParentAndChildNodeGenericExtensions
    {
        /// <summary>
        /// Traverses the start nodes parent child nodes beginning with the first sibling following the start node.
        /// The siblings are returned inside a <see cref="IEnumerable{T}"/>/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes without the <paramref name="startNode"/>.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="getChildren">Delegate to retrieve a nodes children</param>
        /// <param name="tryGetParent">Delegate to retrieve a nodes parent</param>
        public static IEnumerable<TNode> FollowingSiblings<TNode>(this TNode startNode, TryGetParent<TNode> tryGetParent, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            if (tryGetParent == null)
                throw new ArgumentNullException(nameof(tryGetParent));

            if (getChildren == null)
                throw new ArgumentNullException(nameof(getChildren));

            // if there is no parnet node, no sbilings are enumerated.
            TNode parentNode;
            if (tryGetParent(startNode, out parentNode))
                return getChildren(parentNode).SkipWhile(n => !n.Equals(startNode)).Skip(1);

            return Enumerable.Empty<TNode>();
        }

        /// <summary>
        /// Traverses the <paramref name="startNode"/>s parent child nodes beginning with the first child of the parent untol the start node is reached.
        /// The siblings are returned inside a <see cref="IEnumerable{TNode}"/>/>.
        /// The <paramref name="startNode"/> is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="getChildren">Delegate to retrieve a nodes children</param>
        /// <param name="tryGetParent">Delegate to retrieve a nodes parent</param>
        public static IEnumerable<TNode> PrecedingSiblings<TNode>(this TNode startNode, TryGetParent<TNode> tryGetParent, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            if (tryGetParent == null)
                throw new ArgumentNullException(nameof(tryGetParent));

            if (getChildren == null)
                throw new ArgumentNullException(nameof(getChildren));

            // if there is no parnet node, no sbilings are enumerated.
            TNode parentNode;
            if (tryGetParent(startNode, out parentNode))
                return getChildren(parentNode).TakeWhile(n => !n.Equals(startNode));

            return Enumerable.Empty<TNode>();
        }
    }
}