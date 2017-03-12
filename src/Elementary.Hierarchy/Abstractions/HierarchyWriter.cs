namespace Elementary.Hierarchy.Abstractions
{
    /// <summary>
    ///
    /// </summary>
    public class HierarchyWriter<TNode>
    {
        /// <summary>
        /// Descends breadth-first to the child nodes. 
        /// the result value of a Visit call indicates the change of the hierarchy.
        /// - returning null means: remove this node from the hierarchy
        /// </summary>
        /// <param name="node"></param>
        /// <returns>an identical or changes node or null</returns>
        public virtual IHierarchyNodeWriter<TNode> Visit(IHierarchyNodeWriter<TNode> node)
        {
            foreach (var child in node.Children())
            {
                var returnedChild = this.Visit(child);
                if (returnedChild == null)
                    node.RemoveChild(child);
                else if (!returnedChild.Equals(child))
                    node.ReplaceChild(child, returnedChild);
            }
            return node;
        }
    }
}