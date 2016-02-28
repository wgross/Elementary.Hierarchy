namespace Elementary.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HasParentAndChildNodesExtensions
    {
        /// <summary>
        /// Traverses the start nodes parent child nodes beginning with the first sibling following the start node.
        /// The siblings are returned inside a <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
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
        /// The siblings are returned inside a <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
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

    public static class HasParentAndChildNodeGenericExtensions
    {
        /// <summary>
        /// Traverses the start nodes parent child nodes beginning with the first sibling following the start node.
        /// The siblings are returned inside a <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
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
        /// Traverses the start nodes parent child nodes beginning with the first child of the parent untol the start node is reached.
        /// The siblings are returned inside a <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
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