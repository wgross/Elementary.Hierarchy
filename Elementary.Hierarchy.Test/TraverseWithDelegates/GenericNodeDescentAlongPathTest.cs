namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    public class GenericNodeDescentAlongPathTest
    {
        [Test]
        public void D_root_returns_nothing_empty_path_on_DescentAlongPath()
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

            string[] result = "startNode".DescentAlongPath(nodeHierarchy, HierarchyPath.Create<string>()).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_root_returns_child_on_DescentAlongPath()
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

            string[] result = "startNode".DescentAlongPath(nodeHierarchy, HierarchyPath.Create<string>("childNode")).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new[] { "childNode" }, result);
        }

        [Test]
        public void D_root_returns_child_and_grandchild_on_DescentAlongPath()
        {
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

            string[] result = "startNode".DescentAlongPath(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode")).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { "childNode", "grandChildNode" }, result);
        }

        [Test]
        public void D_root_return_incomplete_list_on_DescentAlongPath()
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

            string[] result = "startNode".DescentAlongPath(nodeHierarchy, HierarchyPath.Create("childNode")).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }
    }
}