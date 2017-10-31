using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Nodes
{
    public class TraversalDecorator<N> : IHasParentNode<TraversalDecorator<N>>, IHasChildNodes<TraversalDecorator<N>>
        where N : IHasChildNodes<N>
    {
        #region Construction and initialization

        public TraversalDecorator(N node)
        {
            this.Node = node;
        }

        private TraversalDecorator(N node, TraversalDecorator<N> parent)
        {
            this.Node = node;
            this.parent = parent;
        }

        public N Node { get; }

        private readonly TraversalDecorator<N> parent;

        #endregion Construction and initialization

        public bool HasParentNode => this.ParentNode != null;

        public TraversalDecorator<N> ParentNode => this.parent;

        public bool HasChildNodes => this.Node.HasChildNodes;

        public IEnumerable<TraversalDecorator<N>> ChildNodes => this.Node.ChildNodes.Select(n => new TraversalDecorator<N>(n, this));
    }
}