namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class GenericNodeTryGetDescendantAtTest
    {
        [Test]
        public void D_root_returns_child_on_TryGetDescendantAt()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string resultNode = null;
            bool result = "startNode".TryGetDescendantAt(nodeHierarchy, HierarchyPath.Create<string>(), out resultNode);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual("startNode", resultNode);
        }

        [Test]
        public void D_root_returns_itself_on_TryGetDescendantAt()
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

            string resultNode;
            bool result = "startNode".TryGetDescendantAt(nodeHierarchy, HierarchyPath.Create<string>(), out resultNode);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual("startNode", resultNode);
        }

        [Test]
        public void D_root_returns_grandchild_on_TryGetDescendentAt()
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

            string resultNode;
            bool result = "startNode".TryGetDescendantAt(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"), out resultNode);

            // ASSERT

            Assert.IsTrue(result);
            Assert.AreEqual("grandChildNode", resultNode);
        }

        [Test]
        public void D_root_node_throws_KeyNotFoundException_on_invalid_childId_on_TryGetDescendantAt()
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

            string resultNode;
            bool result = "startNode".TryGetDescendantAt(nodeHierarchy, HierarchyPath.Create("childNode"), out resultNode);

            // ASSERT

            Assert.IsFalse(result);
        }
    }
}