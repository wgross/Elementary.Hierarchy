using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class LiteDbHierarchyNode : IHasChildNodes<KeyValuePair<string, BsonValue>>
    {
        public ObjectId _Id { get; set; }

        public IDictionary<string, BsonValue> _ChildNodeIds { get; set; } = new Dictionary<string, BsonValue>();

        public string Key { get; set; }

        public bool HasChildNodes => _ChildNodeIds.Any();

        public IEnumerable<KeyValuePair<string, BsonValue>> ChildNodes => this._ChildNodeIds;

        public object AddChildNode(string key)
        {
            throw new NotImplementedException();
        }
    }
}