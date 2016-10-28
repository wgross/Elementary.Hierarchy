namespace Elementary.Hierarchy.Test.TraverseUsingInterfaces
{
    using Moq;
    using NSubstitute;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class HasParentAndChildNodesFollowingSiblingTest
    {
        public interface MockableNodeType : IHasChildNodes<MockableNodeType>, IHasParentNode<MockableNodeType>
        { }

        private MockableNodeType rootNode;
        private MockableNodeType leftNode;
        private MockableNodeType rightNode;

        private MockableNodeType rightLeaf1;
        private MockableNodeType rightLeaf2;
        private MockableNodeType rightLeaf3;

        [SetUp]
        public void ArrangeAllTests()
        {
            //                rootNode
            //              /         \
            //        leftNode         rightNode
            //                         /     \               \
            //                rightLeaf1  rightRightLeaf2   rightLeaf3

            this.rightLeaf1 = Substitute.For<MockableNodeType>();
            this.rightLeaf1.HasChildNodes.Returns(false);
            this.rightLeaf1.HasParentNode.Returns(true);

            this.rightLeaf2 = Substitute.For<MockableNodeType>();
            this.rightLeaf2.HasChildNodes.Returns(false);
            this.rightLeaf2.HasParentNode.Returns(true);

            this.rightLeaf3 = Substitute.For<MockableNodeType>();
            this.rightLeaf3.HasChildNodes.Returns(false);
            this.rightLeaf3.HasParentNode.Returns(true);

            this.leftNode = Substitute.For<MockableNodeType>();
            this.leftNode.HasParentNode.Returns(true);
            this.leftNode.HasChildNodes.Returns(false);

            this.rightNode = Substitute.For<MockableNodeType>();
            this.rightNode.HasParentNode.Returns(true);
            this.rightNode.HasChildNodes.Returns(true);
            this.rightNode.ChildNodes.Returns(new[] { this.rightLeaf1, this.rightLeaf2, this.rightLeaf3 });
            this.rightLeaf1.ParentNode.Returns(this.rightNode);
            this.rightLeaf2.ParentNode.Returns(this.rightNode);
            this.rightLeaf3.ParentNode.Returns(this.rightNode);

            this.rootNode = Substitute.For<MockableNodeType>();
            this.rootNode.HasChildNodes.Returns(true);
            this.rootNode.ChildNodes.Returns(new[] { this.leftNode, this.rightNode });
            this.rightNode.ParentNode.Returns(this.rootNode);
            this.leftNode.ParentNode.Returns(this.rootNode);
        }

        [Test]
        public void Root_node_has_no_following_siblings()
        {
            // ACT

            MockableNodeType[] result = this.rootNode.FollowingSiblings().ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Return_single_sibling_of_root_child()
        {
            // ACT

            MockableNodeType[] result = this.leftNode.FollowingSiblings().ToArray();

            // ASSERT
            Assert.AreSame(this.rootNode, this.leftNode.Parent());
            Assert.AreSame(this.leftNode, this.rootNode.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightNode, this.rootNode.ChildNodes.ElementAt(1));
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(this.rightNode, result.Single());
        }

        [Test]
        public void Return_empty_siblings_for_inner_node_for_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightNode.FollowingSiblings().ToArray();

            // ASSERT

            Assert.AreSame(this.rootNode, this.leftNode.Parent());
            Assert.AreSame(this.leftNode, this.rootNode.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightNode, this.rootNode.ChildNodes.ElementAt(1));
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void Return_multiple_siblings_for_inner_node_on_FollowingSiblings()
        {
            // ACT

            MockableNodeType[] result = this.rightLeaf1.FollowingSiblings().ToArray();

            // ASSERT

            Assert.AreSame(this.rightNode, this.rightLeaf1.Parent());
            Assert.AreSame(this.rightLeaf1, this.rightNode.ChildNodes.ElementAt(0));
            Assert.AreSame(this.rightLeaf2, this.rightNode.ChildNodes.ElementAt(1));
            Assert.AreSame(this.rightLeaf3, this.rightNode.ChildNodes.ElementAt(2));
            Assert.AreEqual(2, result.Count());
            Assert.AreSame(this.rightLeaf2, result.ElementAt(0));
            Assert.AreSame(this.rightLeaf3, result.ElementAt(1));
        }
    }
}