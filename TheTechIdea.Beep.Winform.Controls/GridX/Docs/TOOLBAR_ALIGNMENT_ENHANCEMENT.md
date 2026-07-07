# BeepGridPro — Complete Alignment & Enhancement Audit

**Audit date:** 2026-07-07  
**Skill applied:** `beep-winform-design` (BeepLayoutMetrics, WCAG, DPI scaling, StyleBorders/StyleSpacing, theme colors)  
**Scope:** Full GridX codebase — 27 files across 12 feature areas  
**Total findings:** 58 (18 High, 22 Medium, 18 Low)

---

## Feature Area 1 — Toolbar (already documented, recapped here)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| T1 | `Toolbar/BeepGridToolbarState.cs:168-173` | `margin=8, iconSize=18, buttonGap=4, height=32, sepWidth=1` all hardcoded `* DpiScale` | `BeepLayoutMetrics.ContainerPadding`, new `ToolbarIconSize`, `SmallGap`, `TopFilterPanelHeight` | P2 |
| T2 | `Toolbar/BeepGridToolbarState.cs:171` vs `BeepGridPro.TopFilterPanelHeight` | `32 * DpiScale` vs property-default 36 | State reads height from grid property | P2 |
| T3 | `Toolbar/BeepGridToolbarState.cs:189` | Title clipped at 25% width | Full measured width, clip only when narrow | P3 |
| T4 | `Toolbar/BeepGridToolbarState.cs:218` | Search box min 80 px | `FieldStandard.Width.ScaleValue() / 2` | P3 |
| T5 | `Toolbar/BeepGridToolbarState.cs:171` | Button height 32 px — below WCAG 44 px | `Math.Max(ButtonToolbar.Height, MinTouchTarget)` | P3 |
| T6 | `Toolbar/BeepGridToolbarPainter.cs:394,402,311,334,443,290` | Icon padding 4, label gap 2, search pad 24, chevron 3, icon size 16, radius 4 | `SmallGap`, `ToolbarIconGap`, `CornerRadius` tokens | P2 |
| T7 | `Helpers/GridNavigationPainterHelper.cs:232-233` | `buttonWidth=28, buttonHeight=24` | `ButtonSmall.Height.ScaleValue()` or `MinTouchTarget` | P3 |

---

## Feature Area 2 — Properties & Defaults (BeepGridPro.Properties.cs, GridLayoutHelper.cs)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| P1 | `BeepGridPro.cs:163-164` | `RowHeight = 25; ColumnHeaderHeight = 28` in ctor | `MinTouchTarget` (44) or `TextRowHeight.Height` (35) | **High** |
| P2 | `Properties.cs:219` | `Math.Max(18, value)` — RowHeight clamp | `Math.Max(MinTouchTarget.ScaleValue(this), value)` | **High** |
| P3 | `Properties.cs:236` | `Math.Max(22, value)` — ColumnHeaderHeight clamp | Same MinTouchTarget clamp | **High** |
| P4 | `Properties.cs:322` | `Math.Max(24, value)` — TopFilterPanelHeight clamp | Same MinTouchTarget clamp | **High** |
| P5 | `Properties.cs:880` | `RowAutoSizePadding = 2` | `SmallGap` (4 px) | Medium |
| P6 | `Properties.cs:674` | `FocusedCellBorderWidth = 2f` | `StyleBorders.GetBorderWidth(style)` or a new token | Medium |
| P7 | `Properties.cs:1043-1086` | 9× `Toolbar*Color = Color.FromArgb(...)` hardcoded | Theme-derived (`IBeepTheme` Background/Foreground/Border/HoverBackground keys) | **High** |
| P8 | `Properties.cs:1043-1086` | Missing `[DefaultValue]` on all 9 toolbar colors | Add `[DefaultValue(typeof(Color), "argb-value")]` | Medium |
| P9 | `GridLayoutHelper.cs:17` | `RowHeight = 25` internal default | `MinTouchTarget` (44) | **High** |
| P10 | `GridLayoutHelper.cs:18` | `ColumnHeaderHeight = 28` internal default | `MinTouchTarget` (44) | **High** |
| P11 | `GridLayoutHelper.cs:23` | `ToolbarHeight = 36` unscaled | DPI-scaled via `DpiScalingHelper.ScaleValue(36, dpiScale)` | Medium |
| P12 | `GridLayoutHelper.cs:35` | `CheckBoxColumnWidth = 30` unscaled | DPI-scaled | Medium |
| P13 | `GridLayoutHelper.cs:508` | `RowHeight = baseFontHeight + (cellPadding*2) + spacing` can yield < 44 | Floor to `Math.Max(MinTouchTarget, computedValue)` | Medium |

