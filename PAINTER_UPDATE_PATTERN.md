# Painter Update Pattern - All Drawing in Painters

## Architecture Change

**OLD (WRONG)**:
- Control's DrawContent() calls painter.Draw() for background
- Control's DrawContent() then draws items, icons, text
- Painters only draw container/background

**NEW (CORRECT)**:
- Control's DrawContent() ONLY calls painter.Draw()
- Painter.Draw() does EVERYTHING: background + items + icons + text
- BaseControl only draws borders/shadows

## Pattern for All Painters

### 1. Add using statements
```csharp
using TheTechIdea.Beep.Winform.Controls.Models;
```

### 2. Update Draw() method
```csharp
public override void Draw(BeepSideMenu menu, Graphics g, Rectangle bounds)
{
    g.SmoothingMode = SmoothingMode.AntiAlias;

    // Step 1: Draw background/container (painter-specific)
    // ... your existing background drawing code ...

    // Step 2: Get hover and selection state from adapter
    int? hoveredIndex = null;
    SimpleItem selectedItem = null;
    
    if (menu is BeepSideBarAdapter adapter)
    {
        hoveredIndex = adapter.HoveredItemIndex >= 0 ? adapter.HoveredItemIndex : (int?)null;
        selectedItem = adapter.SelectedItem;
    }
    
    // Step 3: Draw all menu items using helper method
    DrawMenuItems(menu, g, bounds, hoveredIndex, selectedItem);
}
```

## Helper Methods Available from BaseSideMenuPainter

### DrawMenuItems()
```csharp
protected void DrawMenuItems(BeepSideMenu menu, Graphics g, Rectangle bounds, 
    int? hoveredIndex = null, SimpleItem selectedItem = null)
```
- Iterates through all items
- Calls DrawHover() for hovered item
- Calls DrawSelection() for selected item  
- Calls DrawMenuItem() for each item

### DrawMenuItem()
```csharp
protected virtual void DrawMenuItem(BeepSideMenu menu, Graphics g, 
    SimpleItem item, Rectangle itemRect)
```
- Draws icon (if available) using DrawMenuItemIcon()
- Draws text (if not collapsed) using DrawMenuItemText()
- Can be overridden for custom item rendering

### DrawMenuItemIcon()
```csharp
protected virtual void DrawMenuItemIcon(BeepSideMenu menu, Graphics g, 
    SimpleItem item, Rectangle iconRect)
```
- Uses ImagePainter with item.ImagePath
- Applies theme if menu.CurrentTheme != null
- Sets ApplyThemeOnImage = true
- Sets ImageEmbededin = ImageEmbededin.SideBar

### DrawMenuItemText()
```csharp
protected virtual void DrawMenuItemText(BeepSideMenu menu, Graphics g, 
    SimpleItem item, Rectangle textRect)
```
- Uses theme.MenuItemForeColor (enabled) or theme.DisabledForeColor (disabled)
- Falls back to menu.ForeColor if no theme
- Proper StringFormat with ellipsis

## Theme Color Usage

The helpers automatically use theme colors when available:

**Icons**:
- If `menu.CurrentTheme != null` → Apply theme to SVG icons
- ImagePainter.ApplyThemeOnImage = true
- ImagePainter.ImageEmbededin = ImageEmbededin.SideBar

**Text**:
- If `menu.CurrentTheme != null`:
  - Enabled: theme.MenuItemForeColor
  - Disabled: theme.DisabledForeColor
- Else:
  - Enabled: menu.ForeColor
  - Disabled: Color.Gray

**Hover/Selection**:
- Uses menu.AccentColor (which comes from adapter)
- Adapter returns theme.AccentColor if UseThemeColors=true

## Benefits

✅ Painters handle ALL drawing
✅ ImagePainter used correctly with ImagePath property
✅ Theme colors applied automatically
✅ Consistent item rendering across all painters
✅ Easy to override for custom rendering
✅ Control stays clean - only calls painter.Draw()
