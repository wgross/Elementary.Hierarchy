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
            yield return new object[] { new MutableHierarchyEx<string, string>() };
            yield return new object[] { new ImmutableHierarchyEx<string, string>() };
            yield return new object[] { new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}