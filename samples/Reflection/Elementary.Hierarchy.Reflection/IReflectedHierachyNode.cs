namespace Elementary.Hierarchy.Reflection
{
    public interface IReflectedHierarchyNode : IHasChildNodes<IReflectedHierarchyNode>, IHasIdentifiableChildNodes<string, IReflectedHierarchyNode>
    {
        (bool, T) TryGetValue<T>();

        bool TrySetValue<T>(T value);

    }
}