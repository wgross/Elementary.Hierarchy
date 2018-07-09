using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override bool HasChildNodes => ((Array)this.NodeValue).Length != 0;

        public override IEnumerable<IReflectedHierarchyNode> ChildNodes
        {
            get
            {
                Array ary = (Array)this.NodeValue;
                for (int i = 0; i < ary.Length; i++)
                    yield return this.nodeFactory.Create(ary.GetValue(i), i.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion IHasChildNodes members

        #region IReflectedHierarchyNode members

        public bool TrySetValue<T>(T value)
        {
            throw new NotImplementedException();
        }

        public override (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            if (!int.TryParse(id, out var index))
                return (false, null);

            var tmp = this.nodeFactory.Create(((Array)this.NodeValue).GetValue(int.Parse(id)), id);
            return (tmp != null, tmp);
        }

        #endregion IReflectedHierarchyNode members
    }
}