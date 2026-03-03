# BeepDisplayContainer2 — Tab Sizing & Resize Blank Fix Plan

## Problem Summary

Two user-reported bugs in `BeepDisplayContainer2`:

1. **Tab drawing/sizing incorrect** — tabs are not sized correctly based on font size; text clips or overflows.
2. **Resize shows blank** — resizing the container does not repaint; content area goes blank.

## Root Cause Analysis

### Bug 1: Tab Sizing Incorrect

| ID | Issue | Location |
|----|-------|----------|
| 1a | **Font mismatch between layout and paint** — `CalculateTabLayout()` and `ComputeAutoTabHeight()` use `Font` (WinForms property) but `DrawTab()` uses `BeepThemesManager.ToFont(_currentTheme?.TabFont)`. BaseControl convention is `TextFont` (already set from theme in `ApplyTheme()`). All three diverge. | `Layout.cs:93`, `Layout.cs:268`, `Painting.cs:576` |
| 1b | **Hardcoded pixel offsets in paint** — `DrawProfessionalTab()` computes text bounds with raw `8`, `2`, `36`, `16`, `4` pixel values. `TabLayoutHelper` already DPI-scales equivalent values (`ScaleValue(16)` for padding, `ScaleValue(20)` for close button). At non-100% DPI these diverge → text clips or wastes space. | `TabPaintHelper.cs:146-147` |
| 1c | **Bold active tab not measured** — `DrawTabText()` renders the active tab in `FontStyle.Bold`, but `CalculateTabContentWidth()` always measures with Regular weight. Bold text is wider → active tab text overflows its measured bounds. | `TabPaintHelper.cs:467`, `TabLayoutHelper.cs:117` |
| 1d | **`UpdateTabPainterStyle()`** and **`OnFontChanged()`** also pass `Font` instead of `TextFont` to the layout helper. | `BeepDisplayContainer2.cs:296`, `Layout.cs:250` |

### Bug 2: Resize Shows Blank

| ID | Issue | Location |
|----|-------|----------|
| 2a | **`RecalculateLayout()` never calls `Invalidate()`** — relies on every caller to remember. Multiple callers (`ActivateTab()`, internal flows) forget. | `Layout.cs:76` |
| 2b | **`OnResize()` ordering** — `base.OnResize(e)` triggers `Invalidate()` with stale layout rectangles before `RecalculateLayout()` updates `_tabArea`/`_contentArea`. If WinForms processes the first invalidation synchronously, it paints with old bounds. | `Layout.cs:233-238` |
| 2c | **Region leak in `PositionActiveAddin()`** — every call creates `new Region(clipRect)` without disposing the previous `control.Region`. GDI handles leak during rapid resize. | `Layout.cs:218` |
| 2d | **Active child not invalidated after repositioning** — `PositionActiveAddin()` explicitly says "Don't invalidate during positioning". After bounds/region change the child needs an `Invalidate()` to repaint at its new size. | `Layout.cs:228` |

## Planned Changes

### Fix 1a–1d: Use `TextFont` Everywhere (Font Unification)

**Rationale**: `TextFont` is the authoritative font for all BaseControl-derived controls. `ApplyTheme()` already sets `TextFont = BeepThemesManager.ToFont(tabTypography)` which returns an already-scaled font. Using `TextFont` as the single source of truth for both layout measurement and paint rendering guarantees consistency.

**Files & Changes**:

