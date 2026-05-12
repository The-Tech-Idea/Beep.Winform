# BeepGridPro Enhancement & Fix Plan

## Overview

This plan addresses three critical issues in BeepGridPro:
1. **Grid hover effect** - Entire grid highlights/borders on mouse hover (inherited from BaseControl)
2. **Navigation controls overlapping toolbar** - Legacy inline buttons created even when painter navigation is enabled
3. **Toolbar icon theme colors** - Icons not using theme-aware coloring (using explicit tint instead of theme)

---

## Issue 1: Grid Hover Effect (HIGH PRIORITY)

### Problem
When the mouse hovers over BeepGridPro, the entire control gets a hover background and border drawn by BaseControl's painter. This is inappropriate for a data grid - only rows should have hover effects, not the entire container.

### Root Cause
BeepGridPro inherits from `BaseControl`, which has:
```csharp
// BaseControl.Events.cs
protected override void OnMouseEnter(EventArgs e) 
{
    base.OnMouseEnter(e); 
    IsHovered = true; 
    _input.OnMouseEnter(); 
    Invalidate();
}

protected override void OnMouseLeave(EventArgs e) 
{
    base.OnMouseLeave(e); 
    IsHovered = false; 
    _input.OnMouseLeave();
    Invalidate();
}
```

The `ClassicBaseControlPainter` then uses `IsHovered` to draw:
```csharp
// ClassicBaseControlPainter.cs:291
if (owner.IsHovered && owner.CanBeHovered) return owner.HoverBackColor;

// ClassicBaseControlPainter.cs:368
else if (owner.IsHovered) borderColor = owner.HoverBorderColor;
```

Since `BeepGridPro` doesn't override `CanBeHovered` (defaults to `true`), the entire grid gets a hover background/border on mouse enter.

### Solution

**Option A: Disable hover at the control level (RECOMMENDED)**

Add to `BeepGridPro.cs`:
```csharp
public BeepGridPro() : base()
{
    // ... existing code ...
    
    // Disable BaseControl hover effects - grid handles its own row hover
    CanBeHovered = false;
    HoverBackColor = Color.Empty;
    HoverBorderColor = Color.Empty;
}
```

**Option B: Override OnPaint to skip BaseControl hover drawing**

In `BeepGridPro.cs`:
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    // Temporarily disable hover state for base painting
    bool wasHovered = IsHovered;
    IsHovered = false;
    
    base.OnPaint(e);
    
    IsHovered = wasHovered;
}
```

**Option C: Custom painter that ignores hover for grids**

Create a `GridBaseControlPainter` that inherits from `ClassicBaseControlPainter` and overrides hover handling.

### Recommended Approach: Option A

Simple, explicit, and prevents the issue at the source. Add to constructor:

```csharp
public BeepGridPro() : base()
{
    // ... existing initialization ...
    
    // Grid is a container - disable BaseControl hover effects
    // Row hover is handled by GridRenderHelper, not BaseControl
    CanBeHovered = false;
}
```

### Files to Modify
- `GridX/BeepGridPro.cs` - Add `CanBeHovered = false` in constructor

### Validation
- [ ] Hovering over grid does not show background color change on entire control
- [ ] Hovering over grid does not show border around entire control
- [ ] Row hover effects still work correctly
- [ ] Toolbar hover effects still work correctly
- [ ] No regression in other BaseControl features (theme, etc.)

---

## Issue 2: Navigation Controls Overlapping Toolbar (HIGH PRIORITY)

### Problem
Legacy inline BeepButton controls are created for navigation even when `UsePainterNavigation = true`. These buttons are never added to the Controls collection but are instantiated and may interfere with hit-testing or cause other issues. When `ShowNavigator = false`, the buttons are still created.

### Root Cause

In `GridNavigationPainterHelper.cs`:
```csharp
private void EnsureNavigatorButtons()
{
    if (_btnFirst != null) return; // Only checks if already created

    // Creates ALL buttons regardless of whether they're needed
    _btnInsert = new BeepButton { ImagePath = Svgs.NavPlus, Theme = _grid.Theme };
    _btnDelete = new BeepButton { ImagePath = Svgs.NavMinus, Theme = _grid.Theme };
    // ... 10 more buttons ...
    _btnQuery = new BeepButton { ImagePath = Svgs.NavSearch, Theme = _grid.Theme };
    
    foreach (var btn in new[] { _btnInsert, ... })
    {
        btn.Text = "";
        btn.UseThemeFont = true;
        btn.AutoSize = false;
    }
}
```

Problems:
1. Buttons created even when `UsePainterNavigation = true` (painter mode doesn't need them)
2. Buttons created even when `ShowNavigator = false`
3. Buttons are never disposed when switching from legacy to painter mode
4. `SyncButtonThemes()` is called on every paint cycle in legacy mode

### Solution

**Step 1: Lazy-create buttons only when needed**

```csharp
private void EnsureNavigatorButtons()
{
    // Only create buttons if using legacy navigation
    if (UsePainterNavigation) return;
    
    if (_btnFirst != null) return;

    // Create buttons only when legacy mode is actually used
    _btnInsert = new BeepButton { ImagePath = Svgs.NavPlus, Theme = _grid.Theme };
    // ... etc ...
}
```

**Step 2: Dispose buttons when switching to painter mode**

```csharp
public bool UsePainterNavigation
{
    get => _usePainterNavigation;
    set
    {
        if (_usePainterNavigation != value)
        {
            _usePainterNavigation = value;
            
            // Clean up legacy buttons when switching to painter
            if (value)
            {
                DisposeNavigatorButtons();
            }
        }
    }
}

