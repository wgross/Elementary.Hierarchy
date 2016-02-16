namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeDescendantsOrSelfTest
    {
        private IEnumerable<string> GetChildNodes(string startNode)
        {
            switch (startNode)
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
        public void D_leaf_returns_itself_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<string> result = "leftLeaf".DescendantsOrSelf(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("leftLeaf", result.ElementAt(0));
        }

        [Test]
        public void D_inconsistent_leaf_returns_itself_on_DescendantsOrSelf()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".DescendantsOrSelf(getChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("badLeaf", result.ElementAt(0));
        }

        [Test]
        public void D_leaf_returns_single_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<string> result = "leftNode".DescendantsOrSelf(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { "leftNode", "leftLeaf" }, result);
        }

        [Test]
        public void D_leaf_returns_left_before_right_child_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<string> result = "rightNode".DescendantsOrSelf(this.GetChildNodes);

            // ASSERT

            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[] { "rightNode", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void D_leaf_returns_descendants_breadthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<string> result = "rootNode".DescendantsOrSelf(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void D_leaf_returns_descendants_depthFirst_on_DescendantsOrSelf()
        {
            // ACT

            IEnumerable<string> result = "rootNode".DescendantsOrSelf(this.GetChildNodes, depthFirst: true).ToArray();

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
                "rootNode",
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }

        [Test]
        public void D_root_returns_children_as_level2_descendants_on_DescendantsOrSelf()
        {
            // ACT

            var descendants = "startNode".DescendantsOrSelf(this.GetChildNodes, maxDepth: 2).Skip(1).ToArray();
            var children = "startNode".Children(this.GetChildNodes).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(children, descendants);
        }
    }
}