using System.Collections.Generic;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedLeafNode : IReflectedHierarchyNode
    {
        private readonly object propertyOwner;
        private readonly PropertyInfo propertyInfo;

        public ReflectedLeafNode(object propertyOwner, PropertyInfo propertyInfo)
        {
            this.propertyOwner = propertyOwner;
            this.propertyInfo = propertyInfo;
        }

        public bool HasChildNodes => throw new System.NotImplementedException();

        public IEnumerable<IReflectedHierarchyNode> ChildNodes => throw new System.NotImplementedException();

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            throw new System.NotImplementedException();
        }

        public (bool, T) TryGetValue<T>()
        {
            return (true, (T)(this.propertyInfo).GetValue(this.propertyOwner));
        }

        public bool TrySetValue<T>(T value)
        {
            this.propertyInfo.SetValue(this.propertyOwner, value);
            return true;
        }
    }
}