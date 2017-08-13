using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Nodes
{
    public class KeyValueNode
    {
        public static KeyValueNode<K,V> Create<K,V>(K key, V value) 
        {
            return new KeyValueNode<K,V>(key, value);
        }    
    }

    public class KeyValueNode<K,V> : IHasChildNodes<KeyValueNode<K,V>>
    {
        private readonly KeyValueNode<K,V>[] childNodes;

        public KeyValueNode(K key, V value)
            : this(key, value, new KeyValueNode<K,V>[0])
        {}

        public KeyValueNode(K key, V value, params KeyValueNode<K,V>[] childNodes)
        {
            this.Key = key;
            this.Value = value;
            this.childNodes = childNodes.ToArray();
        }

        public K Key {get;}

        public V Value { get; }

        #region IHasChildNode members

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<KeyValueNode<K, V>> ChildNodes => this.childNodes;

        #endregion 
    }
}
