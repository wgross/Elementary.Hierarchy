namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class HasIdentifiableChildNodesDescentAlongPathTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Test]
        public void I_root_returns_nothing_for_empty_path_on_DescendAlongPath()
        {
            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.IsTrue(result.Any());

            MockableNodeType childNode;
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void I_root_returns_child_on_DescendAlongPath()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new[] { this.startNode.Object, childNode }, result);

            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void I_root_returns_child_and_grandchild_on_DescentAlongPath()
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

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1, 2)).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { this.startNode.Object, childNode, subChildNode }, result);

            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
            childNodeMock.Verify(n => n.TryGetChildNode(2, out subChildNode), Times.Once);
        }

        [Test]
        public void I_root_return_incomplete_list_on_DescendAlongPath()
        {
            // ARRANGE
            this.startNode
                .Setup(n => n.HasChildNodes).Returns(false);

            // ACT

            MockableNodeType[] result = this.startNode.Object.DescendAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.IsTrue(result.Any());
            CollectionAssert.AreEqual(new[] { this.startNode.Object }, result);

            MockableNodeType childNode;
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }
    }
}