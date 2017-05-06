using Elementary.Hierarchy.Collections.Nodes;
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
                return RewriteHierarchyAfterRemoveNode(node, path, out hasRemovedNode, childNode);
            }
            else
            {
                // traversing further along the path isn't possible: return this node unchanged to stop

                hasRemovedNode = false;
                return node;
            }
        }

        private TNode RewriteHierarchyAfterRemoveNode(TNode node, HierarchyPath<TKey> path, out bool nodeRemoved, TNode childNode)
        {
            // traverse the tree further down along the path

            var returnedChildNode = this.RemoveNode(childNode, path.SplitDescendants(), out nodeRemoved);
            if (returnedChildNode == null)
            {
                // a value of null means this node has to be removed
                return this.RemoveChildNode(node, childNode);
            }
            else if (!object.ReferenceEquals(childNode, returnedChildNode))
            {
                // a changed child node value requires to substitute the old child with the new
                return node.ReplaceChild(childNode, returnedChildNode);
            }
            else
            {
                // an unchanged child node keeps this node unchanged as well
                return node;
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