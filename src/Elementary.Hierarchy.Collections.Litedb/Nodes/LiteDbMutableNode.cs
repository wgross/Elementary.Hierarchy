using Elementary.Hierarchy.Collections.Nodes;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Elementary.Hierarchy.Collections.LiteDb.Nodes
{
    public class LiteDbMutableNode<TKey, TValue> : KeyValueNode<TKey, TValue>,
        IHierarchyNodeWriter<LiteDbMutableNode<TKey, TValue>>,
        IHasIdentifiableChildNodes<TKey, LiteDbMutableNode<TKey, TValue>>,
        IHasChildNodes<LiteDbMutableNode<TKey, TValue>>
    {
        #region Construction and initialization of this instance

        private Action onDocumentChanged;

        public LiteDbMutableNode(Action onDocumentChanged, TKey key)
            : base(key)
        {
        }

        public LiteDbMutableNode(Action onDocumentChanged)
        {
            this.onDocumentChanged = onDocumentChanged;
        }

        #endregion Construction and initialization of this instance

        public bool HasChildNodes => throw new NotImplementedException();

        public IEnumerable<LiteDbMutableNode<TKey, TValue>> ChildNodes => throw new NotImplementedException();

        public LiteDbMutableNode<TKey, TValue> AddChild(LiteDbMutableNode<TKey, TValue> newChild)
        {
            throw new NotImplementedException();
        }

        public LiteDbMutableNode<TKey, TValue> RemoveChild(LiteDbMutableNode<TKey, TValue> child)
        {
            throw new NotImplementedException();
        }

        public LiteDbMutableNode<TKey, TValue> ReplaceChild(LiteDbMutableNode<TKey, TValue> childToReplace, LiteDbMutableNode<TKey, TValue> newChild)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChildNode(TKey id, out LiteDbMutableNode<TKey, TValue> childNode)
        {
            throw new NotImplementedException();
        }
    }
}