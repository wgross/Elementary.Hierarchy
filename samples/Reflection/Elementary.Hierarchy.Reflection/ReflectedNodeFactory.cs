using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class MapValueTypesAndStringAsLeaf : IReflectedHierarchyNodeFactory
    {
        public IReflectedHierarchyNode SelectReflectedNode(object propertyOwner, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.Equals(typeof(string)))
            {
                return new ReflectedLeafNode(propertyOwner, propertyInfo);
            }
            else
            {
                return new ReflectedInnerNode(propertyInfo.GetValue(propertyOwner), this);
            }
        }
    }
}