| File | Line(s) | Current | New |
|------|---------|---------|-----|
| `BeepDisplayContainer2.Painting.cs` | ~576 | `BeepThemesManager.ToFont(_currentTheme?.TabFont)` | `TextFont` |
| `BeepDisplayContainer2.Painting.cs` | ~431 | `new Font(Font.FontFamily, Math.Max(6f, Font.Size * 0.9f), ...)` | `new Font(TextFont.FontFamily, Math.Max(6f, TextFont.Size * 0.9f), ...)` |
| `BeepDisplayContainer2.cs` | ~296 | `_layoutHelper.UpdateStyle(ControlStyle, Font)` | `_layoutHelper.UpdateStyle(ControlStyle, TextFont)` |
| `BeepDisplayContainer2.Layout.cs` | ~93 | `_layoutHelper.UpdateStyle(ControlStyle, Font)` | `_layoutHelper.UpdateStyle(ControlStyle, TextFont)` |
| `BeepDisplayContainer2.Layout.cs` | ~250 | `_layoutHelper?.UpdateStyle(ControlStyle, Font)` | `_layoutHelper?.UpdateStyle(ControlStyle, TextFont)` |
| `BeepDisplayContainer2.Layout.cs` | ~268 | `var measureFont = Font ?? SystemFonts.DefaultFont` | `var measureFont = TextFont ?? SystemFonts.DefaultFont` |

### Fix 1b: DPI-Scale Text Bounds in `DrawProfessionalTab()`

**File**: `TabPaintHelper.cs` (~lines 146-147)

**Current**:
```csharp
var textBounds = showCloseButton ? 
    new Rectangle(bounds.X + 8, bounds.Y + 2, Math.Max(0, bounds.Width - 36), Math.Max(0, bounds.Height - 4)) :
    new Rectangle(bounds.X + 8, bounds.Y + 2, Math.Max(0, bounds.Width - 16), Math.Max(0, bounds.Height - 4));
```

**New**:
```csharp
int hPad = DpiScalingHelper.ScaleValue(8, OwnerControl);
int vPad = DpiScalingHelper.ScaleValue(2, OwnerControl);
int closeW = DpiScalingHelper.ScaleValue(20, OwnerControl);
var textBounds = showCloseButton ?
    new Rectangle(bounds.X + hPad, bounds.Y + vPad,
        Math.Max(0, bounds.Width - hPad - closeW), Math.Max(0, bounds.Height - vPad * 2)) :
    new Rectangle(bounds.X + hPad, bounds.Y + vPad,
        Math.Max(0, bounds.Width - hPad * 2), Math.Max(0, bounds.Height - vPad * 2));
```

This aligns with `TabLayoutHelper.CalculateTabContentWidth()` which uses `ScaleValue(16)` for padding and `ScaleValue(20)` for close button width.

### Fix 1c: Measure with Bold Font in Layout

**File**: `TabLayoutHelper.cs` — `CalculateTabContentWidth()` (~line 117)

**Current**:
```csharp
var textSize = TextRenderer.MeasureText(tab.Title, _font, 
    new Size(int.MaxValue, int.MaxValue), 
    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
```

**New**:
```csharp
// Measure with Bold so tabs are always wide enough for the active state
// (DrawTabText renders active tabs in Bold which is wider than Regular)
Font measureFont = _font;
try { measureFont = new Font(_font.FontFamily, _font.Size, FontStyle.Bold, _font.Unit); }
catch { /* keep regular font on error */ }

var textSize = TextRenderer.MeasureText(tab.Title, measureFont,
    new Size(int.MaxValue, int.MaxValue),
    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

if (measureFont != _font) measureFont.Dispose();
```

Tab widths will be consistent regardless of which tab is active, preventing "jumping" on tab activation.

### Fix 2a: Self-Invalidating `RecalculateLayout()`

**File**: `BeepDisplayContainer2.Layout.cs` — `RecalculateLayout()`

Add at the end of the method (both the Single-mode early return and the Tabbed-mode path):
```csharp
if (!_batchMode && IsHandleCreated)
    Invalidate();
```

Remove the now-redundant explicit `Invalidate()` calls from:
- `OnResize()` (Layout.cs ~237-238)
- `OnFontChanged()` (Layout.cs ~252-253)

Property setters in `Properties.cs` that call both `RecalculateLayout()` and `Invalidate()` can keep their `Invalidate()` — double-invalidate is harmless and property setters are infrequent.

### Fix 2b: Reorder `OnResize()` — Layout Before Base

**File**: `BeepDisplayContainer2.Layout.cs` — `OnResize()`

**Current**:
```csharp
protected override void OnResize(EventArgs e)
{
    base.OnResize(e);           // ← Invalidates with stale layout
    RecalculateLayout();        // ← Computes new layout
    if (!_batchMode && IsHandleCreated)
        Invalidate();           // ← Second invalidate with new layout
}
```

