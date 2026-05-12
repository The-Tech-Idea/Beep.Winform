# Phase 5: Fix Scrollbar

## Problem

Scrollbar appears when not needed or doesn't appear when needed:
- Thumb size not proportional to visible/total ratio
- Scrolls by pixel instead of by item
- Focused item may not be fully visible after navigation

## Root Cause

In `ComboBoxPopupContent.cs`:
```csharp
_vScrollBar.Maximum = totalHeight;
_vScrollBar.LargeChange = Math.Max(1, visibleHeight);
```

`LargeChange` is set to pixel height, but the scrollbar expects this to represent the viewport size. The math is off because `Maximum` should be `totalHeight - viewportHeight`, not `totalHeight`.

## Solution

### Step 5.1: Correct Scrollbar Math

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Replace `UpdateScrollBar`:
```csharp
private void UpdateScrollBar()
{
    int totalHeight = 0;
    foreach (var row in _rows)
        totalHeight += row.Height;

    int viewportHeight = _scrollContainer.ClientSize.Height;
    bool needsScroll = totalHeight > viewportHeight;

    _vScrollBar.Visible = needsScroll;
    if (needsScroll)
    {
        // Maximum = total content minus viewport (so thumb represents viewport)
        _vScrollBar.Maximum = Math.Max(0, totalHeight - viewportHeight);
        _vScrollBar.LargeChange = viewportHeight;
        _vScrollBar.SmallChange = _profile.BaseRowHeight;
        
        // Ensure value is valid
        if (_vScrollBar.Value > _vScrollBar.Maximum)
            _vScrollBar.Value = _vScrollBar.Maximum;
    }
    else
    {
        _vScrollBar.Value = 0;
    }

    _listPanel.Location = new Point(0, -_vScrollBar.Value);
}
```

### Step 5.2: Scroll by Item Height

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Modify `OnListPanelMouseWheel`:
```csharp
private void OnListPanelMouseWheel(object sender, MouseEventArgs e)
{
    if (!_vScrollBar.Visible) return;

    int itemsPerNotch = 3;
    int scrollAmount = e.Delta > 0 ? -itemsPerNotch : itemsPerNotch;
    int newValue = _vScrollBar.Value + (scrollAmount * _profile.BaseRowHeight);
    
    newValue = Math.Max(0, Math.Min(newValue, _vScrollBar.Maximum));
    _vScrollBar.Value = newValue;

    if (e is HandledMouseEventArgs hme)
        hme.Handled = true;
}
```

### Step 5.3: Ensure Focused Item Visible

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Enhance `EnsureRowVisible`:
```csharp
private void EnsureRowVisible(int index)
{
    if (index < 0 || index >= _rows.Count)
        return;

    try
    {
        // Calculate row position
        int rowTop = 0;
        for (int i = 0; i < index; i++)
            rowTop += _rows[i].Height;
        
        int rowBottom = rowTop + _rows[index].Height;
        int viewportTop = _vScrollBar.Value;
        int viewportBottom = viewportTop + _scrollContainer.ClientSize.Height;

        // If row is above viewport, scroll up
        if (rowTop < viewportTop)
        {
            _vScrollBar.Value = rowTop;
        }
        // If row is below viewport, scroll down
        else if (rowBottom > viewportBottom)
        {
            _vScrollBar.Value = rowBottom - _scrollContainer.ClientSize.Height;
        }
    }
    catch
    {
        // Best-effort scrolling only.
    }
}
```

### Step 5.4: Fix ListBox Version Scrollbar

**File:** `ComboBoxes/Popup/ComboBoxListBoxPopupContent.cs`

Ensure BeepListBox handles scrollbar correctly. If BeepListBox has its own scrollbar issues, document them separately.

## Expected Behavior

- Scrollbar only appears when `totalHeight > viewportHeight`
- Thumb size proportional to visible / total ratio
- Mouse wheel scrolls by 3 items per notch
- Focused item always fully visible after keyboard navigation
- Smooth scrolling without jitter

## Validation

- [ ] Scrollbar only appears when content exceeds viewport
- [ ] Thumb size is proportional (e.g., 50% visible = 50% thumb)
- [ ] Mouse wheel scrolls by item height increments
- [ ] After PageDown, focused item is fully visible
- [ ] After clicking last visible item, scrollbar scrolls to show it
- [ ] No scrollbar for single item
