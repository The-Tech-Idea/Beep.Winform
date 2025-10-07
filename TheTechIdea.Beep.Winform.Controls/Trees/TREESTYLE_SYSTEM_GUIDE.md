# BeepTree TreeStyle System - Implementation Guide

## Overview

The **TreeStyle** system provides distinct visual styles for BeepTree, separate from the `ControlStyle` property inherited from BaseControl. This allows trees to have specialized UI designs based on popular frameworks and applications while maintaining theme color integration.

---

## Key Concepts

### ControlStyle vs TreeStyle

- **ControlStyle** (from BaseControl): Controls the base styling (Material3, iOS15, Fluent2, etc.) and provides theme colors
- **TreeStyle** (BeepTree-specific): Controls the tree's visual layout, animations, and UI patterns

**Both work together:**
```csharp
// BaseControl.ControlStyle provides theme colors
var theme = BeepThemesManager.GetTheme(this.Theme);

// BeepTree.TreeStyle selects visual design pattern
this.TreeStyle = TreeStyle.FileManagerTree;
```

---

## TreeStyle Enum

Located in: `Trees/Models/TreeStyle.cs`

### Categories

#### Application-Specific Styles (Based on Reference Images)
```csharp
TreeStyle.InfrastructureTree    // VMware vSphere style - dark, colored tags, status indicators
TreeStyle.PortfolioTree          // Jira/Atlassian - progress bars, effort tracking, grouped themes
TreeStyle.FileManagerTree        // Google Drive/OneDrive - clean folders, rounded selection
TreeStyle.ActivityLogTree        // Timeline style - status icons, timestamps, expandable activities
TreeStyle.DragDropTree           // Visual drop zones, connection lines, parent/child relationships
TreeStyle.ComponentTree          // Figma/VS Code sidebar - drag handles, grouped sections
TreeStyle.FileBrowserTree        // Deep nesting, type-specific icons, folder hierarchies
TreeStyle.DocumentTree           // Mixed content types (folders, files, documents, media)
```

#### Modern Framework Styles
```csharp
TreeStyle.Material3              // Google Material Design 3 - elevation, rounded corners
TreeStyle.Fluent2                // Microsoft Fluent 2 - acrylic, subtle shadows
TreeStyle.iOS15                  // Apple iOS 15 - SF Symbols, rounded rectangles
TreeStyle.MacOSBigSur            // macOS Big Sur - sidebar design, transparency
TreeStyle.NotionMinimal          // Notion - clean indentation, database hierarchy
TreeStyle.VercelClean            // Vercel dashboard - minimalist, subtle hover effects
TreeStyle.Discord                // Discord channels - dark theme, channel/server hierarchy
TreeStyle.AntDesign              // Ant Design - enterprise Chinese design system
TreeStyle.ChakraUI               // Chakra UI - accessible React components
TreeStyle.Bootstrap              // Bootstrap - classic framework aesthetics
TreeStyle.TailwindCard           // Tailwind CSS - utility-first, card-based nodes
```

#### Enterprise Component Library Styles
```csharp
TreeStyle.DevExpress             // DevExpress - professional enterprise controls
TreeStyle.Syncfusion             // Syncfusion - modern enterprise components
TreeStyle.Telerik                // Telerik UI - polished professional controls
```

#### Modern Effects
```csharp
TreeStyle.Windows11Mica          // Windows 11 - Mica material, dynamic backdrop
TreeStyle.GlassAcrylic           // Transparent glass with blur effects
TreeStyle.Neumorphism            // Soft UI - subtle shadows and highlights
TreeStyle.DarkGlow               // High contrast dark with accent glows
TreeStyle.GradientModern         // Colorful gradients for states
TreeStyle.PillRail               // Rounded pill-shaped selection (navigation)
TreeStyle.StripeDashboard        // Stripe - clean fintech dashboard design
TreeStyle.FigmaCard              // Figma layers panel - card-style hierarchy
```