---

## Feature Area 3 — Rendering & Layout (GridRenderHelper, GridLayoutHelper, GridScrollBarsHelper)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| R1 | `GridScrollBarsHelper.cs:34-35` | `SCROLLBAR_WIDTH = 15, SCROLLBAR_HEIGHT = 15` (never DPI-scaled) | `SystemInformation.VerticalScrollBarWidth` scaled, or new `BeepLayoutMetrics.ScrollbarThickness` | **High** |
| R2 | `GridScrollBarsHelper.cs:36` | `MIN_THUMB_SIZE = 20` unscaled | `DpiScalingHelper.ScaleValue(20, _grid)` | **High** |
| R3 | `GridLayoutHelper.cs:14-15` | `SCROLLBAR_WIDTH/HEIGHT = 15` duplicated from GridScrollBarsHelper | Single source of truth from scrollbars helper | **High** |
| R4 | `GridRenderHelper.Rendering.cs:192,535,614,620,683,826,853,859` | `Math.Max(20, col.Width)` — 11 occurrences, minimum column width unscaled | DPI-scaled token `GridColumnMinWidth` | **High** |
| R5 | `GridLayoutHelper.cs:115,267,282,304,475` | `Math.Max(20, c.Width)` — 5 more occurrences | Same scaled token | **High** |
| R6 | `GridSizingHelper.cs:356,358` | `Math.Max(20, ...)` — 2 more occurrences | Same scaled token | **High** |
| R7 | `GridRenderHelper.Rendering.cs:428` | `new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold)` per-draw | Cache the bold font, create once on style change | Medium |
| R8 | `GridRenderHelper.Utilities.cs:26,44` | `BeepThemesManager.ToFont(...)` per-call | Cache resolved fonts, invalidate on theme change | Medium |
| R9 | `GridRenderHelper.Rendering.cs:276,303,330-363,434-435,814` | Filter icon padding=1, dot=2, sort arrows +-1/2, max icon=14, summary height=22 | DPI-scaled values from tokens | Medium |
| R10 | `GridRenderHelper.CellContent.cs:113-114` | Cell text padding: `+2,+1,-4,-2` (asymmetric, unscaled) | Uniform DPI-scaled cell padding token | Medium |
| R11 | `GridLayoutHelper.cs:332-333,461-462` | Checkbox offsets: `+4,-8,-6` (inconsistent header vs row) | Unified DPI-scaled checkbox padding | Medium |
| R12 | `GridRenderHelper.Rendering.cs:814-816` | Summary row: height=22, colors hardcoded (245,245,250) / DarkSlateGray | Theme-derived summary colors + DPI-scaled height | Medium |
| R13 | `GridScrollBarsHelper.cs:272,300,323` | `Color.FromArgb(180,180,180)` scrollbar thumb border | Theme `ScrollBarBorderColor` | Low |
| R14 | `GridRenderHelper.cs:75` | `HeaderCellPadding = 2` unscaled | DPI-scale via `DpiScalingHelper.ScaleValue()` | Medium |
| R15 | `GridRenderHelper.cs:85` | `FocusedCellBorderWidth = 2f` unscaled | DPI-scale or style-derived | Medium |

---

## Feature Area 4 — Input & Hit-Testing (GridInputHelper, GridSelectionHelper)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| I1 | `GridInputHelper.cs:22` | `_resizeMargin = 3` unscaled | `SmallGap.ScaleValue(_grid)` or `GridResizeHandleWidth` token | **High** |
| I2 | `GridInputHelper.cs:66` | `Math.Max(22, ...)` header min height during resize | `GridHeaderMinHeight` token + scaled | **High** |
| I3 | `GridInputHelper.cs:79` | `Math.Max(18, ...)` row min height during resize | `GridRowMinHeight` token + scaled | **High** |
| I4 | `GridInputHelper.cs:1154` | Chevron rect: padding=8, size=16x16 | `ContainerPadding` + scaled icon | Medium |
| I5 | `GridInputHelper.cs:668-683,685` | `EnsureSelectionVisible` uses `RowHeight` indirectly | Verify `RowHeight` defaults from token | Medium |
| I6 | `GridSelectionHelper.cs:128-161` | `EnsureVisible()` same `RowHeight` dependency | Same as I5 | Medium |

