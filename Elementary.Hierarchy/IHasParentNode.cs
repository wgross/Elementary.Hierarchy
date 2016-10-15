namespace Elementary.Hierarchy
{
    /// <summary>
    /// Implementaion contract of the concept of 'having a parent'
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface IHasParentNode<TNode>
    {
        /// <summary>
        /// Determines if a <see cref="IHasParentNode{TNode}"/> instance has a parent
        /// </summary>
        bool HasParentNode { get; }

        /// <summary>
        /// Returns the parent node instance
        /// </summary>
        TNode ParentNode { get; }
    }
}