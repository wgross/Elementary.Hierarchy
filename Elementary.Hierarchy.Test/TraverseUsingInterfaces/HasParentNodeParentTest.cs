namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HasParentNodeParentTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private MockableNodeType startNode = null;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = Substitute.For<MockableNodeType>();
        }

        [Test]
        public void Root_node_resturns_null_for_Parent()
        {
            // ARRANGE

            startNode.HasParentNode.Returns(false);

            // ACT

            MockableNodeType result = startNode.Parent();

            // ASSERT

            Assert.AreEqual(null, result);

            var temp = startNode.Received(1).HasParentNode;
            var t2 = startNode.DidNotReceive().ParentNode;
        }

        [Test]
        public void Inner_node_returns_parent_for_Parent()
        {
            // ARRANGE

            var rootNode = Substitute.For<MockableNodeType>();
            rootNode.HasParentNode.Returns(false);

            var parentOfStartNode = Substitute.For<MockableNodeType>();
            parentOfStartNode.HasParentNode.Returns(true);
            parentOfStartNode.ParentNode.Returns(rootNode);

            this.startNode.HasParentNode.Returns(true);
            this.startNode.ParentNode.Returns(parentOfStartNode);

            // ACT

            MockableNodeType result = this.startNode.Parent();

            // ASSERT

            Assert.AreSame(parentOfStartNode, result);
        }
    }
}