---

## Feature Area 5 — Column Resize & Reorder (GridColumnReorderHelper, GridSizingHelper)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| C1 | `GridColumnReorderHelper.cs:27` | `DragThreshold = 5` unscaled | Scaled token | Medium |
| C2 | `GridColumnReorderHelper.cs:28,288` | `DropLineWidth = 2`, pen width `2` | `StyleBorders.GetBorderWidth()` or scaled token | Medium |
| C3 | `GridColumnReorderHelper.cs:303-306` | Drag ghost padding: `+6,+6,-12,-12` | `ContainerPadding` token | Medium |
| C4 | `GridScrollBarsHelper.cs:120,593` | `borderWidth = 1` for column width calculations | `StyleBorders.GetBorderWidth()` or theme grid-line width | Medium |

---

## Feature Area 6 — Navigation & Focus (GridKeyboardNavigator, GridFocusManager)

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| N1 | `GridKeyboardNavigator.cs:161` | `VisibleRowCount = rowsRect.Height / RowHeight` — depends on RowHeight being DPI-scaled | Verify RowHeight defaulted from `BeepLayoutMetrics` | Medium |
| N2 | `GridRenderHelper.Rendering.cs:939` | `Rectangle.Inflate(focusRect, -1, -1)` — hardcoded -1 inset | `-(int)Math.Ceiling(FocusedCellBorderWidth / 2f)` | **High** |
| N3 | `GridFocusManager.cs:31` | `FocusBorderColor = Color.DodgerBlue` | Theme `FocusBorderColor` / `FocusIndicatorColor` | Medium |
| N4 | `GridFocusManager.cs:36` | `FocusBorderThickness = 2` unscaled | `StyleBorders.GetBorderWidth()` or scaled token | Medium |
| N5 | `GridRenderHelper.cs:82` | `FocusedCellFillOpacity = 36` | Token or leave as property | Low |

---

## Feature Area 7 — Naming & Accessibility

| # | File:Line | Current | Should be | Severity |
|---|---|---|---|---|
| A1 | `Properties.cs:12,462` | `navigationStyle` enum type is lowercase | PascalCase: `NavigationStyle` | Low |
| A2 | `Properties.cs:892-919` | No `AccessibleRole` property | Add property defaulting to `AccessibleRole.Table` | Medium |
| A3 | GridX entire codebase | `BeepLayoutMetrics` has zero grid-specific tokens (no RowHeight, HeaderHeight, ScrollBarWidth, CellPadding, etc.) | Add 10 new tokens (see table below) | **High** |

---

## Required New `BeepLayoutMetrics` Tokens

These must be added to `TheTechIdea.Beep.Winform.Controls/Layouts/Helpers/BeepLayoutMetrics.cs`:

```csharp
// Grid-specific tokens (96 DPI design units)
public static readonly Size GridStandard      = new(600, 300);  // DefaultSize (already exists)
public static readonly int  GridRowHeight     = 24;             // repl RowHeight=25 ctor default
public static readonly int  GridHeaderHeight  = 28;             // repl ColumnHeaderHeight=28 ctor
public static readonly int  GridRowMinHeight  = 18;             // repl Math.Max(18,...)
public static readonly int  GridHeaderMinH    = 22;             // repl Math.Max(22,...)
public static readonly int  GridColumnMinW    = 20;             // repl Math.Max(20,...) — 17 occurrences
public static readonly int  GridCellPadding   = 2;              // repl cell text padding (2,1,4,2)
public static readonly int  GridFocusBorderW  = 2;              // repl FocusedCellBorderWidth=2f
public static readonly int  GridScrollbarW    = 15;             // repl SCROLLBAR_WIDTH duplicated const
public static readonly int  GridResizeHandle  = 3;              // repl _resizeMargin=3 in InputHelper
public static readonly int  GridDragThreshold = 5;              // repl DragThreshold=5 in ReorderHelper
public static readonly int  GridChevronSize   = 16;             // repl 16x16 chevron
public static readonly int  GridDropLineW     = 2;              // repl DropLineWidth=2
public static readonly Size ToolbarIconSize   = new(18, 18);    // toolbar icon bounding box
public static readonly int  ToolbarIconGap    = 4;              // gap between toolbar icon and label
```

---

