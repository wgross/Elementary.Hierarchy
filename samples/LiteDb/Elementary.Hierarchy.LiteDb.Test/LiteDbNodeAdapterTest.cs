using LiteDB;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyNodeAdapterTest : IDisposable
    {
        private readonly MockRepository mocks = new MockRepository(MockBehavior.Strict);
        private readonly Mock<ILiteDbHierarchyNodeRepository> repository;
        private LiteDbHierarchyNodeAdapter root;
        private LiteCollection<LiteDbHierarchyNode> nodes;

        public LiteDbHierarchyNodeAdapterTest()
        {
            this.repository = this.mocks.Create<ILiteDbHierarchyNodeRepository>();
            this.repository.Setup(r => r.Root).Returns(new LiteDbHierarchyNode());
            this.root = new LiteDbHierarchyNodeAdapter(this.repository.Object, this.repository.Object.Root);
        }

        public void Dispose()
        {
            this.mocks.VerifyAll();
        }

        [Fact]
        public void LiteDbHierarchyNodeAdapter_has_no_child_nodes_when_empty()
        {
            // ASSERT

            Assert.Null(this.root.Key);
            Assert.False(this.root.HasChildNodes);
            Assert.False(this.root.ChildNodes.Any());
        }

        [Fact]
        public void LiteDbHierarchyNodeAdapter_adds_new_node_as_child()
        {
            // ARRANGE
            // node node mast be added
            this.repository
                .Setup(r => r.TryInsert(It.IsAny<LiteDbHierarchyNode>()))
                .Returns((true, new BsonValue("id")));
            // parent node must be updated
            this.repository
                .Setup(r => r.Update(this.root.InnerNode))
                .Returns(true);

            // ACT

            var result = this.root.AddChildNode(key: "child");

            // ASSERT
            // root has now a child
            Assert.True(this.root.HasChildNodes);
            Assert.Same(result, this.root.ChildNodes.Single());
            // root referebces the child node
            Assert.Equal(new BsonValue("id"), this.root.InnerNode._ChildNodeIds["child"]);
            // the child has no children
            Assert.False(result.HasChildNodes);
            Assert.Equal("child", result.Key);
        }

        //[Fact]
        //public void LiteDbHierarchyNodeAdapter_adding_new_node_as_child_throws_on_duplicate_id()
        //{
        //    // ARRANGE

        //    var child1 = new LiteDbHierarchyNodeAdapter("child", new BsonDocument());
        //    var node = new LiteDbHierarchyNodeAdapter(string.Empty, new BsonDocument());
        //    node.AddChildNode(child1);

        //    var child2 = new LiteDbHierarchyNodeAdapter("child", new BsonDocument());

        //    // ACT

        //    var result = Assert.Throws<InvalidOperationException>(() => node.AddChildNode(child2));

        //    // ASSERT

        //    Assert.Equal("duplicate key: 'child'", result.Message);
        //    Assert.Same(child1, node.ChildNodes.Single());
        //    Assert.Equal(BsonValue.Null, node.InnerNode["cn"].AsDocument["child"]);
        //}

        //[Fact]
        //public void LiteDbHierarchyNodeAdapter_removes_child_node()
        //{
        //    // ARRANGE

        //    var child = new LiteDbHierarchyNodeAdapter("child", new BsonDocument());
        //    var parent = new LiteDbHierarchyNodeAdapter(string.Empty, new BsonDocument());
        //    parent.AddChildNode(child);

        //    // ACT

        //    var result = parent.RemoveChildNode(child);

        //    // ASSERT

        //    Assert.True(result);
        //    Assert.False(parent.HasChildNodes);
        //    Assert.NotNull(parent.InnerNode["cn"].AsDocument);
        //    Assert.False(parent.InnerNode["cn"].AsDocument.Any());
        //}

        //[Fact]
        //public void LiteDbHierarchyNodeAdapter_removing_child_node_fails_silently_for_unknown_child()
        //{
        //    // ARRANGE

        //    var child = new LiteDbHierarchyNodeAdapter("child", new BsonDocument());
        //    var parent = new LiteDbHierarchyNodeAdapter(string.Empty, new BsonDocument());

        //    // ACT

        //    var result = parent.RemoveChildNode(child);

        //    // ASSERT

        //    Assert.False(result);
        //    Assert.False(parent.HasChildNodes);
        //    Assert.NotNull(parent.InnerNode["cn"].AsDocument);
        //    Assert.False(parent.InnerNode["cn"].AsDocument.Any());
        //}
    }
}