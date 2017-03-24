using System;
using System.Collections.Generic;
using System.Text;

namespace Elementary.Hierarchy.Collections.Operations
{
    public interface IHierarchyValueReader<TValue>
    {
        bool TryGetValue(out TValue value);
    }
}
