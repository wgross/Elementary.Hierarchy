using System.Collections.Generic;
using System.Linq;
using Elementary.Hierarchy.Generic;

namespace Elementary.Hierarchy.Reflection
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> FlattenAsDictionary(this object root)
        {
            var h = ReflectedHierarchy.Create(root);
            var flatted_h = new Dictionary<string, object>();

            h.VisitDescendants((path, n) =>
            {
                var (success, value) = n.TryGetValue<object>();

                flatted_h.Add(string.Join(":", string.Join(":", path.Select(p => p.Id), n.Id)), value);
            });

            return flatted_h;
        }
    }
}