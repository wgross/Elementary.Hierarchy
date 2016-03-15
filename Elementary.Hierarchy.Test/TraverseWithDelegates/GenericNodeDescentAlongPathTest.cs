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
        public void D_root_returns_itself_for_empty_path_on_DescendAlongPath()
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

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create<string>()).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("startNode",result.ElementAt(0));
        }

        [Test]
        public void D_root_returns_child_on_DescendAlongPath()
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

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create<string>("childNode")).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(new[] { "startNode", "childNode" }, result);
        }

        [Test]
        public void D_root_returns_child_and_grandchild_on_DescendAlongPath()
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

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode")).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { "startNode", "childNode", "grandChildNode" }, result);
        }

        [Test]
        public void D_root_return_incomplete_list_on_DescendAlongPath()
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

            string[] result = "startNode".DescendAlongPath(nodeHierarchy, HierarchyPath.Create("childNode")).ToArray();

            // ASSERT

            Assert.IsTrue(result.Any());
            CollectionAssert.AreEqual(new[] { "startNode" }, result);
        }
    }
}