using LiteDB;
using Moq;
using System;
using System.Collections.Generic;
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
            // root has now a child in the inner node.
            Assert.True(this.root.InnerNode.HasChildNodes);
            Assert.Equal<BsonValue>(childId, this.root.InnerNode.ChildNodes.Single().Value);
            Assert.Equal<BsonValue>(childId, this.root.InnerNode.ChildNodeIds["child"]);
            // root has child
            Assert.True(this.root.HasChildNodes);
            Assert.Single(this.root.ChildNodes);
            Assert.Equal<BsonValue>(childId, this.root.ChildNodes.Single().InnerNode._Id);
            // the child has no children
            Assert.False(result.InnerNode.HasChildNodes);
            Assert.Equal("child", result.InnerNode.Key);
        }

        [Fact]
        public void LiteDbHierarchyNode_adding_new_node_as_child_returns_false_on_failed_insert()
        {
            // ARRANGE

            var childId = ObjectId.NewObjectId();
            // node node must be added
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                .Returns((false, BsonValue.Null));

            // ACT

            var result = this.root.AddChildNode(key: "child");

            // ASSERT

            Assert.Null(result);
            // root has now a child in the inner node.
            Assert.False(this.root.InnerNode.HasChildNodes);
            // root has child
            Assert.False(this.root.HasChildNodes);
        }

        [Fact]
        public void LiteDbHierarchyNode_adding_new_node_as_child_returns_false_on_failed_update()
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
                .Returns(false);
            // the orphaned node is deleted
            this.repository
                .Setup(r => r.DeleteNode(childId, false))
                .Returns(true);

            // ACT

            var result = this.root.AddChildNode(key: "child");

            // ASSERT

            Assert.Null(result);
            // root has now a child in the inner node.
            Assert.False(this.root.InnerNode.HasChildNodes);
            // root has child
            Assert.False(this.root.HasChildNodes);
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
            // child node is read from db
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { Key = "child", _Id = childId });

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
        public void LiteDbHierarchyNode_reads_child_nodes_by_name()
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
            // child node is read from db
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { Key = "child", _Id = childId });

            this.root.AddChildNode("child");

            // ACT

            var (success, result) = this.root.TryGetChildNode("child");

            // ASSERT

            Assert.True(success);
            Assert.Equal<ObjectId>(childId, result.InnerNode._Id);
        }

        [Fact]
        public void LiteDbHierarchyNode_reading_child_nodes_by_name_returns_false_on_unkown_name()
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
            // child node is read from db
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { Key = "child", _Id = childId });

            this.root.AddChildNode("child");

            // ACT

            var (success, result) = this.root.TryGetChildNode("child");

            // ASSERT

            Assert.True(success);
            Assert.Equal<ObjectId>(childId, result.InnerNode._Id);
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
            // root node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);
            // child is read from repo
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { _Id = childId, Key = "child" });

            var childNode = this.root.AddChildNode(key: "child");

            // child was deleted
            this.repository
                .Setup(r => r.DeleteNode(childNode.InnerNode._Id, false))
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
        public void LiteDbHierarchyNode_removes_child_node_recursively()
        {
            // ARRANGE
            // node node must be added
            var childId = ObjectId.NewObjectId();
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                .Callback<LiteDbHierarchyNodeEntity>(n => n._Id = childId)
                .Returns((true, childId));
            // root node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);
            // child is read from repo
            this.repository
                .Setup(r => r.Read(childId))
                .Returns(new LiteDbHierarchyNodeEntity { _Id = childId, Key = "child" });

            var childNode = this.root.AddChildNode(key: "child");

            // child was deleted
            this.repository
                .Setup(r => r.DeleteNode(childNode.InnerNode._Id, true))
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
            // ARRANGE

            // node node must be added
            var nodes = new List<LiteDbHierarchyNodeEntity>();
            this.repository
                 .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNodeEntity>()))
                 .Callback<LiteDbHierarchyNodeEntity>(n => { nodes.Add(n); n._Id = ObjectId.NewObjectId(); })
                 .Returns<LiteDbHierarchyNodeEntity>(n => (true, n._Id));
            // root node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);
            // child nodes are read from repo
            this.repository
                .Setup(r => r.Read(It.IsAny<BsonValue>()))
                .Returns<BsonValue>(id => nodes.Single(n => n._Id.Equals(id)));
            // child was deleted
            this.repository
                .Setup(r => r.DeleteNode(It.Is<BsonValue>(v => v.Equals(nodes.Single(n => n.Key.Equals("child"))._Id)), false))
                .Returns(true);

            var child = this.root.AddChildNode(key: "child");
            var grandChild1 = this.root.AddChildNode(key: "grandchild1");
            var grandChild2 = this.root.AddChildNode(key: "grandchild2");
            var grandGrandChild1 = this.root.AddChildNode(key: "grandgrandchild1");

            // ACT

            var result = this.root.RemoveChildNode(child.Key);

            // ASSERT

            Assert.True(result);
        }
    }
}