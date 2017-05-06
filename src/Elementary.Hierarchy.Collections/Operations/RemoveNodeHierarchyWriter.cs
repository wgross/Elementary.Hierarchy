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

        public bool HasRemovedNode { get; private set; } = false;

        public TNode RemoveNode(TNode node, HierarchyPath<TKey> path)
        {
            TNode returnedNode = node;

            if (!path.IsRoot)
            {
                // we are not yet there.
                // descend further down the tree

                if (node.TryGetChildNode(path.Items.First(), out var childNode))
                {
                    var returnedChildNode = this.RemoveNode(childNode, path.SplitDescendants());
                    if (returnedChildNode == null)
                    {
                        // a value of null measn this node has to be removed
                        returnedNode = this.RemoveChildNode(node, childNode);
                        this.HasRemovedNode = true;
                    }
                    else if (!object.ReferenceEquals(childNode, returnedChildNode))
                    {
                        // a changed child node value requires to substitute the old child with the new
                        returnedNode = node.ReplaceChild(childNode, returnedChildNode);
                    }
                    else
                    {
                        // an unchanged child node keeps this node unchanged as well
                        returnedNode = node;
                    }
                }
            }
            else
            {
                // this is the node to delete.
                // Validate if the node may be deleted

                if (this.recurse)
                    return null; // delete this node with ist children

                if (node.HasChildNodes)
                    return node; // no recurse, not remains untouched

                // no recurse, but also no children -> delete it
                return null;
            }

            return returnedNode;
        }

        public TNode RemoveChildNodes(TNode node)
        {
            var returnedNode = node;
            if (node.HasChildNodes)
            {
                foreach (var childNode in node.ChildNodes.ToArray())
                {
                    returnedNode = this.RemoveChildNode(returnedNode, childNode);
                    this.HasRemovedNode = true;
                }
            }

            return returnedNode;
        }

        virtual protected TNode RemoveChildNode(TNode node, TNode childNode)
        {
            return node.RemoveChild(childNode);
        }
    }
}