---

## Painter Architecture

### Interface: ITreePainter

```csharp
public interface ITreePainter
{
    void Initialize(BeepTree owner, IBeepTheme theme);
    void Paint(Graphics g, BeepTree owner, Rectangle bounds);
    void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected);
    void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered);
    void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered);
    void PaintIcon(Graphics g, Rectangle iconRect, string imagePath);
    void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered);
    void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected);
    int GetPreferredRowHeight(SimpleItem item, Font font);
}
```

### Base Class: BaseTreePainter

Provides default implementations that:
- Use **theme colors** for all visual elements
- Respect `_theme.ForeColor`, `_theme.BackColor`, `_theme.AccentColor`, etc.
- Provide consistent fallback behavior

```csharp
public abstract class BaseTreePainter : ITreePainter
{
    protected BeepTree _owner;
    protected IBeepTheme _theme;  // ALWAYS use theme colors!
    
    public virtual void Initialize(BeepTree owner, IBeepTheme theme)
    {
        _owner = owner;
        _theme = theme;
    }
    
    // Default implementations use theme colors
    protected Color GetTextColor(bool isSelected) 
        => isSelected ? _theme.SelectedForeColor : _theme.ForeColor;
    
    protected Color GetBackColor(bool isSelected, bool isHovered)
    {
        if (isSelected) return _theme.SelectedRowBackColor;
        if (isHovered) return _theme.HoverBackColor;
        return _theme.BackColor;
    }
}
```

### Factory Pattern

```csharp
// BeepTreePainterFactory.cs
public static ITreePainter CreatePainter(TreeStyle style, BeepTree owner, IBeepTheme theme)
{
    // Cache painters for performance
    if (_painterCache.TryGetValue(style, out var cached))
    {
        cached.Initialize(owner, theme);
        return cached;
    }
    
    ITreePainter painter = style switch
    {
        TreeStyle.InfrastructureTree => new InfrastructureTreePainter(),
        TreeStyle.FileManagerTree => new FileManagerTreePainter(),
        // ... all 32 styles
        _ => new StandardTreePainter()
    };
    
    painter.Initialize(owner, theme);
    _painterCache[style] = painter;
    return painter;
}
```

---

## Theme Integration Rules

### ✅ CORRECT: Use Theme Colors

```csharp
public class MyTreePainter : BaseTreePainter
{
    public override void PaintNodeBackground(Graphics g, Rectangle bounds, bool isHovered, bool isSelected)
    {
        Color backColor = isSelected 
            ? _theme.SelectedRowBackColor  // ✅ From theme
            : isHovered 
                ? _theme.HoverBackColor     // ✅ From theme
                : _theme.BackColor;          // ✅ From theme
        
        using (var brush = new SolidBrush(backColor))
            g.FillRectangle(brush, bounds);
    }
    
    public override void PaintText(Graphics g, Rectangle rect, string text, Font font, bool isSelected, bool isHovered)
    {
        Color textColor = isSelected 
            ? _theme.SelectedForeColor  // ✅ From theme
            : _theme.ForeColor;          // ✅ From theme
        
        TextRenderer.DrawText(g, text, font, rect, textColor, ...);
    }
    
    public override void PaintToggle(Graphics g, Rectangle rect, bool isExpanded, bool hasChildren, bool isHovered)
    {
        Color chevronColor = isHovered 
            ? _theme.AccentColor  // ✅ From theme
            : _theme.ForeColor;    // ✅ From theme
        
        using (var pen = new Pen(chevronColor, 2))
            // Draw chevron...
    }
}
```

### ❌ WRONG: Hardcoded Colors

```csharp
// ❌ DON'T DO THIS!
Color backColor = Color.FromArgb(230, 240, 255);  // ❌ Hardcoded
Color textColor = Color.FromArgb(60, 64, 67);     // ❌ Hardcoded
Color hoverColor = Color.FromArgb(245, 247, 250); // ❌ Hardcoded
```

