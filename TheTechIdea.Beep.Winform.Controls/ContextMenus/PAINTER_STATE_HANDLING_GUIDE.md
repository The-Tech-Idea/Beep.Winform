# Context Menu Painter State Handling Guide

## State Priority Order

All context menu painters follow this priority order for visual states:

1. **Selected** (highest priority) - item is currently selected
2. **Hovered** - mouse is over the item
3. **Normal** - default state
4. **Disabled** - item is not enabled (overrides all other states)

## Visual State Implementation

### Modern Painter State Handling

**Selected State:**
- Fill with `metrics.ItemSelectedBackColor`
- Text color: `metrics.ItemSelectedForeColor`
- Subtle accent border (40 alpha)
- Rounded corners based on `metrics.ItemBorderRadius`

**Hovered State:**
- Fill with `metrics.ItemHoverBackColor`
- Text color: `metrics.ItemHoverForeColor`
- No border
- Rounded corners

**Disabled State:**
- Text color: `metrics.ItemDisabledForeColor`
- Grayed-out icon using `ControlPaint.DrawImageDisabled`
- No background highlight

### Fluent Painter State Handling

**Selected State:**
- Gradient fill from `ItemSelectedBackColor` (245 alpha) to (225 alpha)
- Stronger reveal border (80 alpha accent color)
- Text color: `metrics.ItemSelectedForeColor`

**Hovered State:**
- Gradient fill from `ItemHoverBackColor` (240 alpha) to (220 alpha)
- Subtle reveal border (40 alpha accent color)
- Text color: `metrics.ItemHoverForeColor`

**Disabled State:**
- Same as Modern painter
- Fluent-style checkbox uses accent color with reduced opacity

## Checkbox States

Checkboxes support all item states:

**Checked + Enabled:**
- Border: `metrics.AccentColor` (Fluent) or `metrics.ItemForeColor` (Modern)
- Fill: Accent color background (Fluent only)
- Check mark: White (Fluent) or ForeColor (Modern)

**Unchecked + Enabled:**
- Border: `metrics.AccentColor` or `metrics.ItemForeColor`
- No fill

**Disabled:**
- Border and check mark: `metrics.ItemDisabledForeColor`
- Reduced opacity (100 alpha for Fluent)

## Shortcut Text States

Shortcuts change color based on item state:

- **Normal**: `metrics.ShortcutForeColor`
- **Hovered**: `metrics.ShortcutHoverForeColor`
- **Selected**: `metrics.ItemSelectedForeColor`
- **Disabled**: `metrics.ItemDisabledForeColor`

## Code Pattern

```csharp
private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, Rectangle itemRect,
    bool isSelected, bool isHovered, ContextMenuMetrics metrics, IBeepTheme theme)
{
    // Priority: Selected > Hovered > Normal
    if (item.IsEnabled)
    {
        if (isSelected)
        {
            // Draw selected state (highest priority)
        }
        else if (isHovered)
        {
            // Draw hover state
        }
    }
    
    // Draw content with state-appropriate colors
    var textColor = !item.IsEnabled ? metrics.ItemDisabledForeColor :
                   isSelected ? metrics.ItemSelectedForeColor :
                   isHovered ? metrics.ItemHoverForeColor :
                   metrics.ItemForeColor;
}
```

## Testing Checklist

For each painter, verify:

- [ ] Selected items have distinct visual appearance
- [ ] Hover shows feedback without obscuring selection
- [ ] Disabled items are clearly grayed out
- [ ] Checkbox states are visible in all item states
- [ ] Shortcut text is readable in all states
- [ ] Submenu arrow is visible in all states
- [ ] Icons are properly disabled when item is disabled
- [ ] State transitions are smooth (if animated)
- [ ] High contrast mode is supported
- [ ] Color contrast meets WCAG 2.1 AA standards

## Color Contrast Requirements

Minimum contrast ratios (WCAG 2.1 AA):
- Normal text: 4.5:1
- Large text (14pt bold or 18pt): 3:1
- UI components: 3:1

Ensure these ratios are met:
- Selected text vs selected background
- Hovered text vs hover background
- Normal text vs normal background
- Disabled text vs background

## Animation Considerations

When implementing animations:
1. Transition duration: 150-200ms for hover
2. Easing function: ease-out for smooth feel
3. Animate: background color, border color
4. Do NOT animate: text color (instant change)
5. Respect accessibility settings (reduce motion)

---

**Last Updated**: 2025
**Applies To**: All ContextMenuPainter implementations
