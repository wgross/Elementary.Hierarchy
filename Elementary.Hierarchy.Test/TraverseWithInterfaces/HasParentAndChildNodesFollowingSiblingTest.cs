namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class HasParentAndChildNodesFollowingSiblingTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>, IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;

        private Mock<MockableNodeType> rightLeaf1;
        private Mock<MockableNodeType> rightLeaf2;
        private Mock<MockableNodeType> rightLeaf3;

        [SetUp]
        public void ArrangeAllTests()
        {
            //                rootNode
            //              /         \
            //        leftNode         rightNode
            //                         /     \               \
            //                rightLeaf1  rightRightLeaf2   rightLeaf3

            this.rightLeaf1 = new Mock<MockableNodeType>();
            this.rightLeaf1 // has no children
                .Setup(n => n.HasChildNodes).Returns(false);
            this.rightLeaf1 // has parent
                .Setup(n => n.HasParentNode).Returns(true);

            this.rightLeaf2 = new Mock<MockableNodeType>();
            this.rightLeaf2 // has no children
                .Setup(n => n.HasChildNodes).Returns(false);
            this.rightLeaf2 // has parent
                .Setup(n => n.HasParentNode).Returns(true);

            this.rightLeaf3 = new Mock<MockableNodeType>();
            this.rightLeaf3 // has no children
                .Setup(n => n.HasChildNodes).Returns(false);
            this.rightLeaf3 // has parent
                .Setup(n => n.HasParentNode).Returns(true);

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode // has rootNode as parent
                .Setup(n => n.HasParentNode).Returns(true);
            this.leftNode // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode // has root as parent
                .Setup(n => n.HasParentNode).Returns(true);
            this.rightNode // has three children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rightNode // return rightLeaf 1- 3 as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.rightLeaf1.Object, this.rightLeaf2.Object, this.rightLeaf3.Object });
            this.rightLeaf1 // rightNode is parent
                .Setup(n => n.ParentNode).Returns(this.rightNode.Object);
            this.rightLeaf2 // rightNode is parent
                .Setup(n => n.ParentNode).Returns(this.rightNode.Object);
            this.rightLeaf3 // rightNode is parent
                .Setup(n => n.ParentNode).Returns(this.rightNode.Object);

            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode // has a two children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rootNode // returns the left node and right node as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
            this.rightNode // has root as parent
                .Setup(n => n.ParentNode).Returns(this.rootNode.Object);
            this.leftNode // has root as parent
                .Setup(n => n.ParentNode).Returns(this.rootNode.Object);
        }

        [Test]
        public void RootNodeDoesntAllowSiblingNavigation()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void GetNextSiblingFromFirstRootChild()
        {
            // ACT

            MockableNodeType[] result = this.leftNode.Object.FollowingSiblings().ToArray();

            // ASSERT
            Assert.AreSame(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.AreSame(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(this.rightNode.Object, result.Single());
        }

        [Test]
        public void GetNoSiblingFromLastRootChild()
        {
            // ACT

            MockableNodeType[] result = this.rightNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            Assert.AreSame(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.AreSame(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetAllSiblingsFromFirstLeafNode()
        {
            // ACT

            MockableNodeType[] result = this.rightLeaf1.Object.FollowingSiblings().ToArray();

            // ASSERT

            Assert.AreSame(this.rightNode.Object, this.rightLeaf1.Object.Parent());
            Assert.AreSame(this.rightLeaf1.Object, this.rightNode.Object.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightLeaf2.Object, this.rightNode.Object.ChildNodes.ElementAt(1));
            Assert.AreSame(this.rightLeaf3.Object, this.rightNode.Object.ChildNodes.ElementAt(2));
            Assert.AreEqual(2, result.Count());
            Assert.AreSame(this.rightLeaf2.Object, result.ElementAt(0));
            Assert.AreSame(this.rightLeaf3.Object, result.ElementAt(1));
        }
    }
}