# Phase 3 — Sizing, Spacing & Responsive Behavior

**Priority:** HIGH  
**Effort:** ~1.5 h  
**Goal:** Use existing `StyleSpacing` for all spacing values, add responsive max-width per layout variant, and ensure DPI-awareness.

---

## Existing Infrastructure (DO NOT DUPLICATE)

| System | Key APIs |
|---|---|
| `StyleSpacing.GetPadding(style)` | Per-style padding (4–20 px depending on style) |
| `StyleSpacing.GetItemSpacing(style)` | Per-style item gap (2–12 px) |
| `StyleSpacing.GetIconSize(style)` | Per-style icon size (16–32 px) |
| `StyleSpacing.GetItemHeight(style)` | Per-style item height (32–56 px) |
| `BeepFontManager.GetFontForPainter()` | DPI-scaled font for custom painters |
| `DpiScalingHelper.GetDpiScaleFactor()` | Current DPI scale factor |

> [!IMPORTANT]
> **No new `ToolTipSpacing` class needed.** The existing `StyleSpacing` already provides per-style spacing on a 4-point grid. Phase 3 is about *wiring* it into ToolTip logic.

---

## 3.1 Wire `StyleSpacing` Into All Painters

Replace all magic-number spacing (12, 8, 16, etc.) in painters with `StyleSpacing.*` calls:

### Files to modify

| File | Change |
|---|---|
| `Painters/BeepStyledToolTipPainter.cs` | Use `StyleSpacing.GetPadding(style)` and `StyleSpacing.GetItemSpacing(style)` in `GetContentRectangle` and `PaintContent` |
| `Painters/ToolTipPainterBase.cs` | Update `DefaultPadding`, `DefaultIconSize`, `DefaultIconMargin`, `DefaultTitleSpacing` to resolve from `StyleSpacing` |
| `Painters/GlassToolTipPainter.cs` | Same spacing delegation |
| `Painters/PreviewToolTipPainter.cs` | Same spacing delegation |
| `Painters/TourToolTipPainter.cs` | Same spacing delegation |

---

## 3.2 Responsive Max-Width Per Layout Variant

Current `CalculateOptimalSize` uses a flat `maxWidth` parameter. Add responsive constraints:

| Variant | Max Width Rule |
|---------|---------------|
| `Simple` | `min(280, 40% of screen)`, capped at 560 px |
| `Rich` / `Card` | `min(320, 50% of screen)`, capped at 640 px |
| `Preview` | `PreviewImageSize.Width + 2 × padding` |
| `Tour` | 400 px fixed width |
| `Shortcut` | `min(240, 35% of screen)` |
| `Glass` | Same as Rich |

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Add `GetResponsiveMaxWidth(ToolTipLayoutVariant, Screen)` static method |
| `CustomToolTip.Methods.cs` | Use responsive max-width when `config.MaxSize == null` |

---

## 3.3 Min-Height Per Variant

Ensure compact tooltips don't collapse below readable sizes:

| Variant | Min Height |
|---------|-----------|
| `Simple` | `StyleSpacing.GetItemHeight(style)` |
| `Rich` | `2 × StyleSpacing.GetItemHeight(style)` |
| `Card` | `3 × StyleSpacing.GetItemHeight(style)` |
| `Preview` | `PreviewImageSize.Height + 2 × padding + 40` |
| `Tour` | `3 × StyleSpacing.GetItemHeight(style) + 40` |
| `Shortcut` | `StyleSpacing.GetItemHeight(style)` |

### Files to modify

| File | Change |
|---|---|
| `Helpers/ToolTipLayoutHelpers.cs` | Apply min-height after `CalculateOptimalSize` |

---

## 3.4 Multi-Monitor DPI Handling

`CustomToolTip.Positioning.cs` constrains to screen but doesn't account for per-monitor DPI differences.

**Fix:** When positioning across monitors, use `DpiScalingHelper.GetDpiScaleFactor()` from the target screen.

### Files to modify

| File | Change |
|---|---|
| `CustomToolTip.Positioning.cs` | Get DPI scale from target screen, scale fonts/spacing if different |
| `Helpers/ToolTipPositionResolver.cs` | Include screen DPI in position calculation |

---

## Verification

- **Build:** `dotnet build`
- **Visual:** Show Simple and Card tooltips; verify padding proportional to style
- **User check:** Ask user to verify multi-monitor behavior
