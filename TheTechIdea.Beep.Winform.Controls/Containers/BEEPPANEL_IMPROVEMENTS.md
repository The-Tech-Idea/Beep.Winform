# BeepPanel Modern UI/UX Improvements

> **Note (2026-08-21):** A cleanup pass followed the work described below. The dead
> inline border pipeline (`PaintBorder`, `GetShapePath`, `CreateGroupBoxBorderPath`,
> `BeepPanelPainter.BuildGroupBoxBorderPath`, `GetTitleArea`, `DrawGroupBoxTitle`) was
> removed — border painting is handled by the BaseControl `BorderPainter` pipeline. The
> inert "Phase 1 UX" surface (`Elevation`, `ElevationOnHover`, `StickyHeader`, `State`,
> `EmptyStateMessage`, and the `PanelState`/`TitlePosition` enums) was deleted. The broken
> hand-rolled `VScrollBar`/`HScrollBar` scroll system (which never moved children) was
> removed in favor of the framework `ScrollableControl.AutoScroll`. `UseThemeFont` now
> actually drives the title font, and `_textFont` is no longer disposed (it may be a
> shared/theme-cached/user-owned font).

## Overview
Revised BeepPanel drawing and title rendering to follow **Material Design 3** and modern UI/UX best practices.

## CSS-style corner radius via Region (2026-08-21)

`BeepPanel` clips its children through a Windows Forms `Region`, so the panel can
simulate web/CSS shapes. In addition to the existing `PanelShape` (`Rectangle`,
`RoundedRectangle`, `Ellipse`, `Custom`) and `CustomShapePath` (a CSS `clip-path`
equivalent), the panel now supports **per-corner radius** — CSS `border-radius: a b c d`.

- `CornerRadius` (`Forms.ModernForm.Painters.CornerRadius`) exposes `TopLeft`, `TopRight`,
  `BottomLeft`, `BottomRight`.
- When **not** set explicitly, each corner follows the inherited `BorderRadius`
  (the theme/uniform value), preserving existing behaviour.
- Setting any corner makes `CornerRadius` authoritative; the clip region is built with
  `GraphicsExtensions.CreateRoundedRectanglePath(bounds, CornerRadius)`.
- Helpers: `SetCornerRadius(int all)` (uniform, also syncs `BorderRadius`) and
  `SetCornerRadius(int topLeft, int topRight, int bottomRight, int bottomLeft)` (CSS order).

Example — web-style asymmetric rounding:

```csharp
panel.SetCornerRadius(topLeft: 24, topRight: 24, bottomRight: 4, bottomLeft: 4);
```

Note: the border *painter* and theme sync use the uniform `BorderRadius`; an asymmetric
`CornerRadius` primarily reshapes the clip region (and therefore the visible
background/content edge).

## Web/CSS shapes & tab-header title (2026-08-21)

**Shapes** — `PanelShape` gained `Pill` (stadium, CSS `border-radius: 9999px`) and
`Squircle` (iOS/SwiftUI superellipse). Both are applied through the clip `Region`.

**Title styles** — `PanelTitleStyle` gained:

- `Divider` — title centered with horizontal rules on both sides (CSS "heading with rules").
- `Badge` — title inside a pill chip (CSS chip/badge).
- `TopHeader` — a proper **tab-header** band: a solid, full-width header across the top.
  The title and icon are vertically centered with padding (DPI-scaled from `TitleGap`),
  respect `TitleAlignment` (left/center/right), and use a contrast color against the band.
  When `ShowTitleLine` is on, a full-width divider is drawn at the band's bottom edge.
  The band color is the existing accent (`TitleLineColor` / theme `CardTitleForeColor`).

```csharp
panel.TitleStyle   = PanelTitleStyle.TopHeader;
panel.ShowTitleLine = true;   // divider under the tab header
panel.TitleLineColor = Color.FromArgb(220, 38, 38); // e.g. "URGENT" red band
```

## Key Changes

