using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Nodes
{
    /// <summary>
    /// A mutable node allows to change the hierarchy strcture (child nodes) in place.
    /// It inherits teh capability to store a key and a value from the <see cref="KeyValueNode{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("key={key},value={value}")]
    public class MutableNode<TKey, TValue> : KeyValueNode<TKey, TValue>,
        IHierarchyNodeWriter<MutableNode<TKey, TValue>>,
        IHasIdentifiableChildNodes<TKey, MutableNode<TKey, TValue>>,
        IHasChildNodes<MutableNode<TKey, TValue>>
    {
        #region Some factory methods for disambiguization

        public static MutableNode<TKey, TValue> CreateRoot()
        {
            return new MutableNode<TKey, TValue>();
        }

        #endregion Some factory methods for disambiguization

        #region Construction and initialization of this instance

        /// <summary>
        /// Ctor to create a hierachy root node withot a value
        /// </summary>
        private MutableNode()
        {
        }

        public MutableNode(TKey key)
            : base(key)
        { }

        public MutableNode(TKey key, TValue value)
            : base(key, value)
        { }

        public MutableNode(TKey key, IEnumerable<MutableNode<TKey, TValue>> childNodes)
            : base(key)
        {
            this.childNodes = childNodes.ToArray();
        }

        public MutableNode(TKey key, TValue value, IEnumerable<MutableNode<TKey, TValue>> childNodes)
            : base(key, value)
        {
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

        #region IHasChildNodes members

        private MutableNode<TKey, TValue>[] childNodes = new MutableNode<TKey, TValue>[0];

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<MutableNode<TKey, TValue>> ChildNodes => this.childNodes;

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildren members

        public bool TryGetChildNode(TKey id, out MutableNode<TKey, TValue> childNode)
        {
            childNode = this.childNodes.SingleOrDefault(n =>
            {
                if (n.TryGetKey(out var key))
                    return EqualityComparer<TKey>.Default.Equals(key, id);
                else // this is unexpected. ChildNodes always have keys.
                    throw new InvalidOperationException("child must have a key");
            });
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
            if (object.ReferenceEquals(childToReplace, newChild))
                return this; // nothing to do

            if (!childToReplace.TryGetKey(out var childToReplaceKey))
                throw new ArgumentException("child node must have a key", nameof(childToReplace));

            if (!newChild.TryGetKey(out var newChildKey))
                throw new ArgumentException("child node must have a key", nameof(newChild));

            if (!EqualityComparer<TKey>.Default.Equals(childToReplaceKey, newChildKey))
                throw new InvalidOperationException($"Key of child to replace (key='{childToReplaceKey}') and new child (key='{newChildKey}') must be equal");

            for (int i = 0; i < this.childNodes.Length; i++)
            {
                if (this.childNodes[i].Equals(childToReplace))
                {
                    this.childNodes[i] = newChild;
                    return this;
                }
            }

            throw new InvalidOperationException($"The node (id={newChildKey}) doesn't substutite any of the existing child nodes");
        }

        #endregion IHierarchyNodeWriter members
    }
}