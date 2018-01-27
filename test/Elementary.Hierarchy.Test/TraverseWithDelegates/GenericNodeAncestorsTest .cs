namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeAncestorsTest
    {
        [Fact]
        public void D_returns_empty_collection_on_Ancestors()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Ancestors(DelegateTreeDefinition.TryGetParentNode).ToArray();

            // ASSERT

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void D_inner_node_returns_path_to_root_on_Ancestors()
        {
            // ACT

            IEnumerable<string> result = "leftLeaf".Ancestors(DelegateTreeDefinition.TryGetParentNode).ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal("leftNode", result.ElementAt(0));
            Assert.Equal("rootNode", result.ElementAt(1));
        }
    }
}