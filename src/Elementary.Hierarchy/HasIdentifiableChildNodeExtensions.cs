namespace Elementary.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using Elementary.Hierarchy.Generic;

    /// <summary>
    /// Provides extensions to the interface <see cref="IHasIdentifiableChildNodes{TKey, TNode}"/>
    /// </summary>
    public static class HasIdentifiableChildNodeExtensions
    {
        #region DescendantAt

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> or throws <see cref="KeyNotFoundException"/> if the
        /// <paramref name="path"/> can't be followed completely.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <returns>the request TNode instance</returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            return startNode.DescendantAt((TNode p, TKey k) => p.TryGetChildNode(k), path);
        }

        #endregion DescendantAt

        #region TryGetDescendantAt

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns false if not found.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <returns>(true, node) if node was found, false otherwise</returns>
        public static (bool, TNode) TryGetDescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            return startNode.TryGetDescendantAt((TNode p, TKey k) => p.TryGetChildNode(k), path);
        }

        #endregion TryGetDescendantAt

        #region DescendantAtOrDefault

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns a substitute value
        /// Which can be supplied by the <paramref name="createDefault"/> delegate.
        /// If no delegate was specified default(TNode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="createDefault">factory delegate to return a substitute value instead of the requested node</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            HierarchyPath<TKey> foundAncestor;
            return startNode.DescendantAtOrDefault(path, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns a substitute value
        /// Which can be supplied by the <paramref name="createDefault"/> delegate.
        /// If no delegate was specified default(TNode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="foundKey">Contains the path of the node where the search stopped</param>
        /// <param name="createDefault">factory delegate to return a substitute value instead of the requested node</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            return startNode.DescendantAtOrDefault((TNode p, TKey k) => p.TryGetChildNode(k), path, out foundKey, createDefault);
        }

        #endregion DescendantAtOrDefault

        #region DescendAlongPath

        /// <summary>
        /// The Tree is traversed from <paramref name="startNode"/>  until the node is reached which is specified by the <paramref name="path"/>.
        /// The traversal stops if the destination node cannot be reached.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy path items</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node to start the traversal at</param>
        /// <param name="path">path of node ids to follow</param>
        /// <returns>Collection of the nodes which where visited along the traversal beginning with <paramref name="startNode"/>.</returns>
        public static IEnumerable<TNode> DescendAlongPath<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            return startNode.DescendAlongPath((TNode p, TKey k) => p.TryGetChildNode(k), path);
        }

        #endregion DescendAlongPath
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extensions to any type which may support the concept of having 'identifieable children' using delegates.
    /// </summary>
    public static class HasIdentifiableChildNodeExtensions
    {
        #region DescendantAt

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> or throws <see cref="KeyNotFoundException"/> if the
        /// <paramref name="path"/> can't be followed completely.
        /// The child nodes are retrieved with the specified <paramref name="tryGetChildNode"/> delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <returns>the requested TNode instance</returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, Func<TNode, TKey, (bool, TNode)> tryGetChildNode, HierarchyPath<TKey> path)
        {
            var pathSegments = path.Items.ToArray();
            TNode childNode = startNode;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                var (found, node) = tryGetChildNode(childNode, pathSegments[i]);
                if (!found)
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", pathSegments.Take(i + 1))}'");
                childNode = node;
            }
            return childNode;
        }

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> or throws <see cref="KeyNotFoundException"/> if the
        /// <paramref name="path"/> can't be followed completely.
        /// The child nodes are retrieved with the specified <paramref name="path"/> delegates.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="path"></param>
        /// <param name="getChildNodes">Retrieves a nodes child nodes</param>
        /// <returns></returns>
        public static TNode DescendantAt<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, params Func<IEnumerable<TNode>, (bool, TNode)>[] path)
        {
            (var found, var childNode) = path.Aggregate((true, startNode), (result, pathItem) => pathItem(getChildNodes(result.Item2)));
            if (!found)
                throw new KeyNotFoundException("Key not found");
            return childNode;
        }

        #endregion DescendantAt

        #region TryGetDescendantAt

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> or returns false if not found.
        /// The child nodes are retrieved with the specified tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <returns>(true, found node) if node was found, false otherwise</returns>
        public static (bool, TNode) TryGetDescendantAt<TKey, TNode>(this TNode startNode, Func<TNode, TKey, (bool, TNode)> tryGetChildNode, HierarchyPath<TKey> path)
        {
            var pathSegments = path.Items.ToArray();
            TNode currentNode = startNode;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                var (found, childNode) = tryGetChildNode(currentNode, pathSegments[i]);
                if (!found)
                    return (false, default(TNode));
                currentNode = childNode;
            }
            return (true, currentNode);
        }

        #endregion TryGetDescendantAt

        #region DescendantAtOrDefault

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns a substitute value
        /// Which can be supplied by the <paramref name="createDefault"/> delegate.
        /// If no delegate was specified default(TNode) is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="path"></param>
        /// <param name="tryGetChildNode">delegate to retrieve a child node by specified key</param>
        /// <param name="createDefault">supplies default value in case the requested node isn't found</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, Func<TNode, TKey, (bool, TNode)> tryGetChildNode, HierarchyPath<TKey> path, Func<TNode> createDefault = null)
        {
            var foundAncestor = HierarchyPath.Create<TKey>();
            return startNode.DescendantAtOrDefault(tryGetChildNode, path, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns a substitute value
        /// Which can be supplied by the <paramref name="createDefault"/> delegate.
        /// If no delegate was specified default(TNode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">child retrieval strategy</param>
        /// <param name="foundKey">the hierachy path of the deepes found node in the <paramref name="path"/></param>
        /// <param name="createDefault">returns a substitue value in case the node at <paramref name="path"/> is not found</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, Func<TNode, TKey, (bool, TNode)> tryGetChildNode, HierarchyPath<TKey> path, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
        {
            foundKey = HierarchyPath.Create<TKey>();
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
            {
                bool found = false;
                (found, childNode) = tryGetChildNode(childNode, keyItems[i]);
                if (found)
                    foundKey = foundKey.Join(keyItems[i]); // add current key to 'found' path
                else
                    return (createDefault ?? (() => default(TNode)))();
            }
            return childNode;
        }

        /// <summary>
        /// Retrieves a descendant of the <paramref name="startNode"/> specifed by the <paramref name="path"/> or returns a substitute value
        /// Which can be supplied by the <paramref name="createDefault"/> delegate.
        /// If no delegate was specified default(TNode) is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="path"></param>
        /// <param name="getChildNodes">delegate to retrieve the child nodes of a node</param>
        /// <param name="createDefault">supplies default value in case the requested node isn't found</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, Func<TNode> createDefault = null, params Func<IEnumerable<TNode>, (bool, TNode)>[] path)
        {
            var (found, childNode) = path.Aggregate((true, startNode), (result, pathItem) => pathItem(getChildNodes(result.Item2)));
            if (!found)
                return (createDefault ?? (() => default(TNode)))();
            return childNode;
        }

        #endregion DescendantAtOrDefault

        #region DescendAlongPath

        /// <summary>
        /// The Tree is traversed from <paramref name="startNode"/>  until the node is reached which is specified by the <paramref name="path"/>.
        /// The traversal stops if the destination node cannot be reached.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy path items</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node, mist implement IHasIdentifiableChildNodes</typeparam>
        /// <param name="startNode">node to start the traversal</param>
        /// <param name="path">path of node ids to follow down</param>
        /// <param name="tryGetChildNode">delegate which defines the tree structure</param>
        /// <returns>Collection of the nodes which where visited along the traversal beginning with <paramref name="startNode"/>.</returns>
        public static IEnumerable<TNode> DescendAlongPath<TKey, TNode>(this TNode startNode, Func<TNode, TKey, (bool, TNode)> tryGetChildNode, HierarchyPath<TKey> path)
        {
            // return the start node as the first node to traverse.
            // this makes sure that at least one node is contained on the result

            yield return startNode;

            // now descend from the start node, if there ars items left in the path
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
            {
                bool found = false;
                (found, childNode) = tryGetChildNode(childNode, keyItems[i]);
                if (found)
                    yield return childNode;
                else
                    yield break;
            }
        }

        /// <summary>
        /// The Tree is traversed from <paramref name="startNode"/>  until the node is reached which is specified by the <paramref name="path"/>.
        /// The traversal stops if the destination node cannot be reached.
        /// </summary>
        /// <typeparam name="TNode">Type of the hierarchy node, mist implement IHasIdentifiableChildNodes</typeparam>
        /// <param name="startNode">node to start the traversal</param>
        /// <param name="path">path of delegates for choosing child nodes </param>
        /// <param name="getChildNodes">delegate which defines the tree structure by mapping a node to its child nodes</param>
        /// <returns>Collection of the nodes which where visited along the traversal beginning with <paramref name="startNode"/>.</returns>
        public static IEnumerable<TNode> DescendAlongPath<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, params Func<IEnumerable<TNode>, (bool, TNode)>[] path)
        {
            // return the start node as the first node to traverse.
            // this makes sure that at least one node is contained on the result

            yield return startNode;

            // now descend from the start node, if there are items left in the path
            TNode childNode = startNode;
            for (int i = 0; i < path.Length; i++)
            {
                bool found = false;
                (found, childNode) = path[i](getChildNodes(childNode));
                if (found)
                    yield return childNode;
                else
                    yield break;
            }
        }

        #endregion DescendAlongPath
    }
}