using Elementary.Hierarchy.Generic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    [TestFixture]
    public class GenericNodeLeavesTest
    {
        [Test]
        public void D_empty_root_returns_itself_on_Leaves()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = p => Enumerable.Empty<string>();

            // ACT

            IEnumerable<string> result = "root".Leaves(getChildNodes).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("root", result.ElementAt(0));
        }

        [Test]
        public void D_root_returns_its_descendant_leaves_on_Leaves()
        {
            // ARRANGE

            Func<string, IEnumerable<string>> getChildNodes = p =>
            {
                switch (p)
                {
                    case "root":
                        return new[] { "leftNode", "rightNode" };

                    case "leftNode":
                        return new[] { "leftLeaf" };

                    case "rightNode":
                        return new[] { "leftRightLeaf", "rightRightLeaf" };
                }
                return Enumerable.Empty<string>();
            };

            // ACT

            IEnumerable<string> result = "root".Leaves(getChildNodes).ToArray();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(new[] { "leftLeaf", "leftRightLeaf", "rightRightLeaf" }, result.ToArray());
        }
    }
}