﻿using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HasChildNodesVisitDescendantsExTest
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

        public HasChildNodesVisitDescendantsExTest()
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
        public void I_visit_complete_tree_breadthFirst_on_VisitDescendants()
        {
            // ACT

            var result = new List<Tuple<List<MockableNodeType>, MockableNodeType>>();

            this.rootNode.Object.VisitDescendants((bc, n) => result.Add(Tuple.Create(bc.ToList(), n)));

            // ASSERT

            // Check list of visited nodes
            Assert.Equal(5, result.Count());
            Assert.Equal(new[]
            {
                this.leftNode,
                this.rightNode,
                this.leftLeaf,
                this.leftRightLeaf,
                this.rightRightLeaf
            }.Select(n => n.Object), result.Select(i => i.Item2));

            // check presented breadcrumbs
            Assert.Equal(new[] { this.rootNode.Object }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { this.rootNode.Object }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { this.rootNode.Object, this.leftNode.Object }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { this.rootNode.Object, this.rightNode.Object }, result.ElementAt(4).Item1);

            this.rootNode.VerifyAll();
            this.leftNode.VerifyAll();
            this.rightNode.VerifyAll();
            this.leftLeaf.VerifyAll();
            this.leftRightLeaf.VerifyAll();
            this.rightRightLeaf.VerifyAll();
        }
    }
}