---

## Available Theme Colors

```csharp
// From IBeepTheme interface
_theme.BackColor                  // Main background
_theme.ForeColor                  // Main text
_theme.BorderColor                // Borders and separators
_theme.AccentColor                // Highlights and focus
_theme.SelectedRowBackColor       // Selected item background
_theme.SelectedForeColor          // Selected item text
_theme.HoverBackColor             // Hover state background
_theme.DisabledBackColor          // Disabled state
_theme.DisabledForeColor          // Disabled text
_theme.ButtonBackColor            // Button backgrounds
_theme.ButtonForeColor            // Button text
_theme.SecondaryBackColor         // Secondary elements
_theme.SecondaryForeColor         // Secondary text
_theme.SuccessColor               // Success states
_theme.WarningColor               // Warning states
_theme.ErrorColor                 // Error states
_theme.InfoColor                  // Information states
```

---

## Example Implementations

### InfrastructureTreePainter (VMware Style)

**Key Features:**
- Dark theme optimized (respects theme colors)
- Colored status pills using `_theme.AccentColor`
- Metric badges with theme-based colors
- Hierarchical connection lines using `_theme.BorderColor`

```csharp
public class InfrastructureTreePainter : BaseTreePainter
{
    public override void PaintNodeBackground(Graphics g, Rectangle bounds, bool isHovered, bool isSelected)
    {
        Color backColor = isSelected 
            ? _theme.SelectedRowBackColor   // Theme-aware
            : isHovered 
                ? _theme.HoverBackColor 
                : Color.Transparent;
        
        if (backColor != Color.Transparent)
        {
            using (var brush = new SolidBrush(backColor))
                g.FillRectangle(brush, bounds);
                
            if (isSelected)
            {
                using (var pen = new Pen(_theme.AccentColor, 1))
                    g.DrawRectangle(pen, bounds);
            }
        }
    }
    
    public void PaintStatusPill(Graphics g, Rectangle pill, string status, Color baseColor)
    {
        // Use theme accent color blended with status color
        Color pillColor = Color.FromArgb(
            (baseColor.R + _theme.AccentColor.R) / 2,
            (baseColor.G + _theme.AccentColor.G) / 2,
            (baseColor.B + _theme.AccentColor.B) / 2);
        // ... draw pill
    }
}
```

### FileManagerTreePainter (Google Drive Style)

**Key Features:**
- Clean, light theme (respects theme colors)
- Rounded selection rectangles using `_theme.SelectedRowBackColor`
- Modern folder icons with theme accent color gradient
- Comfortable spacing

```csharp
public class FileManagerTreePainter : BaseTreePainter
{
    public override void PaintNodeBackground(Graphics g, Rectangle bounds, bool isHovered, bool isSelected)
    {
        if (isSelected)
        {
            using (var path = CreateRoundedRectangle(bounds, 6))
            {
                using (var brush = new SolidBrush(_theme.SelectedRowBackColor))
                    g.FillPath(brush, path);
                    
                using (var pen = new Pen(_theme.AccentColor, 1))
                    g.DrawPath(pen, path);
            }
        }
        else if (isHovered)
        {
            using (var path = CreateRoundedRectangle(bounds, 6))
            using (var brush = new SolidBrush(_theme.HoverBackColor))
                g.FillPath(brush, path);
        }
    }
    
    private void PaintFolderIcon(Graphics g, Rectangle iconRect)
    {
        // Gradient using theme accent color
        Color color1 = _theme.AccentColor;
        Color color2 = Color.FromArgb(
            Math.Min(255, color1.R + 30),
            Math.Min(255, color1.G + 30),
            Math.Min(255, color1.B + 30));
        
        using (var brush = new LinearGradientBrush(iconRect, color1, color2, LinearGradientMode.Vertical))
        {
            // Draw folder shape...
        }
    }
}
```

