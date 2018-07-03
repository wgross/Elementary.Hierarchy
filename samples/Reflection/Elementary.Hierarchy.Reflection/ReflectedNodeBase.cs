using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public abstract class ReflectedNodeBase
    {
        protected readonly object instance;

        public ReflectedNodeBase(object instance)
        {
            this.instance = instance;
        }

        protected abstract object NodeValue { get; set; }

        /// <summary>
        /// The child nodes of this node are all properties of this NodeValue.
        /// </summary>
        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.NodeValue.GetType().GetProperties();

        public bool HasChildNodes => ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => new ReflectedHierarchyNode(this.instance, pi));

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            var childNode = this.ChildPropertyInfos
                .Where(pi => pi.Name.Equals(id))
                .Select(pi => new ReflectedHierarchyNode(this.NodeValue, pi))
                .FirstOrDefault();

            return (childNode != null, childNode);
        }

        public (bool, T) TryGetValue<T>()
        {
            var nodeValue = this.NodeValue;
            if (!typeof(T).IsAssignableFrom(nodeValue.GetType()))
                return (false, default(T));

            return (true, (T)nodeValue);
        }
    }
}