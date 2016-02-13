namespace Elementary.Hierarchy
{
    using System.Collections.Generic;

    public interface IHasChildNodes<TNode>
    {
        bool HasChildNodes { get; }

        IEnumerable<TNode> ChildNodes { get; }
    }
}