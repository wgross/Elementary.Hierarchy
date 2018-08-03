using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    public class HasChildNodesChildrenTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>
        { }

        private readonly Mock<MockableNodeType> rootNode;
        private readonly Mock<MockableNodeType> leftNode;
        private readonly Mock<MockableNodeType> rightNode;

        public HasChildNodesChildrenTest()
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode // has single child
                .Setup(n => n.HasChildNodes).Returns(false);

            this.rightNode = new Mock<MockableNodeType>();
            this.rightNode // has two children
                .Setup(n => n.HasChildNodes).Returns(false);

            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode // has a tw children
                .Setup(n => n.HasChildNodes).Returns(true);
            this.rootNode // returns the left node and right node as children
                .Setup(n => n.ChildNodes).Returns(new[] { this.leftNode.Object, this.rightNode.Object });
        }

        [Fact]
        public void IHasChildNodes_calls_ChildNodes_to_retrieve_children_on_Children()
        {
            // ACT

            var children = this.rootNode.Object.Children().ToArray();

            // ASSERT
            // first HasChildNodes is called, then ChildNodes

            this.rootNode.Verify(r => r.HasChildNodes, Times.Once());
            this.rootNode.Verify(r => r.ChildNodes, Times.Once());

            Assert.Equal(new[] { this.leftNode.Object, this.rightNode.Object }, children);
        }

        [Fact]
        public void IHasChildNodes_checks_HasChildNodes_before_calling_ChildNodes_on_Children()
        {
            // ACT
            // get child node from leave

            var children = this.leftNode.Object.Children().ToArray();

            // ASSERT
            // HashChildNOdes was called, but ChildNodes never

            this.leftNode.Verify(r => r.HasChildNodes, Times.Once());
            this.leftNode.Verify(r => r.ChildNodes, Times.Never());
        }

        [Fact]
        public void IHasChildNodes_converts_null_to_empty_collection_on_Children()
        {
            // ARRANGE
            // make a inconsisstent mockup

            var node = new Mock<MockableNodeType>();
            node.Setup(n => n.HasChildNodes).Returns(true);
            node.Setup(n => n.ChildNodes).Returns((IEnumerable<MockableNodeType>)null);

            // ACT
            // ask for children

            var result = node.Object.Children();

            // ASSERT
            // result isn't null but empty

            Assert.NotNull(result);
            Assert.False(result.Any());
        }
    }
}