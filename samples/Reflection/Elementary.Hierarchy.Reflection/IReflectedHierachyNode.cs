using System;

namespace Elementary.Hierarchy.Reflection
{
    public interface IReflectedHierarchyNode : IHasChildNodes<IReflectedHierarchyNode>, IHasIdentifiableChildNodes<string, IReflectedHierarchyNode>
    {
        string Id { get; }

        (bool, T) TryGetValue<T>();

        bool TrySetValue<T>(T value);

        bool TrySetValue<T>(Func<T, T> generateNewValue);
    }
}