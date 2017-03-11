namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasChildNodesDescendantsOrSelfTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>
        {
        }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;
        private Mock<MockableNodeType> leftLeaf;
        private Mock<MockableNodeType> leftRightLeaf;
        private Mock<MockableNodeType> rightRightLeaf;

        public HasChildNodesDescendantsOrSelfTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf

            this.rightRightLeaf = new Mock<MockableNodeType>();
            this.rightRightLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftRightLeaf = new Mock<MockableNodeType>();
            this.leftRightLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftLeaf = new Mock<MockableNodeType>();
            this.leftLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode // has single child
                .Setup(n => n.HasChildNodes).Returns(true);
            this.leftNode // returns leftLeaf as child
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftLeaf.Object });

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode // has two children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rightNode // return leftRight and rightRightLeaf as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object });

            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode // has a tw children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rootNode // returns the left node and right node as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
        }

        [Fact]
        public void I_leaf_returns_itself_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());

            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Once());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void I_inconsistent_leaf_returns_itself_on_DescendantsOrSelf()
        {
            // ARRANGE

            var badLeaf = new Mock<MockableNodeType>();
            badLeaf // claims to hav subnodes
                .Setup(n => n.HasChildNodes).Returns(true);
            badLeaf //  but returns empty set of subnodes
                .Setup(n => n.ChildNodes).Returns(Enumerable.Empty<MockableNodeType>());

            // ACT

            IEnumerable<MockableNodeType> result = badLeaf.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same(badLeaf.Object, result.ElementAt(0));

            badLeaf.Verify(n => n.HasChildNodes, Times.Once());
            badLeaf.Verify(n => n.ChildNodes, Times.Once());
        }

        [Fact]
        public void I_leaf_returns_single_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { this.leftNode.Object, this.leftLeaf.Object }, result);
            this.leftNode.VerifyAll();
            this.leftLeaf.VerifyAll();
        }

        [Fact]
        public void I_leaf_returns_left_before_right_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.DescendantsOrSelf();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

            this.rightNode.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Fact]
        public void I_leaf_returns_descendants_breadthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                this.rootNode,
                this.leftNode,
                this.rightNode,
                this.leftLeaf,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Fact]
        public void I_leaf_returns_descendants_depthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsOrSelf(depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                this.rootNode,
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Fact]
        public void I_DescendantsOrSelfLevel2AreChildren_on_DescendantsOrSelf()
        {
            // ACT

            var descendantsOrSelf = this.rootNode.Object.DescendantsOrSelf(maxDepth: 2).Skip(1).ToArray();
            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            Assert.Equal(children, descendantsOrSelf);
        }
    }
}