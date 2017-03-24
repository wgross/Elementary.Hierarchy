namespace Elementary.Hierarchy.Collections
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// An immutable hierarchy holds a set of value but never changes ists structure.
    /// Any strcuture change like adding or removing a node produces a new hierarchy.
    /// The data is copied wothe the nodes. If TValue is a reference type all hierechay reference the same data
    /// If TValue is a value type thevalues are cpied with their nodes.
    /// </summary>
    /// <typeparam name="TKey">type of the identifier of the stires data</typeparam>s
    /// <typeparam name="TNode"></typeparam>
    public class MutableHierarchy<TKey, TValue> : IHierarchy<TKey, TValue>
    {
        #region Internal Node class

        /// <summary>
        /// Internal node class: holds a value and child nodes.
        /// </summary>
        [DebuggerDisplay("id={id},hasValue={HasValue},value={value}")]
        public sealed class Node : IHasIdentifiableChildNodes<TKey, Node>, IHasChildNodes<Node>
        {
            /// <summary>
            /// Null value marker
            /// </summary>
            private static readonly object ValueNotSet = new object();

            #region Construction and initialization of this instance

            public Node(TKey key)
            {
                this.key = key;
                this.value = ValueNotSet;
                this.childNodes = new Node[0];
            }

            public Node(TKey key, object value)
            {
                this.key = key;
                this.value = value;
                this.childNodes = new Node[0];
            }

            public Node(TKey key, object value, IEnumerable<Node> childNodes)
            {
                this.key = key;
                this.value = value;
                this.childNodes = childNodes.ToArray();
            }

            public readonly TKey key;

            public object value = ValueNotSet;

            private Node[] childNodes;

            #endregion Construction and initialization of this instance

            #region IHasChildNodes Members

            public bool HasChildNodes => this.childNodes.Any();

            public IEnumerable<Node> ChildNodes => this.childNodes;

            #endregion IHasChildNodes Members

            #region IHasIdentifiableChildNodes Members

            public bool TryGetChildNode(TKey id, out Node childNode)
            {
                childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.key, id));
                return childNode != null;
            }

            #endregion IHasIdentifiableChildNodes Members

            public bool HasValue => this.value != ValueNotSet;

            public Node AddChildNode(Node newChildNode)
            {
                // copy the existing children to a new array, and append the new one.
                Node[] newChildNodes = new Node[this.childNodes.Length + 1];
                Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);
                newChildNodes[this.childNodes.Length] = newChildNode;
                // set nw chile array instead of current child node array
                this.childNodes = newChildNodes;
                return this;
            }

            public Node SetChildNode(Node newChildNode)
            {
                for (int i = 0; i < this.childNodes.Length; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(this.childNodes[i].key, newChildNode.key))
                    {
                        //substitute the existing child node with the new one.
                        this.childNodes[i] = newChildNode;
                        return this;
                    }
                }
                throw new InvalidOperationException($"The node (id={newChildNode.key}) doesn't substutite any of the existing child nodes in (id={this.key})");
            }

            public Node SetValue(TValue value)
            {
                this.value = value;
                return this;
            }

            /// <summary>
            /// Gets value from node. If value is set treu is returned and the out parameter ist set.
            /// </summary>
            /// <param name="value"></param>
            /// <returns>true if node has a value</returns>
            public bool TryGetValue(out TValue value)
            {
                value = default(TValue);
                if (!this.HasValue)
                    return false;

                value = (TValue)this.value;
                return true;
            }

            /// <summary>
            /// Unset ths value of this this node instance.
            /// </summary>
            /// <returns></returns>
            public Node UnsetValue(bool prune = false, bool forcePrune = false)
            {
                this.value = ValueNotSet;

                // pruning is swtched on.
                // if none of the descandant has a value anymore, the children are deleted

                if (prune)
                    if (!this.Descendants().Any(d => d.HasValue))
                        this.ClearChildNodes();

                return this;
            }

            /// <summary>
            /// Forgets all child nodes.
            /// </summary>
            private void ClearChildNodes()
            {
                this.childNodes = new Node[] { };
            }

            public void RemoveChildNode(Node childToRemove)
            {
                this.childNodes = this.ChildNodes.Except(new[] { childToRemove }).ToArray();
            }
        }

        #endregion Internal Node class

        #region Construction and initialization of this instance

        public MutableHierarchy()
            : this(new Node(default(TKey)), pruneOnUnsetValue: false, getDefaultValue: null)
        {
        }

        public MutableHierarchy(Func<HierarchyPath<TKey>, TValue> getDefaultValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: false, getDefaultValue: getDefaultValue)
        {
        }

        public MutableHierarchy(bool pruneOnUnsetValue)
            : this(new Node(default(TKey)), pruneOnUnsetValue: pruneOnUnsetValue, getDefaultValue: null)
        {
        }

        private MutableHierarchy(Node rootNode, bool pruneOnUnsetValue, Func<HierarchyPath<TKey>, TValue> getDefaultValue)
        {
            this.getDefaultValue = getDefaultValue;
            if (this.getDefaultValue != null)
            {
                this.rootNode = rootNode.SetValue(this.getDefaultValue(HierarchyPath.Create<TKey>()));
            }
            else
            {
                this.rootNode = rootNode;
            }

            this.rootNode = rootNode;
            this.pruneOnUnsetValue = pruneOnUnsetValue;
        }

        private readonly Node rootNode;

        private readonly bool pruneOnUnsetValue;

        private readonly Func<HierarchyPath<TKey>, TValue> getDefaultValue;

        #endregion Construction and initialization of this instance

        #region Hierarchy Node Traversal

        public sealed class Traverser : IHierarchyNode<TKey, TValue>
        {
            private readonly Traverser parentTraverser;

            private readonly Node node;

            private readonly Lazy<HierarchyPath<TKey>> path;

            public Traverser(Node node)
            {
                this.parentTraverser = null;
                this.node = node;
                this.path = new Lazy<HierarchyPath<TKey>>(() => HierarchyPath.Create<TKey>(), isThreadSafe: false);
            }

            public Traverser(Traverser parentTraverser, Node node)
            {
                if (parentTraverser == null)
                    throw new ArgumentNullException(nameof(parentTraverser));

                this.parentTraverser = parentTraverser;
                this.node = node;
                this.path = new Lazy<HierarchyPath<TKey>>(() => this.parentTraverser.Path.Join(this.node.key), isThreadSafe: false);
            }

            public IEnumerable<IHierarchyNode<TKey, TValue>> ChildNodes => this.node.Children().Select(c => new Traverser(this, c));

            public bool HasChildNodes => this.node.HasChildNodes;

            public bool HasParentNode => this.parentTraverser != null;

            public IHierarchyNode<TKey, TValue> ParentNode => this.parentTraverser;

            public HierarchyPath<TKey> Path => this.path.Value;

            public bool HasValue => this.node.HasValue;

            public TValue Value => (TValue)this.node.value;

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;

                var objAsTraverser = obj as Traverser;
                if (objAsTraverser == null)
                    return false;

                // Traversers are equals if the point to the same node
                return object.ReferenceEquals(this.node, objAsTraverser.node);
            }

            public override int GetHashCode()
            {
                return this.node.GetHashCode();
            }
        }

        /// <summary>
        /// Starts a traversal of the hierarchy at the root node.
        /// </summary>
        /// <returns>A traversable representation of the root node</returns>
        public IHierarchyNode<TKey, TValue> Traverse(HierarchyPath<TKey> startAt)
        {
            Traverser startNode = new Traverser(this.rootNode);

            // Descend along the soecifed path and buidl ap teh chain of ancestors of the start node.
            // if the start node can't be reached because it doesn't exist in the hierarchy a
            // KeyNotFound exception is thrown

            this.rootNode.DescendantAt(tryGetChildNode: delegate (Node parent, TKey key, out Node child)
            {
                child = null;
                if (!parent.TryGetChildNode(key, out child))
                    throw new KeyNotFoundException($"node '{startAt}'  doesn't exist");
                startNode = new Traverser(startNode, child);
                return true;
            }, path: startAt);

            // Travesal was successul.
            // just return wwhat is now in 'startNode'
            return startNode;
        }

        #endregion Hierarchy Node Traversal

        #region Add/Set a hierarchy nodes value

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
            var nodeToSetValueAt = this.GetOrCreateNode(path);

            if (nodeToSetValueAt.HasValue)
                throw new ArgumentException($"Node at '{path}' already has a value", nameof(path));

            nodeToSetValueAt.SetValue(value);
        }

        private Node GetOrCreateNode(HierarchyPath<TKey> hierarchyPath)
        {
            var currentPosition = HierarchyPath.Create<TKey>();

            return this.rootNode.DescendantAt(delegate (Node current, TKey key, out Node child)
            {
                currentPosition = currentPosition.Join(key);

                // if the chiiled ic not found, just create a new one on-the-fly
                if (!current.TryGetChildNode(key, out child))
                {
                    if (currentPosition.Items.Count() < hierarchyPath.Items.Count() && this.getDefaultValue != null)
                    {
                        current.AddChildNode(child = new Node(key, this.getDefaultValue(currentPosition)));
                    }
                    else
                    {
                        current.AddChildNode(child = new Node(key));
                    }
                }
                return true;
            }, hierarchyPath);
        }

        #endregion Add/Set a hierarchy nodes value

        /// <summary>
        /// Retrieves the nodes value from the immutable hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">path to the value</param>
        /// <param name="value">found value</param>
        /// <returns>zre, if value could be found, false otherwise</returns>
        public bool TryGetValue(HierarchyPath<TKey> hierarchyPath, out TValue value)
        {
            Node descendantNode;
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
            return this.Remove(hierarchyPath, maxDepth.GetValueOrDefault(1), forcePrune: false);
        }

        private bool Remove(HierarchyPath<TKey> hierarchyPath, int maxDepth, bool forcePrune)
        {
            Node startNode;
            if (!this.rootNode.TryGetDescendantAt(hierarchyPath, out startNode))
                return false;

            return Remove(startNode, maxDepth, this.pruneOnUnsetValue);
        }

        private bool Remove(Node startNode, int maxDepth, bool prune)
        {
            if (maxDepth == 1)
            {
                if (!startNode.HasValue)
                    return false;

                startNode.UnsetValue(prune: prune);
                return true;
            }
            else if (maxDepth > 1)
            {
                var descendantsAndSelf = startNode.DescendantsAndSelf(depthFirst: false, maxDepth: maxDepth);

                // inner nodes are pruned only of the leave vaules are removed first.
                if (prune)
                    descendantsAndSelf = descendantsAndSelf.Reverse();

                bool removed = false;
                foreach (var descandant in descendantsAndSelf)
                {
                    if (descandant.HasValue)
                    {
                        descandant.UnsetValue(prune);
                        removed = removed || true;
                    }
                }
                return removed;
            }
            else return false;
        }

        public bool RemoveNode(HierarchyPath<TKey> hierarchyPath, bool recurse)
        {
            Node nodeToRemove;
            if (!this.rootNode.TryGetDescendantAt(hierarchyPath, out nodeToRemove))
                return false; // node doesn't not exist

            if (!recurse && nodeToRemove.Children().Any())
                return false; // don't remove child nodes silently

            var removedNode = this.Remove(nodeToRemove, recurse ? int.MaxValue : 1, prune: recurse);
            if (removedNode && !hierarchyPath.IsRoot && !nodeToRemove.HasChildNodes)
            {
                // remove this node from parent node, if all child could be removed
                // root node is not removed, only cleared

                Node parentNode;
                this.rootNode.TryGetDescendantAt(hierarchyPath.Parent(), out parentNode);
                parentNode.RemoveChildNode(nodeToRemove);
            }
            return removedNode;
        }
    }
}