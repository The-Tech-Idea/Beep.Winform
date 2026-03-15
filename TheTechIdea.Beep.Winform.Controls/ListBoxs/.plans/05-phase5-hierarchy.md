# Phase 5 — Hierarchical List: Flatten Children Tree, Indent, Expand/Collapse

**Priority**: High  
**Status**: Not Started  
**Depends on**: Phase 2 (token-based layout must be in place first)

## Problem

`SimpleItem` already has full hierarchy support in its model:
- `BindingList<SimpleItem> Children` — child items  
- `bool IsExpanded` — expand/collapse state  
- `SimpleItem ParentItem` — parent reference  
- `int ParentID` — parent identifier  
- `string ComposedID` — tree path (e.g., `root.child1.grandchild2`)

But **none of this is used by the list rendering pipeline**:

1. `BeepListBoxHelper.GetVisibleItems()` iterates only `_owner.ListItems` (top-level) — it never recurses into `Children`.
2. `BeepListBoxLayoutHelper.CalculateLayout()` has no concept of depth or indentation.
3. No expand/collapse chevron is drawn by any painter.
4. No keyboard support for ← (collapse) / → (expand).
5. `GroupedListPainter` checks `item.Children.Count > 0` but treats them as flat group headers — it doesn't show nested children.
6. Accessibility doesn't switch to `role="tree"` / `role="treeitem"` when hierarchy is active.

### Reference: Modern Tree-List Standards

**Material Design 3** — Navigation drawer / expandable list:
- Chevron/arrow indicator rotates 90° on expand
- Child items indented by 40dp from parent
- Parent items are bold or have distinct weight

**Fluent UI 2** — TreeView:
- 16px indent per level
- Disclosure triangle on leading edge
- `aria-expanded` on parent nodes

**Figma** — Auto-layout nested frames:
- Each nested level adds consistent left padding
- Collapse/expand is a clickable region, not the whole row

---

## Design

### Architecture: Flatten-on-Demand

Rather than building a separate TreeView control, we **flatten the hierarchy into the existing visible-items list** with depth metadata. This preserves compatibility with all 34 painters.

```
ListItems (top-level):
  ├─ Folder A (expanded)    → depth=0, hasChildren=true
  │   ├─ File 1             → depth=1, hasChildren=false
  │   └─ Subfolder B (collapsed) → depth=1, hasChildren=true
  └─ Folder C               → depth=0, hasChildren=true

Flattened visible list:
  [0] Folder A      depth=0  expanded=true   hasChildren=true
  [1]   File 1      depth=1  expanded=false  hasChildren=false
  [2]   Subfolder B depth=1  expanded=false  hasChildren=true  (children hidden)
  [3] Folder C      depth=0  expanded=false  hasChildren=true  (children hidden)
```

### Three-Zone Row Layout with Chevron

```
┌─────────────────────────────────────────────────────────┐
│ [indent] [▶] [✓] [🔍]  Item Title              [badge] │
│                         Sub-text line            [meta] │
└─────────────────────────────────────────────────────────┘

Leading zone (left):
  indent (depth × indentStep) → chevron → checkbox → icon

Content zone (center, flex):
  title + sub-text

Trailing zone (right):
  badge / metadata / shortcut
```

---

## Steps

### Step 5.1 — Add hierarchy tokens to ListBoxTokens

**File**: `Tokens/ListBoxTokens.cs`  
**Location**: After spacing section

Add:
```csharp
// ── Hierarchy / tree ──────────────────────────────────────────────────────

/// <summary>Horizontal indent per nesting level (px). MD3 uses 40dp; 24px is compact-friendly.</summary>
public const int IndentStepPerLevel = 24;

/// <summary>Expand/collapse chevron icon size (px).</summary>
public const int ChevronSize = 16;

/// <summary>Hit-target around chevron for click detection (px). Must be ≥ MinTouchTargetPx for touch.</summary>
public const int ChevronHitTarget = 28;

/// <summary>Maximum supported nesting depth. Prevents runaway recursion.</summary>
public const int MaxHierarchyDepth = 10;
```

---

### Step 5.2 — Add depth metadata to ListItemInfo

**File**: `Helpers/ListItemInfo.cs` (or wherever `ListItemInfo` is defined in the Models folder)

