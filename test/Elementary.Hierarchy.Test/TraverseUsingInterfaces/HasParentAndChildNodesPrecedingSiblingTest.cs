namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using System.Linq;
    using Xunit;

    public class HasParentAndChildNodesPrecedingSiblingTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>, IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;

        private Mock<MockableNodeType> rightLeaf1;
        private Mock<MockableNodeType> rightLeaf2;
        private Mock<MockableNodeType> rightLeaf3;

        public HasParentAndChildNodesPrecedingSiblingTest()
        {
            //                rootNode
            //              /         \
            //        leftNode         rightNode
            //                         /     \               \
            //                rightLeaf1  rightRightLeaf2   rightLeaf3

            this.rightLeaf1 = new Mock<MockableNodeType>();
            this.rightLeaf1.SetupGet(n => n.HasChildNodes).Returns(false);
            this.rightLeaf1.SetupGet(n => n.HasParentNode).Returns(true);

            this.rightLeaf2 = new Mock<MockableNodeType>();
            this.rightLeaf2.SetupGet(n => n.HasChildNodes).Returns(false);
            this.rightLeaf2.SetupGet(n => n.HasParentNode).Returns(true);

            this.rightLeaf3 = new Mock<MockableNodeType>();
            this.rightLeaf3.SetupGet(n => n.HasChildNodes).Returns(false);
            this.rightLeaf3.SetupGet(n => n.HasParentNode).Returns(true);

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode.SetupGet(n => n.HasParentNode).Returns(true);
            this.leftNode.SetupGet(n => n.HasChildNodes).Returns(false);

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode.SetupGet(n => n.HasParentNode).Returns(true);
            this.rightNode.SetupGet(n => n.HasChildNodes).Returns(true);
            this.rightNode.SetupGet(n => n.ChildNodes).Returns(new[] { this.rightLeaf1.Object, this.rightLeaf2.Object, this.rightLeaf3.Object });
            this.rightLeaf1.SetupGet(n => n.ParentNode).Returns(this.rightNode.Object);
            this.rightLeaf2.SetupGet(n => n.ParentNode).Returns(this.rightNode.Object);
            this.rightLeaf3.SetupGet(n => n.ParentNode).Returns(this.rightNode.Object);

            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode.SetupGet(n => n.HasChildNodes).Returns(true);
            this.rootNode.SetupGet(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
            this.rightNode.SetupGet(n => n.ParentNode).Returns(this.rootNode.Object);
            this.leftNode.SetupGet(n => n.ParentNode).Returns(this.rootNode.Object);
        }

        [Fact]
        public void Root_node_hasno_preceding_siblings()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void Return_preceding_sibling_for_PrecedingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightNode.Object.PrecedingSiblings().ToArray();

            // ASSERT
            Assert.Same(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.Same(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.Equal(1, result.Count());
            Assert.Same(this.leftNode.Object, result.Single());
        }

        [Fact]
        public void Return_empty_siblinngs_for_PrecedingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.leftNode.Object.PrecedingSiblings().ToArray();

            // ASSERT

            Assert.Same(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.Same(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void Return_multiple_siblings_for_PrecedingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightLeaf3.Object.PrecedingSiblings().ToArray();

            // ASSERT

            Assert.Same(this.rightNode.Object, this.rightLeaf1.Object.Parent());
            Assert.Same(this.rightLeaf1.Object, this.rightNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightLeaf2.Object, this.rightNode.Object.ChildNodes.ElementAt(1));
            Assert.Same(this.rightLeaf3.Object, this.rightNode.Object.ChildNodes.ElementAt(2));
            Assert.Equal(2, result.Count());
            Assert.Same(this.rightLeaf1.Object, result.ElementAt(0));
            Assert.Same(this.rightLeaf2.Object, result.ElementAt(1));
        }
    }
}