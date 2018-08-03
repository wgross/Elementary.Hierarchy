using Elementary.Hierarchy.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
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

        [Fact]
        public void D_root_node_has_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rootNode".FollowingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void D_node_returns_right_sibling_on_FollowingSiblings()
        {
            // ACT

            string[] result = "leftNode".FollowingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same("rightNode", result.Single());
        }

        [Fact]
        public void D_node_returns_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rightNode".FollowingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void D_node_returns_all_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "leftRightLeaf".FollowingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Same("rightRightLeaf", result.ElementAt(0));
        }
    }
}