using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections
{
    public class MutableHierarchy<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Construction and initialization of this instance

        public MutableHierarchy()
            : this(pruneOnUnsetValue: false)
        {
        }

        private MutableHierarchy(bool pruneOnUnsetValue)
        {
            this.rootNode = MutableNode<TKey, TValue>.CreateRoot();
            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private MutableNode<TKey, TValue> rootNode;

        private readonly bool pruneOnUnsetValue;

        #endregion Construction and initialization of this instance

        #region IHierarchy Members

        /// <summary>
        /// Starts a traversal of the hierarchy at the specified hierachy node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            return ((IHierarchyNode<TKey, TValue>)new HierarchyTraverser<TKey, TValue, MutableNode<TKey, TValue>>(this.rootNode)).DescendantAt(startAt);
        }

        /// <summary>
        /// Set the value of the specified node of the hierarchy.
        /// if the node doesn't exist, it is created.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TValue this[HierarchyPath<TKey> path]
        {
            get
            {
                if (this.TryGetValue(path, out var value))
                    return value;

                throw new KeyNotFoundException($"path '{path}' doesn't exist or has no value");
            }
            set
            {
                new SetOrAddNodeValueWriter<TKey, TValue, MutableNode<TKey, TValue>>(createNode: key => new MutableNode<TKey, TValue>(key))
                    .SetValue(this.rootNode, path, value);
            }
        }

        /// <summary>
        /// Adds a value to the immutable hierarchy at the specified position.
        /// </summary>
        /// <param name="path">Specifies where to set the value</param>
        /// <param name="value">the value to keep</param>
        /// <returns>returns this</returns>
        public void Add(HierarchyPath<TKey> path, TValue value)
        {
            new SetOrAddNodeValueWriter<TKey, TValue, MutableNode<TKey, TValue>>(createNode: key => new MutableNode<TKey, TValue>(key))
                .AddValue(this.rootNode, path, value);
        }

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="path">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> path, out TValue value)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var (found, node) = this.rootNode.TryGetDescendantAt(path);
            if(found)
                return node.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the value from the specified node in hierarchy.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if value was removed, false otherwise</returns>
        public bool Remove(HierarchyPath<TKey> path)
        {
            var writer = new RemoveValueAndPruneHierarchyWriter<TKey, TValue, MutableNode<TKey, TValue>>();
            writer.ClearValue(this.rootNode, path);

            return writer.ValueWasCleared;
        }

        public bool RemoveNode(HierarchyPath<TKey> path, bool recurse)
        {
            if (path.IsRoot)
            {
                if (!recurse && this.rootNode.HasChildNodes)
                {
                    // is recurse is not set, the root node can be exhanged if the root has no child nodes

                    return false;
                }

                this.rootNode = MutableNode<TKey, TValue>.CreateRoot();
                return true;
            }
            else
            {
                // this isn't a special case.
                // use the hierachy writer for inner nodes
                var writer = new RemoveNodeHierarchyWriter<TKey, MutableNode<TKey, TValue>>();
                var result = writer.RemoveNode(this.rootNode, path, recurse, out var nodeWasRemoved);
                return nodeWasRemoved;
            }
        }

        #endregion IHierarchy Members
    }
}