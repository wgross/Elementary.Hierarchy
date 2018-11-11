using System.Collections.Generic;

namespace LiteDB
{
    public static class BsonDocumentExtensions
    {
        public static void CopyToArray(this BsonDocument thisBsonDocument, KeyValuePair<string, BsonValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, BsonValue>>)thisBsonDocument.RawValue).CopyTo(array, arrayIndex);
        }
    }
}