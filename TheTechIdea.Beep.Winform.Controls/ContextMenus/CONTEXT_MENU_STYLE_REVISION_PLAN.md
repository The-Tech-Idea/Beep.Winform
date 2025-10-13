# BeepContextMenu Style System Revision Plan
## Using FormStyle Enum for Unified Application Skinning

**Date**: 2025
**Purpose**: Revise BeepContextMenu to use the same `FormStyle` enum as BeepiFormPro for consistent application-wide theming

---

## Overview

This plan outlines the complete revision of the BeepContextMenu UI system to use the existing `FormStyle` enum from `BeepiFormPro.Models.cs` instead of the deprecated `ContextMenuType` enum. This ensures unified skinning across your entire application.

---

## Phase 1: Core Models ? COMPLETED

### 1.1 Create ContextMenuMetrics Class ?
**File**: `ContextMenuMetrics.cs` (CREATED)
**Status**: Completed

This class provides style-specific metrics similar to `FormPainterMetrics.DefaultFor()` and includes:
- Dimensions (ItemHeight, IconSize, Padding, etc.)
- Border and Shadow settings
- Color schemes for all FormStyle variants
- Theme integration support
- Style-specific features (ripple effects, elevation, rounded items)

### 1.2 Deprecate ContextMenuType ?
**File**: `ContextMenuType.cs` (UPDATED)
**Status**: Completed

- Marked `ContextMenuType` enum as `[Obsolete]`
- Added `ContextMenuTypeConverter` helper class for backward compatibility
- Provides clear migration path to `FormStyle`

---

## Phase 2: Update Painter Interface

### 2.1 Update IContextMenuPainter Interface
**File**: `Painters\IContextMenuPainter.cs`

**Changes Needed**:
```csharp
public interface IContextMenuPainter
{
    /// <summary>
    /// Gets the FormStyle this painter implements
    /// </summary>
    FormStyle Style { get; }
    
    /// <summary>
    /// Gets the metrics for this painter
    /// </summary>
    ContextMenuMetrics GetMetrics(IBeepTheme theme, bool useThemeColors);
    
    /// <summary>
    /// Draws the background of the context menu
    /// </summary>
    void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
        ContextMenuMetrics metrics, IBeepTheme theme);
    
    /// <summary>
    /// Draws all menu items
    /// </summary>
    void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
        SimpleItem selectedItem, SimpleItem hoveredItem, 
        ContextMenuMetrics metrics, IBeepTheme theme);
    
    /// <summary>
    /// Draws the border of the context menu
    /// </summary>
    void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
        ContextMenuMetrics metrics, IBeepTheme theme);
}
```

---

## Phase 3: Create New Style Painters

### 3.1 Core Painters to Create

Create one painter for each `FormStyle` enum value. Here's the complete list:

#### Existing Painters (Update to new interface):
1. ~~`StandardContextMenuPainter.cs`~~ ? **ModernContextMenuPainter.cs** (FormStyle.Modern)
2. **MaterialContextMenuPainter.cs** ? (FormStyle.Material) - UPDATE
3. **MinimalContextMenuPainter.cs** ? (FormStyle.Minimal) - UPDATE
4. ~~`FlatContextMenuPainter.cs`~~ ? **PaperContextMenuPainter.cs** (FormStyle.Paper)
5. ~~`OfficeContextMenuPainter.cs`~~ ? **FluentContextMenuPainter.cs** (FormStyle.Fluent)

#### New Painters to Create:

**Platform-Inspired**:
6. **MacOSContextMenuPainter.cs** (FormStyle.MacOS)
7. **iOSContextMenuPainter.cs** (FormStyle.iOS)
8. **GNOMEContextMenuPainter.cs** (FormStyle.GNOME)
9. **KDEContextMenuPainter.cs** (FormStyle.KDE)
10. **UbuntuContextMenuPainter.cs** (FormStyle.Ubuntu)

