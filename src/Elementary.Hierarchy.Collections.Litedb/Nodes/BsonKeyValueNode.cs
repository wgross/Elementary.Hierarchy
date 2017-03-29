using Elementary.Hierarchy.Collections.Nodes;
using LiteDB;
using System;

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
    public class BsonKeyValueNode<TValue> :
        IHierarchyKeyReader<string>,
        IHierarchyValueReader<TValue>,
        IHierarchyValueWriter<TValue>
    {
        #region Construction and initialization of this instance

        public BsonDocument BsonDocument => this.bsonDocument;

        private readonly BsonDocument bsonDocument;
        private LiteCollection<BsonDocument> nodes;

        public BsonKeyValueNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument)
        {
            this.nodes = nodes;
            this.bsonDocument = bsonDocument;
            this.EnforceId();
        }

        public BsonKeyValueNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument, string key)
        {
            this.nodes = nodes;
            this.bsonDocument = bsonDocument;
            this.bsonDocument.Set("key", new BsonValue(key));
            this.EnforceId();
        }

        public BsonKeyValueNode(LiteCollection<BsonDocument> nodes, BsonDocument bsonDocument, string key, TValue value)
            : this(nodes, bsonDocument, key)
        {
            this.bsonDocument.Set("value", new BsonValue(value));
        }

        #endregion Construction and initialization of this instance

        #region A BSON node must have an Id

        private void EnforceId()
        {
            if (!this.TryGetId(out var id))
                this.BsonDocument.Set("_id", ObjectId.NewObjectId());
        }

        public bool TryGetId(out BsonValue id)
        {
            return this.BsonDocument.TryGetValue("_id", out id);
        }

        #endregion A BSON node must have an Id

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

        public bool TryGetKey(out string key)
        {
            key = null;

            if (this.bsonDocument.TryGetValue("key", out var bsonValue) && !bsonValue.IsNull)
            {
                key = bsonValue.AsString;
                return true;
            }

            key = null;
            return false;
        }

        public void SetValue(TValue value)
        {
            this.bsonDocument.Set("value", new BsonValue(value));
            this.nodes.Update(this.bsonDocument);
        }

        public bool RemoveValue()
        {
            return this.bsonDocument.Remove("value");
        }

        #region Equals and GetHashCode delegate behavior to _id of inner node

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
                return true;

            var objAsLiteDbMutableNode = obj as BsonKeyValueNode<TValue>;
            if (objAsLiteDbMutableNode == null)
                return false;

            if (this.TryGetId(out var thisId) && objAsLiteDbMutableNode.TryGetId(out var objId))
                return thisId.Equals(objId);

            return false;
        }

        public override int GetHashCode()
        {
            if (this.TryGetId(out var id))
                return id.GetHashCode();

            throw new InvalidOperationException("any node must have an _id");
        }

        #endregion Equals and GetHashCode delegate behavior to _id of inner node
    }
}