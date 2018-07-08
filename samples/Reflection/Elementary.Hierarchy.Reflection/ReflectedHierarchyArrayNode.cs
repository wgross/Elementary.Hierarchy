using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyArrayNode : ReflectedPropertyNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyArrayNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, propertyInfo, nodeFactory)
        {
        }

        #region IHasChildNodes members

        public override bool HasChildNodes => false;

        public override IEnumerable<IReflectedHierarchyNode> ChildNodes => Enumerable.Empty<IReflectedHierarchyNode>();

        #endregion IHasChildNodes members

        #region IReflectedHierarchyNode members

        public bool TrySetValue<T>(T value)
        {
            throw new NotImplementedException();
        }

        public override (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            return (false, null);
        }

        #endregion IReflectedHierarchyNode members
    }
}