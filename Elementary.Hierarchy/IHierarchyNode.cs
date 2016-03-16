namespace Elementary.Hierarchy
{
    public interface IHierarchyNode<TNode> : IHasChildNodes<IHierarchyNode<TNode>>, IHasParentNode<IHierarchyNode<TNode>>
    {
        TNode CurrentNode { get; }
    }
}