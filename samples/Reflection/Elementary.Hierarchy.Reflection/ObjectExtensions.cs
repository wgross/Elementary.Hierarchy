using Elementary.Hierarchy.Generic;
using System;
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

        public static bool EqualsDeep(this object left, object right)
        {
            if (ReferenceEquals(left, right))
                return true;

            var leftLeafEnumerator = left.Flatten().GetEnumerator();
            var rightLeafEnumerator = right.Flatten().GetEnumerator();

            (bool left, bool right) moveNextInStreams = (leftLeafEnumerator.MoveNext(), rightLeafEnumerator.MoveNext());
            do
            {
                if (!StringComparer.InvariantCulture.Equals(leftLeafEnumerator.Current.Key, rightLeafEnumerator.Current.Key))
                    return false;

                if (!EqualityComparer<Type>.Default.Equals(leftLeafEnumerator.Current.Value.GetType(), rightLeafEnumerator.Current.Value.GetType()))
                    return false;

                if (!EqualityComparer<object>.Default.Equals(leftLeafEnumerator.Current.Value, rightLeafEnumerator.Current.Value))
                    return false;

                moveNextInStreams = (leftLeafEnumerator.MoveNext(), rightLeafEnumerator.MoveNext());
                if (moveNextInStreams.left != moveNextInStreams.right)
                    return false;
            }
            while (moveNextInStreams.left && moveNextInStreams.right);

            return true;
        }

        public static CompareDeepResult CompareDeep(this object left, object right)
        {
            if (ReferenceEquals(left, right))
                return new CompareDeepResult();

            var leftLeafEnumerator = left.Flatten().GetEnumerator();
            var rightLeaves = right.Flatten().ToDictionary(kv => kv.Key);
            var compareResult = new CompareDeepResult();

            foreach (var leftLeaf in left.Flatten())
            {
                if (rightLeaves.TryGetValue(leftLeaf.Key, out var rightLeaf))
                {
                    CompareDeepLeafPair(leftLeaf, rightLeaf, compareResult);
                    rightLeaves.Remove(leftLeaf.Key);
                }
                else
                {
                    compareResult.LeftLeafIsMissing.Add(leftLeaf.Key);
                }
            }

            // add all uncompared left proerties to the result object
            return rightLeaves.Aggregate(compareResult, (cr, kv) =>
            {
                cr.RightLeafIsMissing.Add(kv.Key);
                return cr;
            });
        }

        private static void CompareDeepLeafPair(KeyValuePair<string, object> leftLeaf, KeyValuePair<string, object> rightLeaf, CompareDeepResult compareResult)
        {
            if (!EqualityComparer<Type>.Default.Equals(leftLeaf.Value.GetType(), rightLeaf.Value.GetType()))
                compareResult.DifferentTypes.Add(leftLeaf.Key);

            if (!EqualityComparer<object>.Default.Equals(leftLeaf.Value, rightLeaf.Value))
                compareResult.DifferentValues.Add(leftLeaf.Key);
        }
    }
}