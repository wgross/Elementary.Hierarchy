using LiteDB;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyEntityBase
    {
        public ObjectId _Id { get; set; }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var objAsLiteDbHierarchyEntityBase = obj as LiteDbHierarchyEntityBase;
            if (objAsLiteDbHierarchyEntityBase is null)
                return false;

            return (this.GetType(), this._Id).Equals((obj.GetType(), objAsLiteDbHierarchyEntityBase._Id));
        }

        public override int GetHashCode()
        {
            return (this.GetType(), this._Id).GetHashCode();
        }
    }
}