---

## Usage in BeepTree

```csharp
public partial class BeepTree : BaseControl
{
    private TreeStyle _treeStyle = TreeStyle.Standard;
    private ITreePainter _currentPainter;
    
    [Category("Appearance")]
    [Description("Visual style for tree rendering")]
    public TreeStyle TreeStyle
    {
        get => _treeStyle;
        set
        {
            if (_treeStyle != value)
            {
                _treeStyle = value;
                _currentPainter = BeepTreePainterFactory.CreatePainter(value, this, CurrentTheme);
                Invalidate();
            }
        }
    }
    
    protected override void DrawContent(Graphics g)
    {
        if (_currentPainter == null)
        {
            _currentPainter = BeepTreePainterFactory.CreatePainter(_treeStyle, this, CurrentTheme);
        }
        
        // Update painter with current theme
        _currentPainter.Initialize(this, CurrentTheme);
        
        // Paint the tree
        _currentPainter.Paint(g, this, DrawingRect);
    }
}
```

---

## Best Practices

### 1. Always Use Theme Colors
```csharp
// ✅ GOOD
Color color = _theme.AccentColor;

// ❌ BAD
Color color = Color.FromArgb(66, 133, 244);
```

### 2. Respect Theme Changes
```csharp
public override void Initialize(BeepTree owner, IBeepTheme theme)
{
    base.Initialize(owner, theme);
    // Reinitialize any cached colors/brushes/pens
}
```

### 3. Use Semi-Transparency for Layering
```csharp
// Create semi-transparent version of theme color
Color semiTransparent = Color.FromArgb(120, _theme.AccentColor);
```

### 4. Provide Contrast-Safe Colors
```csharp
// Ensure text is readable on background
Color textColor = isSelected 
    ? _theme.SelectedForeColor  // Already contrast-safe
    : _theme.ForeColor;
```

### 5. Cache Painters for Performance
The factory caches painters automatically - don't create new painters on every paint cycle.

---

## Testing Different Styles

```csharp
// In your form or test harness
beepTree1.TreeStyle = TreeStyle.InfrastructureTree;  // VMware style
beepTree1.Theme = "DarkTheme";

beepTree2.TreeStyle = TreeStyle.FileManagerTree;     // Google Drive style
beepTree2.Theme = "LightTheme";

beepTree3.TreeStyle = TreeStyle.Material3;           // Material Design
beepTree3.ControlStyle = BeepControlStyle.Material3; // Material colors

beepTree4.TreeStyle = TreeStyle.Discord;             // Discord style
beepTree4.Theme = "DarkTheme";
```

---

## Implementation Status

### ✅ Completed
- TreeStyle enum with 32 distinct styles
- BeepTreePainterFactory with TreeStyle support
- InfrastructureTreePainter (theme-aware)
- FileManagerTreePainter (theme-aware)

### 🔄 In Progress
- Remaining 30 painter implementations
- BeepTree.TreeStyle property integration

### ⏳ Pending
- All painter implementations
- Designer support for TreeStyle
- Documentation for each style
- Example gallery application

---

## Summary

**TreeStyle System Benefits:**

1. **Separation of Concerns**: Visual design (TreeStyle) separate from color scheme (ControlStyle/Theme)
2. **Theme Integration**: All painters respect theme colors for consistent appearance
3. **Extensibility**: Easy to add new styles without modifying existing code
4. **Performance**: Cached painters for efficiency
5. **Flexibility**: Mix and match TreeStyle with any theme
6. **Designer Support**: Property grid integration for easy configuration

**Key Principle:**
> **TreeStyle** defines HOW the tree looks (layout, animations, UI patterns).  
> **Theme** defines WHAT colors are used (respecting user preferences and accessibility).

Together, they provide powerful, flexible, and theme-aware tree visualization! 🎨🌳
