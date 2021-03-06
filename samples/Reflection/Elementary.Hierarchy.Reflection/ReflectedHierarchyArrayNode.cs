﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchyArrayNode : ReflectedPropertyNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyArrayNode(object instance, PropertyInfo propertyInfo, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, propertyInfo, nodeFactory)
        {
        }

        #region IHasChildNodes members

        public bool HasChildNodes => ((Array)this.NodeValue).Length != 0;

        public IEnumerable<IReflectedHierarchyNode> ChildNodes
        {
            get
            {
                Array ary = (Array)this.NodeValue;
                for (int i = 0; i < ary.Length; i++)
                    yield return this.nodeFactory.Create(ary.GetValue(i), i.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildNodes

        public (bool, IReflectedHierarchyNode) TryGetChildNode(string id)
        {
            if (!int.TryParse(id, out var index))
                return (false, null);

            var ary = (Array)this.NodeValue;
            if (index > ary.Length - 1)
                return (false, null);

            return (true, this.nodeFactory.Create(ary.GetValue(index), id));
        }

        #endregion IHasIdentifiableChildNodes

        #region IReflectedHierarchyNode members

        public bool TrySetValue<T>(T value)
        {
            if (this.IsNotAssignable<T>())
                return false;

            this.propertyInfo.SetValue(this.instance, value);
            return true;
        }

        public bool TrySetValue<T>(Func<T, T> generateNewValue)
        {
            throw new NotImplementedException();
        }

        #endregion IReflectedHierarchyNode members
    }
}