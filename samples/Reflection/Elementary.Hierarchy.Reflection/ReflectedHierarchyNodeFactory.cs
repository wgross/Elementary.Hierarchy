using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyNodeFactory : IReflectedHierarchyNodeFactory
    {
        public IReflectedHierarchyNode Create(object instance, PropertyInfo property)
        {
            return new ReflectedHierarchyNode(instance, property, nodeFactory: this);
        }
    }
}