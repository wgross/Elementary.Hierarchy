namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extensions to classes implementing <see cref="IHasParentNode{TNode}"/> 
    /// </summary>
    public static class HasParentNodeExtensions
    {
        /// <summary>
        /// Returns the first node at the ancestor axis
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <returns>The parent node instance or default(TNode)</returns>
        public static TNode Parent<TNode>(this TNode startNode)
            where TNode : class, IHasParentNode<TNode>
        {
            //return startNode.Parent(n => n.HasParentNode ? n.ParentNode : null);
            return startNode.Parent((TNode n, out TNode p) =>
            {
                p = null;
                if (n.HasParentNode)
                    p = n.ParentNode;

                return (p != null);
            });
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{T}"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        public static IEnumerable<TNode> Ancestors<TNode>(this TNode startNode)
            where TNode : class, IHasParentNode<TNode>
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (!startNode.HasParentNode)
                yield break;

            TNode current = startNode;
            while (current != null && current.HasParentNode)
                yield return (current = current.ParentNode);
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The start node is returned as well.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        public static IEnumerable<TNode> AncestorsOrSelf<TNode>(this TNode startNode)
            where TNode : class, IHasParentNode<TNode>
        {
            return startNode.AncestorsOrSelf((TNode n, out TNode p) =>
            {
                p = null;
                if (n.HasParentNode)
                    p = n.ParentNode;

                return (p != null);
            });
        }
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Providing extsions to any type which implements the concept of 'having a parent node' with delegates
    /// </summary>
    public static class HasParentNodeGenericExtensions
    {
        /// <summary>
        /// Returns the first node at the ancestor axis
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="tryGetParentNode">Delegate to calculate the parent node of the star node</param>
        /// <returns>the parent Node or throws InvalidOperationException</returns>
        public static TNode Parent<TNode>(this TNode startNode, TryGetParent<TNode> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            TNode parentNode;
            if (tryGetParentNode(startNode, out parentNode))
                return parentNode;

            throw new InvalidOperationException($"{startNode} has no parent");
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The start node isn't returned. The structure of the tree is explored through the two given delegates.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="tryGetParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> Ancestors<TNode>(this TNode startNode, TryGetParent<TNode> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            TNode current = startNode;
            while (tryGetParentNode(current, out current))
                yield return current;
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{T}"/>.
        /// The start node is returned as well. The structure of the tree is explored through the two given delegates.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="tryGetParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> AncestorsOrSelf<TNode>(this TNode startNode, TryGetParent<TNode> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            return Enumerable.Repeat(startNode, 1).Union(startNode.Ancestors(tryGetParentNode));
        }
    }
}