private void DisposeNavigatorButtons()
{
    _btnInsert?.Dispose(); _btnInsert = null;
    _btnDelete?.Dispose(); _btnDelete = null;
    _btnSave?.Dispose(); _btnSave = null;
    _btnCancel?.Dispose(); _btnCancel = null;
    _btnFirst?.Dispose(); _btnFirst = null;
    _btnPrev?.Dispose(); _btnPrev = null;
    _btnNext?.Dispose(); _btnNext = null;
    _btnLast?.Dispose(); _btnLast = null;
    _btnQuery?.Dispose(); _btnQuery = null;
    _btnFilter?.Dispose(); _btnFilter = null;
    _btnPrint?.Dispose(); _btnPrint = null;
    _lblPageInfo?.Dispose(); _lblPageInfo = null;
}
```

**Step 3: Skip legacy drawing when navigator is hidden**

```csharp
public void DrawNavigatorArea(Graphics g)
{
    var navRect = _grid.Layout.NavigatorRect;
    
    if (navRect.IsEmpty) return;
    
    // Don't draw anything if navigator is hidden
    if (!_grid.ShowNavigator) return;

    if (UsePainterNavigation)
    {
        DrawPainterNavigation(g, navRect);
        return;
    }

    DrawLegacyNavigation(g, navRect);
}
```

**Step 4: Don't create buttons in constructor**

Currently buttons are created in `EnsureNavigatorButtons()` which is called from `DrawLegacyNavigation()`. This is already lazy, but we should also check `ShowNavigator` before calling it.

### Files to Modify
- `GridX/Helpers/GridNavigationPainterHelper.cs` - Add `DisposeNavigatorButtons()`, check `ShowNavigator`, check `UsePainterNavigation`
- `GridX/BeepGridPro.Properties.cs` - Ensure `ShowNavigator = false` sets `Layout.NavigatorHeight = 0`

### Validation
- [ ] When `UsePainterNavigation = true`, no legacy buttons are created
- [ ] When `ShowNavigator = false`, no navigation area is drawn
- [ ] Switching from legacy to painter mode disposes old buttons
- [ ] Memory usage doesn't grow when toggling navigation modes
- [ ] Hit-testing works correctly in both modes

---

## Issue 3: Toolbar Icon Theme Colors (HIGH PRIORITY)

### Problem
Toolbar icons (search, filter, export, action buttons) don't use theme-aware coloring. They use `StyledImagePainter.PaintWithTint()` which applies an explicit tint color via ColorMatrix, bypassing the theme system entirely.

### Root Cause

The toolbar painter uses:
```csharp
// BeepGridToolbarPainter.cs:77
StyledImagePainter.PaintWithTint(g, iconRect, iconPath, _grid.ToolbarForeColor, 0.8f);
```

`StyledImagePainter.PaintWithTint()` works in two ways:

**For PNG images:** Uses ColorMatrix to tint the entire image:
```csharp
var cm = new ColorMatrix(new float[][] {
    new float[] { rFactor, 0, 0, 0, 0 },
    new float[] { 0, gFactor, 0, 0, 0 },
    new float[] { 0, 0, bFactor, 0, 0 },
    new float[] { 0, 0, 0, aFactor, 0 },
    new float[] { 0, 0, 0, 0, 1 }
});
```
This multiplies every pixel by the tint color - it doesn't respect SVG paths or theme semantics.

**For SVG images:** Uses `ImagePainter` but overrides theme:
```csharp
var painter = GetOrCreatePainter(imagePath);
painter.ApplyThemeOnImage = true;
painter.FillColor = tint;  // <-- OVERRIDES THEME COLOR!
painter.DrawImage(g, bounds);
```

The `ImagePainter` has theme-aware logic in `ApplyThemeToSvg()`:
```csharp
// ImagePainter.Theme.cs
case ImageEmbededin.DataGridView:
    actualFillColor = _currentTheme.GridHeaderForeColor;  // Theme-aware!
    break;