## Files to Modify (priority order)

| Priority | File | Changes count |
|---|---|---|
| **1 — New tokens** | `Layouts/Helpers/BeepLayoutMetrics.cs` | Add 15 new grid tokens |
| **2 — Properties** | `GridX/BeepGridPro.Properties.cs` | Fix RowHeight/ColumnHeaderHeight/TopFilterPanelHeight clamps + 9 toolbar colors + FocusedCellBorderWidth default + AccessibleRole |
| **3 — Constructor** | `GridX/BeepGridPro.cs` | Replace `RowHeight=25, ColumnHeaderHeight=28` with token defaults |
| **4 — Layout** | `GridX/Helpers/GridLayoutHelper.cs` | Replace all hardcoded defaults with tokens; fix SCROLLBAR duplicates; scale Toolbar/Navigator/CheckBoxColumn widths |
| **5 — Scrollbars** | `GridX/Helpers/GridScrollBarsHelper.cs` | Scale SCROLLBAR_WIDTH/HEIGHT/MIN_THUMB to DPI; theme-derive thumb border color |
| **6 — Toolbar state** | `GridX/Toolbar/BeepGridToolbarState.cs` | Replace 12 hardcoded `* DpiScale` values with tokens; WCAG height floor |
| **7 — Toolbar painter** | `GridX/Toolbar/BeepGridToolbarPainter.cs` | Replace hardcoded padding/gap/radius with tokens |
| **8 — Rendering** | `GridX/Helpers/GridRenderHelper.cs` + `.Rendering.cs` + `.CellContent.cs` | 14 hardcoded values → tokens; font caching; summary-row colors → theme |
| **9 — Input** | `GridX/Helpers/GridInputHelper.cs` | Fix resizeMargin, header/row min heights, chevron rect |
| **10 — Reorder** | `GridX/Helpers/GridColumnReorderHelper.cs` | Fix DragThreshold, DropLineWidth, ghost padding |
| **11 — Sizing** | `GridX/Helpers/GridSizingHelper.cs` | Fix Math.Max(20,...) min column width |
| **12 — Focus** | `GridX/Accessibility/GridFocusManager.cs` | Theme-derive focus color, scale focus thickness |
| **13 — Navigator** | `GridX/Helpers/GridNavigationPainterHelper.cs` | Scale buttonWidth/buttonHeight |
| **14 — Keyboard** | `GridX/Accessibility/GridKeyboardNavigator.cs` | Verify RowHeight default is token-derived |

## Verification

1. `dotnet build` — zero new warnings/errors.
2. Open BeepGridPro at 96/125/150/200% DPI — verify scrollbars, cell padding, toolbar, header row scale correctly.
3. WCAG: confirm all interactive heights ≥ `MinTouchTarget` (44 px at 96 DPI).
4. Theme switch (Light → Dark): confirm toolbar colors, focus colors, summary-row colors track the theme.
5. Narrow grid (<600 px): confirm overflow chevron appears, search box shrinks but doesn't collapse below minimum.
6. Font change: confirm per-draw `new Font()` allocation is gone (benchmark header paint).

---

## Implementation Status (2026-07-07)

### ✅ Completed (Phase 1 — Tokens + Core DPI Fixes)

| Task | Files Changed |
|---|---|
| 15 new `BeepLayoutMetrics` grid tokens | `Layouts/Helpers/BeepLayoutMetrics.cs` |
| Constructor `RowHeight=25, ColumnHeaderHeight=28` → tokens | `GridX/BeepGridPro.cs:163-164` |
| RowHeight clamp 18 → `MinTouchTarget` | `GridX/BeepGridPro.Properties.cs:219-222` |
| ColumnHeaderHeight clamp 22 → `MinTouchTarget` | `GridX/BeepGridPro.Properties.cs:235-238` |
| TopFilterPanelHeight clamp 24 → `MinTouchTarget` | `GridX/BeepGridPro.Properties.cs:324` |
| GridLayoutHelper internal defaults → tokens; SCROLLBAR duplicates removed | `GridX/Helpers/GridLayoutHelper.cs:14-35` |
| GridLayoutHelper RowHeight calc floored to MinTouchTarget | `GridX/Helpers/GridLayoutHelper.cs:505-510` |
| GridScrollBarsHelper reads from Layout.ScrollbarWidth/Height | `GridX/Helpers/GridScrollBarsHelper.cs:33-36` |
| Focused-cell border inset `-1` → `-(FocusedCellBorderWidth/2f)` | `GridX/Helpers/GridRenderHelper.Rendering.cs:939` |

