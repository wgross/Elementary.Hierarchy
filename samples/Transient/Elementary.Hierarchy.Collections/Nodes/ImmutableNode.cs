using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Elementary.Hierarchy.Collections.Nodes
{
    [DebuggerDisplay("key={key},value={value}")]
    public class ImmutableNode<TKey, TValue> : KeyValueNode<TKey, TValue>,
        IHierarchyNodeWriter<ImmutableNode<TKey, TValue>>,
        IHasIdentifiableChildNodes<TKey, ImmutableNode<TKey, TValue>>,
        IHasChildNodes<ImmutableNode<TKey, TValue>>
    {
        #region Construction and initialization of this instance

        public ImmutableNode()
        { }

        public ImmutableNode(TKey key)
            : base(key)
        { }

        public ImmutableNode(TKey key, TValue value)
            : base(key, value)
        { }

        public ImmutableNode(TKey key, TValue value, IEnumerable<ImmutableNode<TKey, TValue>> childNodes)
            : base(key, value)
        {
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

        #region Construction and initialization of clones of this instance

        private ImmutableNode(TKey key, ImmutableNode<TKey, TValue>[] childNodes)
            : base(key)
        {
            this.childNodes = childNodes.ToArray();
        }

        private ImmutableNode(ImmutableNode<TKey, TValue>[] childNodes)
        {
            this.childNodes = childNodes;
        }

        #endregion Construction and initialization of clones of this instance

        #region IHasChildNodes members

        private ImmutableNode<TKey, TValue>[] childNodes = new ImmutableNode<TKey, TValue>[0];

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<ImmutableNode<TKey, TValue>> ChildNodes => this.childNodes;

        #endregion IHasChildNodes members

        #region IHasIdentifiableChildren members

        public (bool,ImmutableNode<TKey,TValue>) TryGetChildNode(TKey id)
        {
            var childNode = this.childNodes.SingleOrDefault(n =>
            {
                if (n.TryGetKey(out var key))
                    return EqualityComparer<TKey>.Default.Equals(key, id);
                else // this is unexpected. ChildNodes always have keys.
                    throw new InvalidOperationException("child must have a key");
            });
            return (childNode != null, childNode);
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
            if (object.ReferenceEquals(childToReplace, newChild))
                return this; // nothing to do

            if (!childToReplace.TryGetKey(out var childToReplaceKey))
                throw new ArgumentException("child node must have a key", nameof(childToReplace));

            if (!newChild.TryGetKey(out var newChildKey))
                throw new ArgumentException("child node must have a key", nameof(newChild));

            if (!EqualityComparer<TKey>.Default.Equals(childToReplaceKey, newChildKey))
                throw new InvalidOperationException($"Key of child to replace (key='{childToReplaceKey}') and new child (key='{newChildKey}') must be equal");

            var newNodesChildren = new ImmutableNode<TKey, TValue>[this.childNodes.Length];
            bool hasReplaced = false;

            for (int i = 0; i < this.childNodes.Length; i++)
            {
                if (this.childNodes[i].Equals(childToReplace))
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

            throw new InvalidOperationException($"The node (id={newChildKey}) doesn't substutite any of the existing child nodes");
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