using System;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class FlattenObjectTest
    {
        [Fact]
        public void Create_flat_map_of_properties()
        {
            // ARRANGE

            var obj = new
            {
                a = 1,
                b = "b"
            };

            // ACT

            var result = obj.FlattenAsDictionary();

            // ASSERT

            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("/a"));
            Assert.True(result.ContainsKey("/b"));
        }

        [Fact]
        public void Create_flat_map_without_descending_into_string()
        {
            // ARRANGE

            var obj = new
            {
                b = "b"
            };

            // ACT

            var result = obj.FlattenAsDictionary();

            // ASSERT

            Assert.Equal(1, result.Count);
            Assert.True(result.ContainsKey("/b"));
            Assert.IsType<string>(result.Single().Value);
        }

        [Fact]
        public void Create_flat_map_without_descending_into_DateTime()
        {
            // ARRANGE

            var obj = new
            {
                a = DateTime.Now
            };

            // ACT

            var result = obj.FlattenAsDictionary();

            // ASSERT

            Assert.Equal(1, result.Count);
            Assert.True(result.ContainsKey("/a"));
            Assert.IsType<DateTime>(result.Single().Value);
        }

        [Fact]
        public void Create_flat_map_without_inner_nodes()
        {
            // ARRANGE

            var obj = new
            {
                a = new
                {
                    b = DateTime.Now
                }
            };

            // ACT

            var result = obj.FlattenAsDictionary();

            // ASSERT

            Assert.Equal(1, result.Count);
            Assert.True(result.ContainsKey("/a/b"));
            Assert.IsType<DateTime>(result.Single().Value);
        }
    }
}