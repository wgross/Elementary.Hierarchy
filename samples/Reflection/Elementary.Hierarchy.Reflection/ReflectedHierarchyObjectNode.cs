using System;

namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchyObjectNode : ReflectedHierarchyObjectNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyObjectNode(object instance, string id, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        {
            this.Id = id;
        }

        #region IReflectedHierarchyNode members

        public string Id { get; }

        public bool TrySetValue<T>(T value)
        {
            this.instance = value;
            return true;
        }

        public bool TrySetValue<T>(Func<T, T> generateNewValue)
        {
            throw new NotImplementedException();
        }

        #endregion IReflectedHierarchyNode members
    }
}