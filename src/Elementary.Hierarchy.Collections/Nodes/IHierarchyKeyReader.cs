namespace Elementary.Hierarchy.Collections.Nodes
{
    public interface IHierarchyKeyReader<TKey>
    {
        bool TryGetKey(out TKey value);
    }
}