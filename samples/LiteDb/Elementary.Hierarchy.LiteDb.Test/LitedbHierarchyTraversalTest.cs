using LiteDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.LiteDb.Test
{
    public class LitedbHierarchyTraversalTest : IDisposable
    {
        private readonly LiteDatabase database;
        private readonly LiteDbHierarchy hierarchy;

        public LitedbHierarchyTraversalTest()
        {
            this.database = new LiteDatabase(new MemoryStream());
            this.hierarchy = new LiteDbHierarchy(new LiteDbHierarchyNodeRepository(this.database, "nodes", "values"));
        }

        public void Dispose()
        {
            this.database.Dispose();
        }

        [Fact]
        public void LiteDbHierarchy_returns_descendant()
        {
            // ARRANGE

            var child = this.hierarchy.Traverse().AddChildNode("child");
            var gchild = child.AddChildNode("gchild");

            // ACT

            var result = this.hierarchy.Traverse().DescendantAt(HierarchyPath.Create("child", "gchild"));

            // ASSERT

            Assert.Equal(gchild.Key, result.Key);
            Assert.Equal(gchild.InnerNode._Id, result.InnerNode._Id);
        }

        [Fact]
        public void LiteDbHierarchy_traverses_depth_first()
        {
            // ARRANGE

            var child1 = this.hierarchy.Traverse().AddChildNode("child1");
            var child2 = this.hierarchy.Traverse().AddChildNode("child2");
            var gchild1 = child1.AddChildNode("gchild1");

            // ACT

            var result = this.hierarchy.Traverse().Descendants(depthFirst: true).ToArray();

            // ASSERT

            Assert.Equal(new[] { "child1", "gchild1", "child2" }, result.Select(n => n.Key).ToArray());
        }
    }
}
;