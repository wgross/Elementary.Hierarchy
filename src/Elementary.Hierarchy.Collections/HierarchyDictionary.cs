using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Collections
{
    /// <summary>
    /// Provides a trabsient hierarchical key value store.
    /// </summary>
    /// <typeparam name="K">type of the keys</typeparam>
    /// <typeparam name="V">type of the values</typeparam>
    public class HierarchyDictionary<K, V>
    {
        #region Implements IDictionary interface based on the hierarchical value storage

        private class ValuesAsDictionaryAdapter : IDictionary<HierarchyPath<K>, V>
        {
            #region Initialization of this instance

            public ValuesAsDictionaryAdapter(IHierarchy<K, V> hierarchy)
            {
                this.hierarchy = hierarchy;
            }

            private readonly IHierarchy<K, V> hierarchy;

            #endregion Initialization of this instance

            public V this[HierarchyPath<K> key]
            {
                get
                {
                    V value;
                    if (this.hierarchy.TryGetValue(key, out value))
                        return value;
                    throw new KeyNotFoundException("The given key was not present in the dictionary.");
                }

                set
                {
                    this.hierarchy[key] = value;
                }
            }

            public int Count => this.TraverseValueNodesFromRoot().Count();

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<HierarchyPath<K>> Keys => this.TraverseValueNodesFromRoot().Select(n => n.Path).ToList();

            public ICollection<V> Values => this.TraverseValueNodesFromRoot().Select(n => n.Value).ToList();

            public void Add(KeyValuePair<HierarchyPath<K>, V> item)
            {
                this.hierarchy.Add(item.Key, item.Value);
            }

            public void Add(HierarchyPath<K> key, V value)
            {
                this.hierarchy.Add(key, value);
            }

            public void Clear()
            {
                this.hierarchy.Remove(HierarchyPath.Create<K>(), int.MaxValue);
            }

            public bool Contains(KeyValuePair<HierarchyPath<K>, V> item)
            {
                V value;
                if (this.hierarchy.TryGetValue(item.Key, out value))
                    if (EqualityComparer<V>.Default.Equals(item.Value, value))
                        return true;
                return false;
            }

            public bool ContainsKey(HierarchyPath<K> key)
            {
                V value;
                return this.hierarchy.TryGetValue(key, out value);
            }

            public void CopyTo(KeyValuePair<HierarchyPath<K>, V>[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                this.TraverseValueNodesFromRoot()
                    .Select(n => new KeyValuePair<HierarchyPath<K>, V>(n.Path, n.Value))
                    .ToList()
                    .CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<HierarchyPath<K>, V>> GetEnumerator()
            {
                return TraverseValueNodesFromRoot()
                  .Select(n => new KeyValuePair<HierarchyPath<K>, V>(n.Path, n.Value))
                  .GetEnumerator();
            }

            private IEnumerable<IHierarchyNode<K, V>> TraverseValueNodesFromRoot()
            {
                return this.hierarchy
                    .Traverse(HierarchyPath.Create<K>())
                    .DescendantsAndSelf()
                    .Where(n => n.HasValue);
            }

            public bool Remove(KeyValuePair<HierarchyPath<K>, V> item)
            {
                V existingValue;
                if (this.hierarchy.TryGetValue(item.Key, out existingValue))
                    if (EqualityComparer<V>.Default.Equals(item.Value, existingValue))
                        return this.hierarchy.Remove(item.Key, 1);
                return false;
            }

            public bool Remove(HierarchyPath<K> key)
            {
                return this.hierarchy.Remove(key, 1);
            }

            public bool TryGetValue(HierarchyPath<K> key, out V value)
            {
                return this.hierarchy.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion Implements IDictionary interface based on the hierarchical value storage

        #region Implements IDictionary interface based on the hierarchical value store

        private class NodesAsDictionaryAdapter : IDictionary<HierarchyPath<K>, IHierarchyNode<K, V>>
        {
            private IHierarchy<K, V> hierarchy;

            public NodesAsDictionaryAdapter(IHierarchy<K, V> hierarchy)
            {
                this.hierarchy = hierarchy;
            }

            public IHierarchyNode<K, V> this[HierarchyPath<K> key]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<HierarchyPath<K>> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<IHierarchyNode<K, V>> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(KeyValuePair<HierarchyPath<K>, IHierarchyNode<K, V>> item)
            {
                throw new NotImplementedException();
            }

            public void Add(HierarchyPath<K> key, IHierarchyNode<K, V> value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<HierarchyPath<K>, IHierarchyNode<K, V>> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(HierarchyPath<K> key)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<HierarchyPath<K>, IHierarchyNode<K, V>>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<HierarchyPath<K>, IHierarchyNode<K, V>>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<HierarchyPath<K>, IHierarchyNode<K, V>> item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(HierarchyPath<K> key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(HierarchyPath<K> key, out IHierarchyNode<K, V> value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        #endregion Implements IDictionary interface based on the hierarchical value store

        #region Construction and initialization of this instance

        public HierarchyDictionary(IHierarchy<K, V> hierarchyImplementation)
        {
            this.hierarchy = hierarchyImplementation;
            this.values = new ValuesAsDictionaryAdapter(this.hierarchy);
            this.nodes = new NodesAsDictionaryAdapter(this.hierarchy);
        }

        private readonly IDictionary<HierarchyPath<K>, V> values;
        private readonly IHierarchy<K, V> hierarchy;
        private readonly IDictionary<HierarchyPath<K>, IHierarchyNode<K, V>> nodes;

        #endregion Construction and initialization of this instance

        /// <summary>
        /// Returns the root node of the tree.
        /// </summary>
        public IHierarchyNode<K, V> Root => this.hierarchy.Traverse(HierarchyPath.Create<K>());

        /// <summary>
        /// Accesses the hierachical key value store as a ditionary which contians ony the nodes havin values.
        /// </summary>
        public IDictionary<HierarchyPath<K>, V> Values => this.values;
    }
}