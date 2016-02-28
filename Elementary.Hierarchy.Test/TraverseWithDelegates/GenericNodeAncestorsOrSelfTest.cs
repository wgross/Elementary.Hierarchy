namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeAncestorsOrSelfTest
    {
        [Test]
        public void D_root_returns_itself_on_AncestorsOrSelf()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                parent = null;
                return false;
            };

            // ACT

            IEnumerable<string> result = "startNode".AncestorsOrSelf(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("startNode", result.ElementAt(0));
        }

        [Test]
        public void D_inner_node_returns_path_to_root_on_AncestorsOrSefl()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                switch (node)
                {
                    case "startNode":
                        parent = "parentOfStartNode";
                        return true;

                    case "parentOfStartNode":
                        parent = "rootNode";
                        return true;
                }
                parent = null;
                return false;
            };

            // ACT

            IEnumerable<string> result = "startNode".AncestorsOrSelf(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("startNode", result.ElementAt(0));
            Assert.AreEqual("parentOfStartNode", result.ElementAt(1));
            Assert.AreEqual("rootNode", result.ElementAt(2));
        }
    }
}