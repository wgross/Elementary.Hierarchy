namespace Elementary.Hierarchy
{
    using System.Collections.Generic;
    using Elementary.Hierarchy.Generic;

    /// <summary>
    /// Provides extensions to classes implementing <see cref="IHasParentNode{TNode}"/>
    /// </summary>
    public static class HasParentNodeExtensions
    {
        /// <summary>
        /// Returns the parent node of the <paramref name="startNode"/>.
        /// If the node hasn't a parent an <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <returns>A parent node instance</returns>
        public static TNode Parent<TNode>(this TNode startNode)
            where TNode : class, IHasParentNode<TNode>
        {
            return startNode.Parent((TNode n) =>
            {
                if (!n.HasParentNode)
                    return (false, default(TNode));
                var parent = n.ParentNode;
                return (parent != null, parent);
                
            });
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The <paramref name="startNode"/> isn't returned.
        /// </summary>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes but not <paramref name="startNode"/>.
        /// If the <paramref name="startNode"/> has no parent the collection is empty.
        /// </returns>
        public static IEnumerable<TNode> Ancestors<TNode>(this TNode startNode)
            where TNode : IHasParentNode<TNode>
        {
            return startNode.Ancestors((TNode n) =>
            {
                if (!n.HasParentNode)
                    return (false, default(TNode));

                return (true, n.ParentNode);
            });
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The <paramref name="startNode"/> is returned first.
        /// </summary>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes including the <paramref name="startNode"/>.
        /// </returns>
        public static IEnumerable<TNode> AncestorsAndSelf<TNode>(this TNode startNode)
            where TNode : IHasParentNode<TNode>
        {
            return startNode.AncestorsAndSelf((TNode n) =>
            {
                if (!n.HasParentNode)
                    return (false, default(TNode));

                return (true, n.ParentNode);
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
    /// Providing extensions to any type which implements the concept of 'having a parent node' with delegates
    /// </summary>
    public static class HasParentNodeGenericExtensions
    {
        /// <summary>
        /// Returns the parent node of the <paramref name="startNode"/>.
        /// If the node hasn't a parent an <see cref="InvalidOperationException"/> is thrown.
        /// The structure of the tree is defined by the <paramref name="tryGetParentNode"/> delegate.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="tryGetParentNode">Delegate to calculate the parent node of the star node</param>
        /// <returns>A parent node instance</returns>
        public static TNode Parent<TNode>(this TNode startNode, Func<TNode, (bool, TNode)> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            var (found, parentNode) = tryGetParentNode(startNode);
            if (found)
                return parentNode;

            throw new InvalidOperationException($"{nameof(startNode)} has no parent");
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The <paramref name="startNode"/> isn't returned. The structure of the tree is defined by the <paramref name="tryGetParentNode"/> delegate.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="tryGetParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> Ancestors<TNode>(this TNode startNode, Func<TNode, (bool, TNode)> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            var (found, current) = tryGetParentNode(startNode);
            while (found)
            {
                yield return current;
                (found, current) = tryGetParentNode(current);
            }
        }

        /// <summary>
        /// Traverses the tree upwards to the root node. The visited nodes are returned inside the <see cref="IEnumerable{TNode}"/>.
        /// The <paramref name="startNode"/> is returned first.
        /// The structure of the tree is defined by the <paramref name="tryGetParentNode"/> delegate.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TNode}"/> containing all visited nodes including the start node.
        /// </returns>
        /// <typeparam name="TNode">Type of the node, implements <see cref="IHasParentNode{TNode}"/></typeparam>
        /// <param name="startNode">reference to the node to start from</param>
        /// <param name="tryGetParentNode">returns the parent node of the inspected node</param>
        public static IEnumerable<TNode> AncestorsAndSelf<TNode>(this TNode startNode, Func<TNode, (bool, TNode)> tryGetParentNode)
        {
            if (tryGetParentNode == null)
                throw new ArgumentNullException(nameof(tryGetParentNode));

            return Enumerable.Repeat(startNode, 1).Union(startNode.Ancestors(tryGetParentNode));
        }
    }
}