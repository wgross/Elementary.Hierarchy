using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    [DebuggerDisplay("key={key},value={value}")]
    public class ImmutableNode<TKey, TValue> :
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

        public ImmutableNode(TKey key, IEnumerable<ImmutableNode<TKey, TValue>> childNodes)
        {
            this.Key = key;
            this.childNodes = childNodes.ToArray();
        }

        public ImmutableNode(TKey key, TValue value, IEnumerable<ImmutableNode<TKey, TValue>> childNodes)
        {
            this.Key = key;
            this.value = (object)value;
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

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
            return new ImmutableNode<TKey, TValue>(this.Key, newChildNodes)
            {
                value = this.value
            };
        }

        public ImmutableNode<TKey, TValue> RemoveChild(ImmutableNode<TKey, TValue> child)
        {
            return new ImmutableNode<TKey, TValue>(this.Key, this.ChildNodes.Except(new[] { child }).ToArray())
            {
                value = this.value
            };
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
                return new ImmutableNode<TKey, TValue>(this.Key, newNodesChildren)
                {
                    value = this.value
                };
            }

            // a replacemen was not done.

            throw new InvalidOperationException($"The node (id={newChild.Key}) doesn't substutite any of the existing child nodes in (id={this.Key})");
        }

        #endregion IHierarchyNodeWriter members

        #region IHierachyValueWriter members

        public void SetValue(TValue value)
        {
            this.value = (object)value;
        }

        public bool TryGetValue(out TValue value)
        {
            value = default(TValue);

            if (this.value == UnsetValue)
                return false;

            value = (TValue)this.value;
            return true;
        }

        public bool RemoveValue()
        {
            if (this.value == UnsetValue)
                return false;

            this.value = UnsetValue;
            return true;
        }

        public bool HasValue => this.value != UnsetValue;

        public TValue Value => (TValue)this.value;

        #endregion IHierachyValueWriter members
    }
}