namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;

    public static class HasIdentifiableChildNodeExtensions
    {
        /// <summary>
        /// Retrieves a descendant of the start node or throws KeyNotFoundException if not found
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <returns></returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> key)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAt(tryGetChildNode, key);
        }

        /// <summary>
        /// Retrieves a descendant of the start node or throws KeyNotFoundException if not found.
        /// The strategy how to retrieve a child node form a parent node is specified by the tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <param name="tryGetChildNode">child retrieval strategy</param>
        /// <returns></returns>
        //public static TNode DescendantAt<TKey, TNode>(this TNode startNode, TKey key, TryGetChildNodeDelegate<TKey, TNode> tryGetChildNode)
        //{
        //    TNode childNode;
        //    if (tryGetChildNode(startNode, key, out childNode))
        //        return childNode;

        //    throw new KeyNotFoundException(string.Format("Key not found:'{0}'", key));
        //}

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> key, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            HierarchyPath<TKey> foundAncestor;
            return startNode.DescendantAtOrDefault(key, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <param name="foundKey">Contains the treekey of the node where the search stopped</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> key, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAtOrDefault(tryGetChildNode, key, out foundKey, createDefault);
        }
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HasIdentifiableChildNodeExtensions
    {
        /// <summary>
        /// Retrieves a descendant of the start node or throws KeyNotFoundException if not found. The child nodes are retrieved with the specified tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <returns></returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> key)
        {
            var keyArray = key.Items.ToArray();
            TNode childNode = startNode;
            for (int i = 0; i < keyArray.Length; i++)
                if (!tryGetChildNode(childNode, keyArray[i], out childNode))
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", keyArray.Take(i + 1))}'");

            return childNode;
        }

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="key"></param>
        /// <param name="tryGetChildNode">delegate to retrieve a child node by specified key</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> key, Func<TNode> createDefault = null)
        {
            var foundAncestor = HierarchyPath.Create<TKey>();
            return startNode.DescendantAtOrDefault(tryGetChildNode, key, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="key">hierarchy key to search</param>
        /// <param name="tryGetChildNode">child retrieval strategy</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> key, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
        {
            foundKey = HierarchyPath.Create<TKey>();
            TNode childNode = startNode;
            var keyItems = key.Items.ToArray();
            for (int i = 0; i < keyItems.Length && childNode != null; i++)
                if (!tryGetChildNode(childNode, keyItems[i], out childNode))
                    return (createDefault ?? (() => default(TNode)))();
                else
                    foundKey = foundKey.Join(keyItems[i]); // add current key to 'found' path

            return childNode;
        }
    }
}