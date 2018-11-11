namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasChildNodesDescendantsAndSelfTest
    {
        private readonly MockRepository mocks = new MockRepository(MockBehavior.Strict);

        public interface MockableNodeType : IHasChildNodes<MockableNodeType>
        {
        }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;
        private Mock<MockableNodeType> leftLeaf;
        private Mock<MockableNodeType> leftRightLeaf;
        private Mock<MockableNodeType> rightRightLeaf;

        public HasChildNodesDescendantsAndSelfTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf

            this.rightRightLeaf = this.mocks.Create<MockableNodeType>();
            this.rightRightLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftRightLeaf = this.mocks.Create<MockableNodeType>();
            this.leftRightLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftLeaf = this.mocks.Create<MockableNodeType>();
            this.leftLeaf // has no children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.leftNode = this.mocks.Create<MockableNodeType>();
            this.leftNode // has single child
                .Setup(n => n.HasChildNodes).Returns(true);
            this.leftNode // returns leftLeaf as child
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftLeaf.Object });

            this.rightNode = this.mocks.Create<MockableNodeType>();
            this.rightNode // has two children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rightNode // return leftRight and rightRightLeaf as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object });

            this.rootNode = this.mocks.Create<MockableNodeType>();
            this.rootNode // has a tw children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rootNode // returns the left node and right node as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
        }

        [Fact]
        public void I_DescendantsAndSelf_rejects_max_depth_less_than_0()
        {
            // ACT

            var result = Assert.Throws<ArgumentException>(() => this.rightRightLeaf.Object.DescendantsAndSelf(maxDepth: -1));

            // ASSERT

            Assert.Equal("maxDepth", result.ParamName);
            Assert.StartsWith("must be > 0", result.Message);
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_itself_as_first_node()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Single(result);

            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Once());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_itself_despite_inconsistent_leaf()
        {
            // ARRANGE

            var badLeaf = this.mocks.Create<MockableNodeType>();
            badLeaf // claims to hav subnodes
                .Setup(n => n.HasChildNodes).Returns(true);
            badLeaf //  but returns empty set of subnodes
                .Setup(n => n.ChildNodes).Returns(Enumerable.Empty<MockableNodeType>());

            // ACT

            IEnumerable<MockableNodeType> result = badLeaf.Object.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.Single(result);
            Assert.Same(badLeaf.Object, result.ElementAt(0));

            badLeaf.Verify(n => n.HasChildNodes, Times.Once());
            badLeaf.Verify(n => n.ChildNodes, Times.Once());
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_single_child()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { this.leftNode.Object, this.leftLeaf.Object }, result);
            this.leftNode.VerifyAll();
            this.leftLeaf.VerifyAll();
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_left_child_before_right_child()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.DescendantsAndSelf();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

            this.rightNode.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_descendants_breadth_first()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsAndSelf().ToArray();

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
        public void I_DescendantsAndSelf_returns_descendants_depth_first()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsAndSelf(depthFirst: true).ToArray();

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
        public void I_DescendantsAndSelf_max_depth_2_restricts_to_start_nodes_children_on_bread_first()
        {
            // ACT

            var descendantsAndSelf = this.rootNode.Object.DescendantsAndSelf(maxDepth: 2).Skip(1).ToArray();
            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            Assert.Equal(children, descendantsAndSelf);
        }

        [Fact]
        public void I_DescendantsAndSelf_max_depth_2_restricts_to_start_nodes_children_on_depth_first()
        {
            // ACT

            var descendantsAndSelf = this.rootNode.Object.DescendantsAndSelf(depthFirst: true, maxDepth: 2).Skip(1).ToArray();
            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            Assert.Equal(children, descendantsAndSelf);
        }
    }
}