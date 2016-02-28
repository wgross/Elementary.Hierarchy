namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeParentTest
    {
        [Test]
        public void D_root_returns_null_on_Parent()
        {
            // ARRANGE

            TryGetParent<string> nodeHierarchy = (string node, out string parent) =>
            {
                parent = null;
                return false;
            };

            // ACT

            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => "startNode".Parent(nodeHierarchy));

            // ASSERT

            Assert.IsTrue(result.Message.Contains("has no parent"));
        }

        [Test]
        public void D_inner_node_returns_Parent()
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

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("parentOfStartNode", result.ElementAt(0));
            Assert.AreEqual("rootNode", result.ElementAt(1));
        }
    }
}