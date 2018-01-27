namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elementary.Hierarchy.Generic;
    using Xunit;

    public class GenericNodeDescendantsAndSelfTest
    {
        [Fact]
        public void D_leaf_returns_itself_on_DescendantsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "leftLeaf".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal("leftLeaf", result.ElementAt(0));
        }

        [Fact]
        public void D_inconsistent_leaf_returns_itself_on_DescendantsAndSelf()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = n => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "badLeaf".DescendantsAndSelf(getChildNodes).ToArray();

            // ASSERT

            Assert.Equal(1, result.Count());
            Assert.Equal("badLeaf", result.ElementAt(0));
        }

        [Fact]
        public void D_leaf_returns_single_child_on_DescendantsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "leftNode".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(2, result.Count());
            Assert.Equal(new[] { "leftNode", "leftLeaf" }, result);
        }

        [Fact]
        public void D_leaf_returns_left_before_right_child_on_DescendantsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "rightNode".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes);

            // ASSERT

            Assert.Equal(3, result.Count());
            Assert.Equal(new[] { "rightNode", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Fact]
        public void D_leaf_returns_descendants_breadthFirst_on_DescendantsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "rootNode".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] { "rootNode", "leftNode", "rightNode", "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result);
        }

        [Fact]
        public void D_leaf_returns_descendants_depthFirst_on_DescendantsAndSelf()
        {
            // ACT

            IEnumerable<string> result = "rootNode".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes, depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(6, result.Count());
            Assert.Equal(new[] {
                "rootNode",
                "leftNode",
                "leftLeaf",
                "rightNode",
                "leftRightLeaf",
                "rightRightLeaf"
            }, result);
        }

        [Fact]
        public void D_root_returns_children_as_level2_descendants_on_DescendantsAndSelf()
        {
            // ACT

            var descendants = "startNode".DescendantsAndSelf(DelegateTreeDefinition.GetChildNodes, maxDepth: 2).Skip(1).ToArray();
            var children = "startNode".Children(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT

            Assert.Equal(children, descendants);
        }
    }
}