using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    public class RemoveNodeHierarchyWriter<TKey, TNode>
        where TNode : class, IHierarchyNodeWriter<TNode>, IHasIdentifiableChildNodes<TKey, TNode>
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

        public TNode Visit(TNode node, HierarchyPath<TKey> path)
        {
            TNode returnedNode = node;

            if (!path.IsRoot)
            {
                // we are not yet there.
                // descend further down the tree

                if (node.TryGetChildNode(path.Items.First(), out var childNode))
                {
                    if (this.Visit(childNode, path.SplitDescendants()) == null)
                    {
                        returnedNode = node.RemoveChild(childNode);
                        this.HasRemovedNode = true;
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
    }
}