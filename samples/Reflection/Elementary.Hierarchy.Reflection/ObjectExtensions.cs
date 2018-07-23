using Elementary.Hierarchy.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public static bool DeepEquals(this object left, object right)
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

        public static DeepCompareResult DeepCompare(this object left, object right)
        {
            if (ReferenceEquals(left, right))
                return new DeepCompareResult();

            var leftLeafEnumerator = left.Flatten().GetEnumerator();
            var rightLeaves = right.Flatten().ToDictionary(kv => kv.Key);
            var compareResult = new DeepCompareResult();

            foreach (var leftLeaf in left.Flatten())
            {
                if (rightLeaves.TryGetValue(leftLeaf.Key, out var rightLeaf))
                {
                    DeppCompareLeaves(leftLeaf, rightLeaf, compareResult);
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

        private static void DeppCompareLeaves(KeyValuePair<string, object> leftLeaf, KeyValuePair<string, object> rightLeaf, DeepCompareResult compareResult)
        {
            if (!EqualityComparer<Type>.Default.Equals(leftLeaf.Value.GetType(), rightLeaf.Value.GetType()))
                compareResult.DifferentTypes.Add(leftLeaf.Key);

            if (!EqualityComparer<object>.Default.Equals(leftLeaf.Value, rightLeaf.Value))
                compareResult.DifferentValues.Add(leftLeaf.Key);
        }

        public static HierarchyPath<string> PropertyPath<TRoot>(this TRoot root, Expression<Func<TRoot, object>> path)
        {
            return HierarchyPath.Create(PathSegments<TRoot>(root, path).Reverse());
        }

        public static IEnumerable<string> PathSegments<T>(T instance, Expression<Func<T, object>> access)
        {
            Expression current = access.Body;

            while (current != null)
            {
                if (current is UnaryExpression && current.NodeType == ExpressionType.Convert)
                {
                    current = ((UnaryExpression)current).Operand;
                }
                else if (current is MemberExpression)
                {
                    var currentMemberExpression = current as MemberExpression;
                    yield return currentMemberExpression.Member.Name;
                    current = currentMemberExpression.Expression;
                }
                else if (current is BinaryExpression)
                {
                    var currentBinaryExpression = current as BinaryExpression;
                    yield return ((ConstantExpression)currentBinaryExpression.Right).Value.ToString();
                    current = currentBinaryExpression.Left;
                }
                else if (current is ParameterExpression)
                {
                    var parameterExpression = current as ParameterExpression;
                    current = null;
                }
            }

            yield break;
        }
    }
}