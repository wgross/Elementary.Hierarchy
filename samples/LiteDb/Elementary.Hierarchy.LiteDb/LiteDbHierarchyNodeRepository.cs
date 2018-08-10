using LiteDB;

namespace Elementary.Hierarchy.LiteDb
{
    public interface ILiteDbHierarchyNodeRepository
    {
        LiteDbHierarchyNode Root { get; }

        (bool, BsonValue) TryInsert(LiteDbHierarchyNode node);

        bool Update(LiteDbHierarchyNode liteDbHierarchyNode);
    }

    public class LiteDbHierarchyNodeRepository : ILiteDbHierarchyNodeRepository
    {
        static LiteDbHierarchyNodeRepository()
        {
            BsonMapper.Global.Entity<LiteDbHierarchyNode>()
                .Id(n => n._Id)
                .Field(n => n._ChildNodeIds, "_cn")
                .Ignore(n => n.HasChildNodes)
                .Ignore(n => n.ChildNodes);
        }

        private readonly LiteCollection<LiteDbHierarchyNode> collection;

        public LiteDbHierarchyNodeRepository(LiteDatabase database, string collectionName)
        {
            this.collection = database.GetCollection<LiteDbHierarchyNode>(collectionName);
            this.collection.EnsureIndex(n => n.Key, unique: true);

            if (this.Root == null)
                this.collection.Insert(new LiteDbHierarchyNode { Key = null });
        }

        public LiteDbHierarchyNode Root => this.collection.FindOne(n => n.Key == null);

        public (bool, BsonValue) TryInsert(LiteDbHierarchyNode node)
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

        public bool Update(LiteDbHierarchyNode liteDbHierarchyNode)
        {
            try
            {
                return this.collection.Update(liteDbHierarchyNode);
            }
            catch (LiteException ex) when (ex.ErrorCode == 110)
            {
                return false;
            }
        }
    }
}