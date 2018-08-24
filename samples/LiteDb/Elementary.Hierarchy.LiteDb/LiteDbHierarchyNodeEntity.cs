using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNodeEntity : IHasChildNodes<KeyValuePair<string, BsonValue>>
    {
        public ObjectId _Id { get; set; }

        public IDictionary<string, BsonValue> ChildNodeIds { get; set; } = new Dictionary<string, BsonValue>();

        public string Key { get; set; }

        #region IHasChildNodes members

        public bool HasChildNodes => this.ChildNodeIds.Any();

        public IEnumerable<KeyValuePair<string, BsonValue>> ChildNodes => this.ChildNodeIds;

        public BsonValue ValueRef { get; set; }

        #endregion IHasChildNodes members
    }
}