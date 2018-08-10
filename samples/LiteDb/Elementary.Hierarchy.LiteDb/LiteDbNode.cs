using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.LiteDb
{
    public class BsonDocumentNodeAdpater : IHasChildNodes<BsonDocumentNodeAdpater>
    {
        private readonly ICollection<BsonDocumentNodeAdpater> childNodes = new List<BsonDocumentNodeAdpater>();
        private readonly string id;
        private readonly BsonDocument bsonDocument;

        public BsonDocumentNodeAdpater(string id, BsonDocument bsonDocument)
        {
            this.id = id;
            this.bsonDocument = bsonDocument;
            this.bsonDocument.Add("cn", new BsonDocument());
        }

        public string Id => this.id;

        public BsonDocument BsonDocument => this.bsonDocument;

        private BsonDocument BsonDocumentChildNodes => this.BsonDocument["cn"].AsDocument;

        public bool HasChildNodes => this.childNodes.Any();

        public IEnumerable<BsonDocumentNodeAdpater> ChildNodes => this.childNodes;

        public void AddChildNode(BsonDocumentNodeAdpater child)
        {
            if (this.BsonDocumentChildNodes.ContainsKey(child.Id))
                throw new InvalidOperationException($"duplicate key: '{child.Id}'");
            this.BsonDocumentChildNodes.Add(child.Id, BsonValue.Null);
            this.childNodes.Add(child);
        }

        public bool RemoveChildNode(BsonDocumentNodeAdpater child)
        {
            if (this.BsonDocumentChildNodes.Remove(child.Id))
                if (this.childNodes.Remove(child))
                    return true;
            return false;
        }
    }
}