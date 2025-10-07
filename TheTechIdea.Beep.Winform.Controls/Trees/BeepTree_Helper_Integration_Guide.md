# BeepTree Helper Integration Guide

## Quick Reference for Updating Partial Classes

### Using BeepTreeHelper (_treeHelper)

**Purpose**: Data structure operations (traversal, search, filtering)

**Common Operations**:

```csharp
// Traverse all nodes (including collapsed)
var allNodes = _treeHelper.TraverseAll(_nodes);

// Traverse only visible nodes (respecting expansion)
var visibleNodes = _treeHelper.TraverseVisible(_nodes);

// Find node by GUID
var node = _treeHelper.FindByGuid(guidId);

// Find node by predicate
var node = _treeHelper.FindByPredicate(n => n.Text == "Search Term");

// Find node by text
var node = _treeHelper.FindByText("Node Text");

// Get node level (depth in tree)
int level = _treeHelper.GetNodeLevel(node);

// Get node parent
var parent = _treeHelper.GetNodeParent(node);

// Get all children (recursive)
var allChildren = _treeHelper.GetAllChildren(node);

// Check if node has children
bool hasChildren = _treeHelper.HasChildren(node);
```

### Using BeepTreeLayoutHelper (_layoutHelper)

**Purpose**: Layout calculation, measurement, and caching

**Common Operations**:

```csharp
// Recalculate entire layout (returns List<NodeInfo>)
_visibleNodes = _layoutHelper.RecalculateLayout();

// Get cached layout
var layout = _layoutHelper.GetCachedLayout();

// Get layout for specific item
NodeInfo? nodeInfo = _layoutHelper.GetCachedLayoutForItem(item);

// Check if node is in viewport
bool inView = _layoutHelper.IsNodeInViewport(nodeInfo);

// Transform coordinates
Rectangle viewportRect = _layoutHelper.TransformToViewport(contentRect);
Point contentPoint = _layoutHelper.TransformToContent(viewportPoint);

// Measure text
Size textSize = _layoutHelper.MeasureText(text, font);

// Calculate dimensions
int rowHeight = _layoutHelper.CalculateRowHeight(textSize);
int indent = _layoutHelper.CalculateIndent(level);

// Get total dimensions
int totalHeight = _layoutHelper.CalculateTotalContentHeight();
int totalWidth = _layoutHelper.CalculateTotalContentWidth();

// Invalidate cache (force recalc)
_layoutHelper.InvalidateCache();
```

### Using BeepTreeHitTestHelper (_hitTestHelper)

**Purpose**: Mouse hit testing and interaction

**Common Operations**:

```csharp
// Register hit areas (call after layout calculation)
_hitTestHelper.RegisterHitAreas();

// Hit test at point
NodeInfo? hitNode = _hitTestHelper.HitTestNode(mousePoint);

// Get specific hit region
var region = _hitTestHelper.GetHitRegion(mousePoint, hitNode);
// region can be: ToggleButton, CheckBox, Icon, Text, Row, None

// Example usage in mouse handler:
private void OnMouseDownHandler(object sender, MouseEventArgs e)
{
    Point clientMouse = new Point(e.X, e.Y);
    NodeInfo? hitNode = _hitTestHelper.HitTestNode(clientMouse);
    
    if (hitNode.HasValue)
    {
        var region = _hitTestHelper.GetHitRegion(clientMouse, hitNode.Value);
        
        switch (region)
        {
            case HitTestRegion.ToggleButton:
                // Handle toggle expand/collapse
                break;
            case HitTestRegion.CheckBox:
                // Handle checkbox toggle
                break;
            case HitTestRegion.Text:
            case HitTestRegion.Icon:
            case HitTestRegion.Row:
                // Handle selection
                break;
        }
    }
}
```

## Integration Examples

### Example 1: Updating BeepTree.Layout.cs

**Before** (direct implementation):
```csharp
internal void RecalculateLayoutCache()
{
    if (_visibleNodes.Count == 0)
        return;

    using (Graphics g = CreateGraphics())
    {
        int y = 0;
        int maxWidth = 0;

        foreach (var nodeInfo in _visibleNodes)
        {
            // 80+ lines of layout calculation...
        }

        _totalContentHeight = y;
        _virtualSize = new Size(maxWidth, _totalContentHeight);
    }
}
```

**After** (using helper):
```csharp
internal void RecalculateLayoutCache()
{
    if (_layoutHelper == null) return;
    
    // Delegate to layout helper
    _visibleNodes = _layoutHelper.RecalculateLayout();
    
    // Update dimensions from helper
    _totalContentHeight = _layoutHelper.CalculateTotalContentHeight();
    int maxWidth = _layoutHelper.CalculateTotalContentWidth();
    _virtualSize = new Size(maxWidth, _totalContentHeight);
    
    // Register hit areas after layout
    _hitTestHelper?.RegisterHitAreas();
}
```

### Example 2: Updating RebuildVisible() in BeepTree.Layout.cs

