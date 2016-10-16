using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elementary.Hierarchy.Generic;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodeDescendantAtOrDefaultTest
    {
        [Test]
        public void D_root_returns_child_on_DescendantAtOrDefault()
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

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundNodePath);

            // ASSERT

            Assert.AreEqual("childNode", result1);
            Assert.AreEqual("childNode", result2);
            Assert.AreEqual(HierarchyPath.Create("childNode"), foundNodePath);
        }

        [Test]
        public void D_root_returns_itself_on_DescendantAtOrDefault()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                childNode = null;
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            HierarchyPath<string> foundNodePath;
            string result = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create<string>(), out foundNodePath);

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual("startNode", result);
            Assert.AreEqual(HierarchyPath.Create<string>(), foundNodePath);
        }

        [Test]
        public void D_root_returns_grandchild_on_DescendentAtOrDefault()
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

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode", "grandChildNode"), out foundNodePath);

            // ASSERT

            Assert.AreSame("grandChildNode", result1);
            Assert.AreSame("grandChildNode", result2);
            Assert.AreEqual(HierarchyPath.Create("childNode", "grandChildNode"), foundNodePath);
        }

        [Test]
        public void D_root_returns_null_on_invalid_childId_on_DescendantOrDefault()
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

            string result1 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"));

            HierarchyPath<string> foundNodePath;
            string result2 = "startNode".DescendantAtOrDefault(nodeHierarchy, HierarchyPath.Create("childNode"), out foundNodePath);

            // ASSERT

            Assert.IsNull(result1);
            Assert.IsNull(result2);
            Assert.AreEqual(HierarchyPath.Create<string>(), foundNodePath);
        }
    }
}
