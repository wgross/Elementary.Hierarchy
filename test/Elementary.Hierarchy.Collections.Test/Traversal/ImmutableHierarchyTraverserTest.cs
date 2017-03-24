using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    
    public class ImmutableHierarchyTraverserTest
    {
        [Fact]
        public void IMH_Same_traverser_are_equal()
        {
            // ARRANGE

            var a = new ImmutableHierarchy<string, int>.Traverser(new ImmutableHierarchy<string, int>.Node(""));

            // ACT

            var result = a.Equals(a);

            // ASSERT

            Assert.True(result);
        }

        [Fact]
        public void IMH_Traversers_are_equal_if_node_is_same()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, int>.Node("");
            var a = new ImmutableHierarchy<string, int>.Traverser(node);
            var b = new ImmutableHierarchy<string, int>.Traverser(node);

            // ACT

            var result = a.Equals(b);

            // ASSERT

            Assert.True(result);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
    }
}