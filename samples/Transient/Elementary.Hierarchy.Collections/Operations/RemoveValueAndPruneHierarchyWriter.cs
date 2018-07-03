﻿using Elementary.Hierarchy.Collections.Nodes;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    public class RemoveValueAndPruneHierarchyWriter<TKey, TValue, TNode>
        where TNode :
            IHierarchyValueWriter<TValue>,
            IHierarchyValueReader<TValue>,
            IHierarchyNodeWriter<TNode>,
            IHasIdentifiableChildNodes<TKey, TNode>,
            IHasChildNodes<TNode>
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

                    // now inspect the result:
                    // try to prune if a vaue has been removes an pruning was requested

                    if (descendantWasReached)
                    {
                        // find out if the cleared childs descandants has values
                        // if a value is still present, keep the sub tree

                        if (childNode.Descendants().Any(n => n.TryGetValue(out var value)))
                            return node;

                        // the childs descandants have no values.
                        // remove this sub tree from the hierarcy completely

                        return node.RemoveChild(childNode);
                    }
                }
            }
            return node;
        }
    }
}