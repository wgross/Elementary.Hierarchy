using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary.Hierarchy.Collections.Test
{
    public class HierarchyVariantSourceWithoutDefaultValue : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()

        {
            yield return new object[] { new ImmutableHierarchy<string, string>() };
            yield return new object[] { new MutableHierarchy<string, string>() };
            yield return new object[] { new MutableHierarchyEx<string, string>() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class HierarchyVariantSourceWithDefaultValue : IEnumerable<object[]>
    {
        public static readonly string DefaultValue = "default value";

        public IEnumerator<object[]> GetEnumerator()

        {
            yield return new object[] { new ImmutableHierarchy<string, string>(getDefaultValue: k => DefaultValue) };
            yield return new object[] { new MutableHierarchy<string, string>(getDefaultValue: k => DefaultValue) };
            yield return new object[] { new MutableHierarchyEx<string, string>(getDefaultValue: k => DefaultValue) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}