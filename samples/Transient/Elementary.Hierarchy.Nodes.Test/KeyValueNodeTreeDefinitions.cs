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
                RootNode<string, int>(0,
                    InnerNode("leftNode", 2,
                        InnerNode("leftLeaf", 4)),
                    InnerNode("rightNode", 3,
                        InnerNode("leftRightLeaf", 5), InnerNode("rightRightLeaf", 6)))
            };
        }
    }
}