### 1. **Material Design 3 Label-on-Border Pattern**
- Title now appears as a **label floating on the top border**
- Clean white/light background for title label
- Improved visual separation and hierarchy
- Professional, minimalist aesthetic

### 2. **Improved DrawGroupBoxTitle Method**
- ? Title label has proper **padding around text** (6px)
- ? **Label height and width** calculated based on text size
- ? Title **vertically centered** on the top border
- ? Supports **three alignment options** (TopLeft, TopCenter, TopRight)
- ? **Theme-aware colors** for consistency
- ? **Optional elevation shadow** for Material Design depth
- ? Proper **_titleBottomY** calculation for child control layout

### 3. **Enhanced CreateGroupBoxBorderPath Method**
- ? Border **gap aligns perfectly** with title label background
- ? Supports **rounded corners** (Material Design radius = 8px)
- ? Proper **overflow prevention** (title stays within bounds)
- ? Both **rectangle and rounded rectangle** shapes supported
- ? Gap is **mathematically precise** for clean rendering

### 4. **Modern Constructor Defaults**
- ? **Thin borders** (1px) for clean, modern look
- ? **Rounded corners enabled** by default
- ? **BorderRadius = 8px** (Material Design 3 standard)
- ? **Material3 theme** as default control style
- ? **Better visual hierarchy** out of the box

### 5. **Better Visual Defaults**
- **TitleStyle**: Defaults to `GroupBox` (modern label-on-border)
- **ShowTitleLine**: Disabled by default (cleaner appearance)
- **TitleGap**: 8px (professional spacing)
- **Elevation support**: Ready for Material Design depth effects

## Visual Improvements

### Before
```
???????????????????????????????????
? Panel Title                     ?
???????????????????????????????????
?                                 ?
?  Content Area                   ?
?                                 ?
???????????????????????????????????
```

### After (Material Design 3)
```
   ?? Panel Title ??
  ????????????????????????????????
  ?                               ?
  ?  Content Area                 ?
  ?                               ?
  ?????????????????????????????????
```

## Design Principles Applied

1. **Visual Hierarchy**: Title label stands out clearly
2. **Clean Aesthetics**: Minimal visual clutter, maximizes content space
3. **Modern Design**: Follows Material Design 3 guidelines
4. **Accessibility**: Proper padding and sizing for readability
5. **Flexibility**: Supports all alignment options and themes
6. **Consistency**: Works with Material3, Modern, and other themes

## Theme Integration

- **Automatic color application** from theme:
  - Background color from theme panel color
  - Text color from theme primary text color
  - Border color from theme border color
- **Elevation support**: Optional Material Design shadow effect
- **Works with all themes**: Material3, Modern, Classic, etc.

## Usage Examples

```csharp
// Create a modern panel with Material Design 3 defaults
var panel = new BeepPanel
{
    TitleText = "My Panel",
    TitleAlignment = ContentAlignment.TopLeft,
    ShowTitle = true,
    Padding = new Padding(20)
};

// Customize title appearance
panel.TitleGap = 10;        // Increase spacing
panel.Elevation = 2;        // Add depth
panel.TitleStyle = PanelTitleStyle.GroupBox;  // Modern label

// Change title alignment
panel.TitleAlignment = ContentAlignment.TopCenter;
```

## Backward Compatibility

- ? All existing `TitleStyle` options still work
- ? All alignment options preserved
- ? No breaking changes to public API
- ? Existing code continues to work as before
- ? Only visual rendering improved

## Testing Checklist

- [x] Build successfully without errors
- [x] GroupBox style renders correctly
- [x] Title alignment (Left, Center, Right) works properly
- [x] Border gap aligns with title label
- [x] Rounded corners render cleanly
- [x] Theme colors apply correctly
- [x] No child control clipping
- [ ] Test with different border radius values
- [ ] Test with long title text
- [ ] Test with different themes
- [ ] Verify elevation shadows work

## Future Enhancements

- Add animated collapse/expand with smooth transitions
- Support custom title icon
- Add title background style options (solid, gradient, transparent)
- Implement sticky header for scrollable content
- Add title tooltip support
