using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Nodes
{
    public class KeyValueNode
    {
        public static KeyValueNode<K, V> RootNode<K, V>(V value, params KeyValueNode<K, V>[] childNodes)
        {
            return new KeyValueNode<K, V>(value, childNodes);
        }

        public static KeyValueNode<K, V> InnerNode<K, V>(K key, V value, params KeyValueNode<K, V>[] childNodes)
        {
            return new KeyValueNode<K, V>(key, value, childNodes);
        }
    }

    public class KeyValueNode<K, V> : IHasChildNodes<KeyValueNode<K, V>>, IHasIdentifiableChildNodes<K, KeyValueNode<K, V>>
    {
        #region Construction and initialization of this instance

        private struct OptionalKey
        {
            public K Key;
        }

        private readonly KeyValueNode<K, V>[] childNodes;
        private readonly OptionalKey? key;

        public KeyValueNode(V value)
            : this(null, value, new KeyValueNode<K, V>[0])
        { }

        public KeyValueNode(V value, params KeyValueNode<K, V>[] childNodes)
            : this(null, value, childNodes)
        { }

        public KeyValueNode(K key, V value)
            : this(new OptionalKey { Key = key }, value, new KeyValueNode<K, V>[0])
        { }

        public KeyValueNode(K key, V value, params KeyValueNode<K, V>[] childNodes)
            : this(new OptionalKey { Key = key }, value, childNodes)
        { }

        private KeyValueNode(OptionalKey? key, V value, KeyValueNode<K, V>[] childNodes)
        {
            this.key = key;
            this.Value = value;
            this.childNodes = childNodes.ToArray();
        }

        #endregion Construction and initialization of this instance

        public K Key => (this.key ?? throw new InvalidOperationException("node has no key")).Key;

        public bool HasKey => this.key.HasValue;

        public V Value { get; }

        #region IHasChildNodes members

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<KeyValueNode<K, V>> ChildNodes => this.childNodes;

        #endregion IHasChildNodes members

        #region IHasIdentfiableChildNodes members

        public (bool, KeyValueNode<K, V>) TryGetChildNode(K id)
        {
            var child = this.childNodes.SingleOrDefault(c => c.Key.Equals(id));

            if (child == null)
                return (false, null);

            return (true, child);
        }

        #endregion IHasIdentfiableChildNodes members
    }
}