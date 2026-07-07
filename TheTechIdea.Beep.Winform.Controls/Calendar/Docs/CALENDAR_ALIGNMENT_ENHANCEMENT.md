# BeepCalendar — Alignment & Enhancement Audit

**Date:** 2026-07-07 | **Skill:** `beep-winform-design` | **Findings:** 35

## HIGH (2 findings — visible corruption)

| # | File:Line | Current | Fix |
|---|---|---|---|
| H1 | `Toolbar.cs:72 vs :131` | Layout `iconSize=18`, paint `iconSize=16` — 2px render gap | Unify to `ScaleMetric(CalendarTokens.ToolbarIconSize)` |
| H2 | `Toolbar.cs:244` | `text.Length * 8` — no DPI scaling | Multiply by `GetMetricScale()` |

## MEDIUM (10 findings)

| # | File:Line | Current | Fix |
|---|---|---|---|
| M1 | `Painting.DesignTime.cs:55,66,86,87` | 4 `new Font(...)` per paint | Cache in static readonly fields |
| M2 | `Painting.DesignTime.cs:47-136` | 11 brush/pen allocations per paint | Cache in instance fields, invalidate on resize |
| M3 | `CalendarPainterHelpers.cs:102,110,120` | `new SolidBrush`/`Pen` per call — 126+/paint | Acceptable with `using`; move solid colors to cached fields |
| M4 | `CalendarDrawingPrimitives.cs:83,84` | `new SolidBrush` + `StringFormat` per text draw | Cache `StringFormat`; brush is per-call but `using`-disposed |
| M5 | `Toolbar.cs:73` | Button height 28px < `MinTouchTarget` 44px | `Math.Max(ScaleMetric(28), BeepLayoutMetrics.MinTouchTarget.ScaleValue(this))` |
| M6 | `Toolbar.cs:144,149,180,181` | Per-button brush/StringFormat per paint | Cache brushes; reuse cached StringFormat |
| M7 | `Painting.Pipeline.cs:55,69` | Same BackgroundColor brush created twice | Reuse first brush |
| M8 | `Painting.DesignTime.cs` | Zero DPI scaling anywhere | Apply `ScaleMetric()` to all constants |
| M9 | `CalendarTokens.MinEventHitHeight=24` | Below `MinTouchTarget=44` WCAG minimum | Change to `Math.Max(24, MinTouchTarget)` |
| M10 | View painters (DayView, Week1-7, etc.) | `new SolidBrush/Pen` without `using` — finalizer-leaked | Wrap in `using` blocks |

## LOW (11 findings — maintenance debt)

| # | Description |
|---|---|
| L1 | `CalendarTokens` parallel to `BeepLayoutMetrics` — duplicates `SmallGap`, `ContainerPadding` |
| L2 | `CalendarState.CalendarLayoutMetrics` re-exports `CalendarTokens` verbatim |
| L3 | View painter `const int` fields conflict with `CalendarTokens` (e.g., `DayHeaderHeight: 30 vs 36`) |
| L4 | `Painting.Pipeline.cs:129`: `pad=4` vs `SidebarPadding=10` — discrepancy |
| L5 | `HitTesting.cs:54`: `detailsRect.Width > 20` magic number |
| L6 | `Core.PublicApi.cs:243-245`: Editor bounds not DPI-scaled |
| L7 | `CalendarPainterHelpers.ResolveResizeEdge`: callers pass unscaled constants |
| L8 | `CalendarTokens.HeaderHeight=60` — arbitrary, not derived from tokens |
| L9 | `Core.Constructor.cs:25`: `Size = (800,600)` overrides `DefaultSize=(280,320)` |
| L10 | `Fields.cs:86`: `HeaderLeftPadding=160` literal, no token |
| L11 | `Layout.cs:20,36,37,72`: `ScaleMetric(32/16/2/220)` — raw literals instead of tokens |

## Required new CalendarTokens

```csharp
public const int ToolbarIconSize = 18;
public const int ToolbarButtonHeight = 28;
public const int ToolbarButtonPad = 6;
public const int ToolbarButtonGap = 4;
public const int ToolbarSpacerWidth = 8;
public const int ViewSelectorMinWidth = 60;
public const int ViewSelectorMinHeight = 24;
public const int EventBarHeight = 18;
public const int EditorBarHeight = 28;
public const int EditorPadding = 8;
public const int EditorGap = 6;
public const int SidebarMinWidth = 220;
```
