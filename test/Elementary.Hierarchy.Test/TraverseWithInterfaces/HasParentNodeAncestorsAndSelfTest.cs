namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasParentAncestorsAndSelfTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode;

        public HasParentAncestorsAndSelfTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_returns_itself_on_AncestorsAndSelf()
        {
            // ARRANGE

            startNode // returns false for 'HasParentNode'
                .Setup(m => m.HasParentNode).Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.Object.AncestorsAndSelf().ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same(startNode.Object, result.ElementAt(0));

            startNode.Verify(m => m.HasParentNode, Times.Once());
            startNode.Verify(m => m.ParentNode, Times.Never());
        }

        [Fact]
        public void I_inner_node_returns_path_to_root_on_AncestorsOrSefl()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();

            rootNode // has no parent
                .Setup(r => r.HasParentNode).Returns(false);

            var parentOfStartNode = new Mock<MockableNodeType>();

            parentOfStartNode // has root node as parent
                .Setup(p => p.HasParentNode).Returns(true);
            parentOfStartNode
                .Setup(p => p.ParentNode).Returns(rootNode.Object);

            this.startNode // has a parant
                .Setup(m => m.HasParentNode).Returns(true);
            this.startNode // returns parent node
                .Setup(m => m.ParentNode).Returns(parentOfStartNode.Object);

            // ACT

            IEnumerable<MockableNodeType> result = this.startNode.Object.AncestorsAndSelf().ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Same(this.startNode.Object, result.ElementAt(0));
            Assert.Same(parentOfStartNode.Object, result.ElementAt(1));
            Assert.Same(rootNode.Object, result.ElementAt(2));
        }
    }
}