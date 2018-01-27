using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Elementary.Hierarchy.Test
{
    public class HierarchyPathParseTest
    {
        #region Parse

        [Fact]
        public void Parse_string_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<string> result = HierarchyPath.Parse(path: "/test/test2", separator: "/");

            // ASSERT

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(new[] { "test", "test2" }, result.Items.ToArray());
        }

        [Fact]
        public void Parse_int_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<int> result = HierarchyPath.Parse<int>(path: "1/2", separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(new[] { 1, 2 }, result.Items.ToArray());
        }

        #endregion Parse

        #region TryParse

        [Fact]
        public void TryParse_string_path_items_from_string_representation()
        {
            // ACT
            HierarchyPath<string> resultPath = null;
            bool result = HierarchyPath.TryParse(path: "/test/test2", hierarchyPath: out resultPath, convertPathItem: i => i, separator: "/");

            // ASSERT

            Assert.True(result);
            Assert.Equal(2, resultPath.Items.Count());
            Assert.Equal(new[] { "test", "test2" }, resultPath.Items.ToArray());
        }

        [Fact]
        public void TryParse_int_path_items_from_string_representation()
        {
            // ACT

            HierarchyPath<int> resultPath = null;
            bool result = HierarchyPath.TryParse<int>(path: "1/2", hierarchyPath: out resultPath, separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            Assert.True(result);
            Assert.Equal(2, resultPath.Items.Count());
            Assert.Equal(new[] { 1, 2 }, resultPath.Items.ToArray());
        }

        [Fact]
        public void TryParse_returns_false_for_null_string_representation()
        {
            // ACT

            HierarchyPath<int> resultPath = null;
            bool result = HierarchyPath.TryParse<int>(path: null, hierarchyPath: out resultPath, separator: "/", convertPathItem: s => int.Parse(s));

            // ASSERT

            Assert.False(result);
            Assert.Null(resultPath);
        }

        #endregion TryParse
    }
}
