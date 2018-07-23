using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class DeepEqualsTest
    {
        [Fact]
        public void Instances_are_equal()
        {
            // ARRANGE

            var left = new
            {
                a = "a"
            };

            var right = new
            {
                a = "a"
            };

            // ACT

            var result = left.DeepEquals(right);

            // ASSERT

            Assert.True(result);
        }

        [Fact]
        public void Instances_are_equal_for_same_instances()
        {
            // ARRANGE

            var left = new
            {
                a = "a"
            };

            // ACT

            var result = left.DeepEquals(left);

            // ASSERT

            Assert.True(result);
        }

        [Fact]
        public void Instances_not_equal_on_additional_property()
        {
            // ARRANGE

            var left = new
            {
                a = "a",
                b = "b"
            };

            var right = new
            {
                a = "a"
            };

            // ACT

            var result1 = left.DeepEquals(right);
            var result2 = right.DeepEquals(left);

            // ASSERT

            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public void Instances_not_equal_on_different_pathes()
        {
            // ARRANGE

            var left = new
            {
                b = "a",
            };

            var right = new
            {
                a = "a"
            };

            // ACT

            var result = left.DeepEquals(right);

            // ASSERT

            Assert.False(result);
        }

        [Fact]
        public void Instances_not_equal_on_different_types()
        {
            // ARRANGE

            var left = new
            {
                a = new[] { 1 },
            };

            var right = new
            {
                a = "a"
            };

            // ACT

            var result = left.DeepEquals(right);

            // ASSERT

            Assert.False(result);
        }
    }
}