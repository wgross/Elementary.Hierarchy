using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public abstract class ReflectedPropertyNodeBase : ReflectedNodeBase
    {
        protected readonly PropertyInfo propertyInfo;

        public ReflectedPropertyNodeBase(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        {
            this.propertyInfo = propertyInfo;
        }

        protected override object NodeValue => this.propertyInfo.GetValue(this.instance);

     

        #region IReflectedHierarchyNode members

        public string Id => this.propertyInfo.Name;

        #endregion IReflectedHierarchyNode members
    }
}