namespace Elementary.Hierarchy.Nodes
{
    public interface IIdentifiableNode<K>
    {
        K Key { get; }

        bool HasKey { get; }
    }
}