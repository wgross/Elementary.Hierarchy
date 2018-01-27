using Elementary.Hierarchy.Collections.LiteDb;
using LiteDB;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Elementary.Hierarchy.Collections.Test
{
    public class InstancesOfAllHierarchyVariants : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()

        {
            yield return new object[] { new MutableHierarchy<string, string>() };
            yield return new object[] { new ImmutableHierarchy<string, string>() };
            yield return new object[] { new LiteDbHierarchy<string>(new LiteDatabase(new MemoryStream()).GetCollection("nodes")) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}