using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Elementary.Hierarchy.Reflection.Test
{
    public class FlattenObjectTest
    {
        [Fact]
        public void Create_flat_map_of_properties()
        {
            // ARRANGE

            var obj = new {
                a = 1,
                b = "b"
            };

            // ACT

            var result = obj.FlattenAsDictionary();

            // ASSERT

            Assert.Equal(3, result.Count);
            Assert.True(result.ContainsKey("/a"));
            Assert.True(result.ContainsKey("/b"));
            Assert.True(result.ContainsKey("/b/Length"));
        }
    }
}