Add two fields:
```csharp
/// <summary>Nesting depth (0 = top-level). Used for indentation.</summary>
public int Depth { get; set; }

/// <summary>True if this item has children (shows chevron regardless of expand state).</summary>
public bool HasChildren { get; set; }

/// <summary>Rectangle for the expand/collapse chevron hit target.</summary>
public Rectangle ChevronRect { get; set; }
```

---

### Step 5.3 — Add ShowHierarchy property

**File**: `BeepListBox.Properties.cs`  
**Location**: After the Grouping region

Add:
```csharp
// ════════════════════════════════════════════════════════════════════════════
//  Hierarchy
// ════════════════════════════════════════════════════════════════════════════

#region Hierarchy

/// <summary>
/// When true, items with Children are rendered as an expandable tree.
/// Children are recursively flattened into the visible list with indentation.
/// </summary>
[Browsable(true)]
[Category("Behavior")]
[Description("Render items with Children as an expandable hierarchical list.")]
[DefaultValue(false)]
public bool ShowHierarchy
{
    get => _showHierarchy;
    set
    {
        if (_showHierarchy != value)
        {
            _showHierarchy = value;
            InvalidateLayoutCache();
        }
    }
}
private bool _showHierarchy;

/// <summary>Raised when an item is expanded.</summary>
public event EventHandler<ListBoxItemEventArgs>? ItemExpanded;

/// <summary>Raised when an item is collapsed.</summary>
public event EventHandler<ListBoxItemEventArgs>? ItemCollapsed;

protected virtual void OnItemExpanded(int index, SimpleItem item)
    => ItemExpanded?.Invoke(this, new ListBoxItemEventArgs(index, item));

protected virtual void OnItemCollapsed(int index, SimpleItem item)
    => ItemCollapsed?.Invoke(this, new ListBoxItemEventArgs(index, item));

#endregion
```

---

### Step 5.4 — Implement FlattenHierarchy() in BeepListBoxHelper

**File**: `Helpers/BeepListBoxHelper.cs`  
**Location**: Add new method, modify `GetVisibleItems()`

Add recursive flattening:
```csharp
/// <summary>
/// Recursively flattens items with their expanded children into a linear list.
/// Each item is tagged with its depth for indentation.
/// </summary>
private List<SimpleItem> FlattenHierarchy(IEnumerable<SimpleItem> items, int depth = 0)
{
    var result = new List<SimpleItem>();
    if (depth > ListBoxTokens.MaxHierarchyDepth) return result;

    foreach (var item in items)
    {
        if (!item.IsVisible) continue;

        // Store depth as a transient tag for layout (avoids adding fields to SimpleItem)
        item.Tag = item.Tag; // preserve existing Tag — we'll use a separate mechanism
        _itemDepthMap[item] = depth;

        result.Add(item);

        // Recurse into expanded children
        if (item.IsExpanded && item.Children != null && item.Children.Count > 0)
        {
            result.AddRange(FlattenHierarchy(item.Children, depth + 1));
        }
    }
    return result;
}

// Depth lookup for layout helper
private readonly Dictionary<SimpleItem, int> _itemDepthMap = new();

/// <summary>Gets the hierarchy depth for an item (0 = top-level).</summary>
internal int GetItemDepth(SimpleItem item)
{
    return _itemDepthMap.TryGetValue(item, out int d) ? d : 0;
}

/// <summary>Returns true if an item has children (for chevron rendering).</summary>
internal bool ItemHasChildren(SimpleItem item)
{
    return item.Children != null && item.Children.Count > 0;
}
```

Modify `GetVisibleItems()`:
```csharp
public List<SimpleItem> GetVisibleItems()
{
    if (_owner.ListItems == null || _owner.ListItems.Count == 0)
        return new List<SimpleItem>();

    var items = _owner.ListItems.ToList();

    // Flatten hierarchy if enabled
    if (_owner.ShowHierarchy)
    {
        _itemDepthMap.Clear();
        items = FlattenHierarchy(items);
    }

    // Apply search filter
    if (_owner.ShowSearch && !string.IsNullOrWhiteSpace(_owner.SearchText))
    {
        items = _owner.ShowHierarchy
            ? FilterHierarchyBySearch(items, _owner.SearchText)
            : items.Where(i => IsSearchMatch(i, _owner.SearchText)).ToList();
    }

    if (_owner.ShowGroups && !_owner.ShowHierarchy)
    {
        return BuildGroupedRows(items);
    }

    return items;
}
```

