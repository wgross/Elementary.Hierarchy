namespace Elementary.Hierarchy.Reflection
{
    public class ReflectedHierarchy
    {
        public static IReflectedHierarchyNode Create<T>(T hierarchy)
        {
            return new ReflectedInnerNode(hierarchy, new MapValueTypesAndStringAsLeaf());
        }
    }
}