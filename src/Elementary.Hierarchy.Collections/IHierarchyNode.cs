using Elementary.Hierarchy.Collections.Nodes;

namespace Elementary.Hierarchy.Collections
{
    /// <summary>
    /// Shared contract of hierarchy nodes holding a value
    /// </summary>
    /// <typeparam name="TKey">type of the path items</typeparam>
    /// <typeparam name="TValue">type of the value</typeparam>
    public interface IHierarchyNode<TKey, TValue> :
        IHasIdentifiableChildNodes<TKey, IHierarchyNode<TKey, TValue>>,
        IHasChildNodes<IHierarchyNode<TKey, TValue>>,
        IHasParentNode<IHierarchyNode<TKey, TValue>>,
        IHierarchyValueReader<TValue>
    {
        /// <summary>
        /// Returns the complete path of the node with the hierarchy
        /// </summary>
        HierarchyPath<TKey> Path { get; }
    }
}