**New**:
```csharp
protected override void OnResize(EventArgs e)
{
    RecalculateLayout();        // Layout first (now self-invalidates)
    base.OnResize(e);           // Base invalidation now sees correct layout
}
```

### Fix 2c: Dispose Old Region in `PositionActiveAddin()`

**File**: `BeepDisplayContainer2.Layout.cs` — `PositionActiveAddin()` (~line 218)

**Current**:
```csharp
var clipRect = new Rectangle(0, 0, targetBounds.Width, targetBounds.Height);
control.Region = new Region(clipRect);
```

**New**:
```csharp
var clipRect = new Rectangle(0, 0, targetBounds.Width, targetBounds.Height);
control.Region?.Dispose();
control.Region = new Region(clipRect);
```

### Fix 2d: Invalidate Active Child After Repositioning

**File**: `BeepDisplayContainer2.Layout.cs` — `PositionActiveAddin()` (~line 228)

**Current**:
```csharp
if (control.Visible != isActive)
{
    control.Visible = isActive;
}

// Don't invalidate during positioning - let caller handle it
```

**New**:
```csharp
if (control.Visible != isActive)
{
    control.Visible = isActive;
}

// Invalidate the active child so it repaints at its new bounds
if (isActive && control.Visible)
{
    control.Invalidate();
}
```

## Files Modified (Summary)

| File | Changes |
|------|---------|
| `BeepDisplayContainer2.cs` | `UpdateTabPainterStyle()`: `Font` → `TextFont` |
| `BeepDisplayContainer2.Painting.cs` | `DrawTab()`: `BeepThemesManager.ToFont(...)` → `TextFont`. `DrawEmptyState()`: `Font` → `TextFont` |
| `BeepDisplayContainer2.Layout.cs` | `CalculateTabLayout()`, `OnFontChanged()`, `ComputeAutoTabHeight()`: `Font` → `TextFont`. `RecalculateLayout()`: add self-`Invalidate()`. `OnResize()`: reorder layout-first. `PositionActiveAddin()`: dispose old Region, invalidate active child |
| `TabPaintHelper.cs` | `DrawProfessionalTab()`: DPI-scale text bounds (replace hardcoded 8/2/36/16/4) |
| `TabLayoutHelper.cs` | `CalculateTabContentWidth()`: measure with Bold font |

## Verification Checklist

- [ ] Add container to a form with 2-3 tabs of varying title lengths
- [ ] Verify tab text is fully visible and vertically centred, not clipped
- [ ] Verify active tab text (Bold) does not overflow its bounds
- [ ] Resize the form — no blank content area, tabs and content repaint immediately
- [ ] Rapid resize — no GDI handle leak (check Task Manager GDI object count stays stable)
- [ ] Switch themes — tabs re-measure for new font, no stale sizing
- [ ] Test at 100%, 125%, 150% DPI scaling
- [ ] Test `TabPosition.Top`, `Bottom`, `Left`, `Right`
- [ ] Test `DisplayMode.Single` — no tabs, full content area, resize works
- [ ] Test `BeginUpdate()`/`EndUpdate()` batch mode — single repaint at end
- [ ] Test empty state (no tabs) — placeholder text renders centred with correct font

## Design Decisions

1. **`TextFont` is the single source of truth** — already set from theme in `ApplyTheme()` via `BeepThemesManager.ToFont(tabTypography)` which returns an already-scaled font. No re-resolving from theme on every paint cycle.
2. **Measure tabs with Bold font unconditionally** — prevents "jumping" tab widths when activation changes. Small width overhead (~10%) is acceptable for stability.
3. **`RecalculateLayout()` self-invalidates** — eliminates the class of bugs where callers forget to invalidate. Guarded by `!_batchMode && IsHandleCreated` so batch mode and pre-handle scenarios are safe.
4. **Layout before base in `OnResize()`** — ensures `base.OnResize()` → `Invalidate()` paints with current layout, not stale rectangles.
