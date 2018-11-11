using System;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchy
    {
        private readonly ILiteDbHierarchyNodeRepository repository;

        public LiteDbHierarchy(ILiteDbHierarchyNodeRepository repository)
        {
            this.repository = repository;
        }

        public LiteDbHierarchyNode Traverse() => new LiteDbHierarchyNode(this.repository, this.repository.Root);
    }
}