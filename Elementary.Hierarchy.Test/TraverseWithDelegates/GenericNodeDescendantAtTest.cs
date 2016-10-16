namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class GenericNodeDescendantAtTest
    {
        #region DescendantAt

        [Test]
        public void D_root_returns_child_on_DescendantAt()
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

            Assert.AreEqual("startNode", result);
        }

        [Test]
        public void D_root_returns_itself_on_DescendantAt()
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

            Assert.AreEqual("startNode", result);
        }

        [Test]
        public void D_root_returns_grandchild_on_DescendentAt()
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

            Assert.AreSame("grandChildNode", result);
        }

        [Test]
        public void D_root_node_throws_on_invalid_childId_on_DescendantAt()
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

            Assert.IsTrue(result.Message.Contains("Key not found:'childNode'"));
        }

        #endregion DescendantAt

        
    }
}