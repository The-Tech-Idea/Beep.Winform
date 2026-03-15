# Phase 3 — Scrolling: Fix Virtual-Size, Offset Clamping & Edge Cases

**Priority**: Medium  
**Status**: Not Started  
**Depends on**: Phase 1 (search area affects available height)

## Problem

Several scrolling issues exist:

1. **Virtual size doesn't subtract search area height**: `CalculateLayout()` sets `totalHeight` based on all items, but `UpdateScrollBars()` compares it against the full inner area height. When `ShowSearch = true`, the search header consumes ~40px that should be subtracted from the available scroll area, causing items to be clipped at the bottom.
2. **Unnecessary Invalidate() when scrollbar hidden**: `OnMouseWheel()` calls `Invalidate()` in the `else` branch even when no scrollbar is visible and no scrolling occurred — a wasted repaint.
3. **InvalidateLayoutCache() doesn't clamp _yOffset**: After items are removed, the cached `_yOffset` can exceed the new virtual height. The layout cache is cleared and a delayed invalidate is requested, but `_yOffset` remains stale until the next `UpdateScrollBars()`. This can cause a brief visual glitch where the scroll position overshoots.
4. **Horizontal SmallChange is always 1px**: `_horizontalScrollBar.SmallChange = Math.Max(1, 1)` — effectively a no-op that scrolls 1 pixel at a time.
5. **Scrollbar bounds don't account for search area**: The vertical scrollbar is positioned from `inner.Top`, but when a search box is shown, it should start below the search area to avoid overlapping.

---

## Steps

### Step 3.1 — Subtract search area height from available scroll area

**File**: `BeepListBox.Core.cs`  
**Location**: `UpdateScrollBars()` method (around line 470–480)

The `GetClientArea()` method returns the full inner rect minus scrollbar space. It also needs to subtract the search header when visible.

Change `GetClientArea()` from:
```csharp
public Rectangle GetClientArea()
{
    var inner = DrawingRect;
    if (inner.Width <= 0 || inner.Height <= 0) return Rectangle.Empty;
    int vBarW = (_verticalScrollBar?.Visible == true) ? _verticalScrollBar.Width : 0;
    int hBarH = (_horizontalScrollBar?.Visible == true) ? _horizontalScrollBar.Height : 0;
    return new Rectangle(inner.Left, inner.Top, Math.Max(0, inner.Width - vBarW), Math.Max(0, inner.Height - hBarH));
}
```

Change to:
```csharp
public Rectangle GetClientArea()
{
    var inner = DrawingRect;
    if (inner.Width <= 0 || inner.Height <= 0) return Rectangle.Empty;
    int vBarW = (_verticalScrollBar?.Visible == true) ? _verticalScrollBar.Width : 0;
    int hBarH = (_horizontalScrollBar?.Visible == true) ? _horizontalScrollBar.Height : 0;
    int searchH = (_showSearch && _searchTextBox != null) ? _searchAreaRect.Height : 0;
    return new Rectangle(
        inner.Left,
        inner.Top + searchH,
        Math.Max(0, inner.Width - vBarW),
        Math.Max(0, inner.Height - searchH - hBarH));
}
```

**Why**: The client area (scrollable content region) starts below the search box. Without this fix, `LargeChange` is too large and the scrollbar thumb doesn't accurately represent the visible portion.

---

### Step 3.2 — Adjust vertical scrollbar bounds to avoid search area overlap

**File**: `BeepListBox.Core.cs`  
**Location**: `UpdateScrollBars()`, vertical scrollbar bounds section

Change from:
```csharp
if (needsV)
{
    int vHeight = inner.Height - (needsH ? hBarH : 0);
    var vBounds = new Rectangle(inner.Right - vBarW, inner.Top, vBarW, Math.Max(0, vHeight));
```

Change to:
```csharp
if (needsV)
{
    int searchH = (_showSearch && _searchTextBox != null) ? _searchAreaRect.Height : 0;
    int vHeight = inner.Height - searchH - (needsH ? hBarH : 0);
    var vBounds = new Rectangle(inner.Right - vBarW, inner.Top + searchH, vBarW, Math.Max(0, vHeight));
```

