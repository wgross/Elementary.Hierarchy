namespace Elementary.Hierarchy
{
    /// <summary>
    /// Visitor
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class HierarchyNodeVisitor<TNode> where TNode : IHasChildNodes<TNode>
    {
        /// <summary>
        /// Override this method to implement your visiting logic
        /// </summary>
        /// <param name="node"></param>
        public virtual void Visit(TNode node)
        {
        }
    }

    /// <summary>
    /// Visitor
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class HierarchyNodeWriter<TNode> where TNode : class, IHasChildNodes<TNode>
    {
        /// <summary>
        /// Override this method to implement your visiting logic.
        /// Returns null must delete the current node, return the same instance does nothing,
        /// substitutes am exsiting node.
        /// </summary>
        /// <param name="node"></param>
        public virtual TNode Visit(TNode node)
        {
            return node;
        }
    }
}