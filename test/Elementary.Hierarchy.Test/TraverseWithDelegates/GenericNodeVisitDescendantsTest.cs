namespace Elementary.Hierarchy.Test.TraverseWithDelegates
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

        #region VisitDescendants

        [Test]
        public void D_visit_complete_tree_breadthFirst_on_VisitDescendants()
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
        public void D_visit_complete_tree_depthFirst_on_VisitDescendants()
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
        public void D_inconsistent_node_visits_no_children_on_VisitDescendants()
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
        public void D_visit_singleChild_on_VisitDescendants()
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
        public void D_visit_leftChild_first_on_VisitDescendants()
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

        #endregion VisitDescendants

        #region VisitDescendantsOrSelf

        [Test]
        public void D_visit_complete_tree_breadthFirst_on_VisitDescendantsOrSelf()
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
        public void D_visit_complete_tree_depthFirst_on_VisitDescendantsOrSelf()
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

        #endregion VisitDescendantsOrSelf
    }
}