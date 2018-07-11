using System;

namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchObjectNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchObjectNode(object instance, string id, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        {
            this.Id = id ?? string.Empty;
        }

        protected override object NodeValue => this.instance;

        #region IReflectedHierarchyNode members

        public string Id { get; }

        public bool TrySetValue<T>(T value)
        {
            return false;
        }

        public bool TrySetValue<T>(Func<T, T> generateNewValue)
        {
            throw new NotImplementedException();
        }

        #endregion IReflectedHierarchyNode members
    }
}