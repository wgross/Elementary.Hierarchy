namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using Xunit;

    public class GenericNodeDescendantAtTest
    {
        #region DescendantAt

        [Fact]
        public void D_returns_child_on_DescendantAt()
        {
            // ACT

            string result = "rootNode".DescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal("rootNode", result);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAt()
        {
            // ACT

            string result = "rootNode".DescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal("rootNode", result);
        }

        [Fact]
        public void D_returns_grandchild_on_DescendentAt()
        {
            // ACT

            string result = "rootNode".DescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("leftNode", "leftLeaf"));

            // ASSERT

            Assert.Same("leftLeaf", result);
        }

        [Fact]
        public void D_throws_on_invalid_childId_on_DescendantAt()
        {
            // ACT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => { "rootNode".DescendantAt(DelegateTreeDefinition.TryGetChildNode, HierarchyPath.Create("unkownNode")); });

            // ASSERT

            Assert.True(result.Message.Contains("Key not found:'unkownNode'"));
        }

        #endregion DescendantAt
    }
}