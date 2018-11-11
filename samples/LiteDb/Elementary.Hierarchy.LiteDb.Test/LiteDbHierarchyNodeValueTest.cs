using LiteDB;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyNodeValueTest : IDisposable
    {
        private readonly MockRepository mocks = new MockRepository(MockBehavior.Strict);
        private readonly ObjectId rootId;
        private readonly Mock<ILiteDbHierarchyNodeRepository> repository;
        private LiteDbHierarchyNode root;
        private LiteCollection<LiteDbHierarchyNodeEntity> nodes;

        public LiteDbHierarchyNodeValueTest()
        {
            this.rootId = ObjectId.NewObjectId();
            this.repository = this.mocks.Create<ILiteDbHierarchyNodeRepository>();
            this.repository.Setup(r => r.Root).Returns(new LiteDbHierarchyNodeEntity() { Id = this.rootId });
            this.root = new LiteDbHierarchyNode(this.repository.Object, this.repository.Object.Root);
        }

        public void Dispose()
        {
            this.mocks.VerifyAll();
        }

        [Fact]
        public void LiteDbHierarchyNode_TryGetValue_returns_false_if_no_value_set()
        {
            // ACT

            var (result, _) = this.root.TryGetValue();

            // ASSERT

            Assert.False(result);
        }

        [Fact]
        public void LiteDbHierarchyNode_TryGetValue_loads_value_from_value_ref()
        {
            // ARRANGE

            var valueId = ObjectId.NewObjectId();
            var valueEntity = new LiteDbHierarchyValueEntity { Id = valueId };
            valueEntity.SetValue(1);

            // fake root value
            this.root.InnerNode.ValueRef = valueId;

            // value is read from repo
            this.repository
                .Setup(r => r.ReadValue(valueId))
                .Returns(valueEntity);

            // ACT

            var (result, value) = this.root.TryGetValue();

            // ASSERT

            Assert.True(result);
            Assert.Equal(1, value);
        }

        [Fact]
        public void LiteDbHierarchyNode_saves_node_and_value_on_new_value()
        {
            // ARRANGE

            var valueId = ObjectId.NewObjectId();

            // value node must be written
            this.repository
                .Setup(r => r.Upsert(It.Is<LiteDbHierarchyValueEntity>(v => v.Value.Equals(1))))
                .Callback<LiteDbHierarchyValueEntity>(v => v.Id = valueId);

            // node must be updated wit reference to value entity
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);

            // ACT

            this.root.SetValue(1);

            // ASSERT

            Assert.Equal<ObjectId>(this.root.InnerValue.Id, this.root.InnerNode.ValueRef);
            Assert.Equal(1, this.root.InnerValue.Value.AsInt32);
        }

        [Fact]
        public void LiteDbHierarchyNode_saves_value_on_updated_value()
        {
            // ARRANGE

            // node alredy has a value
            this.root = new LiteDbHierarchyNode(this.repository.Object, this.repository.Object.Root, new LiteDbHierarchyValueEntity());
            this.root.InnerValue.SetValue(1);
            this.root.InnerValue.Id = ObjectId.NewObjectId();
            this.root.InnerNode.ValueRef = this.root.InnerValue.Id;

            // value node must be written
            this.repository
                .Setup(r => r.Upsert(It.Is<LiteDbHierarchyValueEntity>(v => v.Value.Equals(2))));

            // ACT

            this.root.SetValue(2);
        }

        [Fact]
        public async Task LiteDbHierarchyNode_deletes_node_and_value()
        {
            // ARRANGE

            var valueId = ObjectId.NewObjectId();

            this.root = new LiteDbHierarchyNode(this.repository.Object, this.repository.Object.Root, new LiteDbHierarchyValueEntity() { Id = valueId });
            this.root.InnerNode.ValueRef = valueId;

            // node must be deleted

            this.repository
                .Setup(r => r.Delete(new[] { this.root.InnerNode }))
                .ReturnsAsync(true);

            // ACT

            var result = await this.root.Delete();

            // ASSERT

            Assert.True(result);
        }

        [Fact]
        public async Task LiteDbHierarchyNode_deletes_node_with_value()
        {
            // ARRANGE

            // node must be deleted

            this.repository
                .Setup(r => r.Delete(new[] { this.root.InnerNode }))
                .ReturnsAsync(true);

            // ACT

            var result = await this.root.Delete();

            // ASSERT

            Assert.True(result);
        }
    }
}