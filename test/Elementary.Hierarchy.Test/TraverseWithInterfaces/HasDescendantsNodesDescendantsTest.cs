﻿namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasDescendantNodesDescendantsTest
    {
        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        { }

        private readonly Mock<MockableNodeType> rootNode;
        private readonly Mock<MockableNodeType> leftNode;
        private readonly Mock<MockableNodeType> rightNode;
        private readonly Mock<MockableNodeType> leftLeaf;
        private readonly Mock<MockableNodeType> leftRightLeaf;
        private Mock<MockableNodeType> rightRightLeaf;

        public HasDescendantNodesDescendantsTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf

            // leaves

            this.rightRightLeaf = new Mock<MockableNodeType>();
            this.rightRightLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            this.leftRightLeaf = new Mock<MockableNodeType>();
            this.leftRightLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            this.leftLeaf = new Mock<MockableNodeType>();
            this.leftLeaf
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(Enumerable.Empty<MockableNodeType>());

            // inner nodes

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(new[] { this.leftLeaf.Object });

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode // return leftRight and rightRightLeaf as children
                .Setup(n => n.GetDescendants(false, int.MaxValue)).Returns(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object });

            // root

            this.rootNode = new Mock<MockableNodeType>();
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

            this.rootNode
                .Setup(n => n.GetDescendants(false, 1))
                .Returns(new[]
                {
                    this.leftNode.Object, this.rightNode.Object
                });
        }

        [Fact]
        public void IHasDescendants_leaf_returns_no_children_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.Descendants().ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.False(result.Any());

            this.rightRightLeaf.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
            this.rightRightLeaf.VerifyAll();
        }

        [Fact]
        public void IHasDescendants_node_returns_single_child_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.Descendants().ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same(this.leftLeaf.Object, result.ElementAt(0));

            this.leftNode.VerifyAll();
            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void IHasDescendants_node_returns_left_child_first_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.Descendants();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

            this.rightNode.VerifyAll();
            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void IHasDescendants_root_returns_descendants_breadthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Descendants().ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[]
            {
                this.leftNode.Object,
                this.rightNode.Object,
                this.leftLeaf.Object,
                this.leftRightLeaf.Object,
                this.rightRightLeaf.Object
            }, result);

            this.rootNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void IHasDescendants_root_returns_descendants_depthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Descendants(depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] {
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result);

            this.rootNode.Verify(n => n.GetDescendants(true, int.MaxValue), Times.Once());
            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void IHasDescendants_root_returns_children_as_level1_descendants_on_Descendants()
        {
            // ARRANGE

            // enable IHasChildren at root node
            this.rootNode
                .SetupGet(n => n.HasChildNodes)
                .Returns(true);

            this.rootNode
                .SetupGet(n => n.ChildNodes)
                .Returns(new[] { this.leftNode.Object, this.rightNode.Object });

            // ACT

            var descendants = this.rootNode.Object.Descendants(maxDepth: 1).ToArray();

            // ASSERT

            Assert.Equal(2, descendants.Count());
            var children = new[] { this.leftNode.Object, this.rightNode.Object };
            Assert.Equal(2, children.Count());
            Assert.Equal(children, descendants);

            this.rootNode.Verify(n => n.GetDescendants(false, 1), Times.Once());
        }

        [Fact]
        public void IHasDescendants_root_throws_ArgumentException_on_level0_on_Descendants()
        {
            // ACT

            ArgumentException ex = Assert.Throws<ArgumentException>(() => this.rootNode.Object.Descendants(maxDepth: -1));
            MockableNodeType[] result = this.rootNode.Object.Descendants(maxDepth: 0).ToArray();

            // ASSERT

            Assert.True(ex.Message.Contains("must be > 0"));
            Assert.Equal("maxDepth", ex.ParamName);
            Assert.False(result.Any());
        }

        [Fact]
        public void IHasDescendants_root_returns_all_descendants_on_highLevel_breadthFirst_on_Descendants()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.Descendants(maxDepth: 3).ToArray();

            // ASSERT

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);
            this.rootNode.Verify(n => n.GetDescendants(false, 3), Times.Once());
        }

        [Fact]
        public void IHasDescendants_root_returns_all_descendants_on_highLevel_depthFirst_on_Descendants()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.Descendants(maxDepth: 3, depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(new[] {
                this.leftNode.Object,
                this.leftLeaf.Object,
                this.rightNode.Object,
                this.leftRightLeaf.Object,
                this.rightRightLeaf.Object }, result);

            this.rootNode.Verify(n => n.GetDescendants(true, 3), Times.Once());
        }
    }
}