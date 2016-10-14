namespace Elementary.Hierarchy
{
    using System.Collections.Generic;

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