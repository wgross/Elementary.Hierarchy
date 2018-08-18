namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GenericNodeDescendantsTest
    {
        [Fact]
        public void D_leaf_returns_no_children_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "leftRightLeaf".Descendants(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.False(result.Any());
        }

        [Fact]
        public void D_inconsitent_leaf_returns_no_children_on_Descendants()
        {
            // ARRANGE

            Func<string, bool> hasChildNodes = n => true;
            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".Descendants(getChildNodes).ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void D_inconsistent_leaf_returns_converts_null_to_empty_children_on_Descendants()
        {
            // ARRANGE

            Func<string, bool> hasChildNodes = n => true;
            Func<string, IEnumerable<string>> getChildNodes = n => null;

            // ACT

            IEnumerable<string> result = "badLeaf".Descendants(getChildNodes).ToArray();

            // ASSERT

            Assert.False(result.Any());
        }

        [Fact]
        public void D_node_returns_single_child_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "leftNode".Descendants(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Single(result);
            Assert.Same("leftLeaf", result.ElementAt(0));
        }

        [Fact]
        public void D_node_returns_left_child_first_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rightNode".Descendants(DelegateTreeDefinition.GetChildNodes);

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Fact]
        public void D_root_returns_descendants_breadthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Fact]
        public void D_root_returns_descendants_breadthFirst_on_Descendants_rightest_node_without_child()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes2).ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "leftNode", "rightNode", "leftLeaf" }, result);
        }

        [Fact]
        public void D_root_returns_descendants_depthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }

        [Fact]
        public void D_root_returns_descendants_depthFirst_on_Descendants_rightest_node_without_child()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes2, depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode"
            }, result);
        }

        [Fact]
        public void d_root_returns_children_as_level1_descendants_on_Descendants()
        {
            // ACT

            var descendants = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, maxDepth: 1).ToArray();

            // ASSERT

            Assert.Equal(new[] { "leftNode", "rightNode" }, descendants);
        }

        [Fact]
        public void D_root_returns_children_as_level1_descendants_on_Descendants()
        {
            // ACT

            string[] descendants = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, maxDepth: 0).ToArray();

            // ASSERT

            Assert.False(descendants.Any());
        }

        [Fact]
        public void D_root_throws_ArgumentException_on_level_lt0_on_Descendants()
        {
            // ACT

            ArgumentException ex = Assert.Throws<ArgumentException>(() => "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, maxDepth: -1));

            // ASSERT

            Assert.Contains("must be > 0", ex.Message);
            Assert.Equal("maxDepth", ex.ParamName);
        }

        [Fact]
        public void D_root_returns_all_descendants_on_highLevel_breadthFirst_on_Descendants()
        {
            // ACT

            string[] result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, maxDepth: 3).ToArray();

            // ASSERT

            Assert.Equal(new[]
            {
                "leftNode",
                "rightNode",
                "leftLeaf",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }

        [Fact]
        public void D_root_returns_descendants_on_highLevel_depthFirst_on_Descendants()
        {
            // ACT

            IEnumerable<string> result = "rootNode".Descendants(DelegateTreeDefinition.GetChildNodes, depthFirst: true, maxDepth: 3).ToArray();

            // ASSERT

            Assert.Equal(5, result.Count());
            Assert.Equal(new[] {
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }
    }
}