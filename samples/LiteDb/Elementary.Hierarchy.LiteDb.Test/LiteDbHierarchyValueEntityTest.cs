using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LiteDbHierarchyValueEntityTest
    {
        [Fact]
        public void LiteDbHierarchyValueEntity_retuzrns_true_on_first_value()
        {
            // ARRANGE

            var value = new LiteDbHierarchyValueEntity();

            // ACT

            var result = value.SetValue(1);

            // ASSERT

            Assert.True(result);
            Assert.Equal(1, value.Value.AsInt32);
        }

        [Fact]
        public void LiteDbHierarchyValueEntity_returns_true_on_different_value()
        {
            // ARRANGE

            var value = new LiteDbHierarchyValueEntity();
            value.SetValue(1);

            // ACT

            var result = value.SetValue(2);

            // ASSERT

            Assert.True(result);
            Assert.Equal(2, value.Value.AsInt32);
        }

        [Fact]
        public void LiteDbHierarchyValueEntity_returns_false_on_same_value()
        {
            // ARRANGE

            var value = new LiteDbHierarchyValueEntity();
            value.SetValue(1);

            // ACT

            var result = value.SetValue(1);

            // ASSERT

            Assert.False(result);
        }
    }
}