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
        public void D_root_throws_InvalidOperationException_on_Parent()
        {
            // ACT
            // ask for parent

            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => "rootNode".Parent(DelegateTreeDefinition.TryGetParentNode));

            // ASSERT

            Assert.Contains("has no parent", result.Message);
        }

        [Fact]
        public void D_inner_node_returns_Parent()
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