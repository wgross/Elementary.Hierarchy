namespace Elementary.Hierarchy.Nodes.Test
{
    using System.Collections.Generic;
    using static KeyValueNode;

    public class KeyValueNodeTreeDefinitions
    {
        //                rootNode
        //                /      \
        //        leftNode        rightNode
        //           /            /       \
        //     leftLeaf    leftRightLeaf  rightRightLeaf
        //

        public static IEnumerable<object[]> Default()
        {
            yield return new object[]
            {
                new KeyValueNode<string, int>(0,
                    Create("leftNode", 2,
                        Node("leftLeaf", 4)),
                    Create("rightNode", 3,
                        Node("leftRightLeaf", 5), Node("rightRightLeaf", 6)))
            };
        }
    }
}