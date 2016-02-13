﻿namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasChildNodesVisitVisitDescendantsTest
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

        #region VisitDescendants

        [Test]
        public void VisitDescandantsBreadthFirst()
        {
            // ACT

            var result = new List<Tuple<List<MockableNodeType>, MockableNodeType>>();

            this.rootNode.Object.VisitDescendants((bc, n) => result.Add(Tuple.Create(bc.ToList(), n)));

            // ASSERT

            // Check list of visited nodes
            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                this.leftNode,
                this.rightNode,
                this.leftLeaf,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result.Select(i => i.Item2));

            // check presented breadcrumbs
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(4).Item1);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Test]
        public void VisitDescandantsDepthFirst()
        {
            // ACT

            var result = new List<Tuple<List<MockableNodeType>, MockableNodeType>>();

            this.rootNode.Object.VisitDescendants((bc, n) => result.Add(Tuple.Create(bc.ToList(), n)), depthFirst: true);

            // ASSERT

            // check visited nodes
            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] {
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result.Select(i => i.Item2));

            // check presented breadcrumbs
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(4).Item1);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        #endregion VisitDescendants

        #region DescendantsOrSelf

        [Test]
        public void VisitDescendantsOrSelfBreadthFirst()
        {
            // ACT

            var result = new List<Tuple<List<MockableNodeType>, MockableNodeType>>();

            this.rootNode.Object.VisitDescendantsOrSelf((bc, n) => result.Add(Tuple.Create(bc.ToList(), n)));

            // ASSERT

            // Check list of visited nodes
            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                this.rootNode,
                this.leftNode,
                this.rightNode,
                this.leftLeaf,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result.Select(i => i.Item2));

            // check presented breadcrumbs
            CollectionAssert.AreEqual(new MockableNodeType[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(4).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(5).Item1);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        [Test]
        public void VisitDescandantsOrSelfDepthFirst()
        {
            // ACT

            var result = new List<Tuple<List<MockableNodeType>, MockableNodeType>>();

            this.rootNode.Object.VisitDescendantsOrSelf((bc, n) => result.Add(Tuple.Create(bc.ToList(), n)), depthFirst: true);

            // ASSERT

            // check visited nodes
            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
                this.rootNode,
                this.leftNode,
                this.leftLeaf,
                this.rightNode,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result.Select(i => i.Item2));

            // check presented breadcrumbs
            CollectionAssert.AreEqual(new MockableNodeType[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.leftNode.Object }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(4).Item1);
            CollectionAssert.AreEqual(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(5).Item1);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }

        #endregion DescendantsOrSelf
    }
}