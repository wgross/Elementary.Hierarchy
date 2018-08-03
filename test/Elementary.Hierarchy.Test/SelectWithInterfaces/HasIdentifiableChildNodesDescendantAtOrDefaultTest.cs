using Moq;
using Xunit;

namespace Elementary.Hierarchy.Test.SelectWithInterfaces
{
    public class HasIdentifiableChildNodesDescendantAtOrDefaultTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<int, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        public HasIdentifiableChildNodesDescendantAtOrDefaultTest()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Fact]
        public void I_root_returns_child_on_DescendantAtOrDefault()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((true, childNode));

            // ACT

            MockableNodeType result1 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            HierarchyPath<int> foundNodePath;
            MockableNodeType result2 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundNodePath);

            // ASSERT

            Assert.NotNull(result1);
            Assert.Same(childNode, result1);
            Assert.Same(childNode, result2);
            Assert.Equal(HierarchyPath.Create(1), foundNodePath);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Exactly(2));
        }

        [Fact]
        public void I_root_returns_itself_on_DescendantAtOrDefault()
        {
            // ACT

            MockableNodeType result1 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create<int>());

            HierarchyPath<int> foundNodePath;
            MockableNodeType result2 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create<int>(), out foundNodePath);

            // ASSERT

            Assert.Same(startNode.Object, result1);
            Assert.Same(startNode.Object, result2);
            Assert.Equal(HierarchyPath.Create<int>(), foundNodePath);

            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Never());
        }

        [Fact]
        public void I_root_returns_grandchild_on_DescendantOrDefault()
        {
            // ARRANGE

            MockableNodeType grandChildNode = new Mock<MockableNodeType>().Object;

            var childNode = new Mock<MockableNodeType>();
            childNode
                .Setup(n => n.TryGetChildNode(2))
                .Returns((true, grandChildNode));

            var childNodeObject = childNode.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((true, childNodeObject));

            // ACT

            MockableNodeType result1 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2));

            HierarchyPath<int> foundNodePath;
            MockableNodeType result2 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1, 2), out foundNodePath);

            // ASSERT

            Assert.NotNull(grandChildNode);
            Assert.Same(grandChildNode, result1);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Exactly(2));
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Exactly(2));
        }

        [Fact]
        public void I_root_returns_null_on_invalid_childId_on_DescendantOrDefault()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((false, childNode));

            // ACT

            MockableNodeType result1 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1));

            HierarchyPath<int> foundNodePath;
            MockableNodeType result2 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundNodePath);

            // ASSERT

            Assert.Null(result1);
            Assert.Null(result2);
            Assert.Equal(HierarchyPath.Create<int>(), foundNodePath);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Exactly(2));
        }

        [Fact]
        public void I_root_returns_substitute_on_invalid_childId_on_DescendantOrDefault()
        {
            // ARRANGE

            MockableNodeType childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(1))
                .Returns((false, childNode));

            MockableNodeType substitute = new Mock<MockableNodeType>().Object;

            // ACT

            MockableNodeType result1 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), createDefault: () => substitute);

            HierarchyPath<int> foundNodePath;
            MockableNodeType result2 = this.startNode.Object.DescendantAtOrDefault(HierarchyPath.Create(1), out foundNodePath, createDefault: () => substitute);

            // ASSERT

            Assert.Same(substitute, result1);
            Assert.Same(substitute, result2);
            Assert.Equal(HierarchyPath.Create<int>(), foundNodePath);
            this.startNode.Verify(n => n.TryGetChildNode(1), Times.Exactly(2));
        }
    }
}