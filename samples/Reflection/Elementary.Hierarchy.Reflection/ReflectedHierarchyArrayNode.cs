using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyArrayNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        private readonly PropertyInfo propertyInfo;

        public ReflectedHierarchyArrayNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)
        {
            this.propertyInfo = propertyInfo;
        }

        #region IHasChildNodes members

        public override bool HasChildNodes => false;

        public override IEnumerable<IReflectedHierarchyNode> ChildNodes => Enumerable.Empty<IReflectedHierarchyNode>();

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildNodes members

        protected override object NodeValue => (Array)this.propertyInfo.GetValue(this.instance);

        #endregion IHasIdentifiableChildNodes members

        #region IReflectedHierarchyNode members

        public override string Id => this.propertyInfo.Name;

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