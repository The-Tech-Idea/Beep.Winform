# BeepTree Theme Color Mappings

## Official Theme Color Properties for Tree Painters

Based on `BeepTree.ApplyTheme()` method, all tree painters **MUST** use these specific theme properties:

### ✅ CORRECT Theme Properties

```csharp
// Background Colors
_theme.TreeBackColor              // Main tree background
_theme.TreeForeColor              // Main tree text color

// Node State Colors
_theme.TreeNodeSelectedBackColor  // Selected node background
_theme.TreeNodeSelectedForeColor  // Selected node text
_theme.TreeNodeHoverBackColor     // Hovered node background  
_theme.TreeNodeHoverForeColor     // Hovered node text (if needed)

// Accent & Borders
_theme.AccentColor                // Highlights, chevrons, borders
_theme.BorderColor                // Separator lines, borders
```

---

## ❌ WRONG - Generic Control Properties

**DO NOT USE** these generic control properties in tree painters:

```csharp
// ❌ WRONG for trees
_theme.BackColor              // Use _theme.TreeBackColor instead
_theme.ForeColor              // Use _theme.TreeForeColor instead
_theme.SelectedRowBackColor   // Use _theme.TreeNodeSelectedBackColor instead
_theme.SelectedForeColor      // Use _theme.TreeNodeSelectedForeColor instead
_theme.HoverBackColor         // Use _theme.TreeNodeHoverBackColor instead
```

---

## Implementation Examples

### ✅ CORRECT: InfrastructureTreePainter

```csharp
public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
{
    Color backColor;
    if (isSelected)
    {
        backColor = _theme.TreeNodeSelectedBackColor;  // ✅ CORRECT
        using (var pen = new Pen(_theme.AccentColor, 1))
            g.DrawRectangle(pen, nodeBounds);
    }
    else if (isHovered)
    {
        backColor = _theme.TreeNodeHoverBackColor;     // ✅ CORRECT
    }
    else
    {
        return; // Transparent
    }

    using (var brush = new SolidBrush(backColor))
        g.FillRectangle(brush, nodeBounds);
}

public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
{
    Color textColor = isSelected 
        ? _theme.TreeNodeSelectedForeColor   // ✅ CORRECT
        : _theme.TreeForeColor;               // ✅ CORRECT
    
    TextRenderer.DrawText(g, text, font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
}

public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
{
    Color chevronColor = isHovered 
        ? _theme.AccentColor        // ✅ CORRECT
        : _theme.TreeForeColor;     // ✅ CORRECT
    
    using (var pen = new Pen(chevronColor, 2))
    {
        // Draw chevron...
    }
}
```

### ✅ CORRECT: FileManagerTreePainter

```csharp
public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
{
    if (isSelected)
    {
        using (var path = CreateRoundedRectangle(nodeBounds, 6))
        {
            using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))  // ✅ CORRECT
                g.FillPath(brush, path);
            
            using (var pen = new Pen(_theme.AccentColor, 1))
                g.DrawPath(pen, path);
        }
    }
    else if (isHovered)
    {
        using (var path = CreateRoundedRectangle(nodeBounds, 6))
        using (var brush = new SolidBrush(_theme.TreeNodeHoverBackColor))  // ✅ CORRECT
            g.FillPath(brush, path);
    }
}

public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
{
    Color textColor = isSelected 
        ? _theme.TreeNodeSelectedForeColor   // ✅ CORRECT
        : _theme.TreeForeColor;               // ✅ CORRECT
    
    TextRenderer.DrawText(g, text, font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
}
```

---

## BeepTree.ApplyTheme() Reference

This is the **authoritative source** for tree color mappings:

```csharp
public override void ApplyTheme()
{
    Size originalSize = this.Size;
    base.ApplyTheme();
    
    if (IsChild)
    {
        ParentBackColor = Parent.BackColor;
        BackColor = ParentBackColor;
    }
    else
    {
        BackColor = _currentTheme.TreeBackColor;  // ← Tree background
    }

    // ... font setup ...

    ForeColor = _currentTheme.TreeForeColor;  // ← Tree text color

    // Configure button (used for node rendering)
    _button.IsColorFromTheme = false;
    _button.BackColor = BackColor;
    _button.ForeColor = _currentTheme.TreeForeColor;                    // ← Node text
    _button.SelectedForeColor = _currentTheme.TreeNodeSelectedForeColor; // ← Selected text
    _button.SelectedBackColor = _currentTheme.TreeNodeSelectedBackColor; // ← Selected background
    _button.BorderColor = _currentTheme.TreeNodeSelectedBackColor;       // ← Selection border
    _button.HoverBackColor = _currentTheme.TreeNodeHoverBackColor;       // ← Hover background
    _button.HoverForeColor = _currentTheme.TreeNodeHoverForeColor;       // ← Hover text
    _button.MaxImageSize = new Size(GetScaledBoxSize(), GetScaledBoxSize());

    // Scrollbars use generic theme
    if (_verticalScrollBar != null)
        _verticalScrollBar.Theme = Theme;
    if (_horizontalScrollBar != null)
        _horizontalScrollBar.Theme = Theme;

    // Restore size
    if (this.Size != originalSize)
    {
        System.Diagnostics.Debug.WriteLine($"Theme changed size from {originalSize} to {Size} - restoring!");
        this.Size = originalSize;
    }
}
```

---

## Complete Theme Property Reference

### IBeepTheme Interface - Tree-Specific Properties

```csharp
// Tree Container
Color TreeBackColor                  // Main tree background
Color TreeForeColor                  // Main tree text

// Tree Nodes
Color TreeNodeSelectedBackColor      // Selected node background
Color TreeNodeSelectedForeColor      // Selected node text
Color TreeNodeHoverBackColor         // Hovered node background
Color TreeNodeHoverForeColor         // Hovered node text

// Shared Accents (OK to use)
Color AccentColor                    // Highlights, focus, active elements
Color BorderColor                    // Lines, separators, borders

// Typography (for reference)
ThemeTypography TreeNodeUnSelectedFont  // Normal node font
ThemeTypography TreeNodeSelectedFont    // Selected node font (if exists)
```

---

## Migration Checklist

When creating new tree painters:

- [ ] ✅ Use `_theme.TreeBackColor` for tree background
- [ ] ✅ Use `_theme.TreeForeColor` for normal text
- [ ] ✅ Use `_theme.TreeNodeSelectedBackColor` for selected node background
- [ ] ✅ Use `_theme.TreeNodeSelectedForeColor` for selected node text
- [ ] ✅ Use `_theme.TreeNodeHoverBackColor` for hover background
- [ ] ✅ Use `_theme.TreeNodeHoverForeColor` for hover text
- [ ] ✅ Use `_theme.AccentColor` for highlights and chevrons
- [ ] ✅ Use `_theme.BorderColor` for lines and borders
- [ ] ❌ Avoid `_theme.BackColor`, `_theme.ForeColor`, `_theme.SelectedRowBackColor`
- [ ] ❌ Never hardcode colors like `Color.FromArgb(230, 240, 255)`

---

## Summary

**Key Principle:**
> Trees have their own dedicated theme properties (`TreeBackColor`, `TreeNodeSelectedBackColor`, etc.)  
> Painters **MUST** use these tree-specific properties, not generic control properties.

This ensures:
1. Consistent appearance with existing BeepTree rendering
2. Proper theme integration across light/dark themes
3. Correct color inheritance from theme system
4. No conflicts with grid/button/other control colors

**Always reference `BeepTree.ApplyTheme()` as the source of truth for color mappings!**