Add hierarchy-aware search filter:
```csharp
/// <summary>
/// Filters a flattened hierarchy list, keeping items that match AND their ancestors.
/// </summary>
private List<SimpleItem> FilterHierarchyBySearch(List<SimpleItem> flatItems, string query)
{
    var matching = new HashSet<SimpleItem>();

    // Find all matching items
    foreach (var item in flatItems)
    {
        if (IsSearchMatch(item, query))
        {
            matching.Add(item);
            // Walk up to root to keep ancestors visible
            var parent = item.ParentItem;
            while (parent != null)
            {
                matching.Add(parent);
                parent = parent.ParentItem;
            }
        }
    }

    return flatItems.Where(i => matching.Contains(i)).ToList();
}
```

---

### Step 5.5 — Add indentation to BeepListBoxLayoutHelper.CalculateLayout()

**File**: `Helpers/BeepListBoxLayoutHelper.cs`  
**Location**: Inside the item loop, before checkbox/icon area calculation

Add indent calculation:
```csharp
// Inside the per-item loop:
int depth = _owner.ShowHierarchy ? _owner.Helper.GetItemDepth(item) : 0;
bool hasChildren = _owner.ShowHierarchy ? _owner.Helper.ItemHasChildren(item) : false;
int indentPx = depth * DpiScalingHelper.ScaleValue(ListBoxTokens.IndentStepPerLevel, ctrl);
int chevronArea = hasChildren ? DpiScalingHelper.ScaleValue(ListBoxTokens.ChevronHitTarget, ctrl) : 0;

// Shift item content right by indent + chevron
int contentStartX = x + paddingX + indentPx + chevronArea;

// Chevron rect (for hit testing)
Rectangle chevronRect = Rectangle.Empty;
if (hasChildren)
{
    int chevronSize = DpiScalingHelper.ScaleValue(ListBoxTokens.ChevronSize, ctrl);
    int chevronX = x + paddingX + indentPx;
    int chevronY = screenY + (itemHeight - chevronSize) / 2;
    chevronRect = new Rectangle(chevronX, chevronY, chevronSize, chevronSize);
}

// Then use contentStartX instead of (x + paddingX) for checkbox/icon/text positioning
```

Update `ListItemInfo`:
```csharp
_layoutCache.Add(new ListItemInfo
{
    Item = item,
    RowRect = row,
    CheckRect = checkRect,
    IconRect = iconRect,
    TextRect = textRect,
    Depth = depth,
    HasChildren = hasChildren,
    ChevronRect = chevronRect
});
```

---

