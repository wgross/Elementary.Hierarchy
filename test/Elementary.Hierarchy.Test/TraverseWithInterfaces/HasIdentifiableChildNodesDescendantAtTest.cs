namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using Xunit;

    public class HasIdentifiableChildNodesDescendantAtTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasIdentifiableChildNodesDescendantAtTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_returns_child_on_DescendantAt()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create(1));

            // ASSERT

            Assert.NotNull(childNode);
            Assert.Same(childNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once());
        }

        [Fact]
        public void I_root_returns_itself_on_DescendantAt()
        {
            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create<int>());

            // ASSERT

            Assert.Same(startNode.Object, result);
        }

        [Fact]
        public void I_root_returns_grandchild_on_DescendentAt()
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

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.Same(subChildNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once());
            childNodeMock.Verify(n => n.TryGetChildNode(2, out subChildNode), Times.Once());
        }

        [Fact]
        public void I_root_node_throws_on_invalid_childId_on_DescendantAt()
        {
            // ARRANGE
            this.startNode
                .Setup(n => n.HasChildNodes).Returns(false);

            // ACT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => { this.startNode.Object.DescendantAt(HierarchyPath.Create(1)); });

            // ASSERT

            Assert.True(result.Message.Contains("Key not found:'1'"));
        }
    }
}