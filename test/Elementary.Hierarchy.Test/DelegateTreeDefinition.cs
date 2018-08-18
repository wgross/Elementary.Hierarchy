using System.Collections.Generic;
using System.Linq;

namespace Elementary.Hierarchy.Test
{
    public class DelegateTreeDefinition
    {
        public static IEnumerable<string> GetChildNodes(string node)
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf
            //
            // unkown node -> {}

            switch (node)
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

        public static IEnumerable<string> GetChildNodes2(string node)
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /
            //     leftLeaf
            //
            // unkown node -> {}

            switch (node)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return new[] { "leftLeaf" };
            }
            return Enumerable.Empty<string>();
        }

        public static (bool, string) TryGetChildNode(string node, string childKey)
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf
            //
            // unkown node -> (false,null)

            var nodeMap = new Dictionary<(string, string), string>
            {
                { ("rootNode","leftNode"), "leftNode" },
                { ("rootNode","rightNode"), "rightNode" },
                { ("leftNode","leftLeaf"), "leftLeaf" },
                { ("rightNode","leftRightLeaf"), "leftRightLeaf" },
                { ("rightNode","rightRightLeaf"), "rightRightLeaf" }
            };

            return nodeMap.TryGetValue((node, childKey), out var child) ? (true, child) : (false, null);
        }

        public static (bool, string) TryGetParentNode(string node)
        {
            //                rootNode
            //                /      \
            //        leftNode        rightNode
            //           /            /       \
            //     leftLeaf    leftRightLeaf  rightRightLeaf
            //
            // unkown node -> (false,null)

            var nodeMap = new Dictionary<string, string>
            {
                { "leftNode", "rootNode" },
                { "rightNode", "rootNode" },
                { "leftLeaf", "leftNode" },
                { "leftRightLeaf", "rightNode" },
                { "rightRightLeaf", "rightNode" }
            };

            return nodeMap.TryGetValue(node, out var parent) ? (true, parent) : (false, null);
        }
    }
}