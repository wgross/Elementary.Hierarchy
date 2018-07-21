using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Reflection
{
    public class DeepCompareResult
    {
        public bool AreEqual => !(this.DifferentValues.Any() || this.DifferentTypes.Any() || this.RightLeafIsMissing.Any() || this.LeftLeafIsMissing.Any());

        public IList<string> DifferentValues { get; } = new List<string>();

        public IList<string> DifferentTypes { get; } = new List<string>();

        public IList<string> RightLeafIsMissing { get; } = new List<string>();

        public IList<string> LeftLeafIsMissing { get; } = new List<string>();
    }
}