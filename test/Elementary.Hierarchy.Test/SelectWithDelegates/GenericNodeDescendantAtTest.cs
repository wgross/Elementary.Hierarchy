namespace Elementary.Hierarchy.Test.SelectWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class GenericNodeDescendantAtTest
    {
        #region DescendantAt

        [Fact]
        public void D_returns_child_on_DescendantAt()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAt(nodeHierarchy, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal("startNode", result);
        }

        [Fact]
        public void D_returns_itself_on_DescendantAt()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAt(nodeHierarchy, HierarchyPath.Create<string>());

            // ASSERT

            Assert.Equal("startNode", result);
        }

        [Fact]
        public void D_returns_grandchild_on_DescendentAt()
        {
            // ARRANGE

            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }
                else if (node == "childNode" && key == "grandChildNode")
                {
                    childNode = "grandChildNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAt(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"));

            // ASSERT

            Assert.Same("grandChildNode", result);
        }

        [Fact]
        public void D_throws_on_invalid_childId_on_DescendantAt()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            KeyNotFoundException result = Assert.Throws<KeyNotFoundException>(() => { "startNode".DescendantAt(nodeHierarchy, HierarchyPath.Create("childNode")); });

            // ASSERT

            Assert.True(result.Message.Contains("Key not found:'childNode'"));
        }

        #endregion DescendantAt
    }
}