namespace Elementary.Hierarchy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Factory class for HierarchyPath instances.
    /// </summary>
    public class HierarchyPath
    {
        /// <summary>
        /// Creates a HierarchyPath<typeparamref name="T"/> instance. T is derived from the type of the path items
        /// specified by pathItems.
        /// </summary>
        /// <typeparam name="T">type of all path items</typeparam>
        /// <param name="pathItems">collection of items to build a path from</param>
        /// <returns></returns>
        public static HierarchyPath<T> Create<T>(params T[] pathItems)
        {
            if (pathItems == null)
                throw new ArgumentNullException(nameof(pathItems));

            return new HierarchyPath<T>(pathItems.ToArray());
        }

        /// <summary>
        /// Creates a HierarchyPath<typeparamref name="T"/> instance. T is derived from the type of the path items
        /// specified by pathItems.
        /// </summary>
        /// <typeparam name="T">type of all path items</typeparam>
        /// <param name="pathItems">enumerable of items to build a path from</param>
        /// <returns></returns>
        public static HierarchyPath<T> Create<T>(IEnumerable<T> pathItems)
        {
            if (pathItems == null)
                throw new ArgumentNullException(nameof(pathItems));

            return new HierarchyPath<T>(pathItems.ToArray());
        }

        /// <summary>
        /// Parses a HierarchyPath instance from its string representation. Type of the path items is a string by default.
        /// </summary>
        /// <param name="path">string representation of the HierarchyPath</param>
        /// <param name="separator">Separator betweeen the path items in the string representation</param>
        /// <returns></returns>
        public static HierarchyPath<string> Parse(string path, string separator)
        {
            return HierarchyPath.Create(path.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Parses a HierarchyPath instance from its string representation.
        /// Type of the path items is specified by the type parameter.
        /// </summary>
        /// <typeparam name="T">type of the path items after conversion</typeparam>
        /// <param name="path">string representation to parse</param>
        /// <param name="convertPathItem">conversion delegate applied to a single parsed path item</param>
        /// <param name="separator">seperator character between the path items in the string representation</param>
        /// <returns></returns>
        public static HierarchyPath<T> Parse<T>(string path, Func<string, T> convertPathItem, string separator = null)
        {
            string separatorSafe = separator ?? "\\";

            return HierarchyPath.Create<T>(path.Split(separatorSafe.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(p => convertPathItem(p)));
        }

        /// <summary>
        /// Tries to parses a HierarchyPath instance from its string representation.
        /// Type of the path items is specified by the type parameter.
        /// </summary>
        /// <typeparam name="T">type of the path items after conversion</typeparam>
        /// <param name="path">string representation to parse</param>
        /// <param name="convertPathItem">conversion delegate applied to a single parsed path item</param>
        /// <param name="separator">seperator character between the path items in the string representation</param>
        /// <param name="hierarchyPath">parsed result, if successful</param>
        /// <returns>true on success, false otherwise</returns>
        public static bool TryParse<T>(string path, out HierarchyPath<T> hierarchyPath, Func<string, T> convertPathItem, string separator)
        {
            try
            {
                hierarchyPath = Parse<T>(path, convertPathItem, separator);
            }
            catch
            {
                hierarchyPath = null;
            }

            return path != null;
        }
    }

    /// <summary>
    /// Represents a unique identifer of a node within a hierarchy.
    /// A HierarchyKey identifies at each level of the hierachy a child node of the current node by its unique name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("path={ToString()}")]
    public sealed class HierarchyPath<T> : HierarchyPath, IHasParentNode<HierarchyPath<T>>
    {
        #region Construction and initialization of this instance

        internal HierarchyPath(T[] pathItems)
        {
            this.Items = pathItems;
            this.HasParentNode = pathItems.Any();
            // has been tested as significant improvement for perf. if using this as key of a Dictionary
            unchecked
            {
                // a path item MAY be null. in thisn has don't call GetHashCode() but use 0 as the hashcode.
                this.hashCode = this.Items.Aggregate((int)2166136261, (hc, i) => hc * (int)16777619 ^ (i?.GetHashCode() ?? 0));
            }
        }

        private readonly int hashCode;

        #endregion Construction and initialization of this instance

        #region IHasParentNode<HierarchyPath<T>> Members

        public bool HasParentNode { get; private set; }

        public HierarchyPath<T> ParentNode
        {
            get
            {
                // TODO:PERF: O(n)+O(n-1)-> needs optimization
                return HierarchyPath.Create(this.Items.Take(this.Items.Count() - 1));
            }
        }

        #endregion IHasParentNode<HierarchyPath<T>> Members

        /// <summary>
        /// Return true is this instance soecifies a root noe path (without path items).
        /// </summary>
        public bool IsRoot => !(this.Items.Any());

        public bool IsAncestorOf(HierarchyPath<T> descendant)
        {
            var itemsOfAncestor = this.Items.ToArray();
            var itemsOfDescendant = descendant.Items.ToArray();

            if (itemsOfAncestor.Length >= itemsOfDescendant.Length)
                return false; // length of ancestor key must be longer then descendents key.

            int lengthOfMatchingPrefix = 0;
            while (lengthOfMatchingPrefix < itemsOfAncestor.Length && EqualityComparer<T>.Default.Equals(itemsOfAncestor[lengthOfMatchingPrefix], itemsOfDescendant[lengthOfMatchingPrefix]))
                lengthOfMatchingPrefix++;

            // all parts of this match the parts of 'of' -> this is prefix of 'of' key
            return lengthOfMatchingPrefix == itemsOfAncestor.Length;
        }

        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Returns the leaf (last item of this HierarchyPath) as a new HierarchyPath instance.
        /// </summary>
        /// <returns>a new HierarchyPathInstance</returns>
        public HierarchyPath<T> Leaf()
        {
            if (this.Items.Any())
                return HierarchyPath.Create(this.Items.Last());
            else
                return this;
        }

        /// <summary>
        /// Creates HierarchyPath instance containing all path items except the first.
        /// </summary>
        /// <returns></returns>
        public HierarchyPath<T> SplitDescendants()
        {
            return HierarchyPath.Create(this.Items.Skip(1));
        }

        #region Override object behaviour

        public override bool Equals(object other)
        {
            return this.Equals(other, EqualityComparer<T>.Default);
        }

        public bool Equals(object other, IEqualityComparer<T> comparer)
        {
            HierarchyPath<T> otherAsTreeKey = other as HierarchyPath<T>;

            if (otherAsTreeKey == null)
                return false; // wrong type

            if (object.ReferenceEquals(this, other))
                return true; // instances are same

            var thisItems = this.Items.ToArray();
            var thatItems = otherAsTreeKey.Items.ToArray();

            if (thisItems.Length != thatItems.Length)
                return false;

            for (int i = 0; i < thisItems.Length; i++)
                if (!comparer.Equals(thisItems[i], thatItems[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return string.Join("/", this.Items);
        }

        #endregion Override object behaviour

        /// <summary>
        /// Creates a new HierarchyPath instance with the given pathItems appended.
        /// </summary>
        /// <param name="pathItem">path item to append to this HierarchyPath instance</param>
        /// <returns>a new HierarchyPath instance, this instance remains unchanged</returns>
        public HierarchyPath<T> Join(T pathItem)
        {
            return HierarchyPath.Create(this.Items.Concat(new[] { pathItem }));
        }

        /// <summary>
        /// Creates a new HierarchyPath instance with the given HierarchyPath instance appended.
        /// </summary>
        /// <param name="otherPath">path to append to this HierarchyPath instance</param>
        /// <returns>a new HierarchyPath instance, this instance remains unchanged</returns>
        public HierarchyPath<T> Join(HierarchyPath<T> otherPath)
        {
            return HierarchyPath.Create(this.Items.Concat((IEnumerable<T>)otherPath.Items));
        }

        public HierarchyPath<T> RelativeToAncestor(HierarchyPath<T> toAncestor)
        {
            if (!toAncestor.Items.Any())
                return this; // relative to root node

            var result = this.Items.ToList();
            bool foundCommonParts = false;
            foreach (var keyPart in toAncestor.Items)
            {
                if (!result.Any())
                    return HierarchyPath.Create<T>(); // no difference at all

                if (EqualityComparer<T>.Default.Equals(result.First(), keyPart))
                {
                    result.RemoveAt(0);
                    foundCommonParts = true;
                }
            }
            if (!foundCommonParts)
                throw new InvalidOperationException("No common key parts");

            return HierarchyPath.Create(result.AsEnumerable());
        }
    }
}