**Designer Styles**:
11. **GlassContextMenuPainter.cs** (FormStyle.Glass)
12. **CartoonContextMenuPainter.cs** (FormStyle.Cartoon)
13. **ChatBubbleContextMenuPainter.cs** (FormStyle.ChatBubble)
14. **MetroContextMenuPainter.cs** (FormStyle.Metro)
15. **Metro2ContextMenuPainter.cs** (FormStyle.Metro2)
16. **NeoMorphismContextMenuPainter.cs** (FormStyle.NeoMorphism)
17. **GlassmorphismContextMenuPainter.cs** (FormStyle.Glassmorphism)
18. **BrutalistContextMenuPainter.cs** (FormStyle.Brutalist)

**Theme-Based Styles**:
19. **DraculaContextMenuPainter.cs** (FormStyle.Dracula)
20. **NordContextMenuPainter.cs** (FormStyle.Nord)
21. **SolarizedContextMenuPainter.cs** (FormStyle.Solarized)
22. **OneDarkContextMenuPainter.cs** (FormStyle.OneDark)
23. **GruvBoxContextMenuPainter.cs** (FormStyle.GruvBox)
24. **TokyoContextMenuPainter.cs** (FormStyle.Tokyo)

**Effect Styles**:
25. **RetroContextMenuPainter.cs** (FormStyle.Retro)
26. **CyberpunkContextMenuPainter.cs** (FormStyle.Cyberpunk)
27. **NeonContextMenuPainter.cs** (FormStyle.Neon)
28. **HolographicContextMenuPainter.cs** (FormStyle.Holographic)

**Specialized**:
29. **NordicContextMenuPainter.cs** (FormStyle.Nordic)
30. **ArcLinuxContextMenuPainter.cs** (FormStyle.ArcLinux)
31. **CustomContextMenuPainter.cs** (FormStyle.Custom)

### 3.2 Painter Template

Each painter should follow this structure:

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// [StyleName] context menu painter
    /// </summary>
    public class [StyleName]ContextMenuPainter : IContextMenuPainter
    {
        public FormStyle Style => FormStyle.[StyleName];

        public ContextMenuMetrics GetMetrics(IBeepTheme theme, bool useThemeColors)
        {
            return ContextMenuMetrics.DefaultFor(Style, theme, useThemeColors);
        }

        public void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Style-specific background rendering
        }

        public void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items,
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Style-specific item rendering
        }

        public void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme)
        {
            // Style-specific border rendering
        }

        // Private helper methods as needed
    }
}
```

---

## Phase 4: Update BeepContextMenu Core

### 4.1 Update BeepContextMenu.Core.cs

**Changes**:

```csharp
// Replace ContextMenuType with FormStyle
private FormStyle _menuStyle = FormStyle.Material;

// Add metrics caching
private ContextMenuMetrics _metrics;

// Update painter creation
public void SetPainter(FormStyle style)
{
    _contextMenuPainter = PainterFactory.CreateContextMenuPainter(style);
    _metrics = _contextMenuPainter.GetMetrics(_currentTheme, UseThemeColors);
    Invalidate();
}

// Add property
[Category("Appearance")]
[Description("The visual style of the context menu")]
public FormStyle MenuStyle
{
    get => _menuStyle;
    set
    {
        if (_menuStyle != value)
        {
            _menuStyle = value;
            SetPainter(value);
        }
    }
}

