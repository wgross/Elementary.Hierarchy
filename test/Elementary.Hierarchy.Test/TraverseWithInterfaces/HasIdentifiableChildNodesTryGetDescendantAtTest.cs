namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using Xunit;

    public class HasIdentifiableChildNodesTryGetDescendantAtTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasIdentifiableChildNodesTryGetDescendantAtTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_returns_child_on_TryGetDescendantAt()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType resultNode;
            bool result = this.startNode.Object.TryGetDescendantAt(HierarchyPath.Create(1), out resultNode);

            // ASSERT

            Assert.True(result);
            Assert.NotNull(resultNode);
            Assert.Same(childNode, resultNode);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void I_root_returns_itself_on_TryGetDescendantAt()
        {
            // ACT

            MockableNodeType resultNode;
            bool result = this.startNode.Object.TryGetDescendantAt(HierarchyPath.Create<int>(), out resultNode);

            // ASSERT

            Assert.True(result);
            Assert.Same(startNode.Object, resultNode);
        }

        [Fact]
        public void I_root_returns_grandchild_on_TryGetDescendentAt()
        {
            // ARRANGE

            var subChildNode = new Mock<MockableNodeType>().Object;

            var childNodeMock = new Mock<MockableNodeType>();
            childNodeMock
                .Setup(n => n.TryGetChildNode(2, out subChildNode))
                .Returns(true);

            var childNode = childNodeMock.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType resultNode;
            bool result = this.startNode.Object.TryGetDescendantAt(HierarchyPath.Create(1, 2), out resultNode);

            // ASSERT

            Assert.True(result);
            Assert.Same(subChildNode, resultNode);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once());
            childNodeMock.Verify(n => n.TryGetChildNode(2, out subChildNode), Times.Once());
        }

        [Fact]
        public void I_root_node_throws_KeyNotFoundException_on_invalid_childId_on_TryGetDescendantAt()
        {
            // ARRANGE
            var childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(false);

            // ACT

            MockableNodeType resultNode;
            bool result = this.startNode.Object.TryGetDescendantAt(HierarchyPath.Create(1), out resultNode);

            // ASSERT

            Assert.False(result);

            startNode.Verify(n => n.TryGetChildNode(It.IsAny<int>(), out childNode), Times.Once());
            startNode.VerifyAll();
        }
    }
}