namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class HasIdentifiableChildNodesDescendantAtTest
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
        public void GetSubNodeWithDescendantAt_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNotNull(childNode);
            Assert.AreSame(childNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void DescendantAtWithRootPathGetsStartNode_IF()
        {
            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAt(HierarchyPath.Create<int>());

            // ASSERT

            Assert.AreSame(startNode.Object, result);
        }

        [Test]
        public void GetDescendantAtTwoLevelUnderStartNode_IF()
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
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DescendantAtFailsIfSubNodeDoesntExist_IF()
        {
            // ARRANGE
            this.startNode
                .Setup(n => n.HasChildNodes).Returns(false);

            // ACT & ASSERT

            this.startNode.Object.DescendantAt(HierarchyPath.Create(1));
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingSubnode_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(childNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsNullIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(false);

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNull(result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsParentPathIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(false);

            // ACT

            HierarchyPath<int> foundPath;
            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundPath);

            // ASSERT

            Assert.IsNull(result);
            Assert.IsNotNull(foundPath);
            Assert.AreEqual((object)HierarchyPath.Create<int>(), (object)foundPath);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingSubnodeAndParentNodePath_IF()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNode))
                .Returns(true);

            // ACT

            HierarchyPath<int> foundKey;
            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundKey);

            // ASSERT

            Assert.IsNotNull(childNode);
            Assert.AreSame(childNode, result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create<int>(1), (object)foundKey);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingGrandchildNode_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode
                .Setup(n => n.TryGetChildNode(2, out grandChildNode))
                .Returns(true);

            var childNodeObject = childNode.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNodeObject))
                .Returns(true);

            // ACT

            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.IsNotNull(grandChildNode);
            Assert.AreSame(grandChildNode, result);
            this.startNode.Verify(n => n.TryGetChildNode(1, out childNodeObject), Times.Once);
            this.startNode.Verify(n => n.TryGetChildNode(1, out grandChildNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingGrandchildNodeAndChildNodePath_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode
                .Setup(n => n.TryGetChildNode(2, out grandChildNode))
                .Returns(true);

            var childNodeObject = childNode.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNodeObject))
                .Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.IsNotNull(grandChildNode);
            Assert.AreSame(grandChildNode, result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create(1, 2), (object)foundKey);

            this.startNode.Verify(n => n.TryGetChildNode(1, out childNodeObject), Times.Once);
            this.startNode.Verify(n => n.TryGetChildNode(1, out grandChildNode), Times.Once);
        }

        [Test]
        public void DescendantAtOrDefaultFailsIfGrandchildNodeDoesnExistReturnsParentPath_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode
                .Setup(n => n.TryGetChildNode(2, out grandChildNode))
                .Returns(false);

            var childNodeObject = childNode.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1, out childNodeObject))
                .Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.IsNull(result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create(1), (object)foundKey);

            this.startNode.Verify(n => n.TryGetChildNode(1, out childNodeObject), Times.Once);
            this.startNode.Verify(n => n.TryGetChildNode(1, out grandChildNode), Times.Once);
        }
    }
}