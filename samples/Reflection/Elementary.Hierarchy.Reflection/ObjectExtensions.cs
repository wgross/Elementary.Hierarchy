using Elementary.Hierarchy.Generic;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Reflection
{
    public static class ObjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, object>> Flatten(this object root)
        {
            var h = ReflectedHierarchy.Create(root, new FlattedObjectHierarchyNodeFactory());
            var flatted_h = new Dictionary<string, object>();
            foreach (var (node, path) in h.DescendantsWithPath(getChildren: n => n.ChildNodes, depthFirst: true, maxDepth: null))
            {
                if (node.HasChildNodes)
                    continue;

                var (success, value) = node.TryGetValue<object>();
                var pathAsString = $"{string.Join("/", path.Select(p => p.Id))}/{node.Id}";
                yield return new KeyValuePair<string, object>(pathAsString, value);
            }
        }
    }
}