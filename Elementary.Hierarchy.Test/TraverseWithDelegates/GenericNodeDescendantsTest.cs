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
        public void LeafReturnsNoChildren()
        {
            // ACT

            IEnumerable<string> result = "startNode".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void LeafReturnsNoChildrenButClaimsToHaveSubnodes()
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
        public void EnumerateSingleChildToLeaf()
        {
            // ACT

            IEnumerable<string> result = "leftNode".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreSame("leftLeaf", result.ElementAt(0));
        }

        [Test]
        public void EnumerateTwoChildrenToLeaf()
        {
            // ACT

            IEnumerable<string> result = "rightNode".Descendants(this.GetChildNodes);

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void EnumerateTreeBreadthFirst()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Test]
        public void EnumerateTreeDepthFirst()
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
        public void DescendantsLevel1AreChildren()
        {
            // ACT

            var descendants = "startNode".Descendants(this.GetChildNodes, maxDepth: 1).ToArray();
            var children = "startNode".Children(this.GetChildNodes).ToArray();

            // ASSERT

            CollectionAssert.AreEqual(children, descendants);
        }
    }
}