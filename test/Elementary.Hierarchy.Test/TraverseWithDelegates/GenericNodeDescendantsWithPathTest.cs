namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeDescendantsWithPathTest
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

        #region DescendantsWithPath

        [Fact]
        public void D_Traverse_complete_tree_breadthFirst_and_return_node_with_path()
        {
            // ACT

            var result = "rootNode".DescendantsWithPath(getChildren: this.GetChildNodes, depthFirst: false, maxDepth: null).ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2).ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Fact]
        public void D_Traverse_complete_tree_depthFirst_and_return_node_with_path()
        {
            // ACT

            var result = "rootNode".DescendantsWithPath(getChildren: this.GetChildNodes, depthFirst: true, maxDepth: null);

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.Item2));

            Assert.Equal(new[] { "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2).ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(1).Item1);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
        }

        [Fact]
        public void D_Traverse_singleChild_on_TraverseWithPath()
        {
            // ACT

            var result = "leftNode".DescendantsWithPath(getChildren: this.GetChildNodes);

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("leftLeaf", result.ElementAt(0).Item2);
            Assert.Equal(new[] { "leftNode" }, result.ElementAt(0).Item1);
        }

        [Fact]
        public void D_visit_leftChild_first_on_TraverseWithPath()
        {
            // ACT

            var result = "rightNode".DescendantsWithPath(this.GetChildNodes);

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(0).Item1);
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(1).Item1);
        }

        #endregion DescendantsWithPath

        #region DescendantsAndSelfWithPath

        [Fact]
        public void D_Traverse_complete_tree_with_DescendantsAndSelfWithPath()
        {
            // ACT

            var result = "rootNode".DescendantsAndSelfWithPath(getChildren: this.GetChildNodes);

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "rightNode",
                "leftLeaf",
                "leftRightLeaf",
                "rightRightLeaf" }, result.Select(i => i.Item2));
            Assert.Empty(result.ElementAt(0).Item1); // parent of startNode isn't known
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).Item1.ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).Item1.ToArray());
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(3).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).Item1);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(5).Item1);
        }

        [Fact]
        public void D_visit_complete_tree_depthFirst_on_DescendantsAndSelfWithPath()
        {
            // ACT

            var result = new List<Tuple<List<string>, string>>();
            "rootNode".VisitDescendantsAndSelf(this.GetChildNodes, (b, n) => result.Add(Tuple.Create(b.ToList(), n)), depthFirst: true);

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

        #endregion DescendantsAndSelfWithPath
    }
}