using Elementary.Hierarchy.Collections.LiteDb.Nodes;
using Elementary.Hierarchy.Collections.Nodes;

namespace Elementary.Hierarchy.Collections.Operations
{
    public class RemoveNodeRecursivlyWriter<TValue, TNode> : RemoveNodeHierarchyWriter<string, TNode>
        where TNode : LiteDbMutableNode<TValue>,
            IHierarchyNodeWriter<TNode>,
            IHasIdentifiableChildNodes<string, TNode>,
            IHasChildNodes<TNode>
    {
        public RemoveNodeRecursivlyWriter(bool recurse)
            : base(recurse)
        { }

        protected override TNode RemoveChildNode(TNode node, TNode childNode)
        {
            if (childNode.EnsureChildNodesAreDeleted())
                return (TNode)node.RemoveChild(childNode);
            else return (TNode)node;
        }
    }
}