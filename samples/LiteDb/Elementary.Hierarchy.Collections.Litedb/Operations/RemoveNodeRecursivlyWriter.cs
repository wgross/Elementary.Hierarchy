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
        
        //protected override TNode RemoveDestinationNode(TNode node, bool recurse, out bool hasRemovedNode)
        //{
        //    if (node.RemoveNode(recurse))
        //    {
        //        hasRemovedNode = true;
        //        return null;

        //    }
        //    else
        //    {
        //        hasRemovedNode = false;
        //        return node;
        //    }
        //}
    }
}