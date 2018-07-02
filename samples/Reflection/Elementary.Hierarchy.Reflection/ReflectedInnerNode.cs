using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedInnerNode : IReflectedHierarchyNode
    {
        private readonly object instance;
        private readonly IReflectedHierarchyNodeFactory childNodeFactory;
        private readonly PropertyInfo propertyInfo;

        public ReflectedInnerNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory childNodeFactory)
        {
            this.instance = instance;
            this.childNodeFactory = childNodeFactory;
            this.propertyInfo = propertyInfo;
        }

        private object PropertyValue => this.propertyInfo?.GetValue(this.instance) ?? this.instance;

        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.PropertyValue.GetType().GetProperties();

        public bool HasChildNodes => ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => this.childNodeFactory.SelectReflectedNode(this.instance, pi));

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            //var childNode = this.ChildPropertyInfos.Where(pi => pi.Name.Equals(id)).Select(pi => this.childNodeFactory.SelectReflectedNode(this.instance, pi)).FirstOrDefault();
            var childNode = this.ChildPropertyInfos.Where(pi => pi.Name.Equals(id)).Select(pi => new ReflectedInnerNode(this.PropertyValue, pi, this.childNodeFactory)).FirstOrDefault();
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