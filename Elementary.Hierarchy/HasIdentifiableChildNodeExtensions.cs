namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extsions to the interface <see cref="IHasIdentifiableChildNodes{TKey, TNode}"/>
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
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAt(tryGetChildNode, path);
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
        /// <param name="descendantAt">contains the wanted descandant node of the search was succesful</param>
        /// <returns>true if node was found, false otherwise</returns>
        public static bool TryGetDescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, out TNode descendantAt)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.TryGetDescendantAt(tryGetChildNode, path, out descendantAt);
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
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAtOrDefault(tryGetChildNode, path, out foundKey, createDefault);
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
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendAlongPath(tryGetChildNode, path);
        }

        #endregion DescendAlongPath

        #region VisitDescendantAtAndAncestors

        /// <summary>
        /// The algorithm descends to the specified descendant and presents it to the visitor delegate.
        /// Afterwards it ascends along the path and presents the ancestors of the descendant until the startNode is reached.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode">the tree node to start to descend</param>
        /// <param name="path">specified the path to descend along from the start node</param>
        /// <param name="visitDescendantAt">the visitor to call at teh descandant</param>
        /// <param name="visitAncestors">the visitor to call for all ancestors</param>
        public static void VisitDescandantAtAndAncestors<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, Action<TNode> visitDescendantAt, Action<TNode> visitAncestors)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            startNode.VisitDescandantAtAndAncestors(tryGetChildNode, path, visitDescendantAt, visitAncestors);
        }

        #endregion VisitDescendantAtAndAncestors
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provided extensions to any type which may support the concept of haing 'identifieable children' using delegates.
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
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path)
        {
            var pathArray = path.Items.ToArray();
            TNode childNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(childNode, pathArray[i], out childNode))
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", pathArray.Take(i + 1))}'");

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
        /// <param name="descendantAt">contains the wanted descendant node of the search was succesful</param>
        /// <returns>true if node was found, false otherwise</returns>
        public static bool TryGetDescendantAt<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, out TNode descendantAt)
        {
            descendantAt = default(TNode);

            var pathArray = path.Items.ToArray();
            TNode currentNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(currentNode, pathArray[i], out currentNode))
                    return false;

            descendantAt = currentNode;
            return true;
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
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, Func<TNode> createDefault = null)
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
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
        {
            foundKey = HierarchyPath.Create<TKey>();
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
                if (!tryGetChildNode(childNode, keyItems[i], out childNode))
                    return (createDefault ?? (() => default(TNode)))();
                else
                    foundKey = foundKey.Join(keyItems[i]); // add current key to 'found' path

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
        public static IEnumerable<TNode> DescendAlongPath<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path)
        {
            // return the start node as the first node to traverse.
            // this makes sure that at least one node is contained on the result

            yield return startNode;

            // now descend from the start node, if there is sometin left in the path
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
                if (tryGetChildNode(childNode, keyItems[i], out childNode))
                    yield return childNode;
                else
                    yield break;
        }

        #endregion DescendAlongPath

        #region VisitDescendantAtAndAncestors

        /// <summary>
        /// The algorithm descends to the specified descendant and presents it to the visitor delegate.
        /// Afterwards it ascends along the path and presents the ancestors of the descendant until the startNode is reached.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode">the tree node to start to descend</param>
        /// <param name="tryGetChildNode">the method to retreev child nodes from a parent node</param>
        /// <param name="path">specified the path to descend along from the start node</param>
        /// <param name="visitDescendantAt">the visitor to call at teh descandant</param>
        /// <param name="visitAncestors">the visitor to call for all ancestors</param>
        public static void VisitDescandantAtAndAncestors<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, Action<TNode> visitDescendantAt, Action<TNode> visitAncestors)
        {
            if (visitDescendantAt == null)
                throw new ArgumentNullException(nameof(visitDescendantAt));

            if (visitAncestors == null)
                throw new ArgumentNullException(nameof(visitAncestors));

            var ancestors = new Stack<TNode>(new[] { startNode });

            // descend down the tree until the descendant is reached.
            // remember all ancestors in a stack for re.visiting them afterwards.
            var pathArray = path.Items.ToArray();
            TNode currentNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(currentNode, pathArray[i], out currentNode))
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", pathArray.Take(i + 1))}'");
                else ancestors.Push(currentNode);

            // the descandant is visited first
            // and afterwards all ancestors are presented.
            visitDescendantAt(ancestors.Pop());
            while (ancestors.Any())
                visitAncestors(ancestors.Pop());
        }

        #endregion VisitDescendantAtAndAncestors
    }
}