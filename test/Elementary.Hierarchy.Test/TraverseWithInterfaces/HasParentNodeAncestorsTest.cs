namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HasParentNodeAncestorsTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasParentNodeAncestorsTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_node_returns_empty_collection_on_Ancestors()
        {
            // ARRANGE

            startNode // returns false for 'HasParentNode'
                .Setup(m => m.HasParentNode).Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.Equal(0, result.Count());

            startNode.Verify(m => m.HasParentNode, Times.Once());
            startNode.Verify(m => m.ParentNode, Times.Never());
        }

        [Fact]
        public void I_inner_node_returns_path_to_root_on_Ancestors()
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

            IEnumerable<MockableNodeType> result = this.startNode.Object.Ancestors().ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Same(parentOfStartNode.Object, result.ElementAt(0));
            Assert.Same(rootNode.Object, result.ElementAt(1));
        }
    }
}