**Before**:
```csharp
private void RebuildVisible()
{
    _visibleNodes.Clear();

    void Recurse(SimpleItem item, int level)
    {
        _visibleNodes.Add(new NodeInfo { Item = item, Level = level });
        if (item.IsExpanded && item.Children?.Count > 0)
        {
            foreach (var child in item.Children)
            {
                Recurse(child, level + 1);
            }
        }
    }

    foreach (var root in _nodes)
    {
        Recurse(root, 0);
    }

    RecalculateLayoutCache();
    if (!DesignMode && IsHandleCreated)
    {
        UpdateScrollBars();
    }
}
```

**After** (using helper):
```csharp
private void RebuildVisible()
{
    if (_treeHelper == null || _layoutHelper == null) return;
    
    // Use tree helper to get visible nodes
    var visibleItems = _treeHelper.TraverseVisible(_nodes).ToList();
    
    // Convert to NodeInfo list and recalculate layout
    _visibleNodes = _layoutHelper.RecalculateLayout();
    
    // Update dimensions
    _totalContentHeight = _layoutHelper.CalculateTotalContentHeight();
    _virtualSize = new Size(_layoutHelper.CalculateTotalContentWidth(), _totalContentHeight);
    
    // Register hit areas
    _hitTestHelper?.RegisterHitAreas();
    
    // Update scrollbars
    if (!DesignMode && IsHandleCreated)
    {
        UpdateScrollBars();
    }
}
```

### Example 3: Updating Node Finding Methods

**Before**:
```csharp
public SimpleItem FindNode(string text)
{
    if (string.IsNullOrEmpty(text))
        return null;

    SimpleItem FindRecursive(IList<SimpleItem> items)
    {
        if (items == null) return null;

        foreach (var item in items)
        {
            if (item.Text?.Equals(text, StringComparison.OrdinalIgnoreCase) == true)
                return item;

            var found = FindRecursive(item.Children);
            if (found != null)
                return found;
        }
        return null;
    }

    return FindRecursive(_nodes);
}
```

**After** (using helper):
```csharp
public SimpleItem FindNode(string text)
{
    return _treeHelper?.FindByText(text);
}

public SimpleItem GetNodeByGuid(string guidid)
{
    return _treeHelper?.FindByGuid(guidid);
}

public SimpleItem GetNode(string nodeName)
{
    return _treeHelper?.FindByText(nodeName);
}
```

### Example 4: Updating BeepTree.Drawing.cs

**Before**:
```csharp
private void DrawVisibleNodes(Graphics g, Rectangle clientArea, ITreePainter painter)
{
    foreach (var nodeInfo in _visibleNodes)
    {
        // Calculate viewport position
        int screenY = nodeInfo.Y - _yOffset;
        
        // Check if in viewport
        if (screenY + nodeInfo.RowHeight < 0 || screenY > clientArea.Height)
            continue;
        
        // ... draw node
    }
}
```

**After** (using helper):
```csharp
private void DrawVisibleNodes(Graphics g, Rectangle clientArea, ITreePainter painter)
{
    if (_layoutHelper == null) return;
    
    var layout = _layoutHelper.GetCachedLayout();
    
    foreach (var nodeInfo in layout)
    {
        // Use helper to check if in viewport
        if (!_layoutHelper.IsNodeInViewport(nodeInfo))
            continue;
        
        // Transform rectangles to viewport space
        Rectangle rowRect = _layoutHelper.TransformToViewport(nodeInfo.RowRectContent);
        Rectangle toggleRect = _layoutHelper.TransformToViewport(nodeInfo.ToggleRectContent);
        Rectangle checkRect = _layoutHelper.TransformToViewport(nodeInfo.CheckRectContent);
        Rectangle iconRect = _layoutHelper.TransformToViewport(nodeInfo.IconRectContent);
        Rectangle textRect = _layoutHelper.TransformToViewport(nodeInfo.TextRectContent);
        
        // Draw using painter...
        painter.PaintNode(g, nodeInfo, rowRect, toggleRect, checkRect, iconRect, textRect);
    }
}
```

### Example 5: Updating BeepTree.Events.cs Mouse Handler

**Before**:
```csharp
private void OnMouseDownHandler(object sender, MouseEventArgs e)
{
    Rectangle clientArea = GetClientArea();
    Point clientMouse = new Point(e.X, e.Y);

    // Find which node was clicked
    NodeInfo clickedNodeInfo = HitTestNode(clientMouse);

    if (clickedNodeInfo != null)
    {
        if (clickedNodeInfo.ToggleRect.Contains(clientMouse))
        {
            // Handle toggle...
        }
        else if (clickedNodeInfo.CheckRect.Contains(clientMouse))
        {
            // Handle checkbox...
        }
        // ... more checks
    }
}
```

