namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchy
    {
        public static IReflectedHierarchyNode Create<T>(T root)
        {
            return new ReflectedHierarchyNodeFactory().Create(root, id: null);
        }
    }
}