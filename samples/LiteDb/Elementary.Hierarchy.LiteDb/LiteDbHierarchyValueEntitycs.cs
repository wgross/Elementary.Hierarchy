using LiteDB;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyValueEntity : LiteDbHierarchyEntityBase
    {
        public BsonValue Value { get; private set; }

        public bool SetValue<T>(T value)
        {
            var bsonValue = new BsonValue(value);
            if (this.Value != null)
                if (this.Value.CompareTo(bsonValue) == 0)
                    return false; // equal by value: no update

            this.Value = bsonValue;
            return true;
        }
    }
}