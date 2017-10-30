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
                        Create("leftLeaf", 4)),
                    Create("rightNode", 3,
                        Create("leftRightLeaf", 5), Create("rightRightLeaf", 6)))
            };
        }
    }
}