**After** (using helper):
```csharp
private void OnMouseDownHandler(object sender, MouseEventArgs e)
{
    if (_hitTestHelper == null) return;
    
    Point clientMouse = new Point(e.X, e.Y);

    // Use hit test helper
    NodeInfo? hitNode = _hitTestHelper.HitTestNode(clientMouse);

    if (hitNode.HasValue)
    {
        var region = _hitTestHelper.GetHitRegion(clientMouse, hitNode.Value);
        ClickedNode = hitNode.Value.Item;
        _lastClickedNode = hitNode.Value.Item;

        switch (region)
        {
            case HitTestRegion.ToggleButton:
                if (hitNode.Value.Item.Children?.Count > 0)
                {
                    // Toggle expand/collapse
                    hitNode.Value.Item.IsExpanded = !hitNode.Value.Item.IsExpanded;
                    RebuildVisible();

                    if (hitNode.Value.Item.IsExpanded)
                        NodeExpanded?.Invoke(this, new BeepMouseEventArgs("NodeExpanded", hitNode.Value.Item));
                    else
                        NodeCollapsed?.Invoke(this, new BeepMouseEventArgs("NodeCollapsed", hitNode.Value.Item));

                    Invalidate();
                }
                break;

            case HitTestRegion.CheckBox:
                if (ShowCheckBox)
                {
                    // Toggle checkbox
                    hitNode.Value.Item.Checked = !hitNode.Value.Item.Checked;

                    if (hitNode.Value.Item.Checked)
                        NodeChecked?.Invoke(this, new BeepMouseEventArgs("NodeChecked", hitNode.Value.Item));
                    else
                        NodeUnchecked?.Invoke(this, new BeepMouseEventArgs("NodeUnchecked", hitNode.Value.Item));

                    Invalidate();
                }
                break;

            case HitTestRegion.Row:
            case HitTestRegion.Text:
            case HitTestRegion.Icon:
                // Handle node selection
                HandleNodeSelection(hitNode.Value.Item, e);
                break;
        }
    }
}
```

## Migration Checklist

### BeepTree.Layout.cs
- [ ] Update `RecalculateLayoutCache()` to use `_layoutHelper.RecalculateLayout()`
- [ ] Update `RebuildVisible()` to use `_treeHelper.TraverseVisible()`
- [ ] Update `FindNode()` to use `_treeHelper.FindByText()`
- [ ] Update `GetNodeByGuid()` to use `_treeHelper.FindByGuid()`
- [ ] Update `ExpandAll()` / `CollapseAll()` to use helper traversal
- [ ] Call `_hitTestHelper.RegisterHitAreas()` after layout updates

### BeepTree.Events.cs
- [ ] Update `OnMouseDownHandler()` to use `_hitTestHelper.HitTestNode()`
- [ ] Update hit region detection to use `_hitTestHelper.GetHitRegion()`
- [ ] Update coordinate transformations to use `_layoutHelper.TransformToContent()`
- [ ] Similar updates for `OnMouseUpHandler()`, `OnMouseMoveHandler()`, etc.

### BeepTree.Drawing.cs
- [ ] Update `DrawVisibleNodes()` to use `_layoutHelper.GetCachedLayout()`
- [ ] Update viewport checks to use `_layoutHelper.IsNodeInViewport()`
- [ ] Update coordinate transformations to use `_layoutHelper.TransformToViewport()`

### BeepTree.Methods.cs
- [ ] Update traversal methods to use `_treeHelper.TraverseAll()`
- [ ] Update search methods to use `_treeHelper.FindByPredicate()`
- [ ] Update node manipulation to use helper methods

## Common Patterns

### Pattern 1: Null Checks
Always check if helpers are initialized:
```csharp
if (_layoutHelper == null || _treeHelper == null) return;
```

### Pattern 2: Layout Update Flow
```csharp
1. Change data (_nodes, expand/collapse, etc.)
2. Call _layoutHelper.RecalculateLayout()
3. Update _totalContentHeight and _virtualSize
4. Call _hitTestHelper.RegisterHitAreas()
5. Call UpdateScrollBars()
6. Call Invalidate()
```

### Pattern 3: Mouse Event Handling
```csharp
1. Get mouse point
2. Call _hitTestHelper.HitTestNode(point)
3. If hit, call _hitTestHelper.GetHitRegion()
4. Switch on region type
5. Perform action
6. Update layout if needed
7. Call Invalidate()
```

## Benefits

1. **Less Code**: Helpers eliminate duplicate code across partial classes
2. **Better Testing**: Helpers can be unit tested independently
3. **Easier Maintenance**: Changes to layout logic happen in one place
4. **Performance**: Layout caching and virtualization are optimized in helpers
5. **Consistency**: All partial classes use the same calculation logic
6. **Integration**: Hit testing integrates with BaseControl's system

## Notes

- The helpers are initialized in the constructor, so they're always available
- Helpers are `internal` so they're accessible from all partial classes
- The public accessor properties allow external code to use helpers if needed
- NodeInfo is now a struct in Models namespace, not an internal class
- All coordinate transformations should go through LayoutHelper for consistency
