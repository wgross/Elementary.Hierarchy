using System;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedPropertyNodeBase : ReflectedNodeBase
    {
        protected readonly PropertyInfo propertyInfo;

        public ReflectedPropertyNodeBase(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        {
            this.propertyInfo = propertyInfo;
        }

        protected override object NodeValue => throw new NotImplementedException();

        #region IReflectedHierarchyNode members

        public string Id => this.propertyInfo.Name;

        #endregion IReflectedHierarchyNode members
    }
}