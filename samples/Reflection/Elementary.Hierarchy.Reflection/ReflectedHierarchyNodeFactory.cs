using System.Collections;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyNodeFactory : IReflectedHierarchyNodeFactory
    {
        public virtual IReflectedHierarchyNode Create(object instance, PropertyInfo property)
        {
            if (property.GetIndexParameters().Any())
                return null; // exclude indexers

            if (property.PropertyType.IsArray)
                return new ReflectedHierarchyArrayNode(instance, property, this);

            if (property.PropertyType != typeof(string)) // string is also enumerable but is treated like a 'value type'
                if (property.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
                    return new ReflectedEnumerableNode(instance, property, this);

            return new ReflectedHierarchyPropertyNode(instance, property, nodeFactory: this);
        }

        public IReflectedHierarchyNode Create(object instance, string id)
        {
            return new ReflectedHierarchyObjectNode(instance, id, this);
        }
    }
}