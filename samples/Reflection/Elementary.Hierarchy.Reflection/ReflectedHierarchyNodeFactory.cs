using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyNodeFactory : IReflectedHierarchyNodeFactory
    {
        public IReflectedHierarchyNode Create(object instance, PropertyInfo property)
        {
            //if (property.GetMethod?.IsSpecialName ?? false)
            //    return null;

            if (property.PropertyType.IsArray)
                return new ReflectedHierarchyArrayNode(instance, property, this);

            return new ReflectedHierarchyNode(instance, property, nodeFactory: this);
        }
    }
}