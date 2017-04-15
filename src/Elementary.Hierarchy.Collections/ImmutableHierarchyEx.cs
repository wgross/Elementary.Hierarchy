using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using System;
using System.Threading;

namespace Elementary.Hierarchy.Collections
{
    public class ImmutableHierarchyEx<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Construction and initialization of this instance

        public ImmutableHierarchyEx()
            : this(pruneOnUnsetValue: false, getDefaultValue: null)
        {
        }

        public ImmutableHierarchyEx(Func<HierarchyPath<TKey>, TValue> getDefaultValue)
            : this(pruneOnUnsetValue: false, getDefaultValue: getDefaultValue)
        {
        }

        public ImmutableHierarchyEx(bool pruneOnUnsetValue)
            : this(pruneOnUnsetValue: pruneOnUnsetValue, getDefaultValue: null)
        {
        }

        private ImmutableHierarchyEx(bool pruneOnUnsetValue, Func<HierarchyPath<TKey>, TValue> getDefaultValue)
        {
            this.rootNode = new ImmutableNode<TKey, TValue>();
            this.getDefaultValue = getDefaultValue;

            if (this.getDefaultValue != null)
            {
                rootNode.SetValue(this.getDefaultValue(HierarchyPath.Create<TKey>()));
            }

            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private ImmutableNode<TKey, TValue> rootNode;

        // MSDN: Do not store SpinLock instances in readonly fields.
        private SpinLock writeLock = new SpinLock();

        private readonly bool pruneOnUnsetValue;

        private readonly Func<HierarchyPath<TKey>, TValue> getDefaultValue;

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
            set
            {
                bool isLocked = false;
                try
                {
                    if (this.getDefaultValue != null)
                        throw new NotSupportedException("default value");

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
                if (this.getDefaultValue != null)
                    throw new NotSupportedException("default value");

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
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            ImmutableNode<TKey, TValue> descendantNode;
            if (this.rootNode.TryGetDescendantAt(hierarchyPath, out descendantNode))
                return descendantNode.TryGetValue(out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the value from the specified node in hierarchy.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns>true if value was removed, false otherwise</returns>
        public bool Remove(HierarchyPath<TKey> hierarchyPath, int? maxDepth = null)
        {
            if (maxDepth != null)
                throw new NotSupportedException(nameof(maxDepth));

            bool isLocked = false;
            try
            {
                var writer = new RemoveValueAndPruneHierarchyWriter<TKey, TValue, ImmutableNode<TKey, TValue>>();
                writer.ClearValue(this.rootNode, hierarchyPath);

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
        /// <param name="hierarchyPath"></param>
        /// <param name="recurse">Indicats if the removal contains ths subnodes</param>
        /// <returns>true, if nod (and subnodes) has been removed, false otherwise</returns>
        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            bool isLocked = false;
            try
            {
                this.writeLock.Enter(ref isLocked);
                if (hierarchyPath.IsRoot)
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
                    var writer = new RemoveNodeHierarchyWriter<TKey, ImmutableNode<TKey, TValue>>(recurse);
                    var resultRootNode = writer.Visit(this.rootNode, hierarchyPath);
                    if (!object.ReferenceEquals(resultRootNode, rootNode))
                        this.rootNode = resultRootNode;

                    return writer.HasRemovedNode;
                }
            }
            finally
            {
                if (isLocked)
                    this.writeLock.Exit();
            }
        }

        #endregion IHierarchy Members

        private ImmutableNode<TKey, TValue> GetOrCreateNode(HierarchyPath<TKey> hierarchyPath)
        {
            GetOrCreateNodeWriter<TKey, ImmutableNode<TKey, TValue>> writer = null;
            if (this.getDefaultValue == null)
                writer = new GetOrCreateNodeWriter<TKey, ImmutableNode<TKey, TValue>>(createNode: key => new ImmutableNode<TKey, TValue>(key));
            else throw new NotSupportedException("default value");

            // if the root node has changed, it substitutes the existing root node.

            var resultRootNode = writer.Visit(this.rootNode, hierarchyPath);
            if (!object.ReferenceEquals(this.rootNode, resultRootNode))
                this.rootNode = resultRootNode;

            return writer.DescandantAt;
        }
    }
}