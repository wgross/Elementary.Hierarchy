using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elementary.Hierarchy.Generic;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    [TestFixture]
    public class GenericNodeParentAndChildNodesFollowingSiblingTest
    {
        private IEnumerable<string> GetChildNodes(string rootNode)
        {
            switch (rootNode)
            {
                case "rootNode":
                    return new[] { "leftNode", "rightNode" };

                case "leftNode":
                    return Enumerable.Empty<string>();

                case "rightNode":
                    return new[] { "rightLeaf1", "rightLeaf2", "rightLeaf3" };
            }
            throw new InvalidOperationException();
        }

        private string GetParent(string startNode)
        {
            switch (startNode)
            {
                case "rootNode":
                    return null;

                case "leftNode":
                case "rightNode":
                    return "rootNode";

                case "rightLeaf1":
                case "rightLeaf2":
                case "rightLeaf3":
                    return "rightNode";
            }
            throw new InvalidOperationException();
        }

        [Test]
        public void D_root_node_has_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rootNode".FollowingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.IsFalse(result.Any());
        }

        [Test]
        public void D_node_returns_right_sibling_on_FollowingSiblings()
        {
            // ACT

            string[] result = "leftNode".FollowingSiblings(this.GetParent, this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreSame("rootNode", "leftNode".Parent(this.GetParent));
            Assert.AreSame("leftNode", "rootNode".Children(this.GetChildNodes).ElementAt(0));
            Assert.AreSame("rightNode", "rootNode".Children(this.GetChildNodes).ElementAt(1));
            Assert.AreEqual(1, result.Count());
            Assert.AreSame("rightNode", result.Single());
        }

        [Test]
        public void D_node_returns_no_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rightNode".FollowingSiblings(this.GetParent,this.GetChildNodes).ToArray();

            // ASSERT

            Assert.AreSame("rootNode", "leftNode".Parent(this.GetParent));
            Assert.AreSame("leftNode", "rootNode".Children(this.GetChildNodes).ElementAt(0));
            Assert.AreSame("rightNode", "rootNode".Children(this.GetChildNodes).ElementAt(1));
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void D_node_returns_all_siblings_on_FollowingSiblings()
        {
            // ACT

            string[] result = "rightLeaf1".FollowingSiblings(this.GetParent,this.GetChildNodes).ToArray();

            // ASSERT
            
            Assert.AreSame("rightNode", "rightLeaf1".Parent(this.GetParent));
            Assert.AreSame("rightLeaf1", "rightNode".Children(this.GetChildNodes).ElementAt(0));
            Assert.AreSame("rightLeaf2", "rightNode".Children(this.GetChildNodes).ElementAt(1));
            Assert.AreSame("rightLeaf3", "rightNode".Children(this.GetChildNodes).ElementAt(2));
            Assert.AreEqual(2, result.Count());
            Assert.AreSame("rightLeaf2", result.ElementAt(0));
            Assert.AreSame("rightLeaf3", result.ElementAt(1));
        }
    }
}
