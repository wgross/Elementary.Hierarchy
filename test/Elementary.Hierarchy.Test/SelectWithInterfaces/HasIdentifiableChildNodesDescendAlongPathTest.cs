namespace Elementary.Hierarchy.Test.SelectWithInterfaces
{
    using Moq;
    using System.Linq;
    using Xunit;

    public class HasIdentifiableChildNodesDescendAlongPathTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasIdentifiableChildNodesDescendAlongPathTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_returns_nothing_for_empty_path_on_DescendAlongPath()
        {
            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.True(result.Any());

            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Once());
        }

        [Fact]
        public void I_root_returns_child_on_DescendAlongPath()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((true, childNode));

            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(new[] { this.startNode.Object, childNode }, result);

            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Once());
        }

        [Fact]
        public void I_root_returns_child_and_grandchild_on_DescendAlongPath()
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

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1, 2)).ToArray();

            // ASSERT

            Assert.Equal(new[] { this.startNode.Object, childNode, subChildNode }, result);

            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Once());
            childNodeMock.Verify(n => n.TryGetChildNode(2), Times.Once());
        }

        [Fact]
        public void I_root_return_incomplete_list_on_DescendAlongPath()
        {
            // ARRANGE

            var childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((false, childNode));

            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.True(result.Any());
            Assert.Equal(new[] { this.startNode.Object }, result);

            startNode.Verify(n => n.TryGetChildNode(It.IsAny<int>()), Times.Once());
            startNode.VerifyAll();
        }
    }
}