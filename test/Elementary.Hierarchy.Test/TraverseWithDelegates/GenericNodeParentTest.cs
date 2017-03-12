namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeParentTest
    {
        [Fact]
        public void D_root_throws_InvalidOperationException__on_Parent()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                parent = null;
                return false;
            };

            // ACT

            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => "startNode".Parent(nodeHierarchy));

            // ASSERT

            Assert.True(result.Message.Contains("has no parent"));
        }

        [Fact]
        public void D_inner_node_returns_Parent()
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