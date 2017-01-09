# Elementary.Hierarchy

Provides basic functionality to create tree data structures. 
This includes tree traversal and identfication of tree nodes with path-like identifiers.

## Installation 

Elementary.Hierarchy is easily installed using NuGet
```
Install-Package Elementary.Hierarchy
```
## Hierarchy Traversal

Elementary.Hierarchy knows how to traverse two kinds of trees. 
The first kind is build from nodes which implement the interfaces defined by Elementary.Hierarchy. Each of these interfaces enables traversal agorithms on every node: 

* __IHasParentNode__ enables _Parent()_ and _Ancestors()_
* __IHasChildNodes__ enables _Children()_ and _Descendants(depthFirst:{true|false})_
* __IHasIdentifieableChildNodes__ enables _DescendantAt()_, _DescendAlongPath()_
* __IHasChildNodes__ and __IHasParentNode__ together enables _FollowingSibings()_ and _PrecedingSiblings()_

A second flavor of implementation doesn't rely on interfaces but on a delegate providing the logic to traverse the child or parent axis of the tree. in other words: its not the node which nows is children pr parent but a delegate provides this information. By seperating the node itself from the tree structure a hierarchy can be implemented on any class or value type.
An Example implements a small tree with strings:

```csharp
using Elementary.Hierarchy.Generic

// define a tree with a delegate

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

More examples of this approach can be found in the test cases at [Elementary.Hierarchy.Test/TraverseWithDelegates](https://github.com/wgross/Elementary.Hierarchy/tree/master/Elementary.Hierarchy.Test/TraverseWithDelegates).

## Identify a node in a Hierarchy

A node in a hierarchy might be identifified with a path like collection of ids. Each of these ids identifiy a child node under a parent node.

To support this use case Elementary.Hierarchy provides the class HierarchyPath<T>. While there are traversal algorithms relying on the HierachyPath<T>, the class itself doesn't deoend from the interfaces or algorithms of 'Elementary.Hierarchy' and can be used as it other contexts.

```csharp
using Elementary.Hierarchy; 

// id of a node under the root node having the id 'a'
var id = HierarchyPath.Create("a");

// id of a node under the root node having the id 'a/b'
var id = HierarchyPath.Create("a","b");

// root id of a hierarchy using string based keys 
var rootId = HierarchyPath.Create<string>();
```

