# Phase 2: Core MDI & Layout Engine Overhaul

> **Sprint:** 21-22 · **Priority:** Critical · **Complexity:** High
> **Dependency:** Phase 1 complete · **Estimated Effort:** 3-4 weeks

---

## Objective

Replace the current flat group system with a full binary tree layout engine supporting unlimited nested splits. This enables layouts like VS Code's multi-pane arrangements, Visual Studio's complex docking, and JetBrains' flexible split system.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Split nesting | Flat (max 4 groups) | Unlimited depth binary tree |
| Split types | Horizontal OR vertical (global) | Mixed (H within V within H...) |
| Splitter bars | Single shared bar | One per split node |
| Layout persistence | Flat group list | Full tree serialization |
| Layout restoration | Basic group restore | Full tree reconstruction |
| Empty group handling | Manual collapse | Auto-collapse with animation |
| Layout validation | None | Tree validation + auto-repair |
| Split ratio adjustment | Single ratio | Per-node ratios |

---

## Tasks

### Task 2.1: Implement Binary Tree Layout Nodes

**Files to Create/Modify:**

```
Layout/
├── ILayoutNode.cs                   ← Interface (existing, enhance)
├── SplitLayoutNode.cs               ← Binary split node (enhance)
├── GroupLayoutNode.cs               ← Leaf group node (enhance)
├── LayoutNodeVisitor.cs             ← Visitor pattern for tree traversal
├── LayoutTreeValidator.cs           ← Validate tree integrity
├── LayoutTreeRepairer.cs            ← Auto-repair invalid trees
└── LayoutNodeExtensions.cs          ← Helper extension methods
```

**`SplitLayoutNode` Structure:**

```csharp
public class SplitLayoutNode : ILayoutNode
{
    public string Id { get; }
    public SplitOrientation Orientation { get; }
    public double Ratio { get; set; }            // 0.0-1.0 (first child's share)
    public ILayoutNode FirstChild { get; set; }  // Left or Top
    public ILayoutNode SecondChild { get; set; } // Right or Bottom
    public double? MinSize { get; set; }
    public double? MaxSize { get; set; }
    public bool IsCollapsible { get; set; }
    public bool IsCollapsed { get; set; }
    public LayoutNodeType NodeType => LayoutNodeType.Split;
}
```

**`GroupLayoutNode` Structure:**

```csharp
public class GroupLayoutNode : ILayoutNode
{
    public string Id { get; }
    public List<string> DocumentIds { get; }     // Ordered document IDs
    public string ActiveDocumentId { get; set; }
    public TabStripPosition TabPosition { get; set; }
    public DocumentTabStyle TabStyle { get; set; }
    public LayoutNodeType NodeType => LayoutNodeType.Group;
}
```

### Task 2.2: Implement Layout Tree Builder

**File:** `Layout/LayoutTreeBuilder.cs`

**Requirements:**

- Traverse live `BeepDocumentHost` state
- Build `ILayoutNode` tree from current split/group topology
- Capture per-node ratios, orientations, document IDs
- Capture active document per group
- Handle edge cases (empty groups, single document)
- Return serializable tree

### Task 2.3: Implement Layout Tree Applier

**File:** `Layout/LayoutTreeApplier.cs`

**Requirements:**

- Take `ILayoutNode` tree and apply to live host
- Create/destroy groups as needed
- Set split ratios and orientations
- Move documents to correct groups
- Set active documents
- Validate tree before applying
- Fire appropriate events during application

### Task 2.4: Implement Layout Tree Validator

**File:** `Layout/LayoutTreeValidator.cs`

**Requirements:**

- Validate tree structure (no cycles, no orphaned nodes)
- Validate document IDs exist in host
- Validate ratios sum correctly (0.0-1.0)
- Validate minimum sizes are achievable
- Return validation report with errors/warnings
- Suggest repairs for invalid trees

### Task 2.5: Implement Layout Tree Repairer

**File:** `Layout/LayoutTreeRepairer.cs`

**Requirements:**

- Auto-repair common invalid states:
  - Remove empty leaf nodes
  - Collapse single-child split nodes
  - Normalize ratios after node removal
  - Merge adjacent splits with same orientation
- Return repair report listing all changes made
- Preserve as much of original layout as possible

### Task 2.6: Implement Splitter Bar System

**Changes:**

- Replace single `_splitterBar` with per-split-node splitter bars
- Each `SplitLayoutNode` gets its own draggable splitter
- Splitter bars support:
  - Drag to adjust ratio
  - Double-click to reset ratio to 0.5
  - Hover cursor change
  - Theme-colored appearance
  - DPI-scaled width
- Animated splitter movement (ease-out cubic, 200ms)

