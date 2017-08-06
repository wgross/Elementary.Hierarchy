using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithInterfaces
{
    public class HasDescandantNodesLeavesTest
    {
        public interface MockableNodeType : IHasDescendantNodes<MockableNodeType>
        { }

        private readonly Mock<MockableNodeType> rootNode;
        private readonly Mock<MockableNodeType> leftNode;

        public HasDescandantNodesLeavesTest()
        {
            //                rootNode
            //                /     
            //        leftNode 

            this.leftNode = new Mock<MockableNodeType>();
            this.leftNode
                .Setup(n => n.GetDescendants(It.IsAny<bool>(),It.IsAny<int>()))
                .Returns((IEnumerable<MockableNodeType>)null);
            this.leftNode
                .Setup(n => n.HasChildNodes)
                .Returns(false);
            this.rootNode = new Mock<MockableNodeType>();
            this.rootNode
                .Setup(n => n.GetDescendants(true, int.MaxValue - 1))
                .Returns(new[] { this.leftNode.Object });
            this.rootNode
                .Setup(n => n.HasChildNodes)
                .Returns(true);
        }

        [Fact]
        public void IHasDescendantNodes_calls_GetDescendantNodes_with_unlimited_depth_on_Leaves()
        {
            // ACT
            // Retrieves Leaves from a node which implements IHasDescandantNodes

            var result = this.rootNode.Object.Leaves().ToArray();

            // ASSERT
            // The hierarchy is retrieved completely. This is not efficient but provides the correct result

            this.rootNode.Verify(r => r.GetDescendants(true, 2147483646), Times.Once());
            this.rootNode.Verify(r => r.HasChildNodes, Times.Once());
            this.rootNode.Verify(r => r.ChildNodes, Times.Never());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Once());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            Assert.Equal(new[] { this.leftNode.Object }, result);
        }

        [Fact]
        public void IHasDescendantNodes_returns_itself_if_no_nodes_cant_be_retrieved_on_Leaves()
        {
            // ACT
            // Retrieves Leaves from a node which implements IHasDescandantNodes

            var result = this.leftNode.Object.Leaves().ToArray();

            // ASSERT
            // The hierarchy is retrieved completely. This is not efficient but provides the correct result

            this.leftNode.Verify(r => r.GetDescendants(true, 2147483646), Times.Once());
            this.leftNode.Verify(n => n.HasChildNodes, Times.Once());
            this.leftNode.Verify(n => n.ChildNodes, Times.Never());

            Assert.Equal(this.leftNode.Object, result.Single());
        }
    }
}