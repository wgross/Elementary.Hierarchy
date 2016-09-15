# Elementary.Hierarchy

Provides basic functionality to create tree data structures. 
This includes tree traversal and identfication of tree nodes with path-like identifiers.

## Installation 

Elementary.Hierachy is easily installed using NuGet
```
Install-Package Elementary.Hierarchy
```
## Hierarchy Traversal

Elementary.Hierarchy knows how to traverse two kinds of trees. The first kind is build from nodes which implement the interfaces defined by Elementary.Hierarchy. Each of these interfaces enables traversal agorithms on every node: 

* __IHasParentNode__ enables _Parent()_ and _Ancestors()_
* __IHasChildNodes__ enables _Children()_ and _Descendants(depthFirst:{true|false})_
* __IHasIdentifieableChildNodes__ enables _DescendantAt()_, _DescendAlongPath()_
* __IHasChildNodes__ and __IHasParentNode__ together enables _FollowingSibings()_ and _PrecedingSiblings()_

All traversal algorithms are implemented in a variant which doesn't rely on the interfaces for traversal: Instead you define how to reach parents or children of a node using a delegate. An Example:

```csharp
using Elementary.Hierarchy.Generic

// define a tree with delegate

IEnumerable<string> GetChildNodes(string rootNode)
{
    switch (rootNode)
    {
        case "rootNode":
            return new[] { "leftNode", "rightNode" };

        case "leftNode":
            return new[] { "leftLeaf" };

        case "rightNode":
            return new[] { "leftRightLeaf", "rightRightLeaf" };
    }
    return Enumerable.Empty<string>();
}

// traverse the descandants 

"rootNode".Descendants(GetChildNodes, depthFirst:false);
```

More examples of this approach this can be found in the test cases at [Elementary.Hierarchy.Test/TraverseWithDelegates](https://github.com/wgross/Elementary.Hierarchy/tree/master/Elementary.Hierarchy.Test/TraverseWithDelegates).

## Identify a node in a Hierarchy

A node in a hierarchy might be identifified with a path like collection of ids. Each of these id indentifies a child node under a parent node.

To support this use case Elementary.Hierarchy provides the class HierarchyPath<T>.

```csharp
using Elementary.Hierarchy; 

// id of a node under the root node having the id 'a'
var id = HierarchyPath.Create("a");

// id of a node under the root node having the id 'a/b'
var id = HierarchyPath.Create("a","b");

// root id of a hierarchy using string based keys 
var rootId = HierarchyPath.Create<string>();
```

