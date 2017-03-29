using Elementary.Hierarchy.Collections.LiteDb;
using LiteDB;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Elementary.Hierarchy.Collections.Test
{
    public class AllHierarchyVariantsWithoutDefaultValue : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()

        {
            yield return new object[] { new ImmutableHierarchy<string, string>() };
            yield return new object[] { new MutableHierarchy<string, string>() };
            yield return new object[] { new MutableHierarchyEx<string, string>() };
            yield return new object[] { new ImmutableHierarchyEx<string, string>() };
            yield return new object[] { new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class AllHierarchyVariantWithDefaultValue : IEnumerable<object[]>
    {
        public static readonly string DefaultValue = "default value";

        public IEnumerator<object[]> GetEnumerator()

        {
            yield return new object[] { new ImmutableHierarchy<string, string>(getDefaultValue: k => DefaultValue) };
            yield return new object[] { new MutableHierarchy<string, string>(getDefaultValue: k => DefaultValue) };
            yield return new object[] { new MutableHierarchyEx<string, string>(getDefaultValue: k => DefaultValue) };
            yield return new object[] { new ImmutableHierarchyEx<string, string>(getDefaultValue: k => DefaultValue) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}