namespace Elementary.Hierarchy.Test
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
        public void RootNodeReturnsItself_GN()
        {
            // ARRANGE

            Func<string, string> nodeHierarchy = node =>
            {
                return null;
            };

            // ACT

            IEnumerable<string> result = "startNode".AncestorsOrSelf(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("startNode", result.ElementAt(0));
        }

        [Test]
        public void InnerNodeReturnsPathToRootNodeIncludingRootNode_GN()
        {
            // ARRANGE

            Func<string, string> nodeHierarchy = node =>
            {
                switch (node)
                {
                    case "startNode": return "parentOfStartNode";
                    case "parentOfStartNode": return "rootNode";
                }
                return null;
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