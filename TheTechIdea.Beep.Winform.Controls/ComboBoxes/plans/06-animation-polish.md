# Phase 6: Animation and Polish

## Problem

- Fade animation feels sluggish (150ms)
- Animation plays even when closing via item selection (should be instant)
- Focus not properly managed when popup opens/closes

## Solution

### Step 6.1: Optimize Animation Duration

**File:** `Forms/BeepPopupForm.cs`

Change default:
```csharp
[DefaultValue(100)]
public int AnimationDurationMs { get; set; } = 100;
```

### Step 6.2: Skip Animation on Selection

**File:** `Forms/BeepPopupForm.cs`

Add property:
```csharp
public bool SkipAnimationOnSelection { get; set; } = true;
```

Modify close logic:
```csharp
public void ClosePopup(bool commit)
{
    if (_form != null)
    {
        _form.FormClosed -= OnFormClosed;
        
        if (SkipAnimationOnSelection && commit)
        {
            // Instant close on selection
            _form.Opacity = 0;
            _form.Close();
        }
        else
        {
            _form.CloseCascade();
        }
        
        _form = null;
        if (!_closeRaised)
        {
            _closeRaised = true;
            PopupClosed?.Invoke(this, new ComboBoxPopupClosedEventArgs(commit));
        }
    }
}
```

### Step 6.3: Focus First Selectable Item

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Modify `UpdateModel`:
```csharp
public void UpdateModel(ComboBoxPopupModel model)
{
    // ... existing code ...
    
    SetKeyboardFocusIndex(model.KeyboardFocusIndex >= 0 ? model.KeyboardFocusIndex : FindFirstSelectableRowIndex());
    
    // Focus the popup content so keyboard works immediately
    if (this.Visible)
    {
        this.Focus();
    }
}
```

### Step 6.4: Restore Focus on Close

**File:** `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`

Modify `ClosePopup`:
```csharp
public void ClosePopup(bool commit)
{
    // Remember owner before closing
    Control? owner = _form?.TriggerControl;
    
    // ... existing close logic ...
    
    // Restore focus to combobox
    owner?.Focus();
}
```

### Step 6.5: Keyboard Navigation Wrapping

**File:** `ComboBoxes/Popup/ComboBoxPopupContent.cs`

Add wrapping support (optional):
```csharp
private bool _wrapKeyboardNavigation = true;

private void MoveFocus(int delta)
{
    // ... existing code ...
    
    // Wrap around if enabled
    if (_wrapKeyboardNavigation)
    {
        if (next >= _rows.Count)
            next = FindFirstSelectableRowIndex();
        else if (next < 0)
            next = FindLastSelectableRowIndex();
    }
    
    // ... rest of method ...
}

private int FindLastSelectableRowIndex()
{
    for (int i = _rows.Count - 1; i >= 0; i--)
    {
        if (ComboBoxPopupRowBehavior.IsSelectable(_rows[i].Model))
            return i;
    }
    return _rows.Count > 0 ? _rows.Count - 1 : -1;
}
```

## Expected Behavior

- Animation is fast and smooth (100ms)
- No animation when selecting with mouse/Enter
- First selectable item focused when popup opens
- Focus returns to combobox when popup closes
- Optional wrap-around keyboard navigation

## Validation

- [ ] Animation completes in <= 100ms
- [ ] No animation when clicking item with mouse
- [ ] No animation when pressing Enter
- [ ] First item focused when popup opens
- [ ] Focus returns to combobox after close
- [ ] Keyboard navigation works immediately without clicking
