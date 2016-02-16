namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;

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
            return startNode.Parent(n => n.HasParentNode ? n.ParentNode : null);
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is not returned.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containeing all visited nodes without the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
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
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is returned as well.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        public static IEnumerable<TNode> AncestorsOrSelf<TNode>(this TNode startNode)
            where TNode : class, IHasParentNode<TNode>
        {
            return startNode.AncestorsOrSelf(n => n.HasParentNode ? n.ParentNode : null);
        }
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HasParentNodeGenericExtensions
    {
        /// <summary>
        /// Returns the first node at the ancestor axis
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <returns></returns>
        public static TNode Parent<TNode>(this TNode startNode, Func<TNode, TNode> getParentNode)
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (getParentNode == null)
                throw new ArgumentNullException(nameof(getParentNode));

            return getParentNode(startNode);
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node isn't returned. The structure of the tree is explored through the two given delegates.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="hasParentNode">returns true if the inspected node has a parent node</param>
        /// <param name="getParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> Ancestors<TNode>(this TNode startNode, Func<TNode, TNode> getParentNode)
            where TNode : class
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (getParentNode == null)
                throw new ArgumentNullException(nameof(getParentNode));

            TNode current = startNode;
            while (current != null)
            {
                current = getParentNode(current);
                if (current == null)
                    yield break;
                else
                    yield return current;
            }
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable"/> of <see cref="TNode"/>.
        /// The start node is returned as well. The structure of the tree is explored through the two given delegates.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> of <see cref="TNode"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="hasParentNode">returns true if the inspected node has a parent node</param>
        /// <param name="getParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> AncestorsOrSelf<TNode>(this TNode startNode, Func<TNode, TNode> getParentNode)
            where TNode : class
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));

            if (getParentNode == null)
                throw new ArgumentNullException(nameof(getParentNode));

            return Enumerable.Repeat(startNode, 1).Union(startNode.Ancestors(getParentNode));
        }
    }
}