using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Elementary.Hierarchy.Reflection
{
    public class ReflectionNode : IHasChildNodes<ReflectionNode>, IHasIdentifiableChildNodes<string, ReflectionNode>
    {
        private class ReflectionLeafNode : ReflectionNode
        {
            private readonly object instance;

            public ReflectionLeafNode(object instance, PropertyInfo pi)
                : base(pi)
            {
                this.instance = instance;
            }

            public override (bool, T) TryGetValue<T>()
            {
                return (true, (T)((PropertyInfo)this.node).GetValue(this.instance));
            }

            public override bool TrySetValue<T>(T value)
            {
                ((PropertyInfo)this.node).SetValue(this.instance, value);
                return true;
            }
        }

        private readonly Type[] childTypes = new[]
        {
            typeof(char),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(string),
        };

        protected readonly object node;

        public ReflectionNode(object root)
        {
            this.node = root;
        }

        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.node.GetType().GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType.Equals(typeof(string)));

        public bool HasChildNodes => this.ChildPropertyInfos.Any();

        public IEnumerable<ReflectionNode> ChildNodes => this.ChildPropertyInfos.Select(pi => new ReflectionLeafNode(this.node, pi));

        public (bool, ReflectionNode) TryGetChildNode(string id)
        {
            var pi = this.ChildPropertyInfos.Where(p => p.Name.Equals(id)).FirstOrDefault();
            return (pi != null, new ReflectionLeafNode(this.node, pi));
        }

        public (bool, ReflectionNode) TryGetChildNode<T>(Expression<Func<T, object>> selector)
        {
            var expression = (MemberExpression)selector.Body;
            string name = expression.Member.Name;
            return this.TryGetChildNode(name);
        }

        public virtual (bool, T) TryGetValue<T>()
        {
            return (false, default(T));
        }

        public virtual bool TrySetValue<T>(T value)
        {
            return false;
        }
    }
}