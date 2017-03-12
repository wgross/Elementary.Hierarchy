namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System.Linq;
    using Xunit;

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

        public HasParentAndChildNodesFollowingSiblingTest()
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

        [Fact]
        public void I_root_node_has_no_siblings_on_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            this.rootNode.Verify(n => n.HasParentNode, Times.Once());
            this.rootNode.Verify(n => n.ParentNode, Times.Never());

            Assert.False(result.Any());
        }

        [Fact]
        public void I_node_returns_right_sibling_on_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.leftNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            this.leftNode.Verify(n => n.HasParentNode, Times.Once());
            this.leftNode.Verify(n => n.ParentNode, Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Once());

            Assert.Same(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.Same(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.Equal(1, result.Count());
            Assert.Same(this.rightNode.Object, result.Single());
        }

        [Fact]
        public void I_node_returns_no_siblings_on_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightNode.Object.FollowingSiblings().ToArray();

            // ASSERT

            this.rightNode.Verify(n => n.HasParentNode, Times.Once());
            this.rightNode.Verify(n => n.ParentNode, Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Once());

            Assert.Same(this.rootNode.Object, this.leftNode.Object.Parent());
            Assert.Same(this.leftNode.Object, this.rootNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightNode.Object, this.rootNode.Object.ChildNodes.ElementAt(1));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void I_node_returns_all_siblings_on_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightLeaf1.Object.FollowingSiblings().ToArray();

            // ASSERT

            this.rightLeaf1.Verify(n => n.HasParentNode, Times.Once());
            this.rightLeaf1.Verify(n => n.ParentNode, Times.Once());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Once());

            Assert.Same(this.rightNode.Object, this.rightLeaf1.Object.Parent());
            Assert.Same(this.rightLeaf1.Object, this.rightNode.Object.ChildNodes.ElementAt(0));
            Assert.Same(this.rightLeaf2.Object, this.rightNode.Object.ChildNodes.ElementAt(1));
            Assert.Same(this.rightLeaf3.Object, this.rightNode.Object.ChildNodes.ElementAt(2));
            Assert.Equal(2, result.Count());
            Assert.Same(this.rightLeaf2.Object, result.ElementAt(0));
            Assert.Same(this.rightLeaf3.Object, result.ElementAt(1));
        }
    }
}