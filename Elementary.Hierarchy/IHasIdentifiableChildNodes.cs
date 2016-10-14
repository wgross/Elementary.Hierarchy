namespace Elementary.Hierarchy
{
    public interface IHasIdentifiableChildNodes<TKey, TNode> : IHasChildNodes<TNode>
    {
        bool TryGetChildNode(TKey id, out TNode childNode);
    }
}