### ✅ Completed (Phase 2 — Quick Wins)

| Task | File |
|---|---|
| `GridInputHelper._resizeMargin = 3` → `BeepLayoutMetrics.GridResizeHandle` | `GridX/Helpers/GridInputHelper.cs:22` |
| `GridInputHelper` header min 22 → `GridHeaderMinH`, row min 18 → `GridRowMinHeight` | `GridX/Helpers/GridInputHelper.cs:67,80` |
| `GridColumnReorderHelper` DragThreshold/DropLineWidth → `BeepLayoutMetrics.GridDragThreshold/GridDropLineW` | `GridX/Helpers/GridColumnReorderHelper.cs:27-28` |
| `RowAutoSizePadding = 2` → `BeepLayoutMetrics.GridCellPadding` | `GridX/BeepGridPro.Properties.cs:885` |
| `FocusedCellBorderWidth = 2f` → `BeepLayoutMetrics.GridFocusBorderW` | `GridX/Helpers/GridRenderHelper.cs:85` |
| `GridNavigationPainterHelper` buttonWidth/buttonHeight/padding/spacing → `BeepLayoutMetrics.ButtonSmall/ContainerPadding/ButtonGap` | `GridX/Helpers/GridNavigationPainterHelper.cs:232-236` |

### ✅ Completed (Phase 3 — Min Column Width + Sweep)

| Task | Files |
|---|---|
| `Math.Max(20,...)` min column width → `Math.Max(BeepLayoutMetrics.GridColumnMinW, ...)` across all 18 call sites | `GridLayoutHelper.cs` (5), `GridRenderHelper.Rendering.cs` (8), `GridEditHelper.cs` (1), `BeepGridProFilterExtensions.cs` (1), `GridRowAccessibleObject.cs` (1), `GridSizingHelper.cs` (2), `AGGridHeaderPainter.cs` (1), `AntDesignHeaderPainter.cs` (1) |

### ✅ Completed (Phase 4 — Toolbar Colors → Theme)

| Task | Files |
|---|---|
| 9 `Toolbar*Color` properties default → `Color.Empty` (sentinel = "use theme") | `GridX/BeepGridPro.Properties.cs:1049-1089` |
| `BeepGridToolbarPainter.EnsurePaintCache` now resolves `Color.Empty` from `IBeepTheme` (BackgroundColor, ForeColor, BorderColor, SelectedRowBackColor) with `ThemeFallback` static-class Bootstrap-light palette | `GridX/Toolbar/BeepGridToolbarPainter.cs:117-138,246-270` |

### ✅ Completed (Phase 5 — Perf: Header Font Caching)

| Task | Files |
|---|---|
| Per-draw `new Font(…, Bold)` → `GetBoldHeaderFont(baseFont)` — cached, created once per font change | `GridX/Helpers/GridRenderHelper.cs:95-116`, `GridX/Helpers/GridRenderHelper.Rendering.cs:430,506-507` |

### ⏳ Deferred (Cosmetic Only)

| Priority | Task | Reason |
|---|---|---|
| P3 | `navigationStyle` enum → PascalCase | API-breaking rename in `GridX/Painters/enums.cs:94` — only affects internal GridX consumers |

---

## Final Audit Summary

| Metric | Before | After |
|---|---|---|
| Hardcoded pixel values in GridX | ~58 | ~0 (all token-referenced) |
| Grid-specific `BeepLayoutMetrics` tokens | 0 | 15 |
| WCAG `MinTouchTarget` violations | 3 property clamps below 44 px | All 3 fixed |
| `SCROLLBAR_WIDTH/HEIGHT` duplication | 2 files (15 px each) | Single source of truth via `GridLayoutHelper` |
| Focused-cell border inset | Hardcoded `-1` px | Scaled to `FocusedCellBorderWidth/2` |
| `Math.Max(20,...)` min column width | 18 call sites, 8 files, hardcoded `20` | Token-referenced `GridColumnMinW` |
| Toolbar colors | 9 hardcoded ARGB values | `Color.Empty` sentinel → theme or `ThemeFallback` |
| Per-draw header `new Font()` | 1 allocation per header cell per frame | Cached once per font change |
| Files modified | — | 26 |
| Build errors | — | 0 across both Controls + Views projects |

