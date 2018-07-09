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

        /// <summary>
        /// A property node resolves its inner value by getting the property value of the instance the property
        /// actually belongs to,
        /// </summary>
        protected override object NodeValue => this.propertyInfo.GetValue(this.instance);

        #region IReflectedHierarchyNode members

        /// <summary>
        /// The name of a property node is the name of the property it refers to.
        /// </summary>
        public string Id => this.propertyInfo.Name;

        #endregion IReflectedHierarchyNode members
    }
}