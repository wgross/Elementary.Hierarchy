namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchyRootNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyRootNode(object instance, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        { }

        protected override object NodeValue => this.instance;

        #region IReflectedHierarchyNode members

        public string Id => string.Empty;

        public bool TrySetValue<T>(T value)
        {
            return false;
        }

        #endregion IReflectedHierarchyNode members
    }
}