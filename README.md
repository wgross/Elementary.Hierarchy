# Elementary.Hierarchy

Provides basic functionality to create tree data structures. 
This includes tree traversal and identfication of tree nodes with path-like identifiers.

## Installation 

Elementary.Hierachy is easily installed using NuGet
```
Install-Package Elementary.Hierarchy
```
## Usage

Elementary.Hierachy knows how to traverse two kinds of trees. The first kind is bilt from nodes which implemen the interfaces defined by Elementary.Hierarchy. Each of these interfaces enables traversal agorithms on every node: 

* _IHasParentNode_ enables: Parent() and Ancestors()
* _IHasChildNodes_ enables: Children() and Descendants(depthFirst:{true|false})
* _IHasIdentifieableChildNodes_ : DescandantAt(), DescendAlongPath()
* _IHasChildNodes_ and _IHasParentNode_ : FollowingSibings() and PrecedingSiblings()

All traversal algorithms are implemented in a variant which doesn't rely on the interfaces for traversal: Instead you defines how to reach parents or children using a delegate. An Example:

```csharp

// define a tree with delegates

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

### Identify a node in a hierarchy

A node in a hierarchy might be identifified with a path like combination of ids which are unique within a parent nodes collection of child nodes only.
To support this use case Elementary.Hierarchy provides the class HierarchyPath<T>.

```csharp
// id of a node under the root node having the id 'a'
var id = HierarchyPath.Create("a");

// id of a node under the root node having the id 'a/b'
var id = HierarchyPath.Create("a","b");

// root id of a hierarchy using string based keys 
var rootId = HierachyPath.Create<string>();
```

