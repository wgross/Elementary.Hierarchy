namespace Elementary.Hierarchy
{
    /// <summary>
    /// This delegate type is used for travesals of hierachies which involve upwards movements in the hierachy
    /// like visiting ancestors or siblings of the start node.
    /// Because Elementary.Hierachy should support value types a generic Func<...> doesn't work.
    /// </summary>
    /// <typeparam name="TNode">type of the hierarchy nodes</typeparam>
    /// <param name="childNode">A node to start upwards traversal</param>
    /// <param name="parentNode">Parent node of the child node</param>
    /// <returns>tre, if a parnet exists, false otherwise</returns>
    public delegate bool TryGetParent<TNode>(TNode childNode, out TNode parentNode);

    /// <summary>
    /// This delegate type is used in cases where e child node is identified by a key aunder its parent node.
    /// Becase ist is not possiblle the seperate deafaul(T) for vakes nodes from a value of default(T) a try-get-delegate is needed here.
    /// </summary>
    /// <typeparam name="TKey">type of the hierarchy path item</typeparam>
    /// <typeparam name="TNode">type of the node</typeparam>
    /// <param name="startNode">node instance to seach for child item</param>
    /// <param name="path">node identifier to search for</param>
    /// <param name="childNode">found child node</param>
    /// <returns>true if child was found, false otherwise</returns>
    public delegate bool TryGetChildNode<TKey, TNode>(TNode startNode, TKey path, out TNode childNode);
}