using Elementary.Hierarchy.Collections.Nodes;
using LiteDB;

namespace Elementary.Hierarchy.Collections.LiteDb.Nodes
{
    /// <summary>
    /// Stores a key and a value in a underlying BsonDocument
    /// Neither key nor value must be given. Both can remain 'unset'.
    /// The key can't be changed afterwards.
    /// This is the base class for all hierarchy node types.
    /// </summary>
    /// <typeparam name="TKey">type of the key</typeparam>
    /// <typeparam name="TValue">type of the value</typeparam>
    public class BsonKeyValueNode<TKey, TValue> :
        IHierarchyKeyReader<TKey>,
        IHierarchyValueReader<TValue>,
        IHierarchyValueWriter<TValue>
    {
        #region Construction and initialization of this instance

        public BsonDocument BsonDocument => this.bsonDocument;

        private readonly BsonDocument bsonDocument;

        public BsonKeyValueNode(BsonDocument bsonDocument)
        {
            this.bsonDocument = bsonDocument;
        }

        public BsonKeyValueNode(BsonDocument bsonDocument, TKey key)
        {
            this.bsonDocument = bsonDocument;
            this.bsonDocument.Set("key", new BsonValue(key));
        }

        public BsonKeyValueNode(BsonDocument bsonDocument, TKey key, TValue value)
            : this(bsonDocument, key)
        {
            this.bsonDocument.Set("value", new BsonValue(value));
        }

        #endregion Construction and initialization of this instance

        public bool TryGetValue(out TValue value)
        {
            // a value must not be there

            if (this.bsonDocument.TryGetValue("value", out var bsonValue))
            {
                value = (TValue)bsonValue.RawValue;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public bool TryGetKey(out TKey key)
        {
            key = default(TKey);

            if (this.bsonDocument.TryGetValue("key", out var bsonValue) && !bsonValue.IsNull)
            {
                key = (TKey)bsonValue.RawValue;
                return true;
            }

            key = default(TKey);
            return false;
        }

        public void SetValue(TValue value)
        {
            this.bsonDocument.Set("value", new BsonValue(value));
        }

        public bool RemoveValue()
        {
            return this.bsonDocument.Remove("value");
        }
    }
}