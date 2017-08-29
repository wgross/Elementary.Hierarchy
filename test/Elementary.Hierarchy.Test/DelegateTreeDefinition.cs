using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elementary.Hierarchy.Test
{
    public class DelegateTreeDefinition
    {
        public static IEnumerable<string> GetChildNodes(string rootNode)
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf
            //
            // unkown node -> {}

            switch (rootNode)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return new[] { "leftLeaf" };

                case "rightNode":
                    return new[] { "leftRightLeaf", "rightRightLeaf" };
            }
            return Enumerable.Empty<string>();
        }
    }
}
