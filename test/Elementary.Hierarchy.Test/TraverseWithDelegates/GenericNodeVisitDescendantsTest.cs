namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

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

        [Fact]
        public void D_visit_complete_tree_breadthFirst_on_VisitDescendants()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Fact]
        public void D_visit_complete_tree_depthFirst_on_VisitDescendants()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)), depthFirst: true);

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.Item2));

            Assert.Equal(new[] { "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Fact]
        public void D_inconsistent_node_visits_no_children_on_VisitDescendants()
        {
            // ARRANGE

            Func<string, bool> hasChildNodes = n => true;
            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".Descendants(getChildNodes).ToArray();

            // ASSERT

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void D_visit_singleChild_on_VisitDescendants()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "leftNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("leftLeaf", result.ElementAt(0).Item2);
            Assert.Equal(new[] { "leftNode" }, result.ElementAt(0).Item1);
        }

        [Fact]
        public void D_visit_leftChild_first_on_VisitDescendants()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rightNode".VisitDescendants(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(1).Item1);
        }

        #endregion VisitDescendants

        #region VisitDescendantsOrSelf

        [Fact]
        public void D_visit_complete_tree_breadthFirst_on_VisitDescendantsOrSelf()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)));

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "rightNode",
                "leftLeaf",
                "leftRightLeaf",
                "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new string[] { }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(5).Item1);
        }

        [Fact]
        public void D_visit_complete_tree_depthFirst_on_VisitDescendantsOrSelf()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendantsOrSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)), depthFirst: true);

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.Item2));

            Assert.Equal(new[] { "rootNode", "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new string[] { }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(5).Item1);
        }

        #endregion VisitDescendantsOrSelf
    }
}