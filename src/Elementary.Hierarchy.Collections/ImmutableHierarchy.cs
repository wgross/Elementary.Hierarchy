using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Elementary.Hierarchy.Collections
{
    public class ImmutableHierarchy <TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Construction and initialization of this instance

        public ImmutableHierarchy()
            : this(pruneOnUnsetValue: false)
        {
        }

        private ImmutableHierarchy(bool pruneOnUnsetValue)
        {
            this.rootNode = new ImmutableNode<TKey, TValue>();
            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private ImmutableNode<TKey, TValue> rootNode;

        // MSDN: Do not store SpinLock instances in readonly fields.
        private SpinLock writeLock = new SpinLock();

        private readonly bool pruneOnUnsetValue;

        #endregion Construction and initialization of this instance

        #region IHierarchy Members

        /// <summary>
        /// Starts a traversal of the hierarchy at the specified hierachy node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            return ((IHierarchyNode<TKey, TValue>)new HierarchyTraverser<TKey, TValue, ImmutableNode<TKey, TValue>>(this.rootNode)).DescendantAt(startAt);
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
                bool isLocked = false;
                try
                {
                    this.writeLock.Enter(ref isLocked);

                    var writer = new SetOrAddNodeValueWriter<TKey, TValue, ImmutableNode<TKey, TValue>>(createNode: key => new ImmutableNode<TKey, TValue>(key));

                    // if the root node has changed, it substitutes the existing root node.

                    this.rootNode = writer.SetValue(this.rootNode, path, value);
                }
                finally
                {
                    if (isLocked)
                        this.writeLock.Exit();
                }
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
            bool isLocked = false;
            try
            {
                this.writeLock.Enter(ref isLocked);

                var writer = new SetOrAddNodeValueWriter<TKey, TValue, ImmutableNode<TKey, TValue>>(createNode: key => new ImmutableNode<TKey, TValue>(key));

                // if the root node has changed, it substitutes the existing root node.

                this.rootNode = writer.AddValue(this.rootNode, path, value);
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
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

            if (this.rootNode.TryGetDescendantAt(path, out var descendantNode))
                return descendantNode.TryGetValue(out value);

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
            bool isLocked = false;
            try
            {
                var writer = new RemoveValueAndPruneHierarchyWriter<TKey, TValue, ImmutableNode<TKey, TValue>>();
                writer.ClearValue(this.rootNode, path);

                return writer.ValueWasCleared;
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
        }

        /// <summary>
        /// Removes the specifed node fro the hierarchy. If the node has child nodes and
        /// <paramref name="recurse"/> is true, the complete subnode is removed.
        /// If recurse is specified removal fails if teh node has subnodes.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recurse">Indicats if the removal contains ths subnodes</param>
        /// <returns>true, if nod (and subnodes) has been removed, false otherwise</returns>
        public bool RemoveNode(HierarchyPath<TKey> path, bool recurse)
        {
            bool isLocked = false;
            try
            {
                this.writeLock.Enter(ref isLocked);
                if (path.IsRoot)
                {
                    if (!recurse && this.rootNode.HasChildNodes)
                    {
                        // is recurse is not set, the root node can be exhanged if the root has no child nodes

                        return false;
                    }

                    this.rootNode = new ImmutableNode<TKey, TValue>();
                    return true;
                }
                else
                {
                    // this isn't a special case.
                    // use the hierachy writer for inner nodes
                    var writer = new RemoveNodeHierarchyWriter<TKey, ImmutableNode<TKey, TValue>>();
                    var resultRootNode = writer.RemoveNode(this.rootNode, path, recurse, out var nodeWasRemoved);
                    if (!object.ReferenceEquals(resultRootNode, rootNode))
                        this.rootNode = resultRootNode;

                    return nodeWasRemoved;
                }
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
        }

        #endregion IHierarchy Members
    }
}