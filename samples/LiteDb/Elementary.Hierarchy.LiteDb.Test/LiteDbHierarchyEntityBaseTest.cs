using LiteDB;
using System.Collections.Generic;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyEntityBaseTest
    {
        public static IEnumerable<object[]> GetEntityBaseInstancesForComparision()
        {
            var refId = ObjectId.NewObjectId();
            var differentId = ObjectId.NewObjectId();

            yield return new object[] { new LiteDbHierarchyNodeEntity { Id = refId }, new LiteDbHierarchyNodeEntity { Id = refId }, new LiteDbHierarchyNodeEntity { Id = differentId }, new LiteDbHierarchyValueEntity { Id = refId } };
            yield return new object[] { new LiteDbHierarchyValueEntity { Id = refId }, new LiteDbHierarchyValueEntity { Id = refId }, new LiteDbHierarchyValueEntity { Id = differentId }, new LiteDbHierarchyNodeEntity { Id = refId } };
        }

        [Theory]
        [MemberData(nameof(GetEntityBaseInstancesForComparision))]
        public void LiteDbHierarchyEntityBase_are_equal_if_Id_are_equal_and_Type(LiteDbHierarchyEntityBase refEntity, LiteDbHierarchyEntityBase sameId, LiteDbHierarchyEntityBase differentId, LiteDbHierarchyEntityBase differentType)
        {
            // ACT & ASSERT

            Assert.Equal(refEntity, refEntity);
            Assert.Equal(refEntity.GetHashCode(), refEntity.GetHashCode());
            Assert.Equal(refEntity, sameId);
            Assert.Equal(refEntity.GetHashCode(), sameId.GetHashCode());
            Assert.NotEqual(refEntity, differentId);
            Assert.NotEqual(refEntity.GetHashCode(), differentId.GetHashCode());
            Assert.NotEqual(refEntity, differentType);
            Assert.NotEqual(refEntity.GetHashCode(), differentType.GetHashCode());
        }
    }
}