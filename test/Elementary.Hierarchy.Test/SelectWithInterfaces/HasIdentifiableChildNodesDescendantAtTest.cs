namespace Elementary.Hierarchy.Test.SelectWithInterfaces
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
                .Setup(n => n.TryGetChildNode(1))
                .Returns((true, childNode));

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create(1));

            // ASSERT

            Assert.NotNull(childNode);
            Assert.Same(childNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Once());
        }

        [Fact]
        public void I_root_returns_itself_on_DescendantAt_with_empty_path()
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
                .Setup(n => n.TryGetChildNode(2))
                .Returns((true, subChildNode));

            var childNode = childNodeMock.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((true, childNode));

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.Same(subChildNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Once());
            childNodeMock.Verify(n => n.TryGetChildNode(2), Times.Once());
        }

        [Fact]
        public void I_root_node_throws_on_invalid_childId_on_DescendantAt()
        {
            // ARRANGE

            var childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((false, null));

            // ACT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => { this.startNode.Object.DescendantAt(HierarchyPath.Create(1)); });

            // ASSERT

            Assert.True(result.Message.Contains("Key not found:'1'"));

            startNode.Verify(n => n.TryGetChildNode(It.IsAny<int>()), Times.Once());
            startNode.VerifyAll();
        }
    }
}