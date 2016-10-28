namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class HasParentNodeParentTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = null;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Test]
        public void Root_node_throws_InvalidOperationExption_on_Parent()
        {
            // ARRANGE

            this.startNode.SetupGet(s => s.HasParentNode).Returns(false);

            // ACT

            var result = Assert.Throws<InvalidOperationException>(()=>this.startNode.Object.Parent());

            // ASSERT

            Assert.IsTrue(result.Message.Contains("startNode has no parent"));

            this.startNode.Verify(s => s.HasParentNode, Times.Once());
            this.startNode.Verify(s => s.ParentNode, Times.Never());
        }

        [Test]
        public void Inner_node_returns_parent_for_Parent()
        {
            // ARRANGE

            var rootNode = new Mock<MockableNodeType>();
            rootNode.Setup(r => r.HasParentNode).Returns(false);

            var rootNodeObject = rootNode.Object;

            var parentOfStartNode = new Mock<MockableNodeType>();
            parentOfStartNode.SetupGet(p => p.HasParentNode).Returns(true);
            parentOfStartNode.SetupGet(p => p.ParentNode).Returns(rootNodeObject);

            var parentOfStartNodeObject = parentOfStartNode.Object;

            this.startNode.SetupGet(s => s.HasParentNode).Returns(true);
            this.startNode.SetupGet(s => s.ParentNode).Returns(parentOfStartNodeObject);

            // ACT

            MockableNodeType result = this.startNode.Object.Parent();

            // ASSERT

            Assert.AreSame(parentOfStartNode.Object, result);
        }
    }
}