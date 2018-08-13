using LiteDB;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyNodeTest : IDisposable
    {
        private readonly MockRepository mocks = new MockRepository(MockBehavior.Strict);
        private readonly Mock<ILiteDbHierarchyNodeRepository> repository;
        private LiteDbHierarchyNode root;
        private LiteCollection<LiteDbHierarchyNodeEntity> nodes;

        public LiteDbHierarchyNodeTest()
        {
            this.repository = this.mocks.Create<ILiteDbHierarchyNodeRepository>();
            this.repository.Setup(r => r.Root).Returns(new LiteDbHierarchyNodeEntity());
            this.root = new LiteDbHierarchyNode(this.repository.Object, this.repository.Object.Root);
        }

        public void Dispose()
        {
            this.mocks.VerifyAll();
        }

        [Fact]
        public void LiteDbHierarchyNode_has_no_child_nodes_when_empty()
        {
            // ASSERT

            Assert.Null(this.root.Key);
            Assert.False(this.root.HasChildNodes);
            Assert.False(this.root.ChildNodes.Any());
        }

        [Fact]
        public void LiteDbHierarchyNode_adds_new_node_as_child()
        {
            // ARRANGE
            var childId = ObjectId.NewObjectId();
            // node node must be added
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                .Returns((true, childId));
            // parent node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);
            // child is read from repo
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { _Id = childId, Key = "child" });

            // ACT

            var result = this.root.AddChildNode(key: "child");

            // ASSERT
            // root has now a child
            Assert.True(this.root.HasChildNodes);
            Assert.Equal(childId, this.root.ChildNodes.Single().InnerNode._Id);
            // root references the child node
            Assert.Equal<BsonValue>(childId, this.root.InnerNode.ChildNodeIds["child"]);
            // the child has no children
            Assert.False(result.HasChildNodes);
            Assert.Equal("child", result.Key);
        }

        [Fact]
        public void LiteDbHierarchyNode_adding_new_node_as_child_throws_on_duplicate_id()
        {
            // ARRANGE
            var childId = ObjectId.NewObjectId();
            // node node must be added
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                .Callback<LiteDbHierarchyNodeEntity>(r => r._Id = childId)
                .Returns((true, childId));
            // parent node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);

            var child = this.root.AddChildNode(key: "child");

            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => this.root.AddChildNode(key: "child"));

            // ASSERT

            Assert.Equal($"Duplicate child node(key='child') under parent node(id='{this.root.InnerNode._Id}') was rejected.", result.Message);
        }

        [Fact]
        public void LiteDbHierarchyNode_reads_child_nodes()
        {
            // ARRANGE

            var childId = ObjectId.NewObjectId();

            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity() { _Id = childId });

            this.root.InnerNode.ChildNodeIds.Add("child", childId);

            // ACT

            var result = this.root.ChildNodes.ToArray();

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void LiteDbHierarchyNode_removes_child_node()
        {
            // ARRANGE
            // node node must be added
            var childId = ObjectId.NewObjectId();
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                .Callback<LiteDbHierarchyNodeEntity>(n => n._Id = childId)
                .Returns((true, childId));
            // parent node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);

            var childNode = this.root.AddChildNode(key: "child");

            // child was deleted
            this.repository
                .Setup(r => r.Remove(childNode.InnerNode._Id))
                .Returns(true);

            // ACT

            var result = this.root.RemoveChildNode(key: "child");

            // ASSERT

            this.repository.Verify(r => r.Update(this.root.InnerNode), Times.Exactly(2));

            Assert.True(result);
            // root has now a child
            Assert.False(this.root.HasChildNodes);
            Assert.Empty(this.root.ChildNodes);
            // root referebces the child node
            Assert.Empty(this.root.InnerNode.ChildNodeIds);
        }

        [Fact]
        public void LiteDbHierarchyNode_removing_child_node_returns_false_on_unknown_child()
        {
            // ACT

            var result = this.root.RemoveChildNode(key: "child");

            // ASSERT

            Assert.False(result);
            // root has now a child
            Assert.False(this.root.HasChildNodes);
            Assert.Empty(this.root.ChildNodes);
            // root referebces the child node
            Assert.Empty(this.root.InnerNode.ChildNodeIds);
        }
    }
}