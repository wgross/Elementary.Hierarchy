//using System;

//namespace Elementary.Hierarchy.Collections.Operations
//{
//    /// <summary>
//    ///
//    /// </summary>
//    public class HierarchyWriter<TNode> where TNode : IHierarchyNodeWriter<TNode>, IHasChildNodes<TNode>
//    {
//        /// <summary>
//        /// Descends breadth-first to the child nodes.
//        /// the result value of a Visit call indicates the change of the hierarchy.
//        /// - returning null means: remove this node from the hierarchy
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns>an identical or changes node or null</returns>
//        public virtual TNode Visit(TNode node)
//        {
//            if (node == null)
//                throw new ArgumentNullException(nameof(node));

//            foreach (var child in node.Children())
//            {
//                var returnedChild = this.Visit(child);
//                if (returnedChild == null)
//                    node = node.RemoveChild(child);
//                else if (!returnedChild.Equals(child))
//                    node = node.ReplaceChild(child, returnedChild);
//            }
//            return node;
//        }
//    }
//}