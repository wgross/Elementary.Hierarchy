namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchyRootNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        public ReflectedHierarchyRootNode(object instance)
                    : base(instance)

        { }

        protected override object NodeValue => this.instance;

        public bool TrySetValue<T>(T value)
        {
            return false;
        }
    }
}