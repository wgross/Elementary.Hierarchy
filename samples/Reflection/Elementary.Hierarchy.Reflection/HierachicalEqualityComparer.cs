using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Reflection
{
    public class HierarchicalObjectComparer
    {
        public static new bool Equals(object left, object right)
        {
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
    }
}