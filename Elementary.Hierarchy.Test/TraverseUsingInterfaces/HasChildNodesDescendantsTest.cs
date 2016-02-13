namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasChildNodesDescendantsTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>
        { }

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

        [Test]
        public void LeafReturnsNoChildren()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightRightLeaf.Object.Descendants().ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());

            this.rightRightLeaf.Verify(n => n.HasChildNodes, Times.Once);
            this.rightRightLeaf.Verify(n => n.ChildNodes, Times.Never);
            this.rightRightLeaf.VerifyAll();
        }

        [Test]
        public void LeafReturnsNoChildrenButClaimsToHaveSubnodes()
        {
            // ARRANGE

            var badLeaf = new Mock<MockableNodeType>();
            badLeaf // claims to hav subnodes
                .Setup(n => n.HasChildNodes).Returns(true);
            badLeaf //  but returns empty set of subnodes
                .Setup(n => n.ChildNodes).Returns(Enumerable.Empty<MockableNodeType>());

            // ACT

            IEnumerable<MockableNodeType> result = badLeaf.Object.Descendants();

            // ASSERT

            Assert.AreEqual(0, result.Count());

            badLeaf.Verify(n => n.HasChildNodes, Times.Once);
            badLeaf.Verify(n => n.ChildNodes, Times.Once);
        }

        [Test]
        public void EnumerateSingleChildToLeaf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftNode.Object.Descendants().ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(this.leftLeaf.Object, result.ElementAt(0));

            this.leftNode.VerifyAll();
            this.leftLeaf.VerifyAll();
        }

        [Test]
        public void EnumerateTwoChildrenToLeaf()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rightNode.Object.Descendants();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result);

            this.rightNode.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Test]
        public void EnumerateTreeBreadthFirst()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Descendants().ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                this.leftNode.Object,
                this.rightNode.Object,
                this.leftLeaf.Object,
                this.leftRightLeaf.Object,
                this.rightRightLeaf.Object
            }, result);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Test]
        public void EnumerateTreeDepthFirst()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Descendants(depthFirst: true).ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] {
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

        [Test]
        public void EnumerateDecendantTreeToSpecifiedDepth()
        {
            // ACT

            var ex = Assert.Throws<ArgumentException>(() => this.rootNode.Object.Descendants(maxDepth: -1));
            var result0 = this.rootNode.Object.Descendants(maxDepth: 0).ToArray();
            var result1 = this.rootNode.Object.Descendants(maxDepth: 1).ToArray();
            var result2 = this.rootNode.Object.Descendants(maxDepth: 2).ToArray();
            var result3 = this.rootNode.Object.Descendants(maxDepth: 3).ToArray();

            // ASSERT

            Assert.IsTrue(ex.Message.Contains("must be > 0"));
            Assert.AreEqual("maxDepth", ex.ParamName);
            Assert.IsFalse(result0.Any());
            CollectionAssert.AreEqual(new[] { this.leftNode.Object, this.rightNode.Object }, result1);
            CollectionAssert.AreEqual(new[] { this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result2);
            CollectionAssert.AreEqual(new[] { this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result3);
        }

        [Test]
        public void EnumerateDescendantOrSelfTreeToSpecifiedDepth()
        {
            // ACT

            var ex = Assert.Throws<ArgumentException>(() => this.rootNode.Object.DescendantsOrSelf(maxDepth: -1).ToArray());
            var result0 = this.rootNode.Object.DescendantsOrSelf(maxDepth: 0).ToArray();
            var result1 = this.rootNode.Object.DescendantsOrSelf(maxDepth: 1).ToArray();
            var result2 = this.rootNode.Object.DescendantsOrSelf(maxDepth: 2).ToArray();
            var result3 = this.rootNode.Object.DescendantsOrSelf(maxDepth: 3).ToArray();
            var result4 = this.rootNode.Object.DescendantsOrSelf(maxDepth: 4).ToArray();

            // ASSERT

            Assert.IsTrue(ex.Message.Contains("must be > 0"));
            Assert.AreEqual("maxDepth", ex.ParamName);
            Assert.IsFalse(result0.Any());
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object, this.rightNode.Object }, result2);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result3);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object, this.rightNode.Object, this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result4);
        }

        [Test]
        public void EnumerateTreeDepthFirstToSpecificDepth()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Descendants(depthFirst: true).ToArray();

            var ex = Assert.Throws<ArgumentException>(() => this.rootNode.Object.Descendants(depthFirst: true, maxDepth: -1));
            var result0 = this.rootNode.Object.Descendants(depthFirst: true, maxDepth: 0).ToArray();
            var result1 = this.rootNode.Object.Descendants(depthFirst: true, maxDepth: 1).ToArray();
            var result2 = this.rootNode.Object.Descendants(depthFirst: true, maxDepth: 2).ToArray();
            var result3 = this.rootNode.Object.Descendants(depthFirst: true, maxDepth: 3).ToArray();

            // ASSERT

            Assert.IsFalse(result0.Any());

            CollectionAssert.AreEqual(new[] {
                this.leftNode,
                this.rightNode,
            }.Select(n => n.Object), result1);

            CollectionAssert.AreEqual(new[] {
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result2);

            CollectionAssert.AreEqual(new[] {
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result3);
        }

        [Test]
        public void DescendantsLevel1AreChildren()
        {
            // ACT

            var descendants = this.rootNode.Object.Descendants(maxDepth: 1).ToArray();
            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT

            CollectionAssert.AreEqual(children, descendants);
        }
    }
}