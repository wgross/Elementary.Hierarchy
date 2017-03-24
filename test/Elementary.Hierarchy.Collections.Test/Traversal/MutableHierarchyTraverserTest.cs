using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    
    public class MutableHierarchyTraverserTest
    {
        [Fact]
        public void MH_Same_traverser_are_equal()
        {
            // ARRANGE

            var a = new MutableHierarchy<string, int>.Traverser(new MutableHierarchy<string, int>.Node(""));

            // ACT

            var result = a.Equals(a);

            // ASSERT

            Assert.True(result);
        }

        [Fact]
        public void MH_Traversers_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new MutableHierarchy<string, int>.Node("");
            var a = new MutableHierarchy<string, int>.Traverser(node);
            var b = new MutableHierarchy<string, int>.Traverser(node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.True(result);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
    }
}