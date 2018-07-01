using System;
using System.Collections.Generic;
using System.Text;

namespace Elementary.Hierarchy.Collections.Nodes
{
    public interface IHierarchyValueReader<TValue>
    {
        bool TryGetValue(out TValue value);
    }
}
