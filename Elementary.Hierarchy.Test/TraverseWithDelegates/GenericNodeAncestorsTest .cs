namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeAncestorsTest
    {
        [Test]
        public void D_root_node_returns_empty_collection_on_Ancestors()
        {
            // ARRANGE

            Func<string, string> nodeHierarchy = node =>
            {
                return null;
            };

            // ACT

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void D_inner_node_returns_path_to_root_on_Ancestors()
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

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("parentOfStartNode", result.ElementAt(0));
            Assert.AreEqual("rootNode", result.ElementAt(1));
        }
    }
}