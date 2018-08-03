using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class FlattedObjectHierarchyNodeFactory : ReflectedHierarchyNodeFactory, IReflectedHierarchyNodeFactory
    {
        public override IReflectedHierarchyNode Create(object instance, PropertyInfo property)
        {
            var instanceType = instance.GetType();

            if (typeof(string).Equals(instanceType))
                return null; // ignore strings properties

            if (instanceType.IsValueType)
                return null;

            return base.Create(instance, property);
        }
    }
}