using System;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class MapValueTypesAndStringAsLeaf : IReflectedHierarchyNodeFactory
    {
        public IReflectedHierarchyNode SelectReflectedNode(object propertyOwner, PropertyInfo propertyInfo)
        {
            //if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.Equals(typeof(string)))
            //{
            //    throw new NotImplementedException();
            //}
            //else
            //{
                return new ReflectedInnerNode(propertyOwner, propertyInfo, this);
            //}
        }
    }
}