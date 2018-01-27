namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeAncestorsAndSelfTest
    {
        [Fact]
        public void D_returns_itself_on_AncestorsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "rootNode".AncestorsAndSelf(DelegateTreeDefinition.TryGetParentNode).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("rootNode", result.ElementAt(0));
        }

        [Fact]
        public void D_inner_node_returns_path_to_root_on_AncestorsOrSefl()
        {
            // ACT

            IEnumerable<string> result = "leftLeaf".AncestorsAndSelf(DelegateTreeDefinition.TryGetParentNode).ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal("leftLeaf", result.ElementAt(0));
            Assert.Equal("leftNode", result.ElementAt(1));
            Assert.Equal("rootNode", result.ElementAt(2));
        }
    }
}