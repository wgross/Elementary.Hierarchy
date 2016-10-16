using Elementary.Hierarchy.Generic;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    [TestFixture]
    public class HasIdentifiableChildNodesVisitDescendantAtAndAncestorsTest
    {
        public interface MockableNodeType : IHasIdentifiableChildNodes<string, MockableNodeType>
        { }

        private Mock<MockableNodeType> startNode = new Mock<MockableNodeType>();

        [SetUp]
        public void ArrangeAllTests()
        {
            this.startNode = new Mock<MockableNodeType>();
        }

        [Test]
        public void I_visit_the_root_node_as_descendant_once_on_VisitDescandantAtAndAncestors()
        {
            // ACT

            MockableNodeType descendant = null;
            MockableNodeType ancestor = null;
            startNode.Object.VisitDescandantAtAndAncestors(HierarchyPath.Create<string>(), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a);

            // ASSERT

            Assert.AreSame(this.startNode.Object, descendant);
            Assert.IsNull(ancestor);
        }

        [Test]
        public void I_visit_a_roots_child_node_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var childNode = new Mock<MockableNodeType>().Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(nameof(childNode), out childNode))
                .Returns(true);

            // ACT

            MockableNodeType descendant = null;
            MockableNodeType ancestor = null;
            startNode.Object.VisitDescandantAtAndAncestors(HierarchyPath.Create(nameof(childNode)), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a);

            // ASSERT

            Assert.AreSame(childNode, descendant);
            Assert.AreSame(this.startNode.Object, ancestor);
        }

        [Test]
        public void I_visit_a_roots_grandchild_node_with_VisitDescandantAtAndAncestors()
        {
            // ARRANGE

            var grandChild = new Mock<MockableNodeType>().Object;

            var childNodeMock = new Mock<MockableNodeType>();
            childNodeMock
                .Setup(n => n.TryGetChildNode(nameof(grandChild), out grandChild))
                .Returns(true);

            var childNode = childNodeMock.Object;

            this.startNode
                .Setup(n => n.TryGetChildNode(nameof(childNode), out childNode))
                .Returns(true);

            // ACT

            MockableNodeType descendant = null;
            List<MockableNodeType> ancestors = new List<MockableNodeType>();
            startNode.Object.VisitDescandantAtAndAncestors(HierarchyPath.Create(nameof(childNode), nameof(grandChild)), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestors.Add(a));

            // ASSERT

            Assert.AreSame(grandChild, descendant);
            CollectionAssert.AreEqual(new[] { childNode, startNode.Object }, ancestors);
        }

        [Test]
        public void I_throws_ArgumentNullException_on_null_descandantVisitor_VisitDescandantAtAndAncestors()
        {
            // ACT & ASSERT

            List<MockableNodeType> ancestors = new List<MockableNodeType>();
            var result = Assert.Throws<ArgumentNullException>(() => startNode.Object.VisitDescandantAtAndAncestors(HierarchyPath.Create<string>(), visitDescendantAt: null, visitAncestors: a => ancestors.Add(a)));

            Assert.AreEqual("visitDescendantAt", result.ParamName);
        }

        [Test]
        public void I_throws_ArgumentNullException_on_null_ancestorVisitor_VisitDescandantAtAndAncestors()
        {
            // ACT & ASSERT

            MockableNodeType descendant = null;
            var result = Assert.Throws<ArgumentNullException>(() => startNode.Object
                .VisitDescandantAtAndAncestors(HierarchyPath.Create<string>(), visitDescendantAt: d => descendant = d, visitAncestors: null));

            Assert.AreEqual("visitAncestor", result.ParamName);
        }

        [Test]
        public void I_throws_KeyNotFoundException_on_invalid_path_on_VisitDescandantAtAndAncestors()
        {
            // ACT & ASSERT

            MockableNodeType descendant = null;
            MockableNodeType ancestor = null;
            var result = Assert.Throws<KeyNotFoundException>(() => startNode.Object.VisitDescandantAtAndAncestors(
                HierarchyPath.Create("childNode"), visitDescendantAt: d => descendant = d, visitAncestors: a => ancestor = a));

            Assert.That(result.Message.Contains("'childNode'"));
        }
    }
}