namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class HasIdentifiableChildNodesDescendantAtTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private MockableNodeType root = Substitute.For<MockableNodeType>();

        [SetUp]
        public void ArrangeAllTests()
        {
            this.root = Substitute.For<MockableNodeType>();
        }

        [Test]
        public void Root_returns_child_at_path_position_on_DescendantAt()
        {
            // ARRANGE

            MockableNodeType childNode = Substitute.For<MockableNodeType>();

            var m = new Mock<MockableNodeType>();
            MockableNodeType cn = null;
            m.Setup(s => s.TryGetChildNode(1, out childNode)).Returns(true);
            
            this.root.TryGetChildNode(1, out cn).Returns(p =>
            {
                p[1] = childNode;
                return true;
            });

            // ACT

//            MockableNodeType result = this.root.DescendantAt(HierarchyPath.Create(1));

            MockableNodeType result=  m.Object.DescendantAt(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(childNode, result);
            this.root.Received(1).TryGetChildNode(1, out cn);
        }

        [Test]
        public void Root_returns_itself_for_empty_path_on_DescendentAt()
        {
            // ACT

            MockableNodeType result = this.root.DescendantAt(HierarchyPath.Create<int>());

            // ASSERT

            Assert.AreSame(root, result);
        }

        [Test]
        public void Root_returns_grandchild_at_path_position_on_DescendantAt()
        {
            // ARRANGE

            var subChildNode = Substitute.For<MockableNodeType>();

            var childNode = Substitute.For<MockableNodeType>();
            childNode.TryGetChildNode(2, out subChildNode).Returns(true);
            
            this.root.TryGetChildNode(1, out childNode).Returns(true);

            // ACT

            MockableNodeType result = this.root.DescendantAt(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.AreSame(subChildNode, result);
            this.root.Received(1).TryGetChildNode(1, out childNode);
            childNode.Received(1).TryGetChildNode(2, out subChildNode);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Root_throws_for_missing_child_on_DescendantAt()
        {
            // ARRANGE
            this.root.HasChildNodes.Returns(false);

            // ACT & ASSERT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(()=> this.root.DescendantAt(HierarchyPath.Create(1)));

            // ASSERT

            Assert.IsTrue(result.Message.Contains("Key not found:'1'"));
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingSubnode_IF()
        {
            // ARRANGE

            MockableNodeType childNode = Substitute.For<MockableNodeType>();

            this.root.TryGetChildNode(1, out childNode).Returns(true);

            // ACT

            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreSame(childNode, result);
            this.root.Received(1).TryGetChildNode(1, out childNode);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsNullIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = Substitute.For<MockableNodeType>();

            this.root.TryGetChildNode(1, out childNode).Returns(false);

            // ACT

            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1));

            // ASSERT

            Assert.IsNull(result);
            this.root.Received(1).TryGetChildNode(1, out childNode);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsParentPathIfSubnodeDoesntExist_IF()
        {
            // ARRANGE

            MockableNodeType childNode = Substitute.For<MockableNodeType>();

            this.root.TryGetChildNode(1, out childNode).Returns(false);

            // ACT

            HierarchyPath<int> foundPath;
            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1), out foundPath);

            // ASSERT

            Assert.IsNull(result);
            Assert.IsNotNull(foundPath);
            Assert.AreEqual((object)HierarchyPath.Create<int>(), (object)foundPath);
            this.root.Received(1).TryGetChildNode(1, out childNode);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingSubnodeAndParentNodePath_IF()
        {
            // ARRANGE

            MockableNodeType childNode = Substitute.For<MockableNodeType>();

            this.root.TryGetChildNode(1, out childNode).Returns(true);

            // ACT

            HierarchyPath<int> foundKey;
            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1), out foundKey);

            // ASSERT

            Assert.IsNotNull(childNode);
            Assert.AreSame(childNode, result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create<int>(1), (object)foundKey);
            this.root.Received(1).TryGetChildNode(1, out childNode);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingGrandchildNode_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = Substitute.For<MockableNodeType>();

            var childNode = Substitute.For<MockableNodeType>();
            childNode.TryGetChildNode(2, out grandChildNode).Returns(true);

            var childNodeObject = childNode;

            this.root.TryGetChildNode(1, out childNodeObject).Returns(true);

            // ACT

            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1, 2));

            // ASSERT

            Assert.IsNotNull(grandChildNode);
            Assert.AreSame(grandChildNode, result);
            this.root.Received(1).TryGetChildNode(1, out childNodeObject);
            this.root.Received(1).TryGetChildNode(1, out grandChildNode);
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingGrandchildNodeAndChildNodePath_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = Substitute.For<MockableNodeType>();

            var childNode = Substitute.For<MockableNodeType>();
            childNode.TryGetChildNode(2, out grandChildNode).Returns(true);

            var childNodeObject = childNode;

            this.root.TryGetChildNode(1, out childNodeObject).Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.IsNotNull(grandChildNode);
            Assert.AreSame(grandChildNode, result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create(1, 2), (object)foundKey);

            this.root.Received(1).TryGetChildNode(1, out childNodeObject);
            this.root.Received(1).TryGetChildNode(1, out grandChildNode);
        }

        [Test]
        public void DescendantAtOrDefaultFailsIfGrandchildNodeDoesnExistReturnsParentPath_IF()
        {
            // ARRANGE

            MockableNodeType grandChildNode = Substitute.For<MockableNodeType>();

            var childNode = Substitute.For<MockableNodeType>();
            childNode.TryGetChildNode(2, out grandChildNode).Returns(false);

            var childNodeObject = childNode;

            this.root.TryGetChildNode(1, out childNodeObject).Returns(true);

            // ACT

            HierarchyPath<int> foundKey = null;
            MockableNodeType result = this.root.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundKey);

            // ASSERT

            Assert.IsNull(result);
            Assert.IsNotNull(foundKey);
            Assert.AreEqual((object)HierarchyPath.Create(1), (object)foundKey);

            this.root.Received(1).TryGetChildNode(1, out childNodeObject);
            this.root.Received(1).TryGetChildNode(1, out grandChildNode);
        }
    }
}