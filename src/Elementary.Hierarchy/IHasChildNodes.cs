namespace Elementary.Hierarchy
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an implementation contract of the concept of 'having children'.
    /// </summary>
    /// <typeparam name="TNode">type of the children</typeparam>
    public interface IHasChildNodes<TNode>
    {
        /// <summary>
        /// Returns true if this node has child nodes
        /// </summary>
        bool HasChildNodes { get; }

        /// <summary>
        /// returns a collection of child nodes.
        /// </summary>
        IEnumerable<TNode> ChildNodes { get; }
    }
}