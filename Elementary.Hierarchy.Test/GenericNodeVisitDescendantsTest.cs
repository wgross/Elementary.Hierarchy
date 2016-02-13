namespace Elementary.Hierarchy.Test
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeVisitDescendantsTest
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
        public void DescandantsReturnsNoChildren_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "startNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void LeafReturnsNoChildrenButClaimsToHaveSubnodes_GN()
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
        public void EnumerateDescendantsSingleChildToLeaf_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "leftNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("leftLeaf", result.ElementAt(0).Item2);
            CollectionAssert.AreEqual(new[] { "leftNode" }, result.ElementAt(0).Item1);
        }

        [Test]
        public void EnumerateDescandantsTwoChildrenToLeaf_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rightNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEqual(new[] { "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new[] { "rightNode" }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rightNode" }, result.ElementAt(1).Item1);
        }

        [Test]
        public void EnumerateDescendantsBreadthFirst_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode" }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Test]
        public void EnumerateDescendantsDepthFirst_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)), depthFirst: true);

            // ASSERT

            Assert.AreEqual(5, result.Count());
            CollectionAssert.AreEqual(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.Item2));

            CollectionAssert.AreEqual(new[] { "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode" }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Test]
        public void DescendantsOrDefaultReturnsLeaf_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "startNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new string[] { }, result.ElementAt(0).Item1);
            Assert.AreEqual("startNode", result.ElementAt(0).Item2);
        }

        [Test]
        public void LeafReturnsItselfButClaimsToHaveSubnodes_GN()
        {
            // ARRANGE

            Func<string, bool> hasChildNodes = n => true;
            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".DescendantsOrSelf(getChildNodes).ToArray();

            // ASSERT

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("badLeaf", result.ElementAt(0));
        }

        [Test]
        public void EnumerateDescandantsOrSelfSingleChildToLeaf_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "leftNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("leftNode", result.ElementAt(0).Item2);
            Assert.AreEqual("leftLeaf", result.ElementAt(1).Item2);
            CollectionAssert.AreEqual(new string[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "leftNode" }, result.ElementAt(1).Item1);
        }

        [Test]
        public void EnumerateDescandantsOrSelfTwoChildrenToLeaf_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rightNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[] { "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new string[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rightNode" }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { "rightNode" }, result.ElementAt(2).Item1);
        }

        [Test]
        public void EnumerateDescendantsOrSelfBreadthFirst_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
                "rootNode",
                "leftNode",
                "rightNode",
                "leftLeaf",
                "leftRightLeaf",
                "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new string[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode" }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(5).Item1);
        }

        [Test]
        public void EnumerateDescandantsOrSelfDepthFirst_GN()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)), depthFirst: true);

            // ASSERT

            Assert.AreEqual(6, result.Count());
            CollectionAssert.AreEqual(new[] {
                "rootNode",
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.Item2));

            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            CollectionAssert.AreEqual(new string[] { }, result.ElementAt(0).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(1).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "leftNode" }, result.ElementAt(2).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode" }, result.ElementAt(3).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
            CollectionAssert.AreEqual(new[] { "rootNode", "rightNode" }, result.ElementAt(5).Item1);
        }
    }
}