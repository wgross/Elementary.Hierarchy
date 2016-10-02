namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class HasParentAncestorsAndSelfTest
    {
        public interface MockableNodeType : IHasParentNode<MockableNodeType>
        { }

        private MockableNodeType startNode;

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = Substitute.For<MockableNodeType>();
        }

        [Test]
        public void Root_returns_self_for_AncestorsOrSelf()
        {
            // ARRANGE

            startNode.HasParentNode.Returns(false);

            // ACT

            IEnumerable<MockableNodeType> result = startNode.AncestorsOrSelf().ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(startNode, result.ElementAt(0));

            var tmp1 = startNode.Received(1).HasParentNode;
            var tmp2 = startNode.DidNotReceive().ParentNode;
        }

        [Test]
        public void Inner_node_returns_path_to_parent_and_self_for_AncestorsOrSelf()
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

            IEnumerable<MockableNodeType> result = this.startNode.AncestorsOrSelf().ToArray();

            // ASSERT

            Assert.AreEqual(3, result.Count());
            Assert.AreSame(this.startNode, result.ElementAt(0));
            Assert.AreSame(parentOfStartNode, result.ElementAt(1));
            Assert.AreSame(rootNode, result.ElementAt(2));
        }
    }
}