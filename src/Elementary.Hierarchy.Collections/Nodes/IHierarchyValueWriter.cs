namespace Elementary.Hierarchy.Collections.Nodes
{
    public interface IHierarchyValueWriter<TValue>
    {
        void SetValue(TValue value);

        bool RemoveValue();
    }
}