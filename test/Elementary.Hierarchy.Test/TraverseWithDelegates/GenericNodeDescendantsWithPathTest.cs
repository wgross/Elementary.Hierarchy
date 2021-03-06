﻿namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
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
        public void D_DescendantsWithPath_traverse_tree_breadth_first()
        {
            // ACT

            var result = "rootNode".DescendantsWithPath(getChildren: this.GetChildNodes, depthFirst: false, maxDepth: null);

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.node).ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).path);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).path);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(2).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).path);
        }

        [Fact]
        public void D__DescendantsWithPath_traverses_tree_depth_first()
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
            }, result.Select(i => i.node));

            Assert.Equal(new[] { "leftNode", "leftLeaf", "rightNode", "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.node).ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(0).path);
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(1).path);
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(3).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).path);
        }

        [Fact]
        public void D_DescendantsWithPath_traverses_single_child()
        {
            // ACT

            var result = "leftNode".DescendantsWithPath(getChildren: this.GetChildNodes);

            // ASSERT

            Assert.Single(result);
            Assert.Equal("leftLeaf", result.ElementAt(0).node);
            Assert.Equal(new[] { "leftNode" }, result.ElementAt(0).path);
        }

        [Fact]
        public void D_DescendantsWithPath_visits_leftChild_first()
        {
            // ACT

            var result = "rightNode".DescendantsWithPath(this.GetChildNodes);

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftRightLeaf", "rightRightLeaf" }, result.Select(i => i.node));
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(0).path);
            Assert.Equal(new[] { "rightNode" }, result.ElementAt(1).path);
        }

        [Fact]
        public void D_DescendantsWithPath_breaks_on_maxDepth()
        {
            // ACT

            var result = "rootNode".DescendantsWithPath(this.GetChildNodes, maxDepth: 1);

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode" }, result.Select(i => i.node));
        }

        #endregion DescendantsWithPath

        #region DescendantsAndSelfWithPath

        [Fact]
        public void D_DescendantsAndSelfWithPath_traverses_tree_breadthFirst()
        {
            // ACT

            var result = "rootNode".DescendantsAndSelfWithPath(getChildren: this.GetChildNodes, depthFirst: false);

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "rightNode",
                "leftLeaf",
                "leftRightLeaf",
                "rightRightLeaf" }, result.Select(i => i.node));
            Assert.Empty(result.ElementAt(0).path); // parent of startNode isn't known
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(1).path.ToArray());
            Assert.Equal(new[] { "rootNode" }, result.ElementAt(2).path.ToArray());
            Assert.Equal(new[] { "rootNode", "leftNode" }, result.ElementAt(3).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(4).path);
            Assert.Equal(new[] { "rootNode", "rightNode" }, result.ElementAt(5).path);
        }

        [Fact]
        public void D_DescendantsAndSelfWithPath_traverses_tree_depthFirst()
        {
            // ACT

            var result = "rootNode".DescendantsAndSelfWithPath(this.GetChildNodes, depthFirst: true);

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

        #endregion DescendantsAndSelfWithPath
    }
}