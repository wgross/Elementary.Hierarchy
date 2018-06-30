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
            public ReflectionLeafNode(PropertyInfo pi)
                : base(pi)
            {
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

        private readonly object node;

        public ReflectionNode(object root)
        {
            this.node = root;
        }

        private IEnumerable<PropertyInfo> ChildPropertyInfos => this.node.GetType().GetProperties().Where(p => childTypes.Contains(p.PropertyType));

        public bool HasChildNodes => this.ChildPropertyInfos.Any();

        public IEnumerable<ReflectionNode> ChildNodes => this.ChildPropertyInfos.Select(pi => new ReflectionLeafNode(pi));

        public (bool, ReflectionNode) TryGetChildNode(string id)
        {
            var pi = this.ChildPropertyInfos.Where(p => p.Name.Equals(id)).FirstOrDefault();
            return (pi != null, new ReflectionLeafNode(pi));
        }

        public (bool, ReflectionNode) TryGetChildNode<T>(Expression<Func<T, object>> selector)
        {
            var expression = (MemberExpression)selector.Body;
            string name = expression.Member.Name;
            return this.TryGetChildNode(name);
        }
    }
}