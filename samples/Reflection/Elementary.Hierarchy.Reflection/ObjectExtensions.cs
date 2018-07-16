using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Reflection
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> Flatten(this object root)
        {
            var h = ReflectedHierarchy.Create(root, new FlattedObjectHierarchyNodeFactory());
            var flatted_h = new Dictionary<string, object>();

            h.VisitDescendants((path, n) =>
            {
                if (n.HasChildNodes)
                    return; // don't add inner nodes to dictionary

                var (success, value) = n.TryGetValue<object>();
                var pathOfParent = string.Join("/", path.Select(p => p.Id));

                flatted_h.Add(string.Join("/", pathOfParent, n.Id), value);
            });

            return flatted_h;
        }
    }
}