using LiteDB;

namespace Elementary.Hierarchy.LiteDb
{
    public interface ILiteDbHierarchyNodeRepository
    {
        LiteDbHierarchyNodeEntity Root { get; }

        (bool, BsonValue) TryInsert(LiteDbHierarchyNodeEntity node);

        bool Update(LiteDbHierarchyNodeEntity liteDbHierarchyNode);

        bool DeleteNode(BsonValue nodeId, bool recurse);

        LiteDbHierarchyNodeEntity Read(BsonValue nodeId);

        void Upsert(LiteDbHierarchyValueEntity liteDbHierarchyValueEntity);

        LiteDbHierarchyValueEntity ReadValue(BsonValue valueId);

        bool DeleteValue(BsonValue id);
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

            BsonMapper.Global.Entity<LiteDbHierarchyValueEntity>()
                .Id(v => v._Id);
        }

        private readonly LiteCollection<LiteDbHierarchyNodeEntity> nodeCollection;
        private readonly LiteCollection<LiteDbHierarchyValueEntity> valueCollection;

        public LiteDbHierarchyNodeRepository(LiteDatabase database, string nodeCollectionName, string valueCollectionName)
        {
            this.nodeCollection = database.GetCollection<LiteDbHierarchyNodeEntity>(nodeCollectionName);
            this.nodeCollection.EnsureIndex(n => n._Id, unique: true);
            this.valueCollection = database.GetCollection<LiteDbHierarchyValueEntity>(valueCollectionName);

            if (this.Root == null)
                this.nodeCollection.Insert(new LiteDbHierarchyNodeEntity { Key = null });
        }

        public LiteDbHierarchyNodeEntity Root => this.nodeCollection.FindOne(n => n.Key == null);

        public (bool, BsonValue) TryInsert(LiteDbHierarchyNodeEntity node)
        {
            try
            {
                return (true, this.nodeCollection.Insert(node));
            }
            catch (LiteException ex) when (ex.ErrorCode == 110)
            {
                return (false, BsonValue.Null);
            }
        }

        public bool Update(LiteDbHierarchyNodeEntity liteDbHierarchyNode) => this.nodeCollection.Update(liteDbHierarchyNode);

        public bool DeleteNode(BsonValue nodeId, bool recurse) => this.nodeCollection.Delete(nodeId);

        public LiteDbHierarchyNodeEntity Read(BsonValue nodeId) => this.nodeCollection.FindById(nodeId);

        public void Upsert(LiteDbHierarchyValueEntity liteDbHierarchyValueEntity) => this.valueCollection.Upsert(liteDbHierarchyValueEntity);

        public LiteDbHierarchyValueEntity ReadValue(BsonValue valueId) => this.valueCollection.FindById(valueId);

        public bool DeleteValue(BsonValue id) => this.valueCollection.Delete(id);
    }
}