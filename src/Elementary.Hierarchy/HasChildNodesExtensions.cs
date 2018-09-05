namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extensions to the interface <see cref="IHasChildNodes{TNode}"/>
    /// </summary>
    public static class HasChildNodesExtensions
    {
        #region Children

        /// <summary>
        /// Returns the child node of a given <paramref name="startNode"/>
        /// The child nodes are retrieved from the ChildNodes property if the <see cref="IHasChildNodes{TNode}"/> interface.
        /// If <see cref="IHasChildNodes{TNode}.ChildNodes"/> returns null, <see cref="Enumerable.Empty{TResult}"/> is returned.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <returns>enumerable children of the <paramref name="startNode"/></returns>
        public static IEnumerable<TNode> Children<TNode>(this TNode startNode)
            where TNode : IHasChildNodes<TNode>
        {
            if (startNode is IHasDescendantNodes<TNode>)
                return ((IHasDescendantNodes<TNode>)startNode).GetDescendants(depthFirst: false, maxDepth: 1);

            return startNode.Children(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>());
        }

        #endregion Children

        #region Descendants/-AndSelf

        /// <summary>
        /// Traverses the tree downwards from the given <paramref name="startNode"/>. All nodes visited are returned inside the <see cref="IEnumerable{TNode}"/> returned.
        /// The <paramref name="startNode"/> isn't returned. By default breadth first is used.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IEnumerable{TNode}"/> containing the descendants down to the leaf level.
        /// </returns>
        /// <param name="startNode">The TNode instance to start traversal at</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is always empty, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        /// <param name="depthFirst">enables depth first traversal instead of breadth first</param>
        /// <typeparam name="TNode">type of the tree nodes. Must implement IHasChildNodes&lt;TNode&gt;</typeparam>
        public static IEnumerable<TNode> Descendants<TNode>(this TNode startNode, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            if (startNode is IHasDescendantNodes<TNode>)
                return ((IHasDescendantNodes<TNode>)startNode).GetDescendants(depthFirst.GetValueOrDefault(false), maxDepth.GetValueOrDefault(int.MaxValue)) ?? Enumerable.Empty<TNode>();

            // startNode's implememtation doesn't provide an optimized accessor to its descendants.
            // just rely on the child nodes

            return startNode.Descendants(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), depthFirst, maxDepth);
        }

        /// <summary>
        /// Traverses the tree downwards from the given <paramref name="startNode"/>. All nodes visited (incuding the <paramref name="startNode"/>) are returned inside the <see cref="IEnumerable{TNode}"/> returned.
        /// By default breadth first is used.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IEnumerable{TNode}"/> containing the <paramref name="startNode"/> and its descendants down to the leaf level.
        /// </returns>
        /// <param name="startNode">The TNode instance to start traversal at</param>
        /// <param name="depthFirst">enables depth first traversal instead of breadth first</param>
        /// <param name="maxDepth">Specifies the maximum depth of enumeration 0 is always empty, 1 is <paramref name="startNode"/>, 2 children of <paramref name="startNode"/></param>
        /// <typeparam name="TNode">type of the tree nodes. Must implement IHasChildNodes&lt;TNode&gt;</typeparam>
        public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this TNode startNode, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            if (startNode is IHasDescendantNodes<TNode>)
                return Enumerable.Concat(new[] { startNode }, startNode.Descendants(depthFirst, maxDepth - 1));

            // startNode's implememtation doesn't provide an optimized accessor to its descendants.
            // just rely on the child nodes

            return startNode.DescendantsAndSelf(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), depthFirst, maxDepth);
        }

        #endregion Descendants/-AndSelf

        #region Leaves

        /// <summary>
        /// Traverses the tree (depth first) and returns all nodes which are leaves of the tree.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">The node instance to start traversal at</param>
        /// <param name="maxDepth">Specifies the maximum depth of enumeration 0 is always empty, 1 is <paramref name="startNode"/>, 2 children of <paramref name="startNode"/></param>
        /// /// <param name="maxDepth">the maximum depth to search at</param>
        /// <returns>An enumerable set of leaf nodes</returns>
        public static IEnumerable<TNode> Leaves<TNode>(this TNode startNode, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            if (startNode is IHasDescendantNodes<TNode>)
                return startNode.DescendantsAndSelf(depthFirst: depthFirst, maxDepth: maxDepth).Where(n => !n.HasChildNodes);

            return startNode.Leaves(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), depthFirst: depthFirst, maxDepth: maxDepth);
        }

        #endregion Leaves

        #region VisitDescandants/-AndSelf

        /// <summary>
        /// Traverses all descendants and the given <paramref name="startNode"/>. All nodes are presented to the visitor.
        /// Additionally the path to the <paramref name="startNode"/> is presented to the visitor.
        /// </summary>
        /// <typeparam name="TNode">type of the herarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="visitor">delegate to present each node and its path to the <paramref name="startNode"/></param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the startNode, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static void VisitDescendantsAndSelf<TNode>(this TNode startNode, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            startNode.VisitDescendantsAndSelf(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), visitor, depthFirst, maxDepth);
        }

        /// <summary>
        /// Traverses all descendants of teh given <paramref name="startNode"/>. All nodes are presented to the visitor.
        /// Additionally the path to the <paramref name="startNode"/> is presented to the visitor.
        /// </summary>
        /// <typeparam name="TNode">type of the herarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="visitor">delegate to present each node and its path to the <paramref name="startNode"/></param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the startNode, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static void VisitDescendants<TNode>(this TNode startNode, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            startNode.VisitDescendants(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), visitor, depthFirst, maxDepth);
        }

        #endregion VisitDescandants/-AndSelf
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension for any type to implemnt the concept of 'having children' with delegates.
    /// </summary>
    public static class HasChildNodesGenericExtensions
    {
        #region Children

        /// <summary>
        /// Returns the child nodes of the given <paramref name="startNode"/>.
        /// The child nodes are retrieved by calling the specified getChildren Delegate.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode"></param>
        /// <param name="getChildren"></param>
        /// <returns>enumerable children of the <paramref name="startNode"/></returns>
        public static IEnumerable<TNode> Children<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            if (getChildren is null)
                throw new ArgumentNullException(nameof(getChildren));

            return getChildren(startNode) ?? Enumerable.Empty<TNode>();
        }

        #endregion Children

        #region Descendants/-AndSelf

        /// <summary>
        /// Traverses the tree downwards from the given <paramref name="startNode"/>. All nodes visited are returned inside the <see cref="IEnumerable{TNode}"/> returned.
        /// The <paramref name="startNode"/> isn't returned. By default breadth first is used.
        /// The tree structure is defined by the two provided delegates.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IEnumerable{TNode}"/> containing the descendants down to the leaf level.
        /// </returns>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode"></param>
        /// <param name="depthFirst">specifies if child noded are enumerated depth first or breadth first</param>
        /// <param name="getChildNodes">delegate retrieved the child nodes of the specified TNode instance</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is always empty, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static IEnumerable<TNode> Descendants<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildNodes is null)
                throw new ArgumentNullException(nameof(getChildNodes));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            if (depthFirst.GetValueOrDefault(false))
                return EnumerateDescendentsAndSelfDepthFirst(startNode,
                    breadcrumbs: null,
                    maxDepth: maxDepth ?? int.MaxValue,
                    getChildNodes: getChildNodes,
                    equalityComparer:EqualityComparer<TNode>.Default).Skip(1);
            else // this is the default case:
                return EnumerateDescendantsAndSelfBreadthFirst(startNode,
                    breadcrumbs: null,
                    maxDepth: maxDepth ?? int.MaxValue,
                    getChildNodes: getChildNodes, 
                    equalityComparer:EqualityComparer<TNode>.Default).Skip(1);
        }

        /// <summary>
        /// Traverses the tree downwards from the given <paramref name="startNode"/>. All nodes visited (incuding the <paramref name="startNode"/>) are returned inside the <see cref="IEnumerable{TNode}"/> returned.
        /// By default breadth first is used.
        /// The tree structure is explored by calling the specified delegates.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IEnumerable{TNode}"/> containing the descendants down to the leaf level and the <paramref name="startNode"/>.
        /// </returns>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">The TNode instance to start traversal at</param>
        /// <param name="depthFirst">specifies if child nodes are enumerated depth first or breadth first</param>
        /// <param name="getChildNodes">delegate retrueved the child nodes of the specified TNode instance</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the <paramref name="startNode"/>, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static IEnumerable<TNode> DescendantsAndSelf<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildNodes is null)
                throw new ArgumentNullException(nameof(getChildNodes));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0.", nameof(maxDepth));

            var evaluatedMaxDepth = maxDepth ?? int.MaxValue;
            if (0 == maxDepth)
                yield break; // depth null means -> do not descend

            yield return startNode;
            foreach (var nextNode in startNode.Descendants(getChildNodes, depthFirst.GetValueOrDefault(false), evaluatedMaxDepth - 1))
                yield return nextNode;
        }

        #endregion Descendants/-AndSelf

        #region DescandantsWithPath/-AndSelf

        /// <summary>
        /// Traverses all descendants of the given <paramref name="startNode"/>.
        /// All nodes are returned and additionally the path to the <paramref name="startNode"/> is presented added as secend member of the enumerated result tuple.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="getChildren">delegate to retruebe children form any node of the hierarchy</param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is always empty, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static IEnumerable<(TNode node, IEnumerable<TNode> path)> DescendantsWithPath<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildren is null)
                throw new ArgumentNullException(nameof(getChildren));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            var breadcrumbs = new List<TNode>();
            return depthFirst.GetValueOrDefault(false)
                ? EnumerateDescendentsAndSelfDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default)
                    .Skip(1)
                    .Select(n => (n, breadcrumbs.ToArray().AsEnumerable()))
                : EnumerateDescendantsAndSelfBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default)
                    .Skip(1)
                    .Select(n => (n, breadcrumbs.ToArray().AsEnumerable()));
        }

        /// <summary>
        /// Traverses all descendants of the given <paramref name="startNode"/>. The start node is returned as first item.
        /// All nodes are returned and additionally the path to the <paramref name="startNode"/> is presented added as secend member of the enumerated result tuple.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="getChildren">delegate to retruebe children form any node of the hierarchy</param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is always empty, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static IEnumerable<(TNode node, IEnumerable<TNode> path)> DescendantsAndSelfWithPath<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildren is null)
                throw new ArgumentNullException(nameof(getChildren));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            var breadcrumbs = new List<TNode>();
            return depthFirst.GetValueOrDefault(false)
                ? EnumerateDescendentsAndSelfDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default)
                    .Select(n => (n, (IEnumerable<TNode>)(breadcrumbs.ToArray())))
                : EnumerateDescendantsAndSelfBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default)
                    .Select(n => (n, (IEnumerable<TNode>)(breadcrumbs.ToArray())));
        }

        #endregion DescandantsWithPath/-AndSelf

        #region Leaves

        /// <summary>
        /// Traverses the tree (depth first) and returns all nodes which are leaves of the tree. Leaves are recognizes because
        /// The given delegate <paramref name="getChildNodes"/> doesn't return any child nodes.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">The node instance to start traversal at</param>
        /// <param name="depthFirst">specifies if child nodes are enumerated depth first or breadth first</param>
        /// <param name="getChildNodes"></param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the <paramref name="startNode"/>, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        /// <returns>a collection of leaves</returns>
        public static IEnumerable<TNode> Leaves<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, bool? depthFirst = null, int? maxDepth = null)
        {
            // all nodes which havn't a child node are leaves of the tree.
            return startNode.DescendantsAndSelf(getChildNodes: getChildNodes, depthFirst: depthFirst, maxDepth: maxDepth).Where(n => !getChildNodes(n).Any());
        }

        #endregion Leaves

        #region VisitDescandants/-AndSelf

        /// <summary>
        /// Traverses all descendants and the given <paramref name="startNode"/>. All nodes are presented to the visitor.
        /// Additionally the path to the <paramref name="startNode"/> is presented to the visitor.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="getChildren">delegate to retruebe children form any node of the hierarchy</param>
        /// <param name="visitor">delegate to present each node and its path to the <paramref name="startNode"/></param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the <paramref name="startNode"/>, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static void VisitDescendantsAndSelf<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildren is null)
                throw new ArgumentNullException(nameof(getChildren));

            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            // visit start node
            var breadcrumbs = new List<TNode>();
            visitor(breadcrumbs, startNode);
            breadcrumbs.Add(startNode);

            // and continue with descendants
            if (depthFirst.GetValueOrDefault(false))
            {
                foreach (var node in EnumerateDescendentsAndSelfDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default).Skip(1))
                    visitor(breadcrumbs, node);
            }
            else // this is the default case:
            {
                foreach (var node in EnumerateDescendantsAndSelfBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default).Skip(1))
                    visitor(breadcrumbs, node);
            }
        }

        /// <summary>
        /// Traverses all descendants of teh given <paramref name="startNode"/>. All nodes are presented to the visitor.
        /// Additionally the path to the <paramref name="startNode"/> is presented to the visitor.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="getChildren">delegate to retruebe children form any node of the hierarchy</param>
        /// <param name="visitor">delegate to present each node and its path to the <paramref name="startNode"/></param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is always empty, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static void VisitDescendants<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildren is null)
                throw new ArgumentNullException(nameof(getChildren));

            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            var breadcrumbs = new List<TNode>();
            if (depthFirst.GetValueOrDefault(false))
            {
                foreach (var node in EnumerateDescendentsAndSelfDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default).Skip(1))
                    visitor(breadcrumbs, node);
            }
            else // this is the default case:
            {
                foreach (var node in EnumerateDescendantsAndSelfBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren, EqualityComparer<TNode>.Default).Skip(1))
                    visitor(breadcrumbs, node);
            }
        }

        #endregion VisitDescandants/-AndSelf

        #region Internal implementation of hierarchy traversal

        private static IEnumerable<TNode> EnumerateDescendantsAndSelfBreadthFirst<TNode>(TNode startNode, List<TNode> breadcrumbs, int maxDepth, Func<TNode, IEnumerable<TNode>> getChildNodes, IEqualityComparer<TNode> equalityComparer)
        {
            // keep track of all visited nodes for cycle detection

            var visitedNodes = new List<TNode>();

            // enable the breadcrumb handling if needed

            Action<List<TNode>, int, TNode> updateBreadcrumbs = delegate { };

            if (breadcrumbs != null)
            {
                updateBreadcrumbs = UpdateBreadcrumbs;
                breadcrumbs.Clear();
            }

            // return start node at level 0

            yield return startNode;

            // start node was visited

            visitedNodes.Add(startNode);

            // add the children of the start node to the queue of nodes to vist

            var nodesToVisit = new Queue<(int level, TNode child, TNode node)>();
            if (0 < maxDepth) // startNode is depth 0, next level is 1
            {
                foreach (var childOfStartNode in getChildNodes(startNode) ?? Enumerable.Empty<TNode>()) // descend one level from the start node
                {
                    if (!visitedNodes.Contains(childOfStartNode, equalityComparer))
                    {
                        // add child to the queue of child nodes to visit
                        nodesToVisit.Enqueue((level: 1, child: childOfStartNode, node: startNode));
                    }
                }
            }

            int lastLevel = 0;
            while (nodesToVisit.Any())
            {
                // get the next node from the front of the queue

                var currentNodeTuple = nodesToVisit.Dequeue();
                var currentNodeLevel = currentNodeTuple.Item1;
                var currentNode = currentNodeTuple.Item2;
                var currentNodeParent = currentNodeTuple.Item3;

                // if currentNode has children, evaluate them on the next iteration step
                // by appending them to the queue

                if (currentNodeLevel < maxDepth) // continue only if the current level is smaller than maxDepth
                {
                    foreach (TNode childOfCurrentNode in getChildNodes(currentNode)) // descend one level
                    {
                        if (!visitedNodes.Contains(childOfCurrentNode, equalityComparer))
                        {
                            // child noes are added only if they are not visted already
                            nodesToVisit.Enqueue((level: currentNodeLevel + 1, child: childOfCurrentNode, node: currentNode));
                        }
                    }
                }
                lastLevel = currentNodeLevel;

                // present the current node for enumeration, including an update of the breadcrumbs to the current node

                updateBreadcrumbs(breadcrumbs, currentNodeLevel, currentNodeParent);
                yield return currentNode;

                // current node was visited

                visitedNodes.Add(currentNode);
            }
            yield break;
        }

        private static IEnumerable<TNode> EnumerateDescendentsAndSelfDepthFirst<TNode>(TNode startNode, List<TNode> breadcrumbs, int maxDepth, Func<TNode, IEnumerable<TNode>> getChildNodes, IEqualityComparer<TNode> equalityComparer)
        {
            // keep track of all visited nodes for cycle detection

            var visitedNodes = new List<TNode>();

            // enable the breadcrumb handling if needed

            Action<List<TNode>, int, TNode> updateBreadcrumbs = delegate { };
            if (breadcrumbs != null)
            {
                updateBreadcrumbs = UpdateBreadcrumbs;
                breadcrumbs.Clear();
            }

            // return start node first as 'self'

            yield return startNode;

            // start node was visited

            visitedNodes.Add(startNode);

            // keep a stack with the enumerators in their current enumeration state

            var nodesToVisit = new Stack<(int level, IEnumerator<TNode> children, TNode node)>();

            // children of startNode are pushed with level 1.
            // descend to children of startNode only if maxDepth is > 0

            if (0 < maxDepth)
                nodesToVisit.Push((level: 1, children: getChildNodes(startNode).GetEnumerator(), node: startNode));

            // process with the chhild nodes of the start node

            while (nodesToVisit.Any())
            {
                // go to the right or up (or to the first node if the enumeration hasn't started yet)

                while (nodesToVisit.Any() && !nodesToVisit.Peek().Item2.MoveNext())
                    nodesToVisit.Pop(); //  no children to inspect left: go up

                if (!nodesToVisit.Any())
                    yield break; // all nodes visited: Leave traversal completely

                // descend and return current node

                var currentNodeTuple = nodesToVisit.Peek();
                var currentNodeLevel = currentNodeTuple.Item1;
                var currentNodeParent = currentNodeTuple.Item3;
                var currentNode = currentNodeTuple.Item2.Current;

                // visit only nodes which are not visited previously

                if (!visitedNodes.Contains(currentNode, equalityComparer))
                {
                    // enumerate the child nodes of this node during the next step

                    if (currentNodeLevel < maxDepth)
                        nodesToVisit.Push((level: currentNodeLevel + 1, children: getChildNodes(currentNode).GetEnumerator(), node: currentNode));

                    // present the current node for enumeration, including an update of the breadcrumbs to the current node
                    updateBreadcrumbs(breadcrumbs, currentNodeLevel, currentNodeParent);
                    yield return currentNode;
                }
            }
            yield break;
        }

        private static void UpdateBreadcrumbs<TNode>(List<TNode> breadcrumbs, int currentNodeLevel, TNode currentNodeParent)
        {
            while (breadcrumbs.Count > currentNodeLevel - 1)
                breadcrumbs.RemoveAt(currentNodeLevel - 1);
            breadcrumbs.Insert(currentNodeLevel - 1, currentNodeParent);
        }

        #endregion Internal implementation of hierarchy traversal
    }
}