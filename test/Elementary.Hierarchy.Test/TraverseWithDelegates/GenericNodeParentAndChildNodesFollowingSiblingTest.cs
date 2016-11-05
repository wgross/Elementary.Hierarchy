using Elementary.Hierarchy.Generic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    [TestFixture]
    public class GenericNodeParentAndChildNodesFollowingSiblingTest
    {
        private IEnumerable<string> GetChildNodes(string rootNode)
        {
            switch (rootNode)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return Enumerable.Empty<string>();

                case "rightNode":
                    return new[] { "rightLeaf1", "rightLeaf2", "rightLeaf3" };
            }
            throw new InvalidOperationException("Shouldn't be reached");
        }

        private bool TryGetParent(string startNode, out string parentNode)
        {
            switch (startNode)
            {
                case "rootNode":
                    break;

                case "leftNode":
                case "rightNode":
                    parentNode = "rootNode";
                    return true;

                case "rightLeaf1":
                case "rightLeaf2":
                case "rightLeaf3":
                    parentNode = "rightNode";
                    return true;
            }
            parentNode = null;
            return false;
        }

        [Test]
        public void D_root_node_has_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rootNode".FollowingSiblings(this.TryGetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_node_returns_right_sibling_on_FollowingSiblings()
        {
            // ACT

            string[] result = "leftNode".FollowingSiblings(this.TryGetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreSame("rightNode", result.Single());
        }

        [Test]
        public void D_node_returns_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rightNode".FollowingSiblings(this.TryGetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void D_node_returns_all_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rightLeaf1".FollowingSiblings(this.TryGetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreSame("rightLeaf2", result.ElementAt(0));
            Assert.AreSame("rightLeaf3", result.ElementAt(1));
        }
    }
}