### Step 5.6 — Draw expand/collapse chevron in BaseListBoxPainter

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawItem()` method, before checkbox drawing

Add:
```csharp
// Draw expand/collapse chevron for hierarchy items
if (info != null && info.HasChildren && !info.ChevronRect.IsEmpty)
{
    DrawChevron(g, info.ChevronRect, item.IsExpanded);
}
```

Add helper method:
```csharp
/// <summary>
/// Draws a expand/collapse chevron (▶ or ▼) for tree items.
/// </summary>
protected virtual void DrawChevron(Graphics g, Rectangle rect, bool isExpanded)
{
    int cx = rect.X + rect.Width / 2;
    int cy = rect.Y + rect.Height / 2;
    int size = Math.Min(rect.Width, rect.Height) / 2;

    var chevronColor = _helper.GetTextColor();
    using var pen = new Pen(Color.FromArgb(ListBoxTokens.SubTextAlpha, chevronColor), 1.5f);

    Point[] points;
    if (isExpanded)
    {
        // ▼ Down-pointing chevron
        points = new[]
        {
            new Point(cx - size, cy - size / 2),
            new Point(cx, cy + size / 2),
            new Point(cx + size, cy - size / 2)
        };
    }
    else
    {
        // ▶ Right-pointing chevron
        points = new[]
        {
            new Point(cx - size / 2, cy - size),
            new Point(cx + size / 2, cy),
            new Point(cx - size / 2, cy + size)
        };
    }

    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
    g.DrawLines(pen, points);
}
```

---

### Step 5.7 — Handle expand/collapse click in events

**File**: `BeepListBox.Events.cs`  
**Location**: `OnMouseClick` or `OnMouseDown`, after hit testing

Add chevron click detection:
```csharp
// In OnMouseDown or OnMouseClick, after getting the clicked item:
if (_showHierarchy && layoutInfo != null && layoutInfo.HasChildren)
{
    // Check if click was on the chevron
    if (layoutInfo.ChevronRect.Contains(e.Location))
    {
        item.IsExpanded = !item.IsExpanded;
        int idx = visibleItems.IndexOf(item);
        if (item.IsExpanded)
            OnItemExpanded(idx, item);
        else
            OnItemCollapsed(idx, item);
        InvalidateLayoutCache();
        return; // Don't select the item
    }
}
```

---

### Step 5.8 — Add keyboard expand/collapse (← / →)

**File**: `BeepListBox.Keyboard.cs`  
**Location**: `OnKeyDown()`, add cases for `Keys.Left` and `Keys.Right`

```csharp
case Keys.Right:
    if (_showHierarchy && current >= 0 && current < count)
    {
        var item = visibleItems[current];
        if (_helper.ItemHasChildren(item))
        {
            if (!item.IsExpanded)
            {
                item.IsExpanded = true;
                OnItemExpanded(current, item);
                InvalidateLayoutCache();
                e.Handled = e.SuppressKeyPress = true;
                break;
            }
            else
            {
                // Already expanded — move into first child
                MoveFocus(current + 1, shift, visibleItems);
                e.Handled = e.SuppressKeyPress = true;
                break;
            }
        }
    }
    break;

case Keys.Left:
    if (_showHierarchy && current >= 0 && current < count)
    {
        var item = visibleItems[current];
        if (item.IsExpanded && _helper.ItemHasChildren(item))
        {
            // Collapse current node
            item.IsExpanded = false;
            OnItemCollapsed(current, item);
            InvalidateLayoutCache();
            e.Handled = e.SuppressKeyPress = true;
            break;
        }
        else if (item.ParentItem != null)
        {
            // Move focus to parent
            int parentIdx = visibleItems.IndexOf(item.ParentItem);
            if (parentIdx >= 0)
            {
                MoveFocus(parentIdx, shift, visibleItems);
            }
            e.Handled = e.SuppressKeyPress = true;
            break;
        }
    }
    break;
```

---

### Step 5.9 — Update accessibility for tree role

**File**: `BeepListBox.Accessibility.cs`  
**Location**: `CreateAccessibilityInstance()` and `BeepListItemAccessible`

When `ShowHierarchy = true`:
- Control role should be `AccessibleRole.Outline` (tree)
- Item role should be `AccessibleRole.OutlineItem`
- Set `AccessibleObject.State` to include `AccessibleStates.Expanded` / `Collapsed`

---

## Token Reference

| Token | Value | Used For |
|-------|-------|----------|
| `IndentStepPerLevel` | 24px | Left margin per nesting level |
| `ChevronSize` | 16px | Expand/collapse arrow dimensions |
| `ChevronHitTarget` | 28px | Click area around chevron (touch-friendly) |
| `MaxHierarchyDepth` | 10 | Recursion guard |

---

## Acceptance Criteria

- [ ] `ShowHierarchy = true` → items with `Children` show expand/collapse chevron
- [ ] Click chevron → children appear/disappear, `IsExpanded` updated
- [ ] 3+ levels deep → consistent 24px indent per level
- [ ] Keyboard → key expands, ← key collapses; when leaf, ← moves to parent
- [ ] Search text filters hierarchy — matching children's ancestors stay visible
- [ ] All 34 painters render hierarchy correctly (indentation + chevron)
- [ ] `ShowHierarchy = false` → no indentation, no chevrons, flat list as before
- [ ] `ShowGroups = true` + `ShowHierarchy = true` → groups disabled (exclusive modes)
- [ ] Expanding/collapsing an item preserves scroll position
- [ ] More than 10 levels deep → recursion stops at `MaxHierarchyDepth`
- [ ] Accessibility: tree role and expanded/collapsed states announced by screen reader
