using Elementary.Hierarchy.Abstractions;
using Moq;
using System;
using Xunit;

namespace Elementary.Hierarchy.Test.Abstractions
{
    public class HierarchyWriterTest
    {
        public class NodeType
        { }

        [Fact]
        public void HierarchyWriter_visits_childnodes()
        {
            // ARRANGE

            var child = new Mock<IHierarchyNodeWriter<NodeType>>();
            child
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child.Object });

            var writer = new HierarchyWriter<NodeType>();

            // ACT

            writer.Visit(parent.Object);

            // ASSERT
            // after parent the c´hild is visited.

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            child.Verify(n => n.HasChildNodes, Times.Once());
            child.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void HierachyWriter_throws_on_null_node()
        {
            // ARRANGE

            var writer = new HierarchyWriter<NodeType>();

            // ACT & ASSERT

            var result = Assert.Throws<ArgumentNullException>(() => writer.Visit(null));

            // ASSERT

            Assert.Equal("node", result.ParamName);
        }

        #region RemoveChild

        [Fact]
        public void HierarchyWriter_deletes_child_if_Visit_returns_null()
        {
            // ARRANGE

            var child = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();
            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child.Object });

            parent // return a modified parent object having child1 removed
              .Setup(n => n.RemoveChild(child.Object))
              .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();
            writer
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer
                .Setup(w => w.Visit(child.Object))
                .Returns((IHierarchyNodeWriter<NodeType>)null);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterFirstChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(child.Object));
            parent.VerifyAll();

            // visitor doesn't descenad further after child is reached.
            child.Verify(n => n.HasChildNodes, Times.Never());
            child.Verify(n => n.ChildNodes, Times.Never());
            child.VerifyAll();
        }

        [Fact]
        public void HierarchyWriter_visits_next_child_after_remove()
        {
            // ARRANGE

            var child1 = new Mock<IHierarchyNodeWriter<NodeType>>();

            var child2 = new Mock<IHierarchyNodeWriter<NodeType>>();
            child2
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();
            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

            parent // return a modified parent object having child1 removed
                .Setup(n => n.RemoveChild(child1.Object))
                .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();

            writer // call base method for parent
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // calls base method for child2
                .Setup(w => w.Visit(child2.Object))
                .CallBase();

            writer // returns null node to indicate removal
                .Setup(w => w.Visit(child1.Object))
                .Returns((IHierarchyNodeWriter<NodeType>)null);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterFirstChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(child1.Object), Times.Once());
            parent.VerifyAll();

            parentAfterFirstChange.VerifyAll();

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());
            child1.VerifyAll();

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Once());
            child2.Verify(n => n.ChildNodes, Times.Never());
            child2.VerifyAll();
        }

        [Fact]
        public void HierarchyWriter_applies_second_change_to_modified_parent_node_on_remove()
        {
            // ARRANGE

            var child1 = new Mock<IHierarchyNodeWriter<NodeType>>();

            var child2 = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterSecondChange = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();
            parentAfterFirstChange
                .Setup(n => n.RemoveChild(child2.Object))
                .Returns(parentAfterSecondChange.Object);

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

            parent // on removal a new instance of the node writer is returned
                .Setup(n => n.RemoveChild(child1.Object))
                .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();

            writer // call base method for parent
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // calls base method for child2
                .Setup(w => w.Visit(child2.Object))
                .CallBase();

            writer // returns null to remove the child from parent
                .Setup(w => w.Visit(child1.Object))
                .Returns((IHierarchyNodeWriter<NodeType>)null);

            writer // returns null to remove the child from parent
               .Setup(w => w.Visit(child2.Object))
               .Returns((IHierarchyNodeWriter<NodeType>)null);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterSecondChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(child1.Object), Times.Once());

            parentAfterFirstChange.Verify(n => n.RemoveChild(child2.Object), Times.Once());
            parentAfterFirstChange.VerifyAll();

            parentAfterSecondChange.Verify(n => n.RemoveChild(It.IsAny<IHierarchyNodeWriter<NodeType>>()), Times.Never());
            parentAfterSecondChange.VerifyAll();

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());
            child1.VerifyAll();

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Never());
            child2.Verify(n => n.ChildNodes, Times.Never());
            child2.VerifyAll();
        }

        #endregion RemoveChild

        #region ReplaceChild

        [Fact]
        public void HierarchyWriter_replaces_child_if_Visit_returns_different_writer_instance()
        {
            // ARRANGE

            var child = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child.Object });

            parent // replacing the child creates a new parent node instance
                .Setup(n => n.ReplaceChild(child.Object, It.IsAny<IHierarchyNodeWriter<NodeType>>()))
                .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();
            writer
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // return different node than child node
                .Setup(w => w.Visit(child.Object))
                .Returns(new Mock<IHierarchyNodeWriter<NodeType>>().Object);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterFirstChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.ReplaceChild(child.Object, It.Is<IHierarchyNodeWriter<NodeType>>(c => !object.ReferenceEquals(c, child.Object))), Times.Once());
            parent.VerifyAll();
            parentAfterFirstChange.VerifyAll();

            // visitor doesn't descend further after child is reached.
            child.Verify(n => n.HasChildNodes, Times.Never());
            child.Verify(n => n.ChildNodes, Times.Never());
            child.VerifyAll();
        }

        [Fact]
        public void HierarchyWriter_visits_next_child_after_replace()
        {
            // ARRANGE

            var child1 = new Mock<IHierarchyNodeWriter<NodeType>>();
            child1
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var child2 = new Mock<IHierarchyNodeWriter<NodeType>>();
            child2
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();
            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

            parent // replacing the child creates a new parent node instance
                .Setup(n => n.ReplaceChild(child1.Object, It.IsAny<IHierarchyNodeWriter<NodeType>>()))
                .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();

            writer // call base method for parent
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // calls base method for child2
                .Setup(w => w.Visit(child2.Object))
                .CallBase();

            writer // returns different node than child node
                .Setup(w => w.Visit(child1.Object))
                .Returns(new Mock<IHierarchyNodeWriter<NodeType>>().Object);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterFirstChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.ReplaceChild(child1.Object, It.Is<IHierarchyNodeWriter<NodeType>>(c => !object.ReferenceEquals(c, child1.Object))), Times.Once());
            parent.VerifyAll();
            parentAfterFirstChange.VerifyAll();

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());
            parent.VerifyAll();

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Once());
            child2.Verify(n => n.ChildNodes, Times.Never());
            child2.VerifyAll();
        }

        [Fact]
        public void HierarchyWriter_applies_second_change_to_modified_parent_node_on_replace()
        {
            // ARRANGE

            var child1 = new Mock<IHierarchyNodeWriter<NodeType>>();

            var child2 = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterSecondChange = new Mock<IHierarchyNodeWriter<NodeType>>();

            var parentAfterFirstChange = new Mock<IHierarchyNodeWriter<NodeType>>();

            parentAfterFirstChange // the second incarnation of the parent returns third one after second change
                .Setup(n => n.ReplaceChild(child2.Object, It.IsAny<IHierarchyNodeWriter<NodeType>>()))
                .Returns(parentAfterSecondChange.Object);

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();

            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

            parent // replacing the child creates a new parent node instance
                .Setup(n => n.ReplaceChild(child1.Object, It.IsAny<IHierarchyNodeWriter<NodeType>>()))
                .Returns(parentAfterFirstChange.Object);

            var writer = new Mock<HierarchyWriter<NodeType>>();

            writer // call base method for parent
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // calls base method for child2
                .Setup(w => w.Visit(child2.Object))
                .Returns(parentAfterSecondChange.Object);

            writer // returns different node than child node
                .Setup(w => w.Visit(child1.Object))
                .Returns(parentAfterFirstChange.Object);

            // ACT

            var result = writer.Object.Visit(parent.Object);

            // ASSERT

            Assert.Same(parentAfterSecondChange.Object, result);

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.ReplaceChild(child1.Object, It.Is<IHierarchyNodeWriter<NodeType>>(c => !object.ReferenceEquals(c, child1.Object))), Times.Once());
            parent.VerifyAll();

            parentAfterFirstChange.Verify(n => n.ReplaceChild(child2.Object, It.IsAny<IHierarchyNodeWriter<NodeType>>()), Times.Once());
            parentAfterFirstChange.VerifyAll();

            parentAfterSecondChange.VerifyAll();

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());
            child1.VerifyAll();

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Never());
            child2.Verify(n => n.ChildNodes, Times.Never());
            child2.VerifyAll();
        }

        #endregion ReplaceChild

        [Fact]
        public void HierarchyWriter_doesnt_modify_if_Visit_returns_same_writer_instance()
        {
            // ARRANGE

            var child = new Mock<IHierarchyNodeWriter<NodeType>>();
            child
                .Setup(n => n.HasChildNodes)
                .Returns(false);

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child.Object });

            var writer = new Mock<HierarchyWriter<NodeType>>();
            writer
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            // return different node than child node
            writer
                .Setup(w => w.Visit(child.Object))
                .Returns(child.Object);

            // ACT

            writer.Object.Visit(parent.Object);

            // ASSERT

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(It.IsAny<IHierarchyNodeWriter<NodeType>>()), Times.Never());
            parent.Verify(n => n.ReplaceChild(It.IsAny<IHierarchyNodeWriter<NodeType>>(), It.IsAny<IHierarchyNodeWriter<NodeType>>()), Times.Never());
            parent.VerifyAll();
        }
    }
}