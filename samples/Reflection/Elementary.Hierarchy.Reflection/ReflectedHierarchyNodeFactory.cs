using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyNodeFactory : IReflectedHierarchyNodeFactory
    {
        public IReflectedHierarchyNode Create(object instance, PropertyInfo property)
        {
            if (property.GetIndexParameters().Any())
                return null; // exclude indexers

            if (property.PropertyType.IsArray)
                return new ReflectedHierarchyArrayNode(instance, property, this);

            return new ReflectedHierarchyPropertyNode(instance, property, nodeFactory: this);
        }

        public IReflectedHierarchyNode Create(object instance, string id)
        {
            return new ReflectedHierarchObjectNode(instance, id, this);
        }
    }
}