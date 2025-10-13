# Context Menu Painter Quick Reference

## Creating a New Painter (5-Minute Guide)

### Step 1: Create the Painter Class

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    public class YourStyleContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.YourStyle;

        public ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public int GetPreferredItemHeight() => GetMetrics().ItemHeight;

        // Implement remaining methods...
    }
}
```

### Step 2: Implement DrawBackground

```csharp
public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
    ContextMenuMetrics metrics, IBeepTheme theme)
{
    // Your custom background rendering
    using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
    {
        using (var brush = new SolidBrush(metrics.BackgroundColor))
        {
            g.FillPath(brush, path);
        }
    }
}
```

### Step 3: Implement DrawItems

```csharp
public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
    SimpleItem selectedItem, SimpleItem hoveredItem, 
    ContextMenuMetrics metrics, IBeepTheme theme)
{
    if (items == null || items.Count == 0) return;

    int y = metrics.Padding;
    foreach (var item in items)
    {
        if (!item.IsVisible) continue;
        
        if (IsSeparator(item))
        {
            DrawSeparator(g, owner, item, y, metrics);
            y += metrics.SeparatorHeight;
            continue;
        }
        
        var itemRect = new Rectangle(metrics.Padding, y, 
            owner.Width - (metrics.Padding * 2), metrics.ItemHeight);
        
        DrawItem(g, owner, item, itemRect, 
            item == selectedItem, item == hoveredItem, metrics, theme);
        
        y += metrics.ItemHeight;
    }
}
```

### Step 4: Implement DrawBorder

```csharp
public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
    ContextMenuMetrics metrics, IBeepTheme theme)
{
    using (var path = GetRoundedRect(bounds, metrics.BorderRadius))
    using (var pen = new Pen(metrics.BorderColor, metrics.BorderWidth))
    {
        g.DrawPath(pen, path);
    }
}
```

### Step 5: Add to PainterFactory

```csharp
// In PainterFactory.cs
public static IContextMenuPainter CreateContextMenuPainter(FormStyle style)
{
    return style switch
    {
        // ...existing styles...
        FormStyle.YourStyle => new YourStyleContextMenuPainter(),
        // ...
    };
}
```

---

## DrawItem Template (Copy & Customize)

```csharp
private void DrawItem(Graphics g, BeepContextMenu owner, SimpleItem item, 
    Rectangle itemRect, bool isSelected, bool isHovered, 
    ContextMenuMetrics metrics, IBeepTheme theme)
{
    if (!item.IsVisible) return;
    
    bool visuallySelected = isSelected || item.IsSelected;
    
    // 1. Background
    if (item.IsEnabled)
    {
        if (visuallySelected)
        {
            // Draw selected background
        }
        else if (isHovered)
        {
            // Draw hover background
        }
    }
    
    int x = itemRect.X + metrics.ItemPaddingLeft;
    
    // 2. Checkbox (optional)
    if (owner.ShowCheckBox && item.IsCheckable)
    {
        DrawCheckbox(g, checkRect, item.IsChecked, !item.IsEnabled, metrics);
        x += metrics.CheckboxSize + metrics.IconTextSpacing;
    }
    
    // 3. Icon (optional)
    if (owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
    {
        DrawIcon(g, iconRect, item.ImagePath, !item.IsEnabled);
        x += metrics.IconSize + metrics.IconTextSpacing;
    }
    
    // 4. Calculate reserved space on right
    int rightReserved = metrics.ItemPaddingRight;
    if (item.Children?.Count > 0)
        rightReserved += metrics.SubmenuArrowSize + 8;
    string shortcut = item.ShortcutText ?? item.Shortcut;
    if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut))
        rightReserved += 60;
    
    // 5. Text
    var textRect = new Rectangle(x, itemRect.Y, 
        itemRect.Width - x - rightReserved, itemRect.Height);
    var textColor = GetTextColor(item, visuallySelected, isHovered, metrics);
    TextRenderer.DrawText(g, item.DisplayField ?? "", owner.TextFont, 
        textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    
    // 6. Shortcut (optional)
    if (owner.ShowShortcuts && !string.IsNullOrEmpty(shortcut))
    {
        DrawShortcut(g, shortcut, itemRect, rightReserved, textColor, owner);
    }
    
    // 7. Submenu arrow (optional)
    if (item.Children?.Count > 0)
    {
        DrawSubmenuArrow(g, itemRect, !item.IsEnabled, metrics, textColor);
    }
}
```

---

## Common Helper Methods

### GetTextColor

```csharp
private Color GetTextColor(SimpleItem item, bool isSelected, bool isHovered, 
    ContextMenuMetrics metrics)
{
    if (!item.IsEnabled) return metrics.ItemDisabledForeColor;
    if (isSelected) return metrics.ItemSelectedForeColor;
    if (isHovered) return metrics.ItemHoverForeColor;
    return metrics.ItemForeColor;
}
```

### DrawIcon (Always Use This)

```csharp
private void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool isDisabled)
{
    if (string.IsNullOrEmpty(imagePath)) return;
    
    try
    {
        if (isDisabled)
        {
            using (var temp = new Bitmap(rect.Width, rect.Height))
            {
                using (var tempG = Graphics.FromImage(temp))
                {
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters
                        .StyledImagePainter.Paint(tempG, 
                        new Rectangle(0, 0, rect.Width, rect.Height), imagePath);
                }
                ControlPaint.DrawImageDisabled(g, temp, rect.X, rect.Y, 
                    Color.Transparent);
            }
        }
        else
        {
            TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters
                .StyledImagePainter.Paint(g, rect, imagePath);
        }
    }
    catch { /* Fail silently */ }
}
```

### GetRoundedRect

```csharp
private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
{
    var path = new GraphicsPath();
    if (radius <= 0)
    {
        path.AddRectangle(rect);
        return path;
    }
    
    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
    path.CloseFigure();
    return path;
}
```

### IsSeparator

```csharp
private bool IsSeparator(SimpleItem item)
{
    return item != null && 
           (item.DisplayField == "-" || 
            item.Tag?.ToString() == "separator");
}
```

---

## Metrics Customization

### In ContextMenuMetrics.cs

```csharp
private static void ApplyYourStyleStyle(ContextMenuMetrics m)
{
    // Dimensions
    m.ItemHeight = 34;
    m.IconSize = 22;
    m.BorderRadius = 10;
    m.ItemBorderRadius = 6;
    m.ShadowDepth = 5;
    
    // Colors
    m.BackgroundColor = Color.FromArgb(255, 250, 250, 250);
    m.ItemHoverBackColor = Color.FromArgb(240, 240, 245);
    m.ItemSelectedBackColor = Color.FromArgb(100, 150, 255);
    m.BorderColor = Color.FromArgb(200, 200, 205);
    
    // Features
    m.ShowElevation = true;
    m.ShowRippleEffect = false;
    m.UseRoundedItems = true;
}
```

---

## Testing Your Painter

### Quick Test Code

```csharp
var menu = new BeepContextMenu();
menu.MenuStyle = FormStyle.YourStyle;

