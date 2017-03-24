using System;
using System.Collections.Generic;
using System.Text;

namespace Elementary.Hierarchy.Collections.Operations
{
    public interface IHierarchyValueWriter<TValue>
    {
        void SetValue(TValue value);

        bool TryGetValue(out TValue value);

        bool RemoveValue();
    }
}
