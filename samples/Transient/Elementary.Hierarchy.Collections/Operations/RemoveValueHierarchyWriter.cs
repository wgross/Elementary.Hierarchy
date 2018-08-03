using Elementary.Hierarchy.Collections.Nodes;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    public class RemoveValueHierarchyWriter<TKey, TValue, TNode>
        where TNode :
            IHierarchyValueWriter<TValue>,
            IHierarchyValueReader<TValue>,
            IHierarchyNodeWriter<TNode>,
            IHasIdentifiableChildNodes<TKey, TNode>
    {
        public bool ValueWasCleared { get; private set; }

        public TNode ClearValue(TNode node, HierarchyPath<TKey> path)
        {
            return this.ClearValue(node, path, out var hasCleared);
        }

        private TNode ClearValue(TNode node, HierarchyPath<TKey> path, out bool descendantWasReached)
        {
            descendantWasReached = path.IsRoot;

            if (descendantWasReached)
            {
                // not path items left. node is the destination of the visitors
                // clear the value tahe tell paranet node if the value has be removed at all

                this.ValueWasCleared = node.RemoveValue();
            }
            else
            {
                var (found, childNode) = node.TryGetChildNode(path.Items.First());
                if (found)
                {
                    // descend further into the tree.

                    this.ClearValue(childNode, path.Descendants(), out descendantWasReached);
                }
            }
            return node;
        }
    }
}