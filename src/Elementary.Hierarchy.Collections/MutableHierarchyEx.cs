using Elementary.Hierarchy.Collections.Nodes;
using Elementary.Hierarchy.Collections.Operations;
using Elementary.Hierarchy.Collections.Traversal;
using System;

namespace Elementary.Hierarchy.Collections
{
    public class MutableHierarchyEx<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Construction and initialization of this instance

        public MutableHierarchyEx()
            : this(pruneOnUnsetValue: false, getDefaultValue: null)
        {
        }

        public MutableHierarchyEx(Func<HierarchyPath<TKey>, TValue> getDefaultValue)
            : this(pruneOnUnsetValue: false, getDefaultValue: getDefaultValue)
        {
        }

        public MutableHierarchyEx(bool pruneOnUnsetValue)
            : this(pruneOnUnsetValue: pruneOnUnsetValue, getDefaultValue: null)
        {
        }

        private MutableHierarchyEx(bool pruneOnUnsetValue, Func<HierarchyPath<TKey>, TValue> getDefaultValue)
        {
            this.rootNode = MutableNode<TKey, TValue>.CreateRoot();
            this.getDefaultValue = getDefaultValue;

            if (this.getDefaultValue != null)
            {
                rootNode.SetValue(this.getDefaultValue(HierarchyPath.Create<TKey>()));
            }

            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private MutableNode<TKey, TValue> rootNode;

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
            return ((IHierarchyNode<TKey, TValue>)new HierarchyTraverser<TKey, TValue, MutableNode<TKey, TValue>>(this.rootNode)).DescendantAt(startAt);
        }

        /// <summary>
        /// Set the value of the specified node of the hierarchy.
        /// if the node doesn't exist, it is created.
        /// </summary>
        /// <param name="hierarchyPath"></param>
        /// <returns></returns>
        public TValue this[HierarchyPath<TKey> hierarchyPath]
        {
            set
            {
                this.GetOrCreateNode(hierarchyPath).SetValue(value);
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
            if (this.getDefaultValue != null)
                throw new NotSupportedException("default value");

            new SetOrAddNodeValueWriter<TKey, TValue, MutableNode<TKey, TValue>>(createNode: key => new MutableNode<TKey, TValue>(key))
                .AddValue(this.rootNode, path, value);
        }

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            MutableNode<TKey, TValue> descendantNode;
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

            var writer = new RemoveValueAndPruneHierarchyWriter<TKey, TValue, MutableNode<TKey, TValue>>();
            writer.ClearValue(this.rootNode, hierarchyPath);

            return writer.ValueWasCleared;
        }

        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            if (hierarchyPath.IsRoot)
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
                var writer = new RemoveNodeHierarchyWriter<TKey, MutableNode<TKey, TValue>>(recurse);
                var result = writer.Visit(this.rootNode, hierarchyPath);
                return writer.HasRemovedNode;
            }
        }

        #endregion IHierarchy Members

        private MutableNode<TKey, TValue> GetOrCreateNode(HierarchyPath<TKey> hierarchyPath)
        {
            GetOrCreateNodeWriter<TKey, MutableNode<TKey, TValue>> writer = null;
            if (this.getDefaultValue == null)
                writer = new GetOrCreateNodeWriter<TKey, MutableNode<TKey, TValue>>(createNode: key => new MutableNode<TKey, TValue>(key));
            else throw new NotSupportedException("default value");

            writer.Visit(this.rootNode, hierarchyPath);

            return writer.DescandantAt;
        }
    }
}