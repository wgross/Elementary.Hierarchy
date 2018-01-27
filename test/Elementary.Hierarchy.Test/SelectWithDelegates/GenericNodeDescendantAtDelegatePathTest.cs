using System;
using System.Collections.Generic;
using System.Linq;
using Elementary.Hierarchy.Generic;
using Xunit;

namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    public class GenericNodeDescendantAtDelegatePathTest
    {
        [Fact]
        public void D_returns_child_on_DescendantAt()
        {
            // ACT
            // provide a child selector and retrieve the child

            var result = "rootNode".DescendantAt(getChildNodes: DelegateTreeDefinition.GetChildNodes, path: ns => (true, ns.First()));

            // ASSERT
            // node was found

            Assert.Equal("leftNode", result);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAt()
        {
            // ACT
            // without a path the node itself is returned

            var result = "rootNode".DescendantAt(DelegateTreeDefinition.GetChildNodes);

            // ASSERT
            // node was found

            Assert.Equal("rootNode", result);
        }

        [Fact]
        public void D_returns_grandChild_on_DescendantAt()
        {
            // ACT
            // provide a child selector and retrieve the child
            
            var result = "rootNode".DescendantAt(DelegateTreeDefinition.GetChildNodes, (c => (true, c.Last())), (c => (true, c.First())));

            // ASSERT
            // node was found

            Assert.Equal("leftRightLeaf", result);
        }

        [Fact]
        public void D_throws_on_invalid_childId_on_DescendantAt()
        {
            // ACT
            // search for node which doesn't exist

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() =>
            {
                "startNode".DescendantAt(DelegateTreeDefinition.GetChildNodes, (c => (false, null)));
            });

            // ASSERT
            // exception was thrown

            Assert.True(result.Message.Contains("Key not found"));
        }
    }
}