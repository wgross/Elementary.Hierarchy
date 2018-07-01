using Elementary.Hierarchy.Collections.Nodes;
using System;

namespace Elementary.Hierarchy.Collections.Operations
{
    /// <summary>
    /// Based on the basic HierarchyWriter writer implemegs the algorothm to
    /// retrieve or create a node at the positon of the given node path.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TKey">type of the path items</typeparam>
    public class SetOrAddNodeValueWriter<TKey, TValue, TNode> : GetOrCreateNodeWriter<TKey, TNode>
        where TNode :
            IHierarchyValueWriter<TValue>,
            IHierarchyValueReader<TValue>,
            IHierarchyNodeWriter<TNode>,
            IHasIdentifiableChildNodes<TKey, TNode>
    {
        public SetOrAddNodeValueWriter(Func<TKey, TNode> createNode)
            : base(createNode)
        { }

        /// <summary>
        /// The value is added to teh hierachy if the specified node doen't hav a value.
        /// In thsi case an ArgumentException is thrown.
        /// </summary>
        /// <param name="startNode">node to start to descend along the path</param>
        /// <param name="path">path to follow from start node</param>
        /// <param name="value">vaue to add</param>
        /// <returns><paramref name="startNode"/>or its susbtitute</returns>
        public TNode AddValue(TNode startNode, HierarchyPath<TKey> path, TValue value)
        {
            var thisNode = base.GetOrCreate(startNode, path, out var descendantAt);
            if (descendantAt.TryGetValue(out var _))
                throw new ArgumentException($"{typeof(TNode).Name} at '{path}' already has a value", nameof(path));

            descendantAt.SetValue(value);

            return thisNode;
        }

        /// <summary>
        /// Sets the value at the specfied node.
        /// </summary>
        /// <param name="startNode">node to start to descend along the path</param>
        /// <param name="path">path to follow from start node</param>
        /// <param name="value">vaue to add</param>
        /// <returns><paramref name="startNode"/>or its susbtitute</returns>
        public TNode SetValue(TNode startNode, HierarchyPath<TKey> path, TValue value)
        {
            var thisNode = base.GetOrCreate(startNode, path, out var descendantAt);
            descendantAt.SetValue(value);
            return thisNode;
        }
    }
}