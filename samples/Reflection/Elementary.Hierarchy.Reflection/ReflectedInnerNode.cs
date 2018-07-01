using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedInnerNode : IReflectedHierarchyNode
    {
        private readonly object instance;
        private readonly IReflectedHierarchyNodeFactory childNodeFactory;

        public ReflectedInnerNode(object instance, IReflectedHierarchyNodeFactory childNodeFactory)
        {
            this.instance = instance;
            this.childNodeFactory = childNodeFactory;
        }

        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.instance.GetType().GetProperties();

        public bool HasChildNodes => ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => this.childNodeFactory.SelectReflectedNode(this.instance, pi));

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            var childNode = this.ChildPropertyInfos.Where(pi => pi.Name.Equals(id)).Select(pi => this.childNodeFactory.SelectReflectedNode(this.instance, pi)).FirstOrDefault();

            return (childNode != null, childNode);
        }

        public (bool, T) TryGetValue<T>()
        {
            return (false, default(T));
        }

        public bool TrySetValue<T>(T value)
        {
            return false;
        }
    }
}