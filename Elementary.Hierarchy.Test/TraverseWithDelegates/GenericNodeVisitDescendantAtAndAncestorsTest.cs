using Elementary.Hierarchy.Generic;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    [TestFixture]
    public class GenericNodeVisitDescendantAtAndAncestorsTest
    {
        [Test]
        public void D_visit_the_root_node_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string descendant = null;
            string ancestor = null; ;
            "startNode".VisitDescandantAtAndAncestors(nodeHierarchy,
                HierarchyPath.Create<string>(), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a);

            // ASSERT

            Assert.AreEqual("startNode", descendant);
            Assert.IsNull(ancestor);
        }

        [Test]
        public void D_visit_a_roots_child_node_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string descendant = null;
            string ancestor = null;
            "startNode".VisitDescandantAtAndAncestors(nodeHierarchy,
                HierarchyPath.Create("childNode"), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a);

            // ASSERT

            Assert.AreEqual("childNode", descendant);
            Assert.AreEqual("startNode", ancestor);
        }

        [Test]
        public void D_visit_a_roots_grandchild_node_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode" && key == "childNode")
                {
                    childNode = "childNode";
                    return true;
                }
                else if (node == "childNode" && key == "grandChild")
                {
                    childNode = "grandChild";
                    return true;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT

            string descendant = null;
            var ancestors = new List<string>();
            "startNode".VisitDescandantAtAndAncestors(nodeHierarchy,
                HierarchyPath.Create("childNode", "grandChild"), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestors.Add(a));

            // ASSERT

            Assert.AreEqual("grandChild", descendant);
            CollectionAssert.AreEqual(new[] { "childNode", "startNode" }, ancestors);
        }

        [Test]
        public void D_visit_a_roots_child_throws_on_invalid_path_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var nodeHierarchy = (TryGetChildNode<string, string>)(delegate (string node, string key, out string childNode)
            {
                if (node == "startNode")
                {
                    childNode = null;
                    return false;
                }

                throw new InvalidOperationException("unknown node");
            });

            // ACT & ASSERT

            string descendant = null;
            string ancestor = null;
            Assert.Throws<KeyNotFoundException>(() => "startNode".VisitDescandantAtAndAncestors(nodeHierarchy,
                HierarchyPath.Create("childNode"), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a));

            // ASSERT
        }
    }
}