namespace Elementary.Hierarchy.Collections.Nodes
{
    public class KeyValueNode
    {
        public static readonly object NoValue = new object();
    }

    /// <summary>
    /// Stores a pari of key and value.
    /// Neither key nor value must be given. Both can be 'unset'
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValueNode<TKey, TValue> : KeyValueNode,
        IHierarchyKeyReader<TKey>,
        IHierarchyValueReader<TValue>,
        IHierarchyValueWriter<TValue>
    {
        private readonly object key = NoValue;
        private object value = NoValue;

        public KeyValueNode()
        { }

        public KeyValueNode(TKey key)
        {
            this.key = key;
        }

        public KeyValueNode(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        #region IHierarchyKeyReader members

        public bool TryGetKey(out TKey key)
        {
            key = default(TKey);

            if (this.key == NoValue)
                return false;

            key = (TKey)this.key;
            return true;
        }

        #endregion IHierarchyKeyReader members

        #region IHierarchyValueReader members

        public bool TryGetValue(out TValue value)
        {
            value = default(TValue);

            if (this.value == NoValue)
                return false;

            value = (TValue)this.value;
            return true;
        }

        #endregion IHierarchyValueReader members

        #region IHierachyValueWriter members

        public void SetValue(TValue value)
        {
            this.value = (object)value;
        }

        public bool RemoveValue()
        {
            if (this.value == NoValue)
                return false;

            this.value = NoValue;
            return true;
        }

        public bool HasValue => this.value != NoValue;

        public TValue Value => (TValue)this.value;

        #endregion IHierachyValueWriter members
    }
}