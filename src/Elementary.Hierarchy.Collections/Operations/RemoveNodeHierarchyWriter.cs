﻿using Elementary.Hierarchy.Collections.Nodes;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    public class RemoveNodeHierarchyWriter<TKey, TNode>
        where TNode : class, IHierarchyNodeWriter<TNode>, IHasIdentifiableChildNodes<TKey, TNode>, IHasChildNodes<TNode>
    {
        private bool recurse;

        public RemoveNodeHierarchyWriter()
            : this(false)
        { }

        public RemoveNodeHierarchyWriter(bool recurse)
        {
            this.recurse = recurse;
        }

        public TNode RemoveNode(TNode node, HierarchyPath<TKey> path, out bool hasRemovedNode)
        {
            if (path.IsRoot)
            {
                return RemoveNode(node, out hasRemovedNode);
            }
            else if (node.TryGetChildNode(path.Items.First(), out var childNode))
            {
                return RewriteParentNodeAfterChildNodeRemoved(node, childNode, path, out hasRemovedNode);
            }
            else
            {
                // traversing further along the path isn't possible: return this node unchanged to stop

                hasRemovedNode = false;
                return node;
            }
        }

        private TNode RewriteParentNodeAfterChildNodeRemoved(TNode parentNode, TNode childNode, HierarchyPath<TKey> remainingPath, out bool nodeRemoved)
        {
            // traverse the tree further down along the path
            // depending on the result of the removal operations this node has to be rewritten

            var returnedChildNode = this.RemoveNode(childNode, remainingPath.SplitDescendants(), out nodeRemoved);

            if (returnedChildNode == null)
            {
                // a value of null means the child node has to be removed from this (parent) node.
                return this.RemoveChildNode(parentNode, childNode);
            }
            else if (object.ReferenceEquals(childNode, returnedChildNode))
            {
                // child node hasn't changhe: keep this node unchanged as well
                return parentNode;
            }
            else
            {
                // a changed child node value requires to substitute the old child with the new
                return parentNode.ReplaceChild(childNode, returnedChildNode);
            }
        }

        private TNode RemoveNode(TNode node, out bool hasRemovedNode)
        {
            // the traversal reached its destination, this is the node to delete

            if (this.recurse)
            {
                hasRemovedNode = true;
                return null; // recursive deletion is always allowed: just return null to commit.
            }

            if (node.HasChildNodes)
            {
                hasRemovedNode = false;
                return node; // not recursive but has child node: deletion is forbidden, return unchanged node
            }

            // not recursive, no child nodes: return null to commit deletion
            hasRemovedNode = true;
            return null;
        }

        public TNode RemoveChildNodes(TNode node, out bool hasRemovedNode)
        {
            var returnedNode = node;
            if (node.HasChildNodes)
            {
                foreach (var childNode in node.ChildNodes.ToArray())
                {
                    returnedNode = this.RemoveChildNode(returnedNode, childNode);
                }

                // after all child nodes hav bee remove he operatin is marked as completed
                hasRemovedNode = true;
            }
            else hasRemovedNode = false; // node has no chldren: a removes dod not happen

            return returnedNode;
        }

        virtual protected TNode RemoveChildNode(TNode node, TNode childNode)
        {
            return node.RemoveChild(childNode);
        }
    }
}