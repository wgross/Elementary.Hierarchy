using System;
using System.Collections.Generic;
using System.Linq;
using Elementary.Hierarchy.Generic;
using Xunit;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    public class GenericNodeChildrenTest
    {
        [Fact]
        public void D_retrieves_children_on_Children()
        {
            // ACT

            var result = "rootNode".Children(DelegateTreeDefinition.GetChildNodes).ToArray();

            // ASSERT
            // only the first layer of child nodes is retrieved

            Assert.Equal(new[] { "leftNode", "rightNode" }, result);
        }

        [Fact]
        public void D_converts_null_to_empty_collection_on_Children()
        {
            // ARRANGE
            // returns (true,null) which is inconsistent bahavior

            Func<string, IEnumerable<string>> getChildNodesWrong = n => null;

            // ACT
            // ask for children

            var result = "rootNode".Children(getChildNodesWrong).ToArray();

            // ASSERT
            // result isn't null but empty-> inconsistent behavior is handled

            Assert.NotNull(result);
            Assert.False(result.Any());
        }
    }
}