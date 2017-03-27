using System.Linq;
using Xunit;

namespace Elementary.Hierarchy.Collections.Test
{
    
    public class ImmutableHierarchyNodeTest
    {
        #region TryGet a nodes value

        [Fact]
        public void IMHN_TryGetValue_is_false_if_node_has_no_value()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id");

            // ACT

            string value;
            var result = node.TryGetValue(out value);

            // ASSERT

            Assert.False(result);
        }

        [Fact]
        public void IMHN_TryGetValue_is_true_if_node_has_value()
        {
            // ARRANGE

            var value = "value";
            var node = new ImmutableHierarchy<string, string>.Node("id", value);

            // ACT

            string value_result;
            var result = node.TryGetValue(out value_result);

            // ASSERT

            Assert.True(result);
            Assert.Same(value, value_result);
        }

        #endregion

        #region Set a nodes value 

        [Fact]
        public void IMHN_Setting_node_value_creates_a_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value");

            // ACT

            var result = node.SetValue("newValue");

            // ASSERT
            // no structural change -> no new node

            Assert.Same(node, result);
            Assert.Equal("newValue", result.value);
        }

        #endregion

        #region Add child nodes 

        [Fact]
        public void IMHN_Add_child_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("id2", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value");

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.NotSame(node, result);

            // old node is unchanged
            Assert.Equal(0, node.ChildNodes.Count());
            Assert.Equal("value", node.value);

            // new node contains child
            Assert.Equal(1, result.ChildNodes.Count());
            Assert.Same(childNode, result.ChildNodes.ElementAt(0));
            Assert.Equal("value2", result.ChildNodes.ElementAt(0).value);
        }

        [Fact]
        public void IMHN_Add_child_twice_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("id2", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value");
            node = node.AddChildNode(childNode);

            // ACT

            var result = node.AddChildNode(childNode);

            // ASSERT

            Assert.NotSame(node, result);

            // old node is unchanged
            Assert.Equal(1, node.ChildNodes.Count());
            Assert.Same(childNode, node.ChildNodes.ElementAt(0));

            // new node contains child twice.
            Assert.Same(childNode, result.ChildNodes.ElementAt(0));
            Assert.Same(childNode, result.ChildNodes.ElementAt(1));
        }

        [Fact]
        public void IMHN_Set_child_node_exchanges_child_and_creates_new_node()
        {
            // ARRANGE

            var childNode = new ImmutableHierarchy<string, string>.Node("child", "value2");
            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] { childNode });

            // ACT

            var childNode2 = new ImmutableHierarchy<string, string>.Node("child", "value");
            var result = node.SetChildNode(childNode2);

            // ASSERT

            Assert.NotSame(node, result);

            // old node is unchanged
            Assert.Equal(1, node.ChildNodes.Count());
            Assert.Same(childNode, node.ChildNodes.ElementAt(0));

            // new node contains the noew child node
            Assert.Equal(1, result.ChildNodes.Count());
            Assert.Same(childNode2, result.ChildNodes.ElementAt(0));
        }

        #endregion 

        #region Unset a nodes value

        [Fact]
        public void IMHN_Unset_value_of_node_doesnt_create_new_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child", "value2")
            });

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.Same(node, result);
            
            // new node has no value
            Assert.False(result.HasValue);
            Assert.Equal(1, result.ChildNodes.Count());
            Assert.Same(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }

        [Fact]
        public void IMHN_Unset_value_twice_doesnt_create_new_node()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child", "value2")
            });

            node = node.UnsetValue();

            // ACT

            var result = node.UnsetValue();

            // ASSERT

            Assert.Same(node, result);
        }

        [Fact]
        public void IMHN_Unset_value_of_parent_node_doesnt_create_node_because_prune_is_applied()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT

            Assert.NotSame(node, result);

            // old node remains unchanged
            Assert.True(node.HasValue);
            Assert.Equal(1, node.ChildNodes.Count());

            // new node has no value
            Assert.False(result.HasValue);
            Assert.Equal(0, result.ChildNodes.Count());
        }

        [Fact]
        public void IMHN_Unset_value_of_parent_node_doesnt_create_node_because_prune_isnt_applied()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child","value2")
            });

            // ACT

            var result = node.UnsetValue(prune: true);

            // ASSERT
            // no prune -> no structural change -> node remains the same

            Assert.Same(node, result);
            
            // new node has no value
            Assert.False(result.HasValue);
            Assert.Equal(1, result.ChildNodes.Count());
            Assert.Same(node.ChildNodes.ElementAt(0), result.ChildNodes.ElementAt(0));
        }

        #endregion

        [Fact]
        public void IMHN_Remove_child_returns_new_node_without_child()
        {
            // ARRANGE

            var node = new ImmutableHierarchy<string, string>.Node("id", "value", new[] {
                new ImmutableHierarchy<string,string>.Node("child","value2")
            });

            // ACT

            var result = node.RemoveChildNode(node.ChildNodes.Single());

            // ASSERT

            Assert.NotNull(result);
            Assert.True(result.HasValue);
            string value;
            Assert.True(result.TryGetValue(out value));
            Assert.Equal("value", value);
            Assert.Equal("id", result.key);
        }
    }
}