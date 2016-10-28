namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces.TraverseUsingInterfaces
{
    using Moq;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasParentNodeAncestorsTest
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
        public void Root_returns_null_for_Ancestors()
        {
            // ARRANGE

            startNode.HasParentNode.Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());

            var t1 = this.startNode.Received(1).HasParentNode;
            var t2 = this.startNode.DidNotReceive().ParentNode;
        }

        [Test]
        public void Inner_node_returns_path_to_root_node()
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

            IEnumerable<MockableNodeType> result = this.startNode.Ancestors().ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreSame(parentOfStartNode, result.ElementAt(0));
            Assert.AreSame(rootNode, result.ElementAt(1));
        }
    }
}