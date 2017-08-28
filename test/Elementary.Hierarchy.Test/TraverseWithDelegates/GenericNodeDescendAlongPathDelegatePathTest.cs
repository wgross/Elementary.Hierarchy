using System.Collections.Generic;
using System.Linq;
using Elementary.Hierarchy.Generic;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodeDescendAlongPathDelegatePathTest
    {
        private IEnumerable<string> GetChildNodes(string rootNode)
        {
            switch (rootNode)
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
        public void D_returns_itself_for_empty_path_on_DescendAlongPath()
        {
            // ACT
            // descend along a path with one item

            string[] result = "rootNode".DescendAlongPath(getChildNodes: this.GetChildNodes).ToArray();

            // ASSERT
            // contains a single item: the start node.

            Assert.Equal(new[] { "rootNode" }, result);
        }

        [Fact]
        public void D_returns_child_on_DescendAlongPath()
        {
            // ACT
            // descend along a path with one item

            string[] result = "rootNode".DescendAlongPath(getChildNodes: this.GetChildNodes, path: c => (true, c.First())).ToArray();

            // ASSERT
            // contains a root and child

            Assert.Equal(new[] { "rootNode", "leftNode" }, result);
        }

        [Fact]
        public void D_returns_child_and_grandchild_on_DescendAlongPath()
        {
            // ACT
            // descend along a path with two items

            string[] result = "rootNode".DescendAlongPath(this.GetChildNodes,
                c => (true, c.Last()), c => (true, c.First())).ToArray();

            // ASSERT
            // contains a root and child

            Assert.Equal(new[] { "rootNode", "rightNode", "leftRightLeaf" }, result);
        }

        [Fact]
        public void D_return_incomplete_list_on_DescendAlongPath()
        {
            // ACT
            // descend along a path, last item cant't be found

            string[] result = "rootNode".DescendAlongPath(this.GetChildNodes,
                c => (true, c.Last()), c => (false, null)).ToArray();

            // ASSERT
            // contains a root and child

            Assert.Equal(new[] { "rootNode", "rightNode" }, result);
        }
    }
}