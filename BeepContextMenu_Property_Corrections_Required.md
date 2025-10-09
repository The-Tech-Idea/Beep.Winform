# BeepContextMenu Property Corrections Required

## Issues Found

### 1. **SimpleItem Property Mappings - WRONG**

#### Current (INCORRECT):
- ❌ `item.Disabled` - This property doesn't exist!
- ❌ `item.Checked` - This property doesn't exist!
- ❌ `item.Shortcut` - This property doesn't exist!

#### Correct (From SimpleItem.cs):
- ✅ `item.IsEnabled` - Use `!item.IsEnabled` for disabled state
- ✅ `item.IsChecked` - For checkbox state
- ✅ `item.KeyCombination` - For keyboard shortcuts

### 2. **Theme Color Mappings - WRONG**

#### Current (INCORRECT) - Using Generic Colors:
- ❌ `theme.BackgroundColor`
- ❌ `theme.HoverBackColor`
- ❌ `theme.SelectedBackColor`
- ❌ `theme.ForegroundColor`
- ❌ `theme.BorderColor`
- ❌ `theme.DisabledForeColor`
- ❌ `theme.PrimaryColor`

#### Correct (From BeepTheme.Menu.cs):
- ✅ `theme.MenuBackColor` - Menu background
- ✅ `theme.MenuForeColor` - Menu text
- ✅ `theme.MenuBorderColor` - Menu borders
- ✅ `theme.MenuItemHoverBackColor` - Item hover background
- ✅ `theme.MenuItemHoverForeColor` - Item hover text
- ✅ `theme.MenuItemSelectedBackColor` - Selected item background
- ✅ `theme.MenuItemSelectedForeColor` - Selected item text
- ✅ `theme.MenuItemForeColor` - Normal item text
- ✅ `theme.DisabledForeColor` - Disabled items (from Core)

---

## Files That Need Correction

1. StandardContextMenuPainter.cs
2. MaterialContextMenuPainter.cs
3. MinimalContextMenuPainter.cs
4. FlatContextMenuPainter.cs
5. OfficeContextMenuPainter.cs

---

## Correct Property Usage Pattern

### SimpleItem Properties:
```csharp
// Check if disabled
if (!item.IsEnabled) { /* disabled logic */ }

// Check if checked
if (item.IsChecked) { /* show checkmark */ }

// Get keyboard shortcut
string shortcut = item.KeyCombination;
```

### Theme Color Usage:
```csharp
// Background
using (var brush = new SolidBrush(theme.MenuBackColor))

// Hover state
if (isHovered && item.IsEnabled)
{
    using (var brush = new SolidBrush(theme.MenuItemHoverBackColor))
    {
        g.FillRectangle(brush, itemRect);
    }
}

// Selected state
if (isSelected && item.IsEnabled)
{
    using (var brush = new SolidBrush(theme.MenuItemSelectedBackColor))
}

// Text color
var textColor = !item.IsEnabled ? theme.DisabledForeColor : 
                isHovered ? theme.MenuItemHoverForeColor :
                isSelected ? theme.MenuItemSelectedForeColor :
                theme.MenuItemForeColor;

// Border
using (var pen = new Pen(theme.MenuBorderColor, 1))
```

---

## All Corrections Needed

### In ALL Painters:

1. Replace `item.Disabled` with `!item.IsEnabled`
2. Replace `item.Checked` with `item.IsChecked`
3. Replace `item.Shortcut` with `item.KeyCombination`
4. Replace `theme.BackgroundColor` with `theme.MenuBackColor`
5. Replace `theme.ForegroundColor` with `theme.MenuItemForeColor`
6. Replace `theme.HoverBackColor` with `theme.MenuItemHoverBackColor`
7. Replace `theme.SelectedBackColor` with `theme.MenuItemSelectedBackColor`
8. Replace `theme.BorderColor` with `theme.MenuBorderColor`
9. Keep `theme.DisabledForeColor` (from Core, not Menu)

---

## Status
**NEEDS IMMEDIATE CORRECTION** - All 5 painters are using wrong property names!
