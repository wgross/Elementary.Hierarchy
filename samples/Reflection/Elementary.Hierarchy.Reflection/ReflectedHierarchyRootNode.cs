namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchyRootNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyRootNode(object instance, IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        { }

        public override string Id => string.Empty;

        protected override object NodeValue => this.instance;

        public bool TrySetValue<T>(T value)
        {
            return false;
        }
    }
}