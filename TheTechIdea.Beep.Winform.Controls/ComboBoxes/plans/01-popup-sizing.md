# Phase 1: Fix Popup Sizing

## Problem

The dropdown height is calculated incorrectly, causing:
- Popup too tall or too short for the item count
- Single-item popups showing excessive whitespace or clipped content
- No concept of `IntegralHeight` (showing only whole items)
- Borders and padding not accounted for in size calculations

## Root Cause

Current calculation in `ComboBoxPopupHostForm.cs`:
```csharp
int count = model.FilteredRows?.Count ?? 1;
int contentHeight = count * profile.BaseRowHeight;
if (model.ShowSearchBox || profile.ForceSearchVisible) contentHeight += profile.SearchBoxHeight;
if (model.ShowFooter || profile.ForceFooterVisible) contentHeight += profile.FooterHeight;
return Math.Min(contentHeight, profile.MaxHeight);
```

This ignores:
- Popup form borders and chrome
- Content panel padding (`ListHorizontalPadding`, `ListVerticalPadding`)
- The fact that partial items should not be shown (IntegralHeight)

## Solution

### Step 1.1: Add `IntegralHeight` to Profile

**File:** `ComboBoxes/Popup/ComboBoxPopupHostProfile.cs`

Add property:
```csharp
public bool IntegralHeight { get; set; } = true;
public int PopupBorderThickness { get; set; } = 1;
```

### Step 1.2: Fix Height Calculation

**File:** `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`

Replace `CalculatePopupHeight` method:
```csharp
protected virtual int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
{
    int count = model.FilteredRows?.Count ?? 0;
    if (count == 0)
        return profile.BaseRowHeight * 2; // Minimum height for empty state

    int searchHeight = (model.ShowSearchBox || profile.ForceSearchVisible) ? profile.SearchBoxHeight : 0;
    int footerHeight = (model.ShowFooter || profile.ForceFooterVisible) ? profile.FooterHeight : 0;
    int padding = profile.ListVerticalPadding * 2;
    int borders = profile.PopupBorderThickness * 2;

    int contentHeight = count * profile.BaseRowHeight;
    int totalHeight = contentHeight + searchHeight + footerHeight + padding + borders;

    // Apply integral height - round down to nearest item multiple
    if (profile.IntegralHeight && count > 0)
    {
        int maxContentHeight = profile.MaxHeight - searchHeight - footerHeight - padding - borders;
        int visibleItems = Math.Max(1, maxContentHeight / profile.BaseRowHeight);
        int actualVisibleItems = Math.Min(count, visibleItems);
        contentHeight = actualVisibleItems * profile.BaseRowHeight;
        totalHeight = contentHeight + searchHeight + footerHeight + padding + borders;
    }

    return Math.Min(totalHeight, profile.MaxHeight);
}
```

### Step 1.3: Add `DropDownRows` Property to BeepComboBox

**File:** `ComboBoxes/BeepComboBox.Properties.cs`

Add property:
```csharp
private int _dropDownRows = 8;

[Browsable(true)]
[Category("Behavior")]
[Description("Maximum number of visible items in the dropdown." )]
[DefaultValue(8)]
public int DropDownRows
{
    get => _dropDownRows;
    set
    {
        _dropDownRows = Math.Max(1, value);
    }
}
```

### Step 1.4: Use DropDownRows in Profile

**File:** `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`

Modify `CreateProfile` to set MaxHeight based on DropDownRows:
```csharp
if (owner is BeepComboBox comboOwner)
{
    profile.MaxHeight = comboOwner.DropDownRows * profile.BaseRowHeight 
                        + profile.SearchBoxHeight 
                        + profile.FooterHeight 
                        + profile.ListVerticalPadding * 2;
}
```

## Patterns from Reference Implementations

### dotnet/winforms (Official WinForms ComboBox)
```csharp
// DefaultDropDownHeight = 106 pixels
// Setting DropDownHeight resets IntegralHeight = false
public int DropDownHeight
{
    set
    {
        Properties.AddValue(s_propDropDownHeight, value);
        IntegralHeight = false; // Must disable to use explicit height
    }
}
```

### CodeProject Custom ComboBox
```csharp
// Height calculation from real implementation:
int h = 0;
int maxItemHeight = 0;
int highestItemHeight = 0;
foreach(object item in _listBox.Items)
{
    int itHeight = _listBox.GetItemHeight(i);
    if (highestItemHeight < itHeight) highestItemHeight = itHeight;
    h = h + itHeight;
    if (i <= (_maxDropDownItems - 1)) maxItemHeight = h;
    i = i + 1;
}
if (maxItemHeight > _dropDownHeight)
    _listBox.Height = _dropDownHeight + 3; // +3 for border
else if (maxItemHeight > highestItemHeight)
    _listBox.Height = maxItemHeight + 3;
else
    _listBox.Height = highestItemHeight + 3;
```

### EWSoftware ListControls
```csharp
// Uses MaxDropDownItems (default 8) for initial display
public int MaxDropDownItems
{
    get => maxDropDownItems;
    set
    {
        if(value < 1 || value > 100)
            throw new ArgumentOutOfRangeException(...);
        maxDropDownItems = value;
    }
}
```

## Expected Behavior

- Single-item popup shows exactly one row with minimal padding
- Multi-item popup shows only whole items (no partial item at bottom)
- Height respects MaxHeight constraint
- Empty list shows minimum height (2 rows)

## Validation

- [ ] Single-item popup shows exactly one row
- [ ] 5-item popup shows exactly 5 rows (if within MaxHeight)
- [ ] 20-item popup with DropDownRows=8 shows exactly 8 rows
- [ ] No partial item visible at bottom of popup
- [ ] Search box and footer accounted for in height
