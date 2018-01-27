using System.Collections.Generic;
using System.Linq;
using Elementary.Hierarchy.Generic;
using Xunit;

namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    public class GenericNodeDescendantAtOrDefaultDelegatePathTest
    {
        [Fact]
        public void D_returns_child_DescendantAtOrDefault()
        {
            // ACT
            // provide a child selector and retrieve the child

            var result = "rootNode".DescendantAtOrDefault(getChildNodes: DelegateTreeDefinition.GetChildNodes, createDefault: () => "default", path: ns => (true, ns.First()));

            // ASSERT
            // node was found

            Assert.Equal("leftNode", result);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAtOrDefault()
        {
            // ACT
            // without a path the node itself is returned

            var result = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.GetChildNodes, createDefault: () => "default");

            // ASSERT
            // node was found

            Assert.Equal("rootNode", result);
        }

        [Fact]
        public void D_returns_grandChild_on_DescendantAtOrDefault()
        {
            // ACT
            // provide a child selector and retrieve the child

            var result = "rootNode".DescendantAtOrDefault(DelegateTreeDefinition.GetChildNodes, () => "default", (c => (true, c.Last())), (c => (true, c.First())));

            // ASSERT
            // node was found

            Assert.Equal("leftRightLeaf", result);
        }

        [Fact]
        public void D_returns_null_on_invalid_childId_on_DescendantOrDefault()
        {
            // ACT
            // provide a child selector and retrieve the child

            var result = "rootNode".DescendantAtOrDefault(getChildNodes: DelegateTreeDefinition.GetChildNodes, createDefault: null, path: ns => (false, "not result"));

            // ASSERT
            // node wasn't found, defauilt not supplied -> default(TNode) is used

            Assert.Null(result);
        }

        [Fact]
        public void D_returns_substitute_on_invalid_childId_on_DescendantOrDefault()
        {
            // ACT
            // provide a child selector and retrieve the child

            var result = "rootNode".DescendantAtOrDefault(getChildNodes: DelegateTreeDefinition.GetChildNodes, createDefault: () => "default", path: ns => (false, "not result"));

            // ASSERT
            // node wasn't found, default delegate was called

            Assert.Equal("default", result);
        }
    }
}