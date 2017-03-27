namespace Elementary.Hierarchy
{
    /// <summary>
    /// Provides an implementation contract of the concept of 'having children which can be identified by an id'.
    /// The id has to be unique for the children of one parent only.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public interface IHasIdentifiableChildNodes<TKey, TNode> //: IHasChildNodes<TNode>
    {
        /// <summary>
        /// Tries to find a child node indetified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">unique identifier of a child node</param>
        /// <param name="childNode">contains the chhild node, if found.</param>
        /// <returns>true if found, false otherwise</returns>
        bool TryGetChildNode(TKey id, out TNode childNode);
    }
}