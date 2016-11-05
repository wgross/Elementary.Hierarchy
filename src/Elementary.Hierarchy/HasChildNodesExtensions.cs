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
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <returns>enumerable children of the <paramref name="startNode"/></returns>
        public static IEnumerable<TNode> Children<TNode>(this TNode startNode)
            where TNode : IHasChildNodes<TNode>
        {
            return startNode.Children(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>());
        }

        #endregion Children

        #region Descendants/-OrSelf

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
                return ((IHasDescendantNodes<TNode>)startNode).GetDescendants(depthFirst.GetValueOrDefault(false), maxDepth.GetValueOrDefault(int.MaxValue));

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
        public static IEnumerable<TNode> DescendantsOrSelf<TNode>(this TNode startNode, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            if (startNode is IHasDescendantNodes<TNode>)
                return Enumerable.Concat(new[] { startNode }, startNode.Descendants(depthFirst, maxDepth - 1));

            // startNode's implememtation doesn't provide an optimized accessor to its descendants. 
            // just rely on the child nodes

            return startNode.DescendantsOrSelf(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), depthFirst, maxDepth);
        }

        #endregion Descendants/-OrSelf

        #region Leaves

        /// <summary>
        /// Traverses the tree (depth first) and returns all nodes which are leaves of the tree. 
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">The node instance to start traversal at</param>
        /// <returns>An enumerableset of leaf nodes</returns>
        public static IEnumerable<TNode> Leaves<TNode>(this TNode startNode)
            where TNode : IHasChildNodes<TNode>
        {
            return startNode.Leaves(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>());
        }

        #endregion Leaves

        #region VisitDescandants/-OrSelf

        /// <summary>
        /// Traverses all descendants and the given <paramref name="startNode"/>. All nodes are presented to the visitor.
        /// Additionally the path to the <paramref name="startNode"/> is presented to the visitor.
        /// </summary>
        /// <typeparam name="TNode">type of the herarchy node</typeparam>
        /// <param name="startNode">node start traversal at</param>
        /// <param name="visitor">delegate to present each node and its path to the <paramref name="startNode"/></param>
        /// <param name="depthFirst">enables deth first traversal, breadth first is default</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the startNode, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static void VisitDescendantsOrSelf<TNode>(this TNode startNode, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
            where TNode : IHasChildNodes<TNode>
        {
            startNode.VisitDescendantsOrSelf(n => n.HasChildNodes ? n.ChildNodes : Enumerable.Empty<TNode>(), visitor, depthFirst, maxDepth);
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

        #endregion VisitDescandants/-OrSelf
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
            if (getChildren == null)
                throw new ArgumentNullException(nameof(getChildren));

            return getChildren(startNode);
        }

        #endregion Children

        #region Descendants/-OrSelf

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
            if (getChildNodes == null)
                throw new ArgumentNullException(nameof(getChildNodes));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            if (depthFirst.GetValueOrDefault(false))
                return EnumerateDescendentsDepthFirst(startNode,
                    breadcrumbs: null,
                    maxDepth: maxDepth ?? int.MaxValue,
                    getChildNodes: getChildNodes);
            else // this is the default case:
                return EnumerateDescendantsBreadthFirst(startNode,
                    breadcrumbs: null,
                    maxDepth: maxDepth ?? int.MaxValue,
                    getChildNodes: getChildNodes);
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
        /// <param name="depthFirst">specifies if child noded are enumerated depth first or breadth first</param>
        /// <param name="getChildNodes">delegate retrueved the child nodes of the specified TNode instance</param>
        /// <param name="maxDepth">specifies the maximum depth of traversal: 0 is the <paramref name="startNode"/>, 1 is the children of the <paramref name="startNode"/> and so on. default is unlimited</param>
        public static IEnumerable<TNode> DescendantsOrSelf<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildNodes == null)
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

        #endregion Descendants/-OrSelf

        #region Leaves

        /// <summary>
        /// Traverses the tree (depth first) and returns all nodes which are leaves of the tree. Leaves are recognizes because
        /// The given delegate <paramref name="getChildNodes"/> doesn't return any child nodes.
        /// </summary>
        /// <typeparam name="TNode">type of the hierarchy node</typeparam>
        /// <param name="startNode">The node instance to start traversal at</param>
        /// <param name="getChildNodes"></param>
        /// <returns></returns>
        public static IEnumerable<TNode> Leaves<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildNodes)
        {
            // all nodes which havn't a child node are leaves of the tree.
            return startNode.DescendantsOrSelf(getChildNodes: getChildNodes).Where(n => !getChildNodes(n).Any());
        }

        #endregion Leaves

        #region VisitDescandants/-OrSelf

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
        public static void VisitDescendantsOrSelf<TNode>(this TNode startNode, Func<TNode, IEnumerable<TNode>> getChildren, Action<IEnumerable<TNode>, TNode> visitor, bool? depthFirst = null, int? maxDepth = null)
        {
            if (getChildren == null)
                throw new ArgumentNullException(nameof(getChildren));

            if (visitor == null)
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
                foreach (var node in EnumerateDescendentsDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren))
                    visitor(breadcrumbs, node);
            }
            else // this is the default case:
            {
                foreach (var node in EnumerateDescendantsBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren))
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
            if (getChildren == null)
                throw new ArgumentNullException(nameof(getChildren));

            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            if (maxDepth.HasValue && maxDepth.Value < 0)
                throw new ArgumentException("must be > 0", nameof(maxDepth));

            var breadcrumbs = new List<TNode>();
            if (depthFirst.GetValueOrDefault(false))
            {
                foreach (var node in EnumerateDescendentsDepthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren))
                    visitor(breadcrumbs, node);
            }
            else // this is the default case:
            {
                foreach (var node in EnumerateDescendantsBreadthFirst(startNode, breadcrumbs, maxDepth ?? int.MaxValue, getChildren))
                    visitor(breadcrumbs, node);
            }
        }

        #endregion VisitDescandants/-OrSelf

        #region Internal implementation of hierarchy traversal

        private static IEnumerable<TNode> EnumerateDescendantsBreadthFirst<TNode>(TNode startNode, List<TNode> breadcrumbs, int maxDepth, Func<TNode, IEnumerable<TNode>> getChildNodes)
        {
            // enable the breadcrumb handling if needed

            Action<List<TNode>, int, TNode> updateBreadcrumbs = delegate { };
            if (breadcrumbs != null)
                updateBreadcrumbs = UpdateBreadcrumbs;

            // add the children of the start node to the queue of nodes to vist

            var nodesToVisit = new Queue<Tuple<int, TNode, TNode>>();
            if (0 < maxDepth) // startNode is depth 0, next level is 1
            {
                foreach (var child in getChildNodes(startNode)) // descend one level from the start node
                {
                    nodesToVisit.Enqueue(Tuple.Create(1, child, startNode));
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
                        nodesToVisit.Enqueue(Tuple.Create(currentNodeLevel + 1, childOfCurrentNode, currentNode));
                    }
                }
                lastLevel = currentNodeLevel;

                // present the current node for enumeration, including an update of the breadcrumbs to the current node
                updateBreadcrumbs(breadcrumbs, currentNodeLevel, currentNodeParent);
                yield return currentNode;
            }
            yield break;
        }

        private static IEnumerable<TNode> EnumerateDescendentsDepthFirst<TNode>(TNode startNode, List<TNode> breadcrumbs, int maxDepth, Func<TNode, IEnumerable<TNode>> getChildNodes)
        {
            // enable the breadcrumb handling if needed

            Action<List<TNode>, int, TNode> updateBreadcrumbs = delegate { };
            if (breadcrumbs != null)
                updateBreadcrumbs = UpdateBreadcrumbs;

            // keep a stack with the enumerators in their current enumeration state

            var nodesToVisit = new Stack<Tuple<int, IEnumerator<TNode>, TNode>>();

            // children of startNode are pushed with level 1.
            // descend to children of startNode only if maxDepth is > 0

            if (0 < maxDepth)
                nodesToVisit.Push(Tuple.Create(1, getChildNodes(startNode).GetEnumerator(), startNode));

            while (nodesToVisit.Any())
            {
                // go to the right or up (or to the first node if the enumeration hasn't started yet)

                while (nodesToVisit.Any() && !nodesToVisit.Peek().Item2.MoveNext())
                    nodesToVisit.Pop();

                if (!nodesToVisit.Any())
                    yield break;

                // descend and return current node

                var currentNodeTuple = nodesToVisit.Peek();
                var currentNodeLevel = currentNodeTuple.Item1;
                var currentNodeParent = currentNodeTuple.Item3;
                var currentNode = currentNodeTuple.Item2.Current;

                // enumerate the child nodes of this node during the next step

                if (currentNodeLevel < maxDepth)
                    nodesToVisit.Push(Tuple.Create(currentNodeLevel + 1, getChildNodes(currentNode).GetEnumerator(), currentNode));

                // present the current node for enumeration, including an update of the breadcrumbs to the current node
                updateBreadcrumbs(breadcrumbs, currentNodeLevel, currentNodeParent);
                yield return currentNode;
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