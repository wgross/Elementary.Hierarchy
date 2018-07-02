using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyNode : IReflectedHierarchyNode
    {
        private readonly object instance;
        private readonly PropertyInfo propertyInfo;

        public ReflectedHierarchyNode(object instance, PropertyInfo propertyInfo)
        {
            this.instance = instance;
            this.propertyInfo = propertyInfo;
        }

        private object PropertyValue => this.propertyInfo?.GetValue(this.instance) ?? this.instance;

        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.PropertyValue.GetType().GetProperties();

        public bool HasChildNodes => ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => new ReflectedHierarchyNode(this.PropertyValue, pi));

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            var childNode = this.ChildPropertyInfos
                .Where(pi => pi.Name.Equals(id))
                .Select(pi => new ReflectedHierarchyNode(this.PropertyValue, pi))
                .FirstOrDefault();

            return (childNode != null, childNode);
        }

        public (bool, T) TryGetValue<T>()
        {
            var value = (T)(this.propertyInfo?.GetValue(this.instance) ?? this.instance);
            return (true, value);
        }

        public bool TrySetValue<T>(T value)
        {
            this.propertyInfo.SetValue(this.instance, value);
            return true;
        }
    }
}