using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Operations
{
    [DebuggerDisplay("key={key},value={value}")]
    public class MutableNode<TKey, TValue> :
        IHierarchyNodeWriter<MutableNode<TKey, TValue>>,
        IHierarchyValueReader<TValue>,
        IHierarchyValueWriter<TValue>,
        IHasIdentifiableChildNodes<TKey, MutableNode<TKey, TValue>>
    {
        #region Construction and initialization of this instance

        public MutableNode(TKey key)
        {
            this.Key = key;
        }

        public MutableNode(TKey key, TValue value)
        {
            this.Key = key;
            this.value = (object)value;
        }

        public MutableNode(TKey key, IEnumerable<MutableNode<TKey, TValue>> childNodes)
        {
            this.Key = key;
            this.childNodes = childNodes.ToArray();
        }

        public MutableNode(TKey key, TValue value, IEnumerable<MutableNode<TKey, TValue>> childNodes)
        {
            this.Key = key;
            this.value = (object)value;
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

        #region IHasChildNodes members

        private MutableNode<TKey, TValue>[] childNodes = new MutableNode<TKey, TValue>[0];
        private static readonly object UnsetValue = new object();
        private object value = UnsetValue;

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<MutableNode<TKey, TValue>> ChildNodes => this.childNodes;

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildren members

        public TKey Key { get; private set; }

        public bool TryGetChildNode(TKey id, out MutableNode<TKey, TValue> childNode)
        {
            childNode = this.childNodes.SingleOrDefault(n => EqualityComparer<TKey>.Default.Equals(n.Key, id));
            return childNode != null;
        }

        #endregion IHasIdentifiableChildren members

        #region IHierarchyNodeWriter members

        public MutableNode<TKey, TValue> AddChild(MutableNode<TKey, TValue> newChild)

        {
            // copy the existing children to a new array, and append the new one.
            MutableNode<TKey, TValue>[] newChildNodes = new MutableNode<TKey, TValue>[this.childNodes.Length + 1];
            Array.Copy(this.childNodes, newChildNodes, this.childNodes.Length);
            newChildNodes[this.childNodes.Length] = newChild;

            // set new child array instead of current child node array
            this.childNodes = newChildNodes;

            return this;
        }

        public MutableNode<TKey, TValue> RemoveChild(MutableNode<TKey, TValue> child)
        {
            this.childNodes = this.ChildNodes.Except(new[] { child }).ToArray();
            return this;
        }

        public MutableNode<TKey, TValue> ReplaceChild(MutableNode<TKey, TValue> childToReplace, MutableNode<TKey, TValue> newChild)
        {
            for (int i = 0; i < this.childNodes.Length; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(this.childNodes[i].Key, newChild.Key))
                {
                    //substitute the existing child node with the new one.
                    this.childNodes[i] = newChild;
                    return this;
                }
            }
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