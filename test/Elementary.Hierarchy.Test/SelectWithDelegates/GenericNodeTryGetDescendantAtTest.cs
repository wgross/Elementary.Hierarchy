namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using Xunit;

    public class GenericNodeTryGetDescendantAtTest
    {
        [Fact]
        public void D_returns_child_on_TryGetDescendantAt()
        {
            // ARRANGE

            var nodeHierarchy = (Func<string, string, (bool, string)>)(delegate (string node, string key)
              {
                  throw new InvalidOperationException("unknown node");
              });

            // ACT

            var (result_found, result_node) = "rootNode".TryGetDescendantAt(nodeHierarchy, HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(result_found);
            Assert.Equal("rootNode", result_node);
        }

        [Fact]
        public void D_returns_itself_on_TryGetDescendantAt()
        {
            // ACT

            var (result_found, result_node) = "rootNode".TryGetDescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create<string>());

            // ASSERT

            Assert.True(result_found);
            Assert.Equal("rootNode", result_node);
        }

        [Fact]
        public void D_returns_grandchild_on_TryGetDescendentAt()
        {
            // ACT

            var (result_found, result_node) = "rootNode".TryGetDescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode", "leftLeaf"));

            // ASSERT

            Assert.True(result_found);
            Assert.Equal("leftLeaf", result_node);
        }

        [Fact]
        public void D_node_throws_KeyNotFoundException_on_invalid_childId_on_TryGetDescendantAt()
        {
            // ACT

            var (result_found, result_node) = "rootNode".TryGetDescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unknownNode"));

            // ASSERT

            Assert.False(result_found);
        }
    }
}