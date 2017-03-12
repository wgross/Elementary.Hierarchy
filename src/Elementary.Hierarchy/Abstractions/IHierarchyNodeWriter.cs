﻿namespace Elementary.Hierarchy.Abstractions
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface IHierarchyNodeWriter<TNode> : IHasChildNodes<IHierarchyNodeWriter<TNode>>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(IHierarchyNodeWriter<TNode> child);

        /// <summary>
        /// Replaces the child node with a new instance
        /// </summary>
        /// <param name="childToReplace"></param>
        /// <param name="newChild"></param>
        void ReplaceChild(IHierarchyNodeWriter<TNode> childToReplace, IHierarchyNodeWriter<TNode> newChild);
    }
}