### Task 2.7: Implement Nested Split Layout

**Changes to `BeepDocumentHost.Layout.cs`:**

- Replace flat `_groups` list with tree-based layout
- `RecalculateLayout()` traverses tree and positions all nodes
- Each split node calculates child bounds based on ratio
- Each group node positions its tab strip and content area
- Handle resize events by re-traversing tree
- Support minimum group sizes (prevent collapse below threshold)

### Task 2.8: Update Layout Serialization

**Changes to `BeepDocumentHost.Serialisation.cs`:**

- Serialize full tree structure (not flat group list)
- Include per-node: orientation, ratio, min/max sizes
- Include per-group: document IDs, active document, tab settings
- Schema version bump to v3
- Migration service: v2 (flat groups) to v3 (tree)

**Migration Strategy:**

```
v2 Layout (flat groups):
{
  "groups": [
    { "documents": ["doc1", "doc2"], "active": "doc1" },
    { "documents": ["doc3"], "active": "doc3" }
  ],
  "splitOrientation": "Horizontal",
  "splitRatio": 0.5
}

v3 Layout (tree):
{
  "root": {
    "type": "split",
    "orientation": "Horizontal",
    "ratio": 0.5,
    "first": {
      "type": "group",
      "documents": ["doc1", "doc2"],
      "active": "doc1"
    },
    "second": {
      "type": "group",
      "documents": ["doc3"],
      "active": "doc3"
    }
  }
}
```

### Task 2.9: Implement Animated Group Collapse

**Requirements:**

- When a group becomes empty, animate its collapse
- 200ms ease-out cubic animation
- Animate splitter bar movement
- Animate content area resize
- Smooth visual transition (no flicker)
- Support both immediate and animated collapse modes

### Task 2.10: Update Split API

**New/Modified APIs:**

```csharp
// Split a specific document into a new pane
host.SplitDocumentHorizontal("doc-001");
host.SplitDocumentVertical("doc-001");

// Split at a specific position (creates nested split)
host.SplitDocumentAt("doc-001", SplitOrientation.Horizontal, SplitPosition.After);

// Merge a specific split node back
host.MergeSplitNode(splitNodeId);

// Merge all secondary groups back into primary
host.MergeAllGroups();

// Get the layout tree
ILayoutNode tree = host.GetLayoutTree();

// Apply a layout tree
host.ApplyLayoutTree(tree);

// Validate layout tree
ValidationReport report = LayoutTreeValidator.Validate(tree);
```

---

## Acceptance Criteria

- [ ] Unlimited nested split depth (tested to 10+ levels)
- [ ] Mixed orientations (H within V within H)
- [ ] Per-node splitter bars (draggable, themed, DPI-aware)
- [ ] Full tree serialization (schema v3)
- [ ] v2 to v3 migration works correctly
- [ ] Tree validation catches all invalid states
- [ ] Tree auto-repair fixes common issues
- [ ] Animated group collapse (200ms ease-out)
- [ ] Layout save/restore preserves full tree structure
- [ ] Minimum group sizes enforced
- [ ] Empty group auto-collapse works
- [ ] All existing split APIs still work
- [ ] New split APIs work correctly
- [ ] Layout tree builder captures live state accurately
- [ ] Layout tree applier restores state correctly

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Tree corruption during layout changes | Validator runs after every mutation |
| Performance degradation with deep trees | Optimize traversal; cache layout results |
| Migration from v2 fails for complex layouts | Comprehensive migration tests with real-world layouts |
| Splitter bar conflicts with nested splits | Each splitter scoped to its split node only |
| Layout restore creates invalid tree | Validate before apply; repair if needed |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentHost.cs` | Refactor (tree-based layout) |
| `BeepDocumentHost.Layout.cs` | Rewrite (tree traversal) |
| `BeepDocumentHost.Serialisation.cs` | Update (schema v3) |
| `BeepDocumentGroup.cs` | Refactor (tree node wrapper) |
| `Layout/ILayoutNode.cs` | Enhance (new members) |
| `Layout/SplitLayoutNode.cs` | Enhance (binary tree) |
| `Layout/GroupLayoutNode.cs` | Enhance (leaf node) |

## Files Created

| File | Purpose |
|------|---------|
| `Layout/LayoutNodeVisitor.cs` | Visitor pattern for tree traversal |
| `Layout/LayoutTreeValidator.cs` | Validate tree integrity |
| `Layout/LayoutTreeRepairer.cs` | Auto-repair invalid trees |
| `Layout/LayoutNodeExtensions.cs` | Helper extension methods |
| `Layout/LayoutTreeBuilder.cs` | Build tree from live state |
| `Layout/LayoutTreeApplier.cs` | Apply tree to live host |
