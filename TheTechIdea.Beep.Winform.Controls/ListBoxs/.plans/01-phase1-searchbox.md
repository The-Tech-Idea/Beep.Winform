# Phase 1 — Search Box: Replace Painted Placeholder with Real BeepTextBox

**Priority**: Critical  
**Status**: Not Started  
**Depends on**: None  

## Problem

When `ShowSearch = true`, `BaseListBoxPainter.DrawSearchArea()` only draws a static gray rectangle with a border line. There is **NO input control** — users cannot type, filter, or interact with the search area at all.

The old implementation (preserved in `BeepListBox.cs.old` lines 1241–1310) used a raw `TextBox` with Win32 `EM_SETCUEBANNER` API for placeholder text, but was removed during the painter refactor and never replaced.

### Current broken flow

```
ShowSearch = true
  → UpdateLayout() calculates _searchAreaRect (36px tall)
  → BaseListBoxPainter.Paint() calls DrawSearchArea()
  → DrawSearchArea() fills a gray rectangle + draws a border line
  → Returns Y offset (no input control exists)
  → User sees a dead gray bar at the top
```

### Target flow

```
ShowSearch = true
  → InitializeSearchTextBox() creates BeepTextBox as child control
  → UpdateLayout() positions BeepTextBox inside _searchAreaRect
  → BaseListBoxPainter.Paint() skips drawing search area (real control handles it)
  → User types in BeepTextBox → TextChanged → SearchText → items filter live
```

---

## Steps

### Step 1.1 — Add BeepTextBox field and initialization method

**File**: `BeepListBox.Core.cs`  
**Location**: Fields section (around line 110, after `_searchAreaRect`)

Add field:
```csharp
private BeepTextBox _searchTextBox;
private bool _isUpdatingSearchText = false; // guard against infinite loop
```

Add initialization method (after `InitializeScrollbars()`):
```csharp
private void InitializeSearchTextBox()
{
    if (_searchTextBox != null) return;

    _searchTextBox = new BeepTextBox
    {
        IsChild = true,
        PlaceholderText = SearchPlaceholderText ?? "Search...",
        BorderRadius = ListBoxTokens.SearchCornerRadius,
        Visible = _showSearch
    };

    _searchTextBox.TextChanged += SearchTextBox_TextChanged;
    Controls.Add(_searchTextBox);
    _searchTextBox.BringToFront();
}

private void DisposeSearchTextBox()
{
    if (_searchTextBox == null) return;
    _searchTextBox.TextChanged -= SearchTextBox_TextChanged;
    Controls.Remove(_searchTextBox);
    _searchTextBox.Dispose();
    _searchTextBox = null;
}

private void SearchTextBox_TextChanged(object sender, EventArgs e)
{
    if (_isUpdatingSearchText) return;
    _isUpdatingSearchText = true;
    try
    {
        SearchText = _searchTextBox.Text;
    }
    finally
    {
        _isUpdatingSearchText = false;
    }
}
```

**Why**: Creates a real `BeepTextBox` control as a child of the list box. The `IsChild = true` flag tells the text box it's embedded (important for theme propagation). The guard flag prevents infinite loops when `SearchText` setter syncs back to the text box.

---

### Step 1.2 — Wire ShowSearch property to create/remove BeepTextBox

**File**: `BeepListBox.Properties.cs`  
**Location**: `ShowSearch` property setter (around line 72)

Change from:
```csharp
set
{
    if (_showSearch != value)
    {
        _showSearch = value;
        _needsLayoutUpdate = true;
        RequestDelayedInvalidate();
    }
}
```

Change to:
```csharp
set
{
    if (_showSearch != value)
    {
        _showSearch = value;
        if (_showSearch)
            InitializeSearchTextBox();
        else
            DisposeSearchTextBox();
        _needsLayoutUpdate = true;
        RequestDelayedInvalidate();
    }
}
```

Also update `SearchPlaceholderText` setter to sync:
```csharp
// In SearchPlaceholderText property setter, add:
if (_searchTextBox != null)
    _searchTextBox.PlaceholderText = value;
```

**Why**: Creates the real text box when search is enabled, removes it when disabled.

---

### Step 1.3 — Position BeepTextBox in UpdateLayout()

**File**: `BeepListBox.Core.cs`  
**Location**: `UpdateLayout()` method (around line 430–465)

After `_searchAreaRect` is calculated, add positioning code:
```csharp
// Position the search text box inside the search area
if (_searchTextBox != null)
{
    int inset = DpiScalingHelper.ScaleValue(4, this);
    _searchTextBox.Bounds = new Rectangle(
        _searchAreaRect.Left + inset,
        _searchAreaRect.Top + inset,
        _searchAreaRect.Width - inset * 2,
        _searchAreaRect.Height - inset * 2
    );
    _searchTextBox.Visible = _showSearch;
    _searchTextBox.BringToFront();
}
```

Also update the search height to use the token:
```csharp
// Change from:
int searchHeight = DpiScalingHelper.ScaleValue(36, this);
// Change to:
int searchHeight = DpiScalingHelper.ScaleValue(ListBoxTokens.SearchBarHeight, this);
```

This requires adding the using for tokens at the top of the file if not present:
```csharp
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
```

**Why**: Positions the text box inside the reserved search area with proper margins. Uses the `SearchBarHeight` token (40px) instead of hardcoded 36px.

---

### Step 1.4 — Remove painted search from BaseListBoxPainter

**File**: `Painters/BaseListBoxPainter.cs`  
**Location**: `DrawSearchArea()` method (around line 125–140)

Change from drawing a gray rectangle to only returning the Y offset:
```csharp
protected virtual int DrawSearchArea(Graphics g, Rectangle drawingRect, int yOffset)
{
    // The real BeepTextBox control handles search UI rendering.
    // This method only computes the Y offset to leave space for it.
    int searchHeight = DpiScalingHelper.ScaleValue(ListBoxTokens.SearchBarHeight, _owner);
    return yOffset + searchHeight + DpiScalingHelper.ScaleValue(4, _owner);
}
```

**Why**: The real `BeepTextBox` control handles rendering. The painter only needs to compute the vertical offset so items start below the search area.

---

### Step 1.5 — Theme the search box

**File**: `BeepListBox.Core.cs` (or whichever partial file contains `ApplyTheme`)  
**Location**: Find `ApplyTheme()` override

Add to the end of the `ApplyTheme()` method:
```csharp
if (_searchTextBox != null)
{
    _searchTextBox.Theme = Theme;
    _searchTextBox.ApplyTheme();
}
```

**Why**: Ensures the search box colors and font update when the list box theme changes.

---

### Step 1.6 — Call InitializeSearchTextBox in constructor if ShowSearch defaults  

**File**: `BeepListBox.Core.cs`  
**Location**: Constructor, before `InitializeScrollbars()` call

Add:
```csharp
// Initialize search text box if ShowSearch was set before constructor
if (_showSearch)
    InitializeSearchTextBox();
```

**Why**: Handles the case where `ShowSearch` is set via designer serialization before the constructor logic runs.

---

## Acceptance Criteria

- [ ] Setting `ShowSearch = true` at runtime shows a themed `BeepTextBox` at the top of the list
- [ ] Setting `ShowSearch = false` removes the text box from the control
- [ ] Typing in the search box filters list items in real-time
- [ ] `SearchText` property reflects the text box content and vice versa
- [ ] `SearchPlaceholderText` updates the text box placeholder
- [ ] Changing themes updates the search box appearance
- [ ] The search box does not scroll with items (fixed at top)
- [ ] `SearchTextChanged` event fires when user types
- [ ] Focus on search box is maintained during typing (no loss to list)