// Deprecated property for backward compatibility
[Obsolete("Use MenuStyle property with FormStyle enum instead")]
[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public ContextMenuType ContextMenuType
{
    get => ContextMenuTypeConverter.ToContextMenuType(_menuStyle);
    set => MenuStyle = ContextMenuTypeConverter.ToFormStyle(value);
}
```

### 4.2 Update BeepContextMenu.Drawing.cs

**Changes**:

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    base.OnPaint(e);
    
    if (_contextMenuPainter == null) return;
    
    var g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
    
    // Use metrics from painter
    var metrics = _metrics ?? _contextMenuPainter.GetMetrics(_currentTheme, UseThemeColors);
    
    // Draw with metrics
    _contextMenuPainter.DrawBackground(g, this, ClientRectangle, metrics, _currentTheme);
    _contextMenuPainter.DrawItems(g, this, _menuItems, _selectedItem, _hoveredItem, metrics, _currentTheme);
    _contextMenuPainter.DrawBorder(g, this, ClientRectangle, metrics, _currentTheme);
}
```

---

## Phase 5: Create Painter Factory

### 5.1 Create PainterFactory Class

**File**: `Painters\PainterFactory.cs` (NEW)

```csharp
using System;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// Factory for creating context menu painters based on FormStyle
    /// </summary>
    public static class PainterFactory
    {
        public static IContextMenuPainter CreateContextMenuPainter(FormStyle style)
        {
            return style switch
            {
                FormStyle.Modern => new ModernContextMenuPainter(),
                FormStyle.Minimal => new MinimalContextMenuPainter(),
                FormStyle.Material => new MaterialContextMenuPainter(),
                FormStyle.Fluent => new FluentContextMenuPainter(),
                FormStyle.MacOS => new MacOSContextMenuPainter(),
                FormStyle.iOS => new iOSContextMenuPainter(),
                FormStyle.Glass => new GlassContextMenuPainter(),
                FormStyle.Cartoon => new CartoonContextMenuPainter(),
                FormStyle.ChatBubble => new ChatBubbleContextMenuPainter(),
                FormStyle.Metro => new MetroContextMenuPainter(),
                FormStyle.Metro2 => new Metro2ContextMenuPainter(),
                FormStyle.GNOME => new GNOMEContextMenuPainter(),
                FormStyle.NeoMorphism => new NeoMorphismContextMenuPainter(),
                FormStyle.Glassmorphism => new GlassmorphismContextMenuPainter(),
                FormStyle.Brutalist => new BrutalistContextMenuPainter(),
                FormStyle.Retro => new RetroContextMenuPainter(),
                FormStyle.Cyberpunk => new CyberpunkContextMenuPainter(),
                FormStyle.Nordic => new NordicContextMenuPainter(),
                FormStyle.Ubuntu => new UbuntuContextMenuPainter(),
                FormStyle.KDE => new KDEContextMenuPainter(),
                FormStyle.ArcLinux => new ArcLinuxContextMenuPainter(),
                FormStyle.Dracula => new DraculaContextMenuPainter(),
                FormStyle.Solarized => new SolarizedContextMenuPainter(),
                FormStyle.OneDark => new OneDarkContextMenuPainter(),
                FormStyle.GruvBox => new GruvBoxContextMenuPainter(),
                FormStyle.Nord => new NordContextMenuPainter(),
                FormStyle.Tokyo => new TokyoContextMenuPainter(),
                FormStyle.Paper => new PaperContextMenuPainter(),
                FormStyle.Neon => new NeonContextMenuPainter(),
                FormStyle.Holographic => new HolographicContextMenuPainter(),
                FormStyle.Custom => new CustomContextMenuPainter(),
                _ => new ModernContextMenuPainter()
            };
        }
    }
}
```

---

## Phase 6: Update Properties and Designer Support

### 6.1 Update BeepContextMenu.Properties.cs

Add designer-friendly properties:

```csharp
[Category("Appearance")]
[Description("The visual style of the context menu")]
[DefaultValue(FormStyle.Material)]
public FormStyle MenuStyle
{
    get => _menuStyle;
    set
    {
        if (_menuStyle != value)
        {
            _menuStyle = value;
            SetPainter(value);
        }
    }
}

[Category("Appearance")]
[Description("Use theme colors instead of style defaults")]
[DefaultValue(false)]
public bool UseThemeColors
{
    get => _useThemeColors;
    set
    {
        if (_useThemeColors != value)
        {
            _useThemeColors = value;
            RefreshMetrics();
            Invalidate();
        }
    }
}
```

### 6.2 Create Type Converter for Designer

**File**: `Design\FormStyleConverter.cs` (NEW)

```csharp
using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Design
{
    /// <summary>
    /// Type converter for FormStyle enum in designer
    /// </summary>
    public class FormStyleConverter : EnumConverter
    {
        public FormStyleConverter() : base(typeof(FormStyle))
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
```

---

## Phase 7: Testing and Validation

### 7.1 Create Test Form

**File**: `Tests\ContextMenuStyleTestForm.cs` (NEW)

Create a test form that:
- Shows a button for each FormStyle
- Displays context menu with that style when clicked
- Demonstrates theme integration
- Tests all 30+ styles

### 7.2 Test Scenarios

1. **Style Switching**: Test switching between all FormStyle values at runtime
2. **Theme Integration**: Test with different IBeepTheme implementations
3. **Theme Toggle**: Test UseThemeColors = true/false for each style
4. **DPI Scaling**: Test at 100%, 125%, 150%, 200% DPI
5. **Dark/Light Modes**: Verify readability in both modes
6. **Hover States**: Verify all hover effects work correctly
7. **Selection States**: Verify selected item highlighting
8. **Disabled Items**: Verify disabled item rendering
9. **Separators**: Verify separator rendering in all styles
10. **Submenus**: Test submenu arrow rendering and behavior

---

## Phase 8: Documentation

### 8.1 Create Style Guide Document

**File**: `CONTEXT_MENU_STYLE_GUIDE.md` (NEW)

Document:
- Each style with screenshot
- Recommended use cases for each style
- Theme integration guidelines
- Custom painter creation guide

### 8.2 Update XML Documentation

Ensure all public members have complete XML documentation including:
- `<summary>`
- `<param>` tags
- `<returns>` tags
- `<example>` usage examples

---

## Phase 9: Migration Guide

### 9.1 Create Migration Document

**File**: `MIGRATION_FROM_CONTEXTMENUTYPE.md` (NEW)

Provide step-by-step guide for migrating existing code from `ContextMenuType` to `FormStyle`.

**Example**:
```csharp
// OLD (Deprecated)
menu.ContextMenuType = ContextMenuType.Material;

// NEW (Recommended)
menu.MenuStyle = FormStyle.Material;
```

---

## Phase 10: Performance Optimization

### 10.1 Implement Caching

- Cache `ContextMenuMetrics` instances
- Cache brushes and pens per style
- Implement painter instance pooling
- Cache rendered item rectangles

### 10.2 Lazy Loading

- Load painters on-demand
- Defer heavy graphics initialization
- Use double-buffering effectively

---

## Implementation Priority

### High Priority (Do First):
1. ? Create `ContextMenuMetrics.cs`
2. ? Update `ContextMenuType.cs` with obsolete attribute
3. Update `IContextMenuPainter` interface
4. Create `PainterFactory.cs`
5. Update `BeepContextMenu.Core.cs` with FormStyle property
6. Update existing painters (Material, Minimal)

### Medium Priority:
7. Create new painters for common styles (Modern, Fluent, MacOS, iOS)
8. Update `BeepContextMenu.Drawing.cs`
9. Test integration with existing code

### Low Priority:
10. Create remaining painters (all theme-based and effect styles)
11. Create test form
12. Create documentation
13. Performance optimization

---

## Success Criteria

? All FormStyle enum values have corresponding painter implementations
? Theme integration works correctly with all styles
? No breaking changes to existing functionality
? Designer support fully functional
? Performance is equal or better than current implementation
? All tests pass
? Documentation is complete

---

## Notes

- Maintain backward compatibility with `ContextMenuType` during transition period
- Consider removing `ContextMenuType` completely in next major version
- Ensure consistent behavior between `BeepiFormPro` and `BeepContextMenu` styles
- Keep painter implementations independent and testable
- Follow existing code patterns from `FormPainterMetrics` and form painters

---

## Next Steps

1. Review and approve this plan
2. Create feature branch: `feature/context-menu-formstyle-integration`
3. Start with High Priority items
4. Create pull request when core functionality is complete
5. Iterate based on testing feedback

---

**End of Plan**
