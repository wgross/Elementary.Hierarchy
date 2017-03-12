namespace Elementary.Hierarchy.Abstractions
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface IHierarchyNodeWriter<TNode> : IHasChildNodes<IHierarchyNodeWriter<TNode>>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        IHierarchyNodeWriter<TNode> RemoveChild(IHierarchyNodeWriter<TNode> child);

        /// <summary>
        /// Replaces the child node with a new instance
        /// </summary>
        /// <param name="childToReplace"></param>
        /// <param name="newChild"></param>
        IHierarchyNodeWriter<TNode> ReplaceChild(IHierarchyNodeWriter<TNode> childToReplace, IHierarchyNodeWriter<TNode> newChild);
    }
}