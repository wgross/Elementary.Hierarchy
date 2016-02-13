namespace Elementary.Hierarchy.Test
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class GenericNodeDescendantAtTest
    {
        [Test]
        public void GetSubNodeWithDescendantAt_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
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
        public void DescendantAtRootPathGetsStartNode_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });
        }

        [Test]
        public void GetDescendantAtTwoLevelUnderStartNode_GN()
        {
            // ARRANGE

            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
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
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DescendantAtFailsIfSubNodeDoesntExist_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            "startNode".DescendantAt(nodeHierarchy, HierarchyPath.Create("childNode"));
        }

        [Test]
        public void DescendantAtOrDefaultGetsExistingSubnode_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            // ASSERT

            Assert.AreEqual("childNode", result);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsChildNode_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    if (key == "childNode")
                    {
                        childNode = "childNode";
                        return true;
                    }
                    return false;
                }
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual("childNode", result);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsNullIfSubnodeDoesntExist_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            // ASSERT

            Assert.IsNull(result);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsParentPathIfSubnodeDoesntExist_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT
            HierarchyPath<string> foundPath;
            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundPath);

            // ASSERT

            Assert.IsNull(result);
            Assert.AreEqual(HierarchyPath.Create<string>(), foundPath);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsRootNode_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    if (key == "childNode")
                    {
                        childNode = "childNode";
                        return true;
                    }
                    return false;
                }
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create<string>());

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual("startNode", result);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsNullAndRootPathIfSubNodeDoesntExist_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            // ASSERT

            Assert.IsNull(result);
        }

        [Test]
        public void DescendantAtOrDefaultReturnsNullIfSubNodeDoesntExist_GN()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNodeDelegate<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            // ASSERT

            Assert.IsNull(result);
        }
    }
}