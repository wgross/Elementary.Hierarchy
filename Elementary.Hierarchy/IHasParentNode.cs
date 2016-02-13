namespace Elementary.Hierarchy
{
    public interface IHasParentNode<TNode>
    {
        bool HasParentNode { get; }

        TNode ParentNode { get; }
    }
}