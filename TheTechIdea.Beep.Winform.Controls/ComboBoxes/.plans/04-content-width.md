# Phase 4: Fix Content Width

## Problem

The popup content doesn't always match the popup width, causing:
- Horizontal scrollbar appearing unnecessarily
- Content clipped on the right
- Empty space on the right when content is narrower
- Long items not handled gracefully

## Root Cause

- Content panel width is calculated from `triggerBounds.Width` but doesn't account for popup borders
- List items may be wider than the popup due to long text
- No auto-width to fit content feature
- No ellipsis or tooltip for truncated text

## Solution

### Step 4.1: Measure Content Width

**File:** `ComboBoxes/Popup/ComboBoxListBoxPopupContent.cs`

Add method to measure widest item:
```csharp
private int MeasureWidestItemWidth()
{
    if (_listBox.ListItems == null || _listBox.ListItems.Count == 0)
        return 0;

    int maxWidth = 0;
    using (Graphics g = _listBox.CreateGraphics())
    {
        foreach (var item in _listBox.ListItems)
        {
            string text = item.Text ?? string.Empty;
            SizeF size = g.MeasureString(text, _listBox.Font);
            maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(size.Width));
            
            // Include subtext if present
            if (!string.IsNullOrEmpty(item.SubText))
            {
                SizeF subSize = g.MeasureString(item.SubText, _tokens.SubTextFont ?? _listBox.Font);
                maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(subSize.Width));
            }
        }
    }
    
    // Add padding for checkbox, image, and margins
    maxWidth += 48; // checkbox + image + margins
    
    return maxWidth;
}
```

### Step 4.2: Auto-Size Popup Width

**File:** `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`

Modify `ShowPopup` to support auto-width:
```csharp
int desiredWidth = Math.Max(triggerBounds.Width, Math.Max(_profile.MinWidth, minWidthOverride));

// If auto-size enabled, measure content width
if (owner is BeepComboBox comboOwner && comboOwner.AutoSizeDropDown)
{
    int contentWidth = MeasureContentWidth(effectiveModel);
    desiredWidth = Math.Max(desiredWidth, contentWidth);
}

// Apply max width constraint
desiredWidth = Math.Min(desiredWidth, _profile.MaxWidth);
```

### Step 4.3: Add Properties to BeepComboBox

**File:** `ComboBoxes/BeepComboBox.Properties.cs`

Add properties:
```csharp
private bool _autoSizeDropDown = true;
private int _dropDownWidth = 0; // 0 = auto

[Browsable(true)]
[Category("Behavior")]
[Description("Automatically size dropdown to fit content width.")]
[DefaultValue(true)]
public bool AutoSizeDropDown
{
    get => _autoSizeDropDown;
    set => _autoSizeDropDown = value;
}

[Browsable(true)]
[Category("Behavior")]
[Description("Explicit dropdown width. 0 = auto-size.")]
[DefaultValue(0)]
public int DropDownWidth
{
    get => _dropDownWidth;
    set => _dropDownWidth = Math.Max(0, value);
}
```

### Step 4.4: Handle Long Items with Ellipsis

**File:** `ComboBoxes/Popup/ComboBoxPopupRow.cs`

Ensure row rendering uses ellipsis:
```csharp
// In paint method:
TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
TextRenderer.DrawText(g, text, font, bounds, color, flags);
```

### Step 4.5: Add Tooltip for Truncated Text

**File:** `ComboBoxes/Popup/ComboBoxPopupRow.cs`

Add tooltip on hover if text is truncated:
```csharp
protected override void OnMouseHover(EventArgs e)
{
    base.OnMouseHover(e);
    
    if (_model != null && IsTextTruncated(_model.Text))
    {
        _toolTip.Show(_model.Text, this, Point.Empty, 2000);
    }
}

private bool IsTextTruncated(string text)
{
    if (string.IsNullOrEmpty(text)) return false;
    
    using (Graphics g = CreateGraphics())
    {
        SizeF size = g.MeasureString(text, Font);
        return size.Width > TextBounds.Width;
    }
}
```

## Expected Behavior

- Popup width = max(combobox width, widest item width + scrollbar width + padding)
- Content fills popup width exactly
- Long items show ellipsis, not horizontal scrollbar
- Tooltip shows full text on hover for truncated items

## Validation

- [ ] Popup width matches combobox width when items are short
- [ ] Popup expands to fit long items when AutoSizeDropDown=true
- [ ] Long items show ellipsis (...)
- [ ] Tooltip appears on hover for truncated items
- [ ] DropDownWidth property overrides auto-size
- [ ] No horizontal scrollbar appears
