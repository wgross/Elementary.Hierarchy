# Elementary.Hierarchy

Provides basic functionality to create tree data structures. 
This includes tree traversal and identfication of tree nodes with path-like identifiers.

# Installation 

Elementary.Hierachy is easily installed using NuGet

Install-Package Elementary.Hierarchy

# Usage

## Identify a node in a hierarchy

A node in a hierarchy might be identifified with a path like comibiation of ids which are unique within a parent nodes collection of child nodes only.
To support this use case Elementary.Hierarchy provides the class HierarchyPath<T>.


```csharp
// id of a node under the root node having the id 'a'
var id = HierarchyPath.Create("a");

// id of a node under the root node having the id 'a/b'
var id = HierarchyPath.Create("a","b");

// root id of a hierarchy using string based keys 
HierachyPath.Create<string>()
```

