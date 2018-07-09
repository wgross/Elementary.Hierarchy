namespace Elementary.Hierarchy.Reflection
{
    public sealed class ReflectedHierarchObjectNode : ReflectedNodeBase, IReflectedHierarchyNode
    {
        
        public ReflectedHierarchObjectNode(object instance, string id,  IReflectedHierarchyNodeFactory nodeFactory)
            : base(instance, nodeFactory)

        {
            this.Id = id ?? string.Empty;
        }

        protected override object NodeValue => this.instance;

        #region IReflectedHierarchyNode members

        public string Id { get;  }

        public bool TrySetValue<T>(T value)
        {
            return false;
        }

        #endregion IReflectedHierarchyNode members
    }
}