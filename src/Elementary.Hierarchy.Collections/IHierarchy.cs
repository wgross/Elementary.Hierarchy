namespace Elementary.Hierarchy.Collections
{
    /// <summary>
    /// Shared contect of hierachical data strcutures.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IHierarchy<TKey, TValue>
    {
        /// <summary>
        /// Sets the value of a hierarchy node.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns></returns>
        TValue this[HierarchyPath<TKey> hierarchyPath] { set; }

        /// <summary>
        /// Adds a value to the hierarchy.
        /// The node must not exits or must no have a value yet
        /// </summary>
        /// <param name="hierarchyPath">hierachical key</param>
        /// <param name="value">value to store</param>
        void Add(HierarchyPath<TKey> hierarchyPath, TValue value);

        /// <summary>
        /// Removes values from the hierarchy by default.
        /// If a maxDepth s spefices > 1 values are removed from
        /// descenadnt nodes.
        /// </summary>
        /// <param name="hierarchyPath">specifed the position of the start node for removal of values</param>
        /// <param name="maxDepth">dept of value removal. 1 removes ath te specified position only, > 1 removes at descendants, 0 removes nothing</param>
        /// <returns>true if at least one level was removed, false otherwise</returns>
        bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = null);

        /// <summary>
        /// Retrieves a value from the hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">hierarchical key</param>
        /// <param name="value">recives the found value</param>
        /// <returns>true if the node exists and has a value</returns>
        bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value);

        /// <summary>
        /// Provides methods for hierachy traversal. The traversal starts at the given node.
        /// The start node must exist
        /// </summary>
        /// <param name="startAt">hierachical key of the start node</param>
        /// <returns></returns>
        IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt);

        /// <summary>
        /// Removes a node from the hierachy. This may include descendants.
        /// Removal fails if descendatns exist but <paramref name="recurse"/> was false.
        /// </summary>
        /// <param name="hierarchyPath">hierachical key of the node the remove</param>
        /// <param name="recurse">indicates if descandants must be removed too</param>
        /// <returns>true, if nodes have been removed</returns>
        bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse);
    }
}