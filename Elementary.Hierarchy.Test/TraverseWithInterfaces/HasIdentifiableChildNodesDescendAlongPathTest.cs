namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
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
        public void I_root_returns_itself_and_child_on_DescentAlongPath()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType[] result = this.startNode.Object.DescentAlongPath(HierarchyPath.Create(1)).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(this.startNode.Object, result[0]);
            Assert.AreSame(childNode, result[1]);
        }

        [Test]
        public void I_root_returns_itself_on_DescendantAt()
        {
            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create<int>());

            // ASSERT

            Assert.AreSame(startNode.Object, result);
        }

        [Test]
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

            Assert.AreSame(subChildNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
            childNodeMock.Verify(n => n.TryGetChildNode(2, out subChildNode), Times.Once);
        }

        [Test]
        public void I_root_node_throws_on_invalid_childId_on_DescendantAt()
        {
            // ARRANGE
            this.startNode
                .Setup(n => n.HasChildNodes).Returns(false);

            // ACT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => { this.startNode.Object.DescendantAt(HierarchyPath.Create(1)); });

            // ASSERT

            Assert.IsTrue(result.Message.Contains("Key not found:'1'"));
        }
    }
}