**Why**: The scrollbar track should not overlap the search header. It starts below the search area and is shortened accordingly.

---

### Step 3.3 — Remove unnecessary Invalidate() in OnMouseWheel

**File**: `BeepListBox.Events.cs`  
**Location**: `OnMouseWheel()` method else branch (around line 338)

Change from:
```csharp
            else
            {
                Invalidate();
            }
```

Change to:
```csharp
            else
            {
                // No scrollbar visible — nothing to scroll, skip repaint
            }
```

Or simply remove the else block entirely.

**Why**: When there's no scrollbar (content fits), a mouse wheel event shouldn't trigger a repaint. This is a minor perf optimization that eliminates redundant paints.

---

### Step 3.4 — Clamp _yOffset in InvalidateLayoutCache()

**File**: `BeepListBox.Core.cs`  
**Location**: `InvalidateLayoutCache()` method (around line 600)

Change from:
```csharp
private void InvalidateLayoutCache()
{
    _layoutHelper?.Clear();
    _needsLayoutUpdate = true;
    RequestDelayedInvalidate();
}
```

Change to:
```csharp
private void InvalidateLayoutCache()
{
    _layoutHelper?.Clear();
    _needsLayoutUpdate = true;
    // Clamp scroll offsets to prevent overshoot after item removal
    if (_virtualSize.Height > 0)
    {
        int maxY = Math.Max(0, _virtualSize.Height - GetClientArea().Height);
        if (_yOffset > maxY) _yOffset = maxY;
    }
    else
    {
        _yOffset = 0;
    }
    RequestDelayedInvalidate();
}
```

**Why**: After items are removed, the stale `_yOffset` could exceed the new content height. Clamping it here prevents a brief visual flash where content appears to scroll past the end.

---

### Step 3.5 — Fix horizontal SmallChange to be meaningful

**File**: `BeepListBox.Core.cs`  
**Location**: `UpdateScrollBars()`, horizontal section (around line 540)

Change from:
```csharp
int newHSmall = Math.Max(1, 1);
```

Change to:
```csharp
int newHSmall = Math.Max(1, DpiScalingHelper.ScaleValue(20, this));
```

**Why**: `1px` is effectively useless for horizontal scrolling. A 20px step (DPI-scaled) gives a reasonable per-click scroll distance.

---

### Step 3.6 — Pass search area height to LayoutHelper.CalculateLayout()

**File**: `Helpers/BeepListBoxLayoutHelper.cs`  
**Location**: Start of `CalculateLayout()` where `drawingRect` dimensions are read

Currently the layout helper uses the full drawing rect. It needs to know about the search area so it starts laying out items below it.

Ensure `drawingRect` in the layout helper accounts for the search area. This is likely handled automatically if `CalculateLayout` reads `GetClientArea()` instead of `DrawingRect`. Verify and fix if needed:

Change the initial rect from:
```csharp
var drawingRect = ctrl.DrawingRect;
```

To:
```csharp
var drawingRect = ctrl.GetClientArea();
```

**Why**: Items should only be laid out in the scrollable client area (below the search header).

---

## Verification

| Scenario | Expected Behavior |
|----------|-------------------|
| 100+ items, no search | Scrollbar appears, all items reachable |
| 100+ items, search shown | Scrollbar starts below search box, all items reachable |
| Delete items until < viewport | Scrollbar disappears, `_yOffset` resets to 0 |
| Mouse wheel without scrollbar | No repaint, no scroll |
| Horizontal overflow | Scrollbar appears, 20px per click step |
| Resize while scrolled | Content doesn't overshoot; `_yOffset` clamped to valid range |
| Filter via search to fewer items | Scroll position resets/clamps, no empty space at bottom |

---

## Acceptance Criteria

- [ ] Search area height is subtracted from scroll calculations
- [ ] Vertical scrollbar doesn't overlap search box
- [ ] No unnecessary repaints on mouse wheel when scrollbar is hidden
- [ ] `_yOffset` is clamped when items are removed
- [ ] Horizontal `SmallChange` is a usable value (20px scaled)
- [ ] Layout helper uses client area (excludes search header)
