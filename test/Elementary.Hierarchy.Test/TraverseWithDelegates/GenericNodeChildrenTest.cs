using System.Collections.Generic;
using System.Linq;
using Xunit;
using Elementary.Hierarchy.Generic;
using System;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodeChildrenTest
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
        public void D_retrieves_children_on_Children()
        {
            // ACT

            var result = "rootNode".Children(this.GetChildNodes).ToArray();

            // ASSERT
            // only the first layer of child nodes is retrieved

            Assert.Equal(new[] { "leftNode", "rightNode" }, result);
        }

         [Fact]
        public void D_converts_null_to_empty_collection_on_Children()
        {
            // ARRANGE

             Func<string,IEnumerable<string>> getChildNodesWrong = n => null;

            // ACT
            // ask for children

            var result = "rootNode".Children(getChildNodesWrong).ToArray();

            // ASSERT
            // result isn't null but empty

            Assert.NotNull(result);
            Assert.False(result.Any());

        }
    }
}