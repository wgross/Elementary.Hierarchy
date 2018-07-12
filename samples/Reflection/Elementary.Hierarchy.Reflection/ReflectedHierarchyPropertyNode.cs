using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    /// <summary>
    /// An inner node of the refleceted hierrachy refers always to an instance and a property info.
    /// The name of the property is the key of the child in the collectin of child nodes of ites owning instance.
    /// </summary>
    public sealed class ReflectedHierarchyPropertyNode : ReflectedPropertyNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyPropertyNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, propertyInfo, nodeFactory)

        {
        }

        #region IHasChildNodes members

        public bool HasChildNodes => this.ChildPropertyInfos.Any();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => this.ChildPropertyInfos.Select(pi => this.nodeFactory.Create(this.instance, pi)).Where(n => n != null);

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

        public bool TrySetValue<T>(T value)
        {
            if (this.IsNotAssignable<T>())
                return false;

            this.propertyInfo.SetValue(this.instance, value);
            return true;
        }

        public bool TrySetValue<T>(Func<T, T> generateNewValue)
        {
            return this.TrySetValue(generateNewValue((T)this.NodeValue));
        }
    }
}