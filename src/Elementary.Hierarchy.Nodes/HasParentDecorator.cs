using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Nodes
{
    public class HasParentDecorator<N> : IHasParentNode<HasParentDecorator<N>>, IHasChildNodes<HasParentDecorator<N>>
        where N : IHasChildNodes<N>
    {
        #region Construction and initialization

        public HasParentDecorator(N node)
        {
            this.Node = node;
        }

        private HasParentDecorator(N node, HasParentDecorator<N> parent)
        {
            this.Node = node;
            this.parent = parent;
        }

        public N Node { get; }

        private readonly HasParentDecorator<N> parent;

        #endregion Construction and initialization

        public bool HasParentNode => this.ParentNode != null;

        public HasParentDecorator<N> ParentNode => this.parent;

        public bool HasChildNodes => this.Node.HasChildNodes;

        public IEnumerable<HasParentDecorator<N>> ChildNodes => this.Node.ChildNodes.Select(n => new HasParentDecorator<N>(n, this));
    }
}