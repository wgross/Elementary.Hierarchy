namespace Elementary.Hierarchy.Nodes
{
    public interface IChangeChildNodes<K, TNode> where TNode : IIdentifiableNode<K>
    {
        void Add(TNode childNode);

        bool Remove(K key);
    }
}