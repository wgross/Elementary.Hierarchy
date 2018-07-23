using System;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchyRootNode : ReflectedHierarchyObjectNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyRootNode(object instance, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)
        { }

        #region IReflectedHierarchyNode members

        public string Id => string.Empty;

        /// <summary>
        /// Root nodes are readonly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
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