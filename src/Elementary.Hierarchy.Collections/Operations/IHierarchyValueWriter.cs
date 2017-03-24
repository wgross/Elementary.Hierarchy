namespace Elementary.Hierarchy.Collections.Operations
{
    public interface IHierarchyValueWriter<TValue>
    {
        void SetValue(TValue value);

        bool RemoveValue();
    }
}