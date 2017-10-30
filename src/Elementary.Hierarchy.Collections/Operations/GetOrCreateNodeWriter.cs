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
    public class GetOrCreateNodeWriter<TKey, TNode>
        where TNode : IHierarchyNodeWriter<TNode>, IHasIdentifiableChildNodes<TKey, TNode>
    {
        private readonly Func<TKey, TNode> createNode;

        public GetOrCreateNodeWriter(Func<TKey, TNode> createNode)
        {
            this.createNode = createNode;
        }

        public TNode GetOrCreate(TNode node, HierarchyPath<TKey> path, out TNode descendantAt)
        {
            if (path.IsRoot)
            {
                descendantAt = node;
                return node;
            }
            else
            {
                var (found, child) = node.TryGetChildNode(path.Items.First());
                if(found)
                    return node.ReplaceChild(child, this.GetOrCreate(child, path.Descendants(), out descendantAt));
                else
                    return node.AddChild(this.GetOrCreate(this.createNode(path.Items.First()), path.Descendants(), out descendantAt));
            }
        }
    }
}