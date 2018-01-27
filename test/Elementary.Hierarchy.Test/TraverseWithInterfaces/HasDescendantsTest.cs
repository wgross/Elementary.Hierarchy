using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    public class HasDescendantTest
    {
        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        { }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;
        private Mock<MockableNodeType> leftLeaf;
        private Mock<MockableNodeType> leftRightLeaf;
        private Mock<MockableNodeType> rightRightLeaf;

        public HasDescendantTest()
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
        }

        [Fact]
        public void IHasDescendentNodes_leaf_returns_no_children_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.Descendants().ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.False(result.Any());

            this.rightRightLeaf.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void IHasDescendentNodes_node_returns_single_child_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.Descendants().ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same(this.leftLeaf.Object, result.ElementAt(0));

            this.leftNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void IHasDescendentNodes_node_returns_left_child_first_on_Descendants()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.Descendants();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

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
        public void IHasDescendentNodes_root_returns_descendants_breadthFirst_on_Descendants()
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
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void IHasDescendentNodes_root_returns_descendants_depthFirst_on_Descendants()
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
            this.rootNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rootNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.rightNode.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightNode.Verify(n => n.HasChildNodes, Times.Never());
            this.rightNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.leftRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftRightLeaf.Verify(n => n.ChildNodes, Times.Never());

            this.rightRightLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void IHasDescendentNodes_root_propegates_maxDepth_to_GetDescendants_on_Descendants()
        {
            // ARRANGE
            // add an additional setup to root

            this.rootNode
                .Setup(n => n.GetDescendants(false, 1))
                .Returns(new[] { this.leftNode.Object, this.rightNode.Object });

            // ACT
            // 

            var descendants = this.rootNode.Object.Descendants(maxDepth: 1).ToArray();

            // ASSERT

            this.rootNode.Verify(n => n.GetDescendants(false, 1), Times.Once());

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object }, descendants);
        }

        [Fact]
        public void IHasDescendentNodes_root_throws_on_level0_on_Descendants()
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
        public void IHasDescendentNodes_root_returns_all_descendants_on_highLevel_breadthFirst_on_Descendants()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.Object.Descendants(maxDepth: 3).ToArray();

            // ASSERT

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);
        }

        [Fact]
        public void IHasDescendentNodes_root_returns_all_descendants_on_highLevel_depthFirst_on_Descendants()
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
        }

        [Fact]
        public void IHasDescendentNodes_DescendantsLevel1AreChildren_on_Descendants()
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

            var descendantsAndSelf = this.rootNode.Object.Descendants(maxDepth: 1).ToArray();

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