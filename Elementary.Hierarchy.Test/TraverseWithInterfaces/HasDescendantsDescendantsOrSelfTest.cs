namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasDescendantsDescendantsOrSelfTest
    {
        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        {
        }

        private Mock<MockableNodeType> rootNode;
        private Mock<MockableNodeType> leftNode;
        private Mock<MockableNodeType> rightNode;
        private Mock<MockableNodeType> leftLeaf;
        private Mock<MockableNodeType> leftRightLeaf;
        private Mock<MockableNodeType> rightRightLeaf;

        [SetUp]
        public void ArrangeAllTests()
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

        [Test]
        public void IHasDescendentNodes_leaf_returns_itself_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(this.rightRightLeaf.Object, result.ElementAt(0));

            this.rightRightLeaf.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Test]
        public void IHasDescendentNodes_leaf_returns_single_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { this.leftNode.Object, this.leftLeaf.Object }, result);

            this.leftNode.Verify(n => n.GetDescendants(false, int.MaxValue), Times.Once());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Never());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            this.leftLeaf.Verify(n => n.GetDescendants(It.IsAny<bool>(), It.IsAny<int>()), Times.Never());
            this.leftLeaf.Verify(n => n.HasChildNodes, Times.Never());
            this.leftLeaf.Verify(n => n.ChildNodes, Times.Never());
        }

        [Test]
        public void IHasDescendentNodes_leaf_returns_left_before_right_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.DescendantsOrSelf();

            // ASSERT

            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[] { this.rightNode.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

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

        [Test]
        public void IHasDescendentNodes_leaf_returns_descendants_breadthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsOrSelf().ToArray();

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
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

        [Test]
        public void IHasDescendentNodes_leaf_returns_descendants_depthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.DescendantsOrSelf(depthFirst: true).ToArray();

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
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

        [Test]
        public void IHasDescendentNodes_DescendantsOrSelfLevel2AreChildren_on_DescendantsOrSelf()
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

            var descendantsOrSelf = this.rootNode.Object.DescendantsOrSelf(maxDepth: 2).Skip(1).ToArray();
            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            CollectionAssert.AreEqual(children, descendantsOrSelf);

            this.rootNode.Verify(n => n.GetDescendants(false, 1), Times.Once());
            this.rootNode.Verify(n => n.HasChildNodes, Times.Once());
            this.rootNode.Verify(n => n.ChildNodes, Times.Once());

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