case ImageEmbededin.Button:
default:
    actualFillColor = _fillColor;  // Just uses explicit color
    break;
```

But:
1. The painter defaults to `ImageEmbededin.Button` 
2. `PaintWithTint` sets `FillColor = tint` which is used instead of theme
3. The painter is cached in `_painterCache` by path, so theme changes don't affect it
4. The `_tintedCache` also caches by tint color, but the SVG is already modified

### Solution

**Option A: Use theme-aware painting without tint override (RECOMMENDED)**

Create a new method in `BeepGridToolbarPainter` that uses `ImagePainter` with proper theme setup:

```csharp
private void PaintToolbarIcon(Graphics g, Rectangle bounds, string iconPath, float opacity = 0.8f)
{
    if (bounds.Width <= 0 || bounds.Height <= 0) return;

    var painter = StyledImagePainter.GetOrCreatePainter(iconPath);
    if (painter == null) return;

    // Set up theme-aware coloring
    var theme = BeepThemesManager.GetTheme(_grid.Theme) ?? BeepThemesManager.GetDefaultTheme();
    painter.CurrentTheme = theme;
    painter.ImageEmbededin = ImageEmbededin.DataGridView;  // Use grid theme colors
    painter.ApplyThemeOnImage = true;  // Enable theme application
    painter.Opacity = opacity;
    
    // Don't set FillColor - let the theme determine it!
    
    painter.DrawImage(g, bounds);
}
```

Then replace all `PaintWithTint` calls:
```csharp
// OLD:
StyledImagePainter.PaintWithTint(g, iconRect, iconPath, _grid.ToolbarForeColor, 0.8f);

// NEW:
PaintToolbarIcon(g, iconRect, iconPath, 0.8f);
```

**Option B: Fix StyledImagePainter to support theme-aware mode**

Add overload to `StyledImagePainter`:
```csharp
public static void PaintWithTheme(Graphics g, Rectangle bounds, string imagePath, 
    IBeepTheme theme, ImageEmbededin embedContext, float opacity = 1f)
{
    var painter = GetOrCreatePainter(imagePath);
    if (painter == null) return;

    painter.CurrentTheme = theme;
    painter.ImageEmbededin = embedContext;
    painter.ApplyThemeOnImage = true;
    painter.Opacity = opacity;
    
    painter.DrawImage(g, bounds);
}
```

**Option C: Clear caches on theme change (workaround)**

If the above is too complex, clear all caches when theme changes:
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    ThemeHelper.ApplyTheme();
    
    // Force all icons to re-render with new theme
    StyledImagePainter.ClearTintCache();
    StyledImagePainter.ClearPainterCache();  // Need to add this
    
    Invalidate();
}
```

### Recommended Approach: Option A

This gives the grid full control over icon theming without modifying the general `StyledImagePainter`.

### Files to Modify
- `GridX/Toolbar/BeepGridToolbarPainter.cs` - Add `PaintToolbarIcon()` method, replace `PaintWithTint` calls
- `GridX/Helpers/GridThemeHelper.cs` - May need adjustments if theme is not available

### Validation
- [ ] Icons show correct theme color on initial load (GridHeaderForeColor)
- [ ] After changing theme, toolbar icons update to new theme color
- [ ] Icons are visible in both light and dark themes
- [ ] Icons don't appear as solid color blocks
- [ ] No performance regression

---

## Implementation Order

1. **Issue 1** (Grid Hover) - One-line fix, high impact
2. **Issue 3** (Icon Theme Colors) - Use theme-aware painting
3. **Issue 2** (Navigation Controls) - Cleanup and optimization

---

## Files to Modify Summary

| File | Issue | Change |
|------|-------|--------|
| `GridX/BeepGridPro.cs` | #1 | Add `CanBeHovered = false` in constructor |
| `GridX/Toolbar/BeepGridToolbarPainter.cs` | #3 | Add `PaintToolbarIcon()` with theme-aware coloring |
| `GridX/Helpers/GridNavigationPainterHelper.cs` | #2 | Lazy-create buttons, dispose on mode switch |

---

## Additional Enhancements

### Enhancement 1: Row Hover Customization
Add property to control row hover intensity:
```csharp
[Browsable(true)]
[Category("Appearance")]
[Description("Opacity of row hover effect (0-255). 0 disables row hover.")]
[DefaultValue(255)]
public int RowHoverOpacity { get; set; } = 255;
```

### Enhancement 2: Navigator Visibility Modes
```csharp
public enum NavigatorVisibilityMode
{
    Always,      // Always show
    AutoHide,    // Hide when not needed (single page)
    Hidden       // Never show
}
```

### Enhancement 3: SVG Icon Color Override
Allow explicit icon color that overrides theme:
```csharp
[Browsable(true)]
[Category("Appearance")]
public Color? IconColorOverride { get; set; }
```
