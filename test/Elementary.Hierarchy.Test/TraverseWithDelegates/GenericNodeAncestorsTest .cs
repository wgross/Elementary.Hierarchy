namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeAncestorsTest
    {
        [Fact]
        public void D_root_node_returns_empty_collection_on_Ancestors()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                parent = null;
                return false;
            };

            // ACT

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void D_inner_node_returns_path_to_root_on_Ancestors()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                switch (node)
                {
                    case "startNode":
                        parent = "parentOfStartNode";
                        return true;

                    case "parentOfStartNode":
                        parent = "rootNode";
                        return true;
                }
                parent = null;
                return false;
            };

            // ACT

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal("parentOfStartNode", result.ElementAt(0));
            Assert.Equal("rootNode", result.ElementAt(1));
        }
    }
}