using Elementary.Hierarchy.Collections.Nodes;
using System;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    /// <summary>
    /// Based on the basic HierarchyWriter writer implemegs the algorothm to
    /// retrieve or create a node at the positon of the given node path.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TKey">type of the path items</typeparam>
    public class GetOrCreateNodeHierarchyWriter<TKey, TNode>
        where TNode : IHierarchyNodeWriter<TNode>, IHasIdentifiableChildNodes<TKey, TNode>
    {
        private readonly Func<TKey, TNode> createNode;

        public TNode DescandantAt { get; private set; }

        public GetOrCreateNodeHierarchyWriter(Func<TKey, TNode> createNode)
        {
            this.createNode = createNode;
        }

        public TNode Visit(TNode node, HierarchyPath<TKey> path)
        {
            if (path.IsRoot)
            {
                this.DescandantAt = node;
                return node;
            }
            else
            {
                if (node.TryGetChildNode(path.Items.First(), out var child))
                    return node.ReplaceChild(child, this.Visit(child, path.SplitDescendants()));
                else
                    return node.AddChild(this.Visit(this.createNode(path.Items.First()), path.SplitDescendants()));
            }
        }
    }
}