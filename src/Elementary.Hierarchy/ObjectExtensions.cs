using System;
using System.Collections.Generic;
using System.Text;

namespace Elementary.Hierarchy
{
    internal static class ObjectExtensions
    {
        internal static IEnumerable<T> Yield<T>(this T instance)
        {
            yield return instance;
        }
    }
}
