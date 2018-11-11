namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasDescendantNodesDescendantsAndSelfTest
    {
        private readonly MockRepository mocks = new MockRepository(MockBehavior.Strict);

        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        {
        }

        private readonly Mock<MockableNodeType> rootNode;
        private readonly Mock<MockableNodeType> leftNode;
        private readonly Mock<MockableNodeType> rightNode;
        private readonly Mock<MockableNodeType> leftLeaf;
        private readonly Mock<MockableNodeType> leftRightLeaf;
        private readonly Mock<MockableNodeType> rightRightLeaf;

        public HasDescendantNodesDescendantsAndSelfTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf

            // leaves

            this.rightRightLeaf = this.mocks.Create<MockableNodeType>();
            this.rightRightLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            this.leftRightLeaf = this.mocks.Create<MockableNodeType>();
            this.leftRightLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            this.leftLeaf = this.mocks.Create<MockableNodeType>();
            this.leftLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            // inner nodes

            this.leftNode = this.mocks.Create<MockableNodeType>();
            this.leftNode
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(new[] { this.leftLeaf.Object });

            this.rightNode = this.mocks.Create<MockableNodeType>();
            this.rightNode // return leftRight and rightRightLeaf as children
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object });

            // root

            this.rootNode = this.mocks.Create<MockableNodeType>();
            this.rootNode
                .Setup(n => n.GetDescendants(false, int.MaxValue))
                .Returns(new[]
                {
                    this.leftNode.Object, this.rightNode.Object,
                    this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object
                });

            this.rootNode
                .Setup(n => n.GetDescendants(true, int.MaxValue))
                .Returns(new[]
                {
                    this.leftNode.Object, this.leftLeaf.Object,
                    this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object
                });

            this.rootNode
                .Setup(n => n.GetDescendants(true, 3))
                .Returns(new[]
                {
                    this.leftNode.Object, this.leftLeaf.Object,
                    this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object
                });

            this.rootNode
                .Setup(n => n.GetDescendants(false, 3))
                .Returns(new[]
                {
                    this.leftNode.Object, this.rightNode.Object,
                    this.leftLeaf.Object,this.leftRightLeaf.Object, this.rightRightLeaf.Object
                });
        }

       

        [Fact]
        public void I_DescendantsAndSelf_returns_itself_as_first_node()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Same(this.rightRightLeaf.Object, result.ElementAt(0));

            this.rightRightLeaf.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_single_child()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.DescendantsAndSelf().ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { this.leftNode.Object, this.leftLeaf.Object }, result);

            this.leftNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_left_before_right_child()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.DescendantsAndSelf();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

            this.rightNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
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

            this.rootNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
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

            this.rootNode.Verify(n => n.GetDescendants(true, int.MaxValue), Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void I_DescendantsAndSelf_returns_descendants_breadth_first_restricted_to_max_Depth()
        {
            // ARRANGE

            this.rootNode
                .Setup(n => n.GetDescendants(false, 1))
                .Returns(new[]
                {
                    this.leftNode.Object, this.rightNode.Object,
                });

            this.rootNode
                .Setup(r => r.HasChildNodes)
                .Returns(true);

            this.rootNode
                .Setup(r => r.ChildNodes)
                .Returns(new[] { this.leftNode.Object, this.rightNode.Object });

            // ACT

            var descendantsAndSelf = this.rootNode.Object.DescendantsAndSelf(maxDepth: 2).Skip(1).ToArray();

            // ASSERT

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object }, descendantsAndSelf);

            this.rootNode.Verify(n => n.GetDescendants(false, 1), Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }
    }
}