namespace Elementary.Hierarchy.Collections.Nodes
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface IHierarchyNodeWriter<TNode> 
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        TNode RemoveChild(TNode child);

        /// <summary>
        /// Replaces the child node with a new instance
        /// </summary>
        /// <param name="childToReplace"></param>
        /// <param name="newChild"></param>
        TNode ReplaceChild(TNode childToReplace, TNode newChild);

        /// <summary>
        /// Add the node <paramref name="newChild"/> to the edited node.
        /// The edited node is returned not the child node.
        /// </summary>
        /// <param name="newChild">node to add as new child to the edited node</param>
        /// <returns>the editined node containing the new child</returns>
        TNode AddChild(TNode newChild);
    }
}