# Phase 3: Fix Page Up/Down Navigation

## Problem

Page Up and Page Down keys move by a hardcoded 6 items, which is incorrect when:
- There is only 1 item (should go to first/last)
- Viewport shows fewer than 6 items (should move by visible count)
- Viewport shows more than 6 items (should move by visible count)

## Root Cause

In `ComboBoxPopupContent.cs`:
```csharp
case Keys.PageDown:
    MoveFocus(6);
    e.Handled = true;
    break;
case Keys.PageUp:
    MoveFocus(-6);
    e.Handled = true;
    break;
```

## Patterns from Reference Implementations

### EWSoftware ListControls (GitHub)
```csharp
// Real implementation - uses MaxDropDownItems for page size:
case Keys.PageUp:
    idx = this.SelectedIndex - maxDropDownItems + 1;
    if(idx < 0)
        idx = 0;
    this.SelectedIndex = idx;
    break;

case Keys.PageDown:
    idx = this.SelectedIndex + maxDropDownItems - 1;
    if(idx >= this.Items.Count)
        idx = this.Items.Count - 1;
    this.SelectedIndex = idx;
    break;
```

### dotnet/winforms (Official)
```csharp
// Native ComboBox uses viewport height / item height
// The CB_GETDROPPEDSTATE message returns true when dropdown is visible
// PageUp/PageDown are handled by the native control, not WinForms code
```

### Key Insights
- **EWSoftware** uses `MaxDropDownItems` (default 8) as page size
- This is the same as the maximum visible items, so it matches viewport perfectly
- No need to calculate from control height - use the configured `DropDownRows`/`MaxDropDownItems`

## Solution

### Step 3.1: Calculate Page Size Dynamically

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Add method:
```csharp
private int GetPageSize()
{
    int viewportHeight = _scrollContainer.ClientSize.Height;
    if (viewportHeight <= 0 || _rows.Count == 0) return 1;
    int avgItemHeight = _rows[0].Height;
    return Math.Max(1, viewportHeight / avgItemHeight);
}
```

### Step 3.2: Update Key Handlers

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Replace PageUp/PageDown handlers:
```csharp
case Keys.PageDown:
    MoveFocus(GetPageSize());
    e.Handled = true;
    break;
case Keys.PageUp:
    MoveFocus(-GetPageSize());
    e.Handled = true;
    break;
```

Also update in `SearchBoxOnKeyDown`:
```csharp
case Keys.PageDown:
    MoveFocus(GetPageSize());
    e.Handled = true;
    break;
case Keys.PageUp:
    MoveFocus(-GetPageSize());
    e.Handled = true;
    break;
```

### Step 3.3: Handle Edge Cases

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Enhance `MoveFocus`:
```csharp
private void MoveFocus(int delta)
{
    if (_rows.Count == 0)
    {
        return;
    }
    int start = _keyboardFocusIndex >= 0 ? _keyboardFocusIndex : FindFirstSelectableRowIndex();
    if (start < 0) return;

    int next = start + delta;
    
    // Clamp to valid range
    next = Math.Max(0, Math.Min(next, _rows.Count - 1));

    // Skip non-selectable rows (group headers, separators)
    int guard = 0;
    while (guard < _rows.Count)
    {
        var m = _rows[next].Model;
        if (ComboBoxPopupRowBehavior.IsSelectable(m))
            break;
        
        // Continue in same direction
        next += delta > 0 ? 1 : -1;
        if (next < 0 || next >= _rows.Count)
        {
            // Revert to start if no selectable item in direction
            next = Math.Max(0, Math.Min(start, _rows.Count - 1));
            break;
        }
        guard++;
    }

    SetKeyboardFocusIndex(next);
}
```

### Step 3.4: Handle ListBoxPopupContent

**File:** `ComboBoxes/Popup/ComboBoxListBoxPopupContent.cs`

The ListBox version uses BeepListBox which should handle PageUp/PageDown natively. Verify that `BeepListBox` implements proper page navigation. If not, add:

```csharp
private void OnListKeyDown(object sender, KeyEventArgs e)
{
    if (e.KeyCode == Keys.Enter && _listBox.SelectedItem != null)
    {
        OnListItemClicked(this, _listBox.SelectedItem);
        e.Handled = true;
        e.SuppressKeyPress = true;
    }
    else if (e.KeyCode == Keys.PageDown)
    {
        int pageSize = _listBox.ClientSize.Height / _listBox.ItemHeight;
        int newIndex = Math.Min(_listBox.SelectedIndex + pageSize, _listBox.ListItems.Count - 1);
        _listBox.SelectedIndex = newIndex;
        _listBox.EnsureIndexVisible(newIndex);
        e.Handled = true;
    }
    else if (e.KeyCode == Keys.PageUp)
    {
        int pageSize = _listBox.ClientSize.Height / _listBox.ItemHeight;
        int newIndex = Math.Max(_listBox.SelectedIndex - pageSize, 0);
        _listBox.SelectedIndex = newIndex;
        _listBox.EnsureIndexVisible(newIndex);
        e.Handled = true;
    }
}
```

## Expected Behavior

- PageDown moves focus by `viewportHeight / itemHeight` items
- PageUp moves focus by `-viewportHeight / itemHeight` items
- If at last item and PageDown pressed: stay at last item
- If at first item and PageUp pressed: stay at first item
- If only 1 item: PageUp/PageDown both select that item

## Validation

- [ ] With 1 item: PageUp and PageDown both select it
- [ ] With 5 visible items: PageDown moves by 5
- [ ] With 3 visible items: PageDown moves by 3
- [ ] At last item: PageDown stays at last item
- [ ] At first item: PageUp stays at first item
- [ ] Skips group headers and separators
