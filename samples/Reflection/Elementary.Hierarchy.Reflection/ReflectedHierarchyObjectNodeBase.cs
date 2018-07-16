using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public abstract class ReflectedHierarchyObjectNodeBase
    {
        protected object instance;
        protected readonly IReflectedHierarchyNodeFactory nodeFactory;

        public ReflectedHierarchyObjectNodeBase(object instance, IReflectedHierarchyNodeFactory nodeFactory)
        {
            this.instance = instance;
            this.nodeFactory = nodeFactory;
        }

        /// <summary>
        /// The value of an object node is the object instance itself
        /// </summary>
        protected object NodeValue => this.instance;

        protected IEnumerable<PropertyInfo> ChildPropertyInfos => this.NodeValue.GetType().GetProperties();

        #region IHasChildNodes members

        public bool HasChildNodes => this.ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this
            .ChildPropertyInfos
            .OrderBy(pi => pi.Name, StringComparer.OrdinalIgnoreCase)
            .Select(pi => this.nodeFactory.Create(this.instance, pi))
            .Where(n => n != null);

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildNode members

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            var childNode = this.ChildPropertyInfos
               .Where(pi => pi.Name.Equals(id))
               .Select(pi => this.nodeFactory.Create(this.NodeValue, pi))
               .FirstOrDefault();

            return (childNode != null, childNode);
        }

        #endregion IHasIdentifiableChildNode members

        #region IReflectedHierarchyNode members

        /// <summary>
        /// The valu eof the root node is the object is wraps
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (bool, T) TryGetValue<T>()
        {
            var nodeValue = this.NodeValue;
            if (!typeof(T).IsAssignableFrom(nodeValue.GetType()))
                return (false, default(T));

            return (true, (T)nodeValue);
        }

        #endregion IReflectedHierarchyNode members
    }
}