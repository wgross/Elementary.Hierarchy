using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public abstract class ReflectedNodeBase
    {
        protected readonly object instance;
        private readonly IReflectedHierarchyNodeFactory nodeFactory;

        public ReflectedNodeBase(object instance, IReflectedHierarchyNodeFactory nodeFactory)
        {
            this.instance = instance;
            this.nodeFactory = nodeFactory;
        }

        protected abstract object NodeValue { get; }

        /// <summary>
        /// The child nodes of this node are all properties of this NodeValue.
        /// </summary>
        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.NodeValue.GetType().GetProperties();

        public virtual bool HasChildNodes => ChildPropertyInfos.Any();

        public virtual IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => this.nodeFactory.Create(this.instance, pi)).Where(n => n != null);

        public virtual (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            var childNode = this.ChildPropertyInfos
                .Where(pi => pi.Name.Equals(id))
                .Select(pi => this.nodeFactory.Create(this.NodeValue, pi))
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