menu.MenuItems.Add(new SimpleItem 
{ 
    DisplayField = "Test Item",
    ImagePath = "icon.png",
    IsCheckable = true,
    ShortcutText = "Ctrl+T"
});

menu.Show(this, new Point(100, 100));
```

---

## Checklist Before Committing

- [ ] Painter implements IContextMenuPainter
- [ ] All 4 interface methods implemented
- [ ] Uses ContextMenuMetrics.DefaultFor()
- [ ] Handles all SimpleItem properties correctly
- [ ] Uses StyledImagePainter for icons
- [ ] Respects IsVisible, IsEnabled, IsSelected
- [ ] Handles IsCheckable and IsChecked
- [ ] Shows submenu arrow for Children
- [ ] Renders shortcut text properly
- [ ] Added to PainterFactory switch
- [ ] Metrics defined in ContextMenuMetrics
- [ ] No compilation errors
- [ ] Tested in runtime

---

## Common Pitfalls to Avoid

? **DON'T**:
- Don't load images directly - use StyledImagePainter
- Don't ignore item.IsVisible
- Don't forget to combine isSelected with item.IsSelected
- Don't hardcode colors - use metrics
- Don't render checkbox without checking IsCheckable
- Don't skip separator detection

? **DO**:
- Always check item.IsVisible first
- Use metrics for all dimensions and colors
- Combine parameter state with property state
- Use GraphicsPath for rounded corners
- Handle all states (selected, hovered, disabled)
- Test with theme integration

---

## Performance Tips

1. **Cache GraphicsPaths** for rounded rectangles
2. **Reuse Brushes and Pens** when possible
3. **Use metrics caching** in BeepContextMenu
4. **Lazy-load painters** via factory
5. **Double-buffer** in BeepContextMenu (already done)

---

## Questions?

Refer to:
- `CONTEXT_MENU_STYLE_REVISION_PLAN.md` - Full implementation plan
- `PAINTER_STATE_HANDLING_GUIDE.md` - State management details
- `SIMPLEITEM_PROPERTY_USAGE_GUIDE.md` - Property reference
- `IMPLEMENTATION_SUMMARY.md` - Current status

---

**Happy Painting! ??**
