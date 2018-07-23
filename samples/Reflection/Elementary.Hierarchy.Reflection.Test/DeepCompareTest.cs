using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class DeepCompareTest
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

            var result = left.DeepCompare(right);

            // ASSERT

            Assert.True(result.AreEqual);
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

            var result = left.DeepCompare(left);

            // ASSERT

            Assert.True(result.AreEqual);
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

            var result1 = left.DeepCompare(right);
            var result2 = right.DeepCompare(left);

            // ASSERT

            Assert.False(result1.AreEqual);
            Assert.False(result2.AreEqual);
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

            var result = left.DeepCompare(right);

            // ASSERT

            Assert.False(result.AreEqual);
            Assert.Equal("/a", result.RightLeafIsMissing.Single());
            Assert.Equal("/b", result.LeftLeafIsMissing.Single());
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

            var result = left.DeepCompare(right);

            // ASSERT

            Assert.False(result.AreEqual);
            Assert.Equal("/a/0", result.LeftLeafIsMissing.Single());
            Assert.Equal("/a", result.RightLeafIsMissing.Single());
        }

        [Fact]
        public void Instances_not_equal_on_different_values()
        {
            // ARRANGE

            var left = new
            {
                a = "b",
            };

            var right = new
            {
                a = "a"
            };

            // ACT

            var result = left.DeepCompare(right);

            // ASSERT

            Assert.False(result.AreEqual);
            Assert.Equal("/a", result.DifferentValues.Single());
        }
    }
}