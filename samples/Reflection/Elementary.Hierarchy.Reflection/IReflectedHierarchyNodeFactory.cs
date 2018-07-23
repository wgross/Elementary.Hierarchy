using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public interface IReflectedHierarchyNodeFactory
    {
        IReflectedHierarchyNode Create(object instance, string id);

        IReflectedHierarchyNode Create(object instance, PropertyInfo property);
    }
}