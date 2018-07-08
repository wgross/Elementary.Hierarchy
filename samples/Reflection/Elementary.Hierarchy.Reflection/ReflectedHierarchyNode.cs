using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    /// <summary>
    /// An inner node of the refleceted hierrachy refers always to an instance and a property info.
    /// The name of the property is the key of the child in the collectin of child nodes of ites owning instance.
    /// </summary>
    public class ReflectedHierarchyNode : ReflectedPropertyNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, propertyInfo, nodeFactory)

        {
        }

        protected override object NodeValue => this.propertyInfo.GetValue(this.instance);

        public bool TrySetValue<T>(T value)
        {
            if (this.propertyInfo.GetSetMethod() == null)
                return false;

            if (!this.propertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
                return false;

            this.propertyInfo.SetValue(this.instance, value);
            return true;
        }
    }
}