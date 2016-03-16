//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Elementary.Hierarchy
//{
//    public static class Traverser
//    {
//        #region NodeAt

//        private sealed class HierarchyTraverser<TNode> : IHierarchyNode<TNode>
//        {
//            #region Construction and initialization of this instance

//            public HierarchyTraverser(TNode currentNode, Func<TNode, TNode> getParentNode, Func<TNode, IEnumerable<TNode>> getChildNodes)
//            {
//                this.currentNode = currentNode;
//                this.getChildNodes = getChildNodes;
//                this.getParentNode = getParentNode;
//            }

//            private readonly Stack<TNode> ancestors = new Stack<TNode>();

//            private readonly TNode currentNode;

//            private readonly Func<TNode, IEnumerable<TNode>> getChildNodes;

//            private readonly Func<TNode, TNode> getParentNode;

//            #endregion Construction and initialization of this instance

//            public IEnumerable<IHierarchyNode<TNode>> ChildNodes => this.getChildNodes(this.currentNode).Select(c => new HierarchyTraverser<TNode>(
//                currentNode: c, getParentNode: _ => this.currentNode, getChildNodes: this.getChildNodes));

//            public TNode CurrentNode => this.currentNode;

//            public bool HasChildNodes => this.ChildNodes.Any();

//            public bool HasParentNode => this.getParentNode != null;

//            public IHierarchyNode<TNode> ParentNode => new HierarchyTraverser<TNode>(this.getParentNode(this.currentNode), this.getParentNode, this.getChildNodes);
//        }

//        /// <summary>
//        /// Returns a hierarchy node traverser positioned where the hierachy path points at.
//        /// </summary>
//        /// <typeparam name="TNode">Type of the underlying hierachy node</typeparam>
//        /// <typeparam name="TKey"> Type if the hierarchy path items</typeparam>
//        /// <param name="startNode">Node to start the search for the node to begin traversal</param>
//        /// <param name="getChildren">Deletegate to retrieve child nodes from the current hierarchy node</param>
//        /// <returns>The hierarchy node traverer or null</returns>
//        public static IHierarchyNode<TNode> TraverseAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> key, Func<TNode, IEnumerable<TNode>> getChildren)
//        {
//            return null;
//        }

//        #endregion NodeAt
//    }
//}