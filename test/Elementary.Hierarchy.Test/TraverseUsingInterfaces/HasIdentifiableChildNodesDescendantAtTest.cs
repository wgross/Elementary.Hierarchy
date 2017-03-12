namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using Xunit;

    public class HasIdentifiableChildNodesDescendantAtTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> root;

        public HasIdentifiableChildNodesDescendantAtTest()
        {
            this.root = new Mock<MockableNodeType>();
        }

        [Fact]
        public void Root_returns_child_at_path_position_on_DescendantAt()
        {
            // ARRANGE

            var childNode = new Mock<MockableNodeType>().Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNode)).Returns(true);

            // ACT

            MockableNodeType result = this.root.Object.DescendantAt(HierarchyPath.Create(1));

            // ASSERT

            Assert.NotNull(result);
            Assert.Same(childNode, result);
            this.root.Verify(r => r.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void Root_returns_itself_for_empty_path_on_DescendentAt()
        {
            // ACT

            MockableNodeType result = this.root.Object.DescendantAt(HierarchyPath.Create<int>());

            // ASSERT

            Assert.Same(this.root.Object, result);
        }

        [Fact]
        public void Root_returns_grandchild_at_path_position_on_DescendantAt()
        {
            // ARRANGE

            var subChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode.Setup(c => c.TryGetChildNode(2, out subChildNode)).Returns(true);

            var childNodeObject = childNode.Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNodeObject)).Returns(true);

            // ACT

            MockableNodeType result = this.root.Object.DescendantAt(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.Same(subChildNode, result);
            this.root.Verify(r => r.TryGetChildNode(1, out childNodeObject), Times.Once());
            childNode.Verify(c => c.TryGetChildNode(2, out subChildNode), Times.Once());
        }

        [Fact]
        public void Root_throws_for_missing_child_on_DescendantAt()
        {
            // ARRANGE
            this.root.SetupGet(r => r.HasChildNodes).Returns(false);

            // ACT & ASSERT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => this.root.Object.DescendantAt(HierarchyPath.Create(1)));

            // ASSERT

            Assert.True(result.Message.Contains("Key not found:'1'"));
        }

        [Fact]
        public void DescendantAtOrDefaultGetsExistingSubnode_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNode)).Returns(true);

            // ACT

            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.NotNull(result);
            Assert.Same(childNode, result);
            this.root.Verify(r => r.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void DescendantAtOrDefaultReturnsNullIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNode)).Returns(false);

            // ACT

            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.Null(result);
            this.root.Verify(r => r.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void DescendantAtOrDefaultReturnsParentPathIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNode)).Returns(false);

            // ACT

            HierarchyPath<int> foundPath;
            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundPath);

            // ASSERT

            Assert.Null(result);
            Assert.NotNull(foundPath);
            Assert.Equal((object)HierarchyPath.Create<int>(), (object)foundPath);
            this.root.Verify(r => r.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Fact]
        public void DescendantAtOrDefaultGetsExistingSubnodeAndParentNodePath_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNode)).Returns(true);

            // ACT

            HierarchyPath<int> foundKey;
            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundKey);

            // ASSERT

            Assert.NotNull(childNode);
            Assert.Same(childNode, result);
            Assert.NotNull(foundKey);
            Assert.Equal((object)HierarchyPath.Create<int>(1), (object)foundKey);
            this.root.Verify(r => r.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void DescendantAtOrDefaultGetsExistingGrandchildNode_IF()
        {
            // ARRANGE

            var grandChildNode = new Mock<MockableNodeType>().Object;
            var childNode = new Mock<MockableNodeType>();

            childNode.Setup(c => c.TryGetChildNode(2, out grandChildNode)).Returns(true);

            var childNodeObject = childNode.Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNodeObject)).Returns(true);

            // ACT

            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.NotNull(grandChildNode);
            Assert.Same(grandChildNode, result);
            this.root.Verify(r => r.TryGetChildNode(1, out childNodeObject), Times.Once());
            this.root.Verify(r => r.TryGetChildNode(1, out grandChildNode), Times.Once());
        }

        [Fact]
        public void DescendantAtOrDefaultGetsExistingGrandchildNodeAndChildNodePath_IF()
        {
            // ARRANGE

            var grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode.Setup(c => c.TryGetChildNode(2, out grandChildNode)).Returns(true);

            var childNodeObject = childNode.Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNodeObject)).Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.NotNull(grandChildNode);
            Assert.Same(grandChildNode, result);
            Assert.NotNull(foundKey);
            Assert.Equal((object)HierarchyPath.Create(1, 2), (object)foundKey);

            this.root.Verify(r => r.TryGetChildNode(1, out childNodeObject), Times.Once());
            this.root.Verify(r => r.TryGetChildNode(1, out grandChildNode), Times.Once());
        }

        [Fact]
        public void DescendantAtOrDefaultFailsIfGrandchildNodeDoesnExistReturnsParentPath_IF()
        {
            // ARRANGE

            var grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();

            childNode.Setup(c => c.TryGetChildNode(2, out grandChildNode)).Returns(false);

            var childNodeObject = childNode.Object;

            this.root.Setup(r => r.TryGetChildNode(1, out childNodeObject)).Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.root.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.Null(result);
            Assert.NotNull(foundKey);
            Assert.Equal((object)HierarchyPath.Create(1), (object)foundKey);

            this.root.Verify(r => r.TryGetChildNode(1, out childNodeObject), Times.Once());
            this.root.Verify(r => r.TryGetChildNode(1, out grandChildNode), Times.Once());
        }
    }
}