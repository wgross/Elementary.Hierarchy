using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    [TestFixture]
    public class HasChildNodesLeavesTest
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
        public void I_empty_root_returns_itself_on_Leaves()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.leftLeaf.Object.Leaves().ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(this.leftLeaf.Object, result.ElementAt(0));
        }

        [Test]
        public void I_root_returns_its_descendant_leaves_on_Leaves()
        {
            // ACT

            IEnumerable<MockableNodeType> result = this.rootNode.Object.Leaves().ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(new[] { this.leftLeaf.Object, this.leftRightLeaf.Object, this.rightRightLeaf.Object }, result.ToArray());
        }
    }
}