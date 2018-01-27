namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeParentAndChildNodesPrecedingSiblingTest
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
        public void D_root_node_has_no_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rootNode".PrecedingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void D_node_returns_left_sibling_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rightNode".PrecedingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("leftNode", result.ElementAt(0));
        }

        [Fact]
        public void D_node_returns_no_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "leftNode".PrecedingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void D_node_returns_all_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rightRightLeaf".PrecedingSiblings(DelegateTreeDefinition.TryGetParentNode, DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("leftRightLeaf", result.ElementAt(0));
        }
    }
}