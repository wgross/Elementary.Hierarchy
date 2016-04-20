using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy
{
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
