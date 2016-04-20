using System.Collections.Generic;

namespace Elementary.Hierarchy
{
    /// <summary>
    /// This interface is meant to allow optimized retrieval of descendents at a specified maxmimum depth
    /// compared to the sthraght forward approach of building a tree of descendants by visiting the child nodes
    /// using <see cref="IHasChildNodes{TNode}"/>. This is useful if node data is stored in a database or has to
    /// be retrieved a webservice.
    /// </summary>
    /// <typeparam name="TNode">Type of teh node to retrieve</typeparam>
    public interface IHasDescendantNodes<TNode> : IHasChildNodes<TNode>
    {
        /// <summary>
        /// Retrieves the descandants of this node. The order of the descendant nodes may be either
        /// depth first or breadth first.
        /// The maximum depth of descending can be restricted.
        /// </summary>
        /// <param name="depthFirst">Descend dethf first or breadth first</param>
        /// <param name="maxDepth">maximum deopths of descending 0 is always empty set, 1 are the children of this node and so on</param>
        /// <returns>Enumerable set of descendant nodes, in the order of visiting</returns>
        IEnumerable<TNode> GetDescendants(bool depthFirst, int maxDepth);
    }
}