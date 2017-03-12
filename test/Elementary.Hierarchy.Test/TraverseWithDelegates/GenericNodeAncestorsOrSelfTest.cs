namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeAncestorsOrSelfTest
    {
        [Fact]
        public void D_root_returns_itself_on_AncestorsOrSelf()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                parent = null;
                return false;
            };

            // ACT

            IEnumerable<string> result = "startNode".AncestorsOrSelf(nodeHierarchy).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("startNode", result.ElementAt(0));
        }

        [Fact]
        public void D_inner_node_returns_path_to_root_on_AncestorsOrSefl()
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

            IEnumerable<string> result = "startNode".AncestorsOrSelf(nodeHierarchy).ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal("startNode", result.ElementAt(0));
            Assert.Equal("parentOfStartNode", result.ElementAt(1));
            Assert.Equal("rootNode", result.ElementAt(2));
        }
    }
}