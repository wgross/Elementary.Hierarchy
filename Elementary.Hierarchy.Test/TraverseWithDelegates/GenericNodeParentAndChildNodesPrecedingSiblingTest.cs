namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Elementary.Hierarchy.Generic;

    [TestFixture]
    public class GenericNodeParentAndChildNodesPrecedingSiblingTest
    {
        private IEnumerable<string> GetChildNodes(string rootNode)
        {
            switch (rootNode)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return Enumerable.Empty<string>();

                case "rightNode":
                    return new[] { "rightLeaf1", "rightLeaf2", "rightLeaf3" };
            }
            return Enumerable.Empty<string>();
        }

        private string GetParent(string startNode)
        {
            switch (startNode)
            {
                case "rootNode":
                    return null;

                case "leftNode":
                case "rightNode":
                    return "rootNode";

                case "rightLeaf1":
                case "rightLeaf2":
                case "rightLeaf3":
                    return "rightNode";
            }
            return null;
        }

        [Test]
        public void D_root_node_has_no_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rootNode".PrecedingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_node_returns_left_sibling_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rightNode".PrecedingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("leftNode", result.ElementAt(0));
        }

        [Test]
        public void D_node_returns_no_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "leftNode".PrecedingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_node_returns_all_siblings_on_PrecedingSiblings()
        {
            // ACT

            string[] result = "rightLeaf3".PrecedingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("rightLeaf1", result.ElementAt(0));
            Assert.AreEqual("rightLeaf2", result.ElementAt(1));
        }
    }
}
