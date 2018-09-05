namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeDescendantsWithPathAvoidCyclesTest
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

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_traverses_tree_depth_first()
        {
            // ACT

            var result = "rootNode".DescendantsAndSelfWithPathAvoidCycles(this.GetChildNodes);

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result.Select(i => i.node));

            Assert.Equal(new[] { "rootNode", "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.node));
            Assert.Empty(result.ElementAt(0).path);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).path);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(2).path);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(3).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(5).path);
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_traverses_single_child()
        {
            // ACT

            var result = "leftNode".DescendantsAndSelfWithPathAvoidCycles(this.GetChildNodes);

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal("leftNode", result.ElementAt(0).node);
            Assert.Equal("leftLeaf", result.ElementAt(1).node);
            Assert.Empty(result.ElementAt(0).path);
            Assert.Equal(new[] { "leftNode" }, result.ElementAt(1).path);
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_visits_left_child_first()
        {
            // ACT

            var result = "rightNode".DescendantsAndSelfWithPathAvoidCycles(this.GetChildNodes);

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.node));
            Assert.Empty(result.ElementAt(0).path);
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(1).path);
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(2).path);
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_breaks_on_maxDepth()
        {
            // ACT

            var result = "rootNode".DescendantsAndSelfWithPathAvoidCycles(this.GetChildNodes, maxDepth: 1);

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "rootNode", "leftNode", "rightNode" }, result.Select(i => i.node));
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_avoids_roots_self_cycle()
        {
            // ARRANGE

            IEnumerable<string> treeWithRootSelfCycle(string node)
            {
                //           -> rootNode
                //          |_____|
                //
                //
                // unkown node -> {}

                switch (node)
                {
                    case "rootNode":
                        return new[] { "rootNode" };
                }
                return Enumerable.Empty<string>();
            };

            // ACT

            var result = "rootNode".DescendantsAndSelfWithPathAvoidCycles(treeWithRootSelfCycle);

            // ASSERT

            Assert.Single(result);
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_avoids_child_root_cycle()
        {
            // ARRANGE

            IEnumerable<string> treeWithRootSelfCycle(string node)
            {
                //              -> rootNode
                //            /    /
                //          leftNode
                //           /
                //     leftLeaf
                //
                // unkown node -> {}

                switch (node)
                {
                    case "rootNode":
                        return new[] { "leftNode" };

                    case "leftNode":
                        return new[] { "leftLeaf", "rootNode" };
                }
                return Enumerable.Empty<string>();
            }

            // ACT

            var result = "rootNode".DescendantsAndSelfWithPathAvoidCycles(treeWithRootSelfCycle);

            // ASSERT

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void D_DescendantsAndSelfWithPathAvoidCycles_avoids_grand_childs_root_cycle()
        {
            // ARRANGE

            IEnumerable<string> treeWithRootSelfCycle(string node)
            {
                //          ---> rootNode
                //        /      /
                //       /   leftNode
                //      /     /
                //     leftLeaf
                //
                // unkown node -> {}

                switch (node)
                {
                    case "rootNode":
                        return new[] { "leftNode" };

                    case "leftNode":
                        return new[] { "leftLeaf" };

                    case "leftLeaf":
                        return new[] { "rootNode" };
                }

                return Enumerable.Empty<string>();
            }

            // ACT

            var result = "rootNode".DescendantsAndSelfWithPathAvoidCycles(treeWithRootSelfCycle);

            // ASSERT

            Assert.Equal(3, result.Count());
        }
    }
}