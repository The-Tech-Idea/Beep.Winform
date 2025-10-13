# SimpleItem Property Usage in Context Menu Painters

## Overview
This document details how `SimpleItem` properties are utilized in context menu painter implementations to ensure consistent and proper rendering of menu items.

---

## SimpleItem Properties Used

### Visual State Properties

| Property | Type | Usage | Painter Implementation |
|----------|------|-------|----------------------|
| **IsVisible** | `bool` | Controls item visibility | Early return if `!item.IsVisible` |
| **IsEnabled** | `bool` | Controls item interactivity | Determines color scheme, disables interactions |
| **IsSelected** | `bool` | Item selection state | Combined with `isSelected` parameter for visual selection |
| **IsChecked** | `bool` | Checkbox checked state | Renders check mark in checkbox |
| **IsCheckable** | `bool` | Shows checkbox for item | Determines if checkbox is rendered |
| **IsExpanded** | `bool` | Submenu expansion (future) | Reserved for tree-like menus |

### Display Properties

| Property | Type | Usage | Painter Implementation |
|----------|------|-------|----------------------|
| **DisplayField** | `string` | Main text displayed | Primary menu item text |
| **Text** | `string` | Fallback text | Used if DisplayField is empty |
| **ImagePath** | `string` | Icon/image path | Rendered using `StyledImagePainter` |
| **ShortcutText** | `string` | Keyboard shortcut display | Displayed on right side (e.g., "Ctrl+S") |
| **Shortcut** | `string` | Fallback shortcut | Used if ShortcutText is empty |
| **ToolTip** | `string` | Tooltip text | Future: hover tooltip implementation |

### Hierarchy Properties

| Property | Type | Usage | Painter Implementation |
|----------|------|-------|----------------------|
| **Children** | `BindingList<SimpleItem>` | Submenu items | Renders submenu arrow if Count > 0 |
| **ParentItem** | `SimpleItem` | Parent reference | Navigation and context |
| **ParentValue** | `string` | Parent identifier | Data binding |

### Data Properties

| Property | Type | Usage | Painter Implementation |
|----------|------|-------|----------------------|
| **GuidId** | `string` | Unique identifier | Item tracking and equality |
| **Name** | `string` | Item name | Internal identification |
| **Value** | `object` | Associated value | Data storage |
| **Item** | `object` | Selected item reference | Data binding |
| **Tag** | `object` | Custom data | Separator detection, custom metadata |

---

## Painter Implementation Details

### State Priority Order

```csharp
// Combine parameter state with property state
bool visuallySelected = isSelected || item.IsSelected;

if (item.IsEnabled)
{
    if (visuallySelected)
    {
        // Draw selected state (highest priority)
    }
    else if (isHovered)
    {
        // Draw hover state
    }
}
// Disabled state overrides all
```

### Checkbox Rendering Logic

```csharp
// Only show checkbox if item is explicitly checkable
if (owner.ShowCheckBox && item.IsCheckable)
{
    DrawCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
}
```

### Icon Rendering Logic

```csharp
// Always use StyledImagePainter for consistent image handling
if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
{
    DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
}

private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool isDisabled)
{
    if (isDisabled)
    {
        // Create grayscale version using ControlPaint
    }
    else
    {
        // Use StyledImagePainter directly
        StyledImagePainter.Paint(g, rect, imagePath);
    }
}
```

### Shortcut Text Logic

```csharp
// Prefer ShortcutText, fallback to Shortcut
string shortcutDisplay = !string.IsNullOrEmpty(item.ShortcutText) 
    ? item.ShortcutText 
    : item.Shortcut;

if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
{
    // Render on right side with appropriate color
}
```

### Submenu Arrow Logic

```csharp
// Show arrow only if children exist
if (item.Children != null && item.Children.Count > 0)
{
    DrawSubmenuArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
}
```

### Separator Detection

```csharp
private bool IsSeparator(SimpleItem item)
{
    return item != null && 
           (item.DisplayField == "-" || 
            item.Tag?.ToString() == "separator");
}
```

---

## Color Application by State

### Text Colors

| State | Property Used | Color Applied |
|-------|--------------|---------------|
| Disabled | `item.IsEnabled = false` | `metrics.ItemDisabledForeColor` |
| Selected | `visuallySelected = true` | `metrics.ItemSelectedForeColor` |
| Hovered | `isHovered = true` | `metrics.ItemHoverForeColor` |
| Normal | Default | `metrics.ItemForeColor` |

### Background Colors

| State | Property Used | Color Applied |
|-------|--------------|---------------|
| Selected | `visuallySelected = true` | `metrics.ItemSelectedBackColor` |
| Hovered | `isHovered = true` | `metrics.ItemHoverBackColor` |
| Normal | Default | Transparent or `metrics.ItemBackColor` |

### Shortcut Colors

| State | Property Used | Color Applied |
|-------|--------------|---------------|
| Disabled | `item.IsEnabled = false` | `metrics.ItemDisabledForeColor` |
| Selected | `visuallySelected = true` | `metrics.ItemSelectedForeColor` |
| Hovered | `isHovered = true` | `metrics.ShortcutHoverForeColor` |
| Normal | Default | `metrics.ShortcutForeColor` |

---

## Layout Calculations

### Right-Side Reserved Space

```csharp
int rightReserved = metrics.ItemPaddingRight;

// Add space for submenu arrow
if (item.Children != null && item.Children.Count > 0)
{
    rightReserved += metrics.SubmenuArrowSize + 8;
}

// Add space for shortcut text
if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcutDisplay))
{
    rightReserved += 60;
}
```

### Left-Side Layout Flow

```csharp
int x = itemRect.X + metrics.ItemPaddingLeft;

// 1. Checkbox (if checkable)
if (owner.ShowCheckBox && item.IsCheckable)
{
    x += metrics.CheckboxSize + metrics.IconTextSpacing;
}

// 2. Icon (if present)
if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
{
    x += metrics.IconSize + metrics.IconTextSpacing;
}

// 3. Text fills remaining space
var textRect = new Rectangle(x, itemRect.Y, 
    itemRect.Width - x - rightReserved, itemRect.Height);
```

---

## Future Enhancements

### Properties to Implement

1. **ToolTip** - Show on hover delay
2. **IsExpanded** - For tree-style menus with persistent expansion
3. **SubText, SubText2, SubText3** - Additional descriptive text
4. **Description** - Detailed description in status bar

### Animation Properties (Future)

```csharp
// Potential animation properties
public TimeSpan HoverDelay { get; set; }
public TimeSpan SubmenuDelay { get; set; }
public bool EnableAnimations { get; set; }
```

---

## Testing Checklist

For each painter implementation, verify:

- [ ] `IsVisible` properly hides items
- [ ] `IsEnabled` shows disabled visual state
- [ ] `IsSelected` highlights selected items
- [ ] `IsChecked` renders check mark correctly
- [ ] `IsCheckable` controls checkbox visibility
- [ ] `DisplayField` or `Text` displays correctly
- [ ] `ImagePath` renders via `StyledImagePainter`
- [ ] `ShortcutText` or `Shortcut` displays on right
- [ ] `Children.Count > 0` shows submenu arrow
- [ ] `Tag = "separator"` renders separator
- [ ] Disabled items have grayed-out icons
- [ ] All state combinations work correctly

---

## Code Review Checklist

When reviewing painter implementations:

1. ? Always check `item.IsVisible` first
2. ? Use `item.IsCheckable` before rendering checkbox
3. ? Prefer `item.ShortcutText` over `item.Shortcut`
4. ? Always use `StyledImagePainter` for icons
5. ? Combine `isSelected` parameter with `item.IsSelected`
6. ? Check `item.Children?.Count > 0` for submenu arrow
7. ? Handle disabled state for all visual elements
8. ? Calculate layout flow consistently (left to right)
9. ? Reserve appropriate right-side space
10. ? Apply proper color based on state priority

---

**Last Updated**: 2025
**Applies To**: All ContextMenuPainter implementations
**SimpleItem Version**: With IsCheckable and ShortcutText properties
