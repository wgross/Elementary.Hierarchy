using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Nodes
{
    [DebuggerDisplay("key={key},value={value}")]
    public class ImmutableNode<TKey, TValue> : KeyValueNode<TKey, TValue>,
        IHierarchyNodeWriter<ImmutableNode<TKey, TValue>>,
        IHierarchyValueWriter<TValue>,
        IHasIdentifiableChildNodes<TKey, ImmutableNode<TKey, TValue>>
    {
        #region Construction and initialization of this instance

        public ImmutableNode(TKey key)
        {
            this.Key = key;
        }

        public ImmutableNode(TKey key, TValue value)
        {
            this.Key = key;
            this.value = (object)value;
        }

        public ImmutableNode(TKey key, TValue value, IEnumerable<ImmutableNode<TKey, TValue>> childNodes)
        {
            this.Key = key;
            this.value = (object)value;
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

        #region Construction and initialization of clones of this instance

        private ImmutableNode(TKey key, ImmutableNode<TKey, TValue>[] childNodes)
        {
            this.Key = key;
            this.childNodes = childNodes.ToArray();
        }

        private ImmutableNode(ImmutableNode<TKey, TValue>[] childNodes)
        {
            this.childNodes = childNodes;
        }

        #endregion Construction and initialization of clones of this instance

        #region IHasChildNodes members

        private ImmutableNode<TKey, TValue>[] childNodes = new ImmutableNode<TKey, TValue>[0];
        private static readonly object UnsetValue = new object();
        private object value = UnsetValue;

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<ImmutableNode<TKey, TValue>> ChildNodes => this.childNodes;

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildren members

        public TKey Key { get; private set; }

        public bool TryGetChildNode(TKey id, out ImmutableNode<TKey, TValue> childNode)
        {
            childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.Key, id));
            return childNode != null;
        }

        #endregion IHasIdentifiableChildren members

        #region IHierarchyNodeWriter members

        public ImmutableNode<TKey, TValue> AddChild(ImmutableNode<TKey, TValue> newChild)
        {
            // copy the existing children to a new array, and append the new one.
            ImmutableNode<TKey, TValue>[] newChildNodes = new ImmutableNode<TKey, TValue>[this.childNodes.Length + 1];
            Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);
            newChildNodes[this.childNodes.Length] = newChild;

            // create new node with new child node.
            return this.CreateClone(newChildNodes);
        }

        public ImmutableNode<TKey, TValue> RemoveChild(ImmutableNode<TKey, TValue> child)
        {
            return this.CreateClone(this.ChildNodes.Except(new[] { child }).ToArray());
        }

        public ImmutableNode<TKey, TValue> ReplaceChild(ImmutableNode<TKey, TValue> childToReplace, ImmutableNode<TKey, TValue> newChild)
        {
            var newNodesChildren = new ImmutableNode<TKey, TValue>[this.childNodes.Length];
            bool hasReplaced = false;

            for (int i = 0; i < this.childNodes.Length; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(this.childNodes[i].Key, newChild.Key))
                {
                    newNodesChildren[i] = newChild;
                    hasReplaced = true;
                }
                else
                {
                    newNodesChildren[i] = this.childNodes[i];
                }
            }

            if (hasReplaced)
            {
                return this.CreateClone(newNodesChildren);
            }

            // a replacemen was not done.

            throw new InvalidOperationException($"The node (id={newChild.Key}) doesn't substutite any of the existing child nodes in (id={this.Key})");
        }

        private ImmutableNode<TKey, TValue> CreateClone(ImmutableNode<TKey, TValue>[] childNodes)
        {
            // create new node with new child node.
            var newNode = this.TryGetKey(out var key)
                ? new ImmutableNode<TKey, TValue>(key, childNodes)
                : new ImmutableNode<TKey, TValue>(childNodes);

            if (this.TryGetValue(out var value))
                newNode.SetValue(value);

            return newNode;
        }

        #endregion IHierarchyNodeWriter members
    }
}