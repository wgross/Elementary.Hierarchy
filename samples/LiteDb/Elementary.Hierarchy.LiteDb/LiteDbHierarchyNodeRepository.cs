using LiteDB;

namespace Elementary.Hierarchy.LiteDb
{
    public interface ILiteDbHierarchyNodeRepository
    {
        LiteDbHierarchyNodeEntity Root { get; }

        (bool, BsonValue) TryInsert(LiteDbHierarchyNodeEntity node);

        bool Update(LiteDbHierarchyNodeEntity liteDbHierarchyNode);

        bool Remove(BsonValue nodeId);

        LiteDbHierarchyNodeEntity Read(BsonValue nodeId);

    }

    public class LiteDbHierarchyNodeRepository : ILiteDbHierarchyNodeRepository
    {
        static LiteDbHierarchyNodeRepository()
        {
            BsonMapper.Global.Entity<LiteDbHierarchyNodeEntity>()
                .Id(n => n._Id)
                .Field(n => n.ChildNodeIds, "_cn")
                .Ignore(n => n.HasChildNodes)
                .Ignore(n => n.ChildNodes);
        }

        private readonly LiteCollection<LiteDbHierarchyNodeEntity> collection;

        public LiteDbHierarchyNodeRepository(LiteDatabase database, string collectionName)
        {
            this.collection = database.GetCollection<LiteDbHierarchyNodeEntity>(collectionName);
            this.collection.EnsureIndex(n => n._Id, unique: true);

            if (this.Root == null)
                this.collection.Insert(new LiteDbHierarchyNodeEntity { Key = null });
        }

        public LiteDbHierarchyNodeEntity Root => this.collection.FindOne(n => n.Key == null);

        public (bool, BsonValue) TryInsert(LiteDbHierarchyNodeEntity node)
        {
            try
            {
                return (true, this.collection.Insert(node));
            }
            catch (LiteException ex) when (ex.ErrorCode == 110)
            {
                return (false, BsonValue.Null);
            }
        }

        public bool Update(LiteDbHierarchyNodeEntity liteDbHierarchyNode) => this.collection.Update(liteDbHierarchyNode);

        public bool Remove(BsonValue nodeId) => this.collection.Delete(nodeId);

        public LiteDbHierarchyNodeEntity Read(BsonValue nodeId) => this.collection.FindById(nodeId);
    }
}