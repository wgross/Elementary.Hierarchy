using Elementary.Hierarchy.Abstractions;
using Moq;
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
        public void HierarchyWriter_deletes_child_if_Visit_returns_null()
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

            writer
                .Setup(w => w.Visit(child.Object))
                .Returns((IHierarchyNodeWriter<NodeType>)null);

            // ACT

            writer.Object.Visit(parent.Object);

            // ASSERT

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(child.Object));

            // visitor doesn't descenad further after child is reached.
            child.Verify(n => n.HasChildNodes, Times.Never());
            child.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void HierarchyWriter_visits_next_child_after_remove()
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

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

            var writer = new Mock<HierarchyWriter<NodeType>>();

            writer // call base method for parent
                .Setup(w => w.Visit(parent.Object))
                .CallBase();

            writer // calls base method for child2
                .Setup(w => w.Visit(child2.Object))
                .CallBase();

            writer // returns different node than child node
                .Setup(w => w.Visit(child1.Object))
                .Returns((IHierarchyNodeWriter<NodeType>)null);

            // ACT

            writer.Object.Visit(parent.Object);

            // ASSERT

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.RemoveChild(child1.Object), Times.Once());

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Once());
            child2.Verify(n => n.ChildNodes, Times.Never());
        }

        [Fact]
        public void HierarchyWriter_replaces_child_if_Visit_returns_different_writer_instance()
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
                .Returns(new Mock<IHierarchyNodeWriter<NodeType>>().Object);

            // ACT

            writer.Object.Visit(parent.Object);

            // ASSERT

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.ReplaceChild(child.Object, It.Is<IHierarchyNodeWriter<NodeType>>(c => !object.ReferenceEquals(c, child.Object))), Times.Once());

            // visitor doesn't descend further after child is reached.
            child.Verify(n => n.HasChildNodes, Times.Never());
            child.Verify(n => n.ChildNodes, Times.Never());
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

            var parent = new Mock<IHierarchyNodeWriter<NodeType>>();
            parent
                .Setup(n => n.HasChildNodes)
                .Returns(true);

            parent
                .Setup(n => n.ChildNodes)
                .Returns(new[] { child1.Object, child2.Object });

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

            writer.Object.Visit(parent.Object);

            // ASSERT

            parent.Verify(n => n.HasChildNodes, Times.Once());
            parent.Verify(n => n.ChildNodes, Times.Once());
            parent.Verify(n => n.ReplaceChild(child1.Object, It.Is<IHierarchyNodeWriter<NodeType>>(c => !object.ReferenceEquals(c, child1.Object))), Times.Once());

            // visitor doesn't descend further into child1
            child1.Verify(n => n.HasChildNodes, Times.Never());
            child1.Verify(n => n.ChildNodes, Times.Never());

            // vistor decends into child2
            child2.Verify(n => n.HasChildNodes, Times.Once());
            child2.Verify(n => n.ChildNodes, Times.Never());
        }

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
        }
    }
}