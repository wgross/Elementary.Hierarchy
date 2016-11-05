namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeDescendantsTest
    {
        private IEnumerable<string> GetChildNodes(string rootNode)
        {
            switch (rootNode)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return new[] { "leftLeaf" };

                case "rightNode":
                    return new[] { "leftRightLeaf", "rightRightLeaf" };
            }
            return Enumerable.Empty<string>();
        }

        [Test]
        public void D_leaf_returns_no_children_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "leftRightLeaf".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_inconsitent_leaf_returns_no_children_on_Descendants()
        {
            // ARRANGE

            Func<string, bool> hasChildNodes = n => true;
            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".Descendants(getChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void D_node_returns_single_child_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "leftNode".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreSame("leftLeaf", result.ElementAt(0));
        }

        [Test]
        public void D_node_returns_left_child_first_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rightNode".Descendants(this.GetChildNodes);

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void D_root_returns_descendants_breadthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void D_root_returns_descendants_depthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(this.GetChildNodes, depthFirst: true).ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }

        [Test]
        public void D_root_returns_children_as_level1_descendants_on_Descendants()
        {
            // ACT

            var descendants = "rootNode".Descendants(this.GetChildNodes, maxDepth: 1).ToArray();
            var children = "rootNode".Children(this.GetChildNodes).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(children, descendants);
        }

        [Test]
        public void D_root_throws_on_level0_on_Descendants()
        {
            // ACT

            ArgumentException ex = Assert.Throws<ArgumentException>(() => "rootNode".Descendants(this.GetChildNodes, maxDepth: -1));
            string[] result = "rootNode".Descendants(this.GetChildNodes, maxDepth: 0).ToArray();

            // ASSERT

            Assert.IsTrue(ex.Message.Contains("must be > 0"));
            Assert.AreEqual("maxDepth", ex.ParamName);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_root_returns_all_descendants_on_highLevel_breadthFirst_on_Descendants()
        {
            // ACT

            string[] result = "rootNode".Descendants(this.GetChildNodes, maxDepth: 3).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void D_root_returns_descendants_on_highLevel_depthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(this.GetChildNodes, depthFirst: true, maxDepth: 3).ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }
    }
}