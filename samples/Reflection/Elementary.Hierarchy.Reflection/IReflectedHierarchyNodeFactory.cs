using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Elementary.Hierarchy.Reflection
{
    public interface IReflectedHierarchyNodeFactory
    {
        IReflectedHierarchyNode SelectReflectedNode(object propertyOwner, PropertyInfo propertyInfo);
    }
}
