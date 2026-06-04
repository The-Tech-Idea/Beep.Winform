# Phase 6 - Pipeline Consolidation And Editor Layer

Priority: High
Status: In progress (W1 done; W2 in progress; build BROKEN until W2 view painters ship)
Depends on: Phase 1-3 code-contract completion

## Goal

Collapse the two parallel paint / hit-test paths in `BeepCalendar` down to a **single per-view painter** (`ICalendarViewPainter`) that owns Layout + Paint + HitTest for one view mode and consumes `IBeepTheme` + `BeepControlStyle` directly (no `ICalendarStylePainter` / `MaterialCalendarPainter` / `MinimalCalendarPainter`). Then introduce a transparent child `Panel` "editor layer" that hosts real Beep controls (text boxes, check boxes, date pickers, combo boxes) for in-place event editing, achieved by overriding **both** `OnPaint` **and** `OnPaintBackground` (not just `OnPaint`).

## Why This Phase Now

Current code has two parallel pipelines that drift in behavior and cost maintenance:

- **Modern** (now removed): `ICalendarStylePainter` + `MaterialCalendarPainter` / `MinimalCalendarPainter` + `CalendarPainterContext` + `CalendarPainterFactory`.
- **Legacy** (now removed): `ICalendarViewRenderer` + `*ViewRenderer.cs` + `CalendarRenderer` + `CalendarRenderContext` + `CommonDrawing.cs`.

The "style painter" abstraction was a mistake: Material3 and Minimal are not separate painters — they are parameters of a single view. There is exactly one painter per view, and it switches on `BeepControlStyle` to vary its metrics, colors, and rendering choices. Replacing the dual pipeline with per-view `ICalendarViewPainter` that reads `IBeepTheme` + `BeepControlStyle` via a single `ViewPaintArgs` bundle collapses the matrix.

Hit-test was duplicated the same way (control-level in `BeepCalendar.Interactions.HitTesting.*` vs. legacy in each `*ViewRenderer.HandleClick` invoked from `OnMouseClick` at `BeepCalendar.Core.Lifecycle.cs:45`).

There is also no in-place editing surface — toolbar is painted (`BeepCalendar.Toolbar.cs:26-203`) and event editing goes through a modal `CalendarEventEditor` dialog. A transparent `Panel` hosted inside `BeepCalendar` that contains real `BeepTextBox` / `BeepCheckBoxBool` / `BeepDateTimePicker` / `BeepComboBox` controls closes that gap.

## Current State (2026-06-04)

- Build: **BROKEN** (W2 in progress). 0 errors before W2 deletions.
- Working tree has uncommitted deletions of all legacy `*ViewRenderer.cs` / `*CalendarPainter*.cs` files and uncommitted rewrites of `BeepCalendar.Painting.*`, `BeepCalendar.LayoutTheme.Layout.cs`, `BeepCalendar.Interactions.HitTesting.*`, `BeepCalendar.Core.Constructor.cs`, `BeepCalendar.Core.Style.cs`, `BeepCalendar.Core.Lifecycle.cs`, `BeepCalendar.Painting.cs`, `BeepCalendar.Fields.cs`.
- `BeepCalendar.Fields.cs` still has `_stylePainter`, `_renderer`, `_usePainterSystem` — to be removed in W2.
- `Rendering/ICalendarViewPainter.cs`, `Rendering/ViewPaintArgs.cs`, `Rendering/ViewPainterFactory.cs` created (W2 partial).
- `Helpers/CalendarSurfaceModel.cs` immutable, built once per `UpdateLayout` (W1 done). Consumes `IBeepTheme` + `BeepControlStyle` indirectly via callers; not part of the painter API.
- `BaseControl.OnPaint` (`BaseControl/BaseControl.Events.cs:194-209`) -> `SafeDraw` -> `ClearDrawingSurface` (`g.Clear(BackColor)` unless `AllowBaseControlClear == false`).
- `BaseControl.ExcludeChildControlsFromClip` (`BaseControl.Events.cs:52-64`) is disabled with comment "DISABLED - was causing child controls to not paint until hover/click".
- `BaseControl.ExcludedPaintRectangles` (`BaseControl.Properties.cs:174-214`) is publicly writable but `ExcludeConfiguredPaintRectangles` (`BaseControl.Events.cs:66-88`) is never invoked from any paint method.
- `BaseControl` sets `WS_CLIPCHILDREN | WS_CLIPSIBLINGS` in `CreateParams` (`BaseControl.cs:434-460`).

## Key Decisions

- **Single pipeline target**: one `ICalendarViewPainter` per `CalendarViewMode` (Month, Week, WorkWeek, Day, Agenda, Timeline, List). The view painter switches on `ViewPaintArgs.ControlStyle` to vary Material3 vs Minimal vs others. **No `ICalendarStylePainter`, no `MaterialCalendarPainter`, no `MinimalCalendarPainter`, no `CalendarPainterFactory`.**
- **Single args bundle**: `ViewPaintArgs` (`Rendering/ViewPaintArgs.cs`) carries theme, control style, state, rects, surface model, event service, events, categories, resources, fonts, hover/selected, owner, resolved color palette, and `CalendarStyleMetrics`. New inputs are additive, never breaking.
- **Style metrics come from `CalendarStyleMetrics.For(BeepControlStyle style)`** which:
  - returns the layout/visual constants (HeaderHeight, CornerRadius, EventCornerRadius, ShowEventShadows, etc.) for the style; and
  - resolves the default `IBeepTheme` for the style via `BeepThemesManager.GetThemeNameForFormStyle(FormStyle)` (namespace `TheTechIdea.Beep.Winform.Controls.ThemeManagement`) + `BeepThemesManager.GetTheme(name)` (namespace `TheTechIdea.Beep.Vis.Modules`).
- **Theme + fonts resolved inside `ViewPaintArgs.ApplyTheme(IBeepTheme)`** which projects `theme.CalendarBackColor` / `CalendarForeColor` / `CalendarBorderColor` / `PrimaryColor` / `BeepTheme.SecondaryColor` into the resolved color palette; and **`ViewPaintArgs.ApplyThemeFonts()`** which re-resolves `HeaderFont` / `DayFont` / `EventFont` / `TimeFont` / `DaysHeaderFont` from `theme.CalendarTitleFont` / `DaysHeaderFont` / `DateFont` / `CalendarSelectedFont` / `CalendarUnSelectedFont` via `BeepThemesManager.ToFont(theme.X)`.
- **Icon painting in view painters uses `TheTechIdea.Beep.Winform.Controls/Styling/ImagePainters/StyledImagePainter`** (static `Paint(g, path, imagePath, style)` with thread-safe `_imageCache` / `_painterCache` / `_tintedCache`) and **`TheTechIdea.Beep.Winform.Controls/IconsManagement/SvgsUIcons`** (`GetResourceName(fileName)`, `GetAllResourceNames()`, `GetResource(fileName)`, `GetIconPath`, `ResourceAssembly`).
- **Chrome stays in `BeepCalendar` partials**: header text + view selector + toolbar + sidebar are still painted by `BeepCalendar` (the view painter handles only the central grid for its view). The view painter fills the `_rects.CalendarGridRect` area.
- **Editor layer approach**: override `OnPaint` **and** `OnPaintBackground` on `BeepCalendar`, apply `Region.Exclude(_editorLayer.Bounds)` clip, then `base.OnPaint(e)`. Also override `AllowBaseControlClear => false` to skip `g.Clear(BackColor)` and `IsContainerControl => true`.
- **Sidebar becomes plug-in host** (deferred — captured in `plans/07-developer-extension-surface.md` follow-up).
- **Painted toolbar chrome is kept** for now; toolbar items can opt into real `BeepButton` controls in the editor layer.
- **Real `BeepTextBox` / `BeepCheckBoxBool` / `BeepDateTimePicker` / `BeepComboBox` exist** under `TextFields/`, `CheckBoxes/`, `Dates/`, `ComboBoxes/` and are real `Control` subclasses — safe to host in the editor layer.
- **`BeepComboBox` dropdown is a `ToolStripDropDown` (top-level window)**, so it is naturally above the painted surface.

## Workstreams

### W1 - CalendarSurfaceModel ✅ DONE

`Helpers/CalendarSurfaceModel.cs` — immutable, built once per `UpdateLayout` call. Computes:

- Header rect, toolbar rect, sidebar rect (per-view width via `BeepCalendar.LayoutTheme.Layout.cs:9-50 GetResponsiveSidebarWidth`).
- Content rect, day-cell rects, week-slot rects, month event bar rects, week event block rects, header text bounds, today marker bounds.
- `GetMonthCellRect(date)`, `GetWeekDayHeaderRect(day)`, `GetWeekDayColumnRect(day)`, `GetTimeRowRect(hour)`, `GetTimedEventRect(column, evt, date)`, `GetListRowRect(row)`, `GetSidebarMiniCalendarRect()`, `GetSidebarEventDetailsRect()`, `GetHeaderTextBounds()`.
- Per-view anchors: `StartOfWeek`, `StartOfWorkWeek`, `FirstDayOfMonth`, `FirstDayOfCalendar`, `WeekDayCount` (5 for WorkWeek else 7).
- `CalendarState` and `CalendarRects` are `public` (promoted in W1) so the model can be built from `BeepCalendar.UpdateLayout`. `CalendarInteractionHitTestResult` is `public` (promoted in W2) so `ICalendarViewPainter.HitTest` can return it.

Consumed by: all view painters (in W2), all hit-test helpers (in W2), sidebar panels, editor layer positioning. Replaces the geometry math previously scattered across `BeepCalendar.LayoutTheme.*` and `Helpers/CalendarLayoutGeometry.cs:8-150`.

### W2 - Per-View Painter Pipeline (in progress, build broken)

**Goal**: delete legacy pipeline, install per-view `ICalendarViewPainter` in its place, rewire `BeepCalendar` to call the painter directly.

**New files**:

- `Rendering/ICalendarViewPainter.cs` — interface: `ViewMode`, `Layout(ViewPaintArgs)`, `Paint(Graphics, ViewPaintArgs)`, `HitTest(Point, ViewPaintArgs)`. **Created.** ✅
- `Rendering/ViewPaintArgs.cs` — `ViewPaintArgs` class + `CalendarStyleMetrics` class. Includes `ApplyTheme(IBeepTheme)`, `ApplyThemeFonts()`, `ResolveThemeColors()`, `GetCategoryColor(int)`. `CalendarStyleMetrics.For(BeepControlStyle)` resolves the default `IBeepTheme` via `BeepThemesManager.GetThemeNameForFormStyle(FormStyle)`. **Created; extensions pending.** ⚠️
- `Rendering/ViewPainterFactory.cs` — static `GetPainter(CalendarViewMode)` with `Dictionary<CalendarViewMode, ICalendarViewPainter>` cache + `Reset()` test hook. **Created.** ✅
- `Rendering/ViewPainters/MonthViewPainter.cs` — handles `Layout` (no-op; layout already on `Surface`), `Paint` (draws day cells + event bars using `ViewPaintArgs` theme + metrics + colors), `HitTest` (date cell / event block / resize edge). Uses `StyledImagePainter` + `SvgsUIcons` for any icon. **TODO.**
- `Rendering/ViewPainters/WeekViewPainter.cs` — handles 7-day timed view. **TODO.**
- `Rendering/ViewPainters/WorkWeekViewPainter.cs` — handles 5-day timed view (`dayCount = surface.WeekDayCount` or hardcoded 5). **TODO.**
- `Rendering/ViewPainters/DayViewPainter.cs` — single-day timed view. **TODO.**
- `Rendering/ViewPainters/AgendaViewPainter.cs` — month events grouped by day. **TODO.**
- `Rendering/ViewPainters/TimelineViewPainter.cs` — resource lanes × day axis; carries over lane virtualization, day axis, lane background, resource accent stripe, lane-event lookup from the deleted `TimelineViewRenderer.cs`. **TODO.**
- `Rendering/ViewPainters/ListViewPainter.cs` — month events as a vertical list of rows. **TODO.**

**Deleted files**:

- `Rendering/ICalendarStylePainter.cs` ✅ DELETED
- `Rendering/StylePainters/MaterialCalendarPainter.cs` ✅ DELETED
- `Rendering/StylePainters/MinimalCalendarPainter.cs` ✅ DELETED
- `Rendering/StylePainters/` (directory) ✅ DELETED
- `Rendering/CalendarPainterFactory.cs` ✅ DELETED
- `Rendering/ICalendarViewRenderer.cs` ✅ DELETED
- `Rendering/CalendarRenderer.cs` ✅ DELETED
- `Rendering/CalendarRenderContext.cs` ✅ DELETED
- `Rendering/CommonDrawing.cs` ✅ DELETED
- `Rendering/CalendarDrawingPrimitives.cs` (old) ✅ DELETED (helpers migrated to `Helpers/CalendarDrawingPrimitives.cs`)
- `Rendering/MonthViewRenderer.cs` ✅ DELETED
- `Rendering/WeekViewRenderer.cs` ✅ DELETED
- `Rendering/WorkWeekViewRenderer.cs` ✅ DELETED
- `Rendering/DayViewRenderer.cs` ✅ DELETED
- `Rendering/AgendaViewRenderer.cs` ✅ DELETED
- `Rendering/TimelineViewRenderer.cs` ✅ DELETED
- `Rendering/ListViewRenderer.cs` ✅ DELETED
- `BeepCalendar.Painting.Pipeline.Legacy.cs:13 DrawWithLegacyRenderer` ✅ DELETED

**BeepCalendar partial rewires**:

- `BeepCalendar.Fields.cs:14-58` — remove `_stylePainter`, `_renderer`, `_usePainterSystem`; add `private ICalendarViewPainter _viewPainter;` (initialized in `BeepCalendar.Core.Constructor.cs`).
- `BeepCalendar.Core.Constructor.cs:9-39` — remove `_renderer = new CalendarRenderer();` + `_stylePainter = CalendarPainterFactory.GetPainter(_calendarStyle);`; add `_viewPainter = ViewPainterFactory.GetPainter(_state.ViewMode);`. After `ApplyThemeTypography`, call `ViewPaintArgs.ApplyTheme(_currentTheme)` to seed the resolved color palette.
- `BeepCalendar.Core.Style.cs:15-47` — `CalendarStyle` setter only updates `_calendarStyle` and calls `Invalidate()`; remove `UsePainterSystem` property entirely.
- `BeepCalendar.Core.PublicApi.cs:13` — `ViewMode` setter swaps `_viewPainter` via `ViewPainterFactory.GetPainter(mode)` and calls `RequestLayoutAndRedraw()`.
- `BeepCalendar.Painting.cs:16-58` — drop the `_usePainterSystem` check; always call `DrawWithPainter`.
- `BeepCalendar.Painting.Pipeline.cs:12 DrawWithPainter` — build a single `ViewPaintArgs` (theme, control style, state, rects, surface, event service, events, categories, resources, fonts, hover, selected, owner, metrics) and call `_viewPainter.Layout(args)` + `_viewPainter.Paint(g, args)`. Header text + view selector + sidebar chrome stay in this method. Telemetry stays.
- `BeepCalendar.Painting.Pipeline.Views.cs:8-34 DrawCalendarViewWithPainter` — REPLACED by direct `_viewPainter.Paint(g, args)` from `DrawWithPainter`. The file is reduced to a no-op or deleted.
- `BeepCalendar.Painting.MonthView.cs:11-66 DrawMonthViewWithPainter` — **DELETED** (logic moved into `MonthViewPainter.Paint`).
- `BeepCalendar.Painting.WeekView.cs:10-91 DrawWeekViewWithPainter` / `DrawTimedWeekViewWithPainter` — **DELETED** (logic moved into `WeekViewPainter` / `WorkWeekViewPainter`).
- `BeepCalendar.Painting.DayView.cs:10-72 DrawDayViewWithPainter` — **DELETED** (logic moved into `DayViewPainter`).
- `BeepCalendar.Painting.ListView.cs:9-37 DrawListViewWithPainter` — **DELETED** (logic moved into `ListViewPainter`; `AgendaViewPainter` shares the event-row draw but builds grouped headers).
- `BeepCalendar.Painting.Sidebar.cs:8-64 DrawSidebarWithPainter` — **DELETED** (chrome stays in `BeepCalendar.Painting.Pipeline.cs`).
- `BeepCalendar.Painting.MonthView.Events.cs:9-30 DrawMonthCellEvents` — **DELETED** (logic moved into `MonthViewPainter.Paint`).
- `BeepCalendar.Painting.MonthView.Headers.cs:9-19 DrawMonthHeaders` — **DELETED** (logic moved into `MonthViewPainter.Paint`).
- `BeepCalendar.Painting.WeekView.Events.cs:9-31 DrawWeekSlotEvents` — **DELETED** (logic moved into `WeekViewPainter.Paint` / `DayViewPainter.Paint`).
- `BeepCalendar.Painting.Pipeline.HeaderFormatting.cs:5 GetHeaderText` — KEEP (used by `BeepCalendar.Painting.Pipeline.cs:12 DrawWithPainter` for chrome header text).
- `BeepCalendar.Painting.Pipeline.Telemetry.cs:7-22` — KEEP (unchanged).
- `BeepCalendar.Painting.DesignTime.cs:14 PaintDesignTimePlaceholder` — KEEP (design-time only; not part of legacy pipeline).
- `BeepCalendar.LayoutTheme.Layout.cs:9-58` — `CalendarSurfaceModel.Build(...)` no longer needs `ICalendarStylePainter` argument; use `CalendarStyleMetrics.For(_calendarStyle).CornerRadius` + `CellPadding` instead of `_stylePainter?.GetMetrics()`.
- `BeepCalendar.LayoutTheme.Helpers.cs:9-23 ApplyThemeTypography` — unchanged; view painter reads resolved fonts from `ViewPaintArgs` (which are populated by `BeepCalendar.ApplyTheme` via `ViewPaintArgs.ApplyThemeFonts()`).
- `BeepCalendar.LayoutTheme.ApplyTheme.cs:7-22 ApplyTheme` — after `ApplyThemeTypography`, call `ViewPaintArgs.ApplyTheme(_currentTheme)` + `ViewPaintArgs.ApplyThemeFonts()` so subsequent `Paint` calls see fresh colors + fonts.
- `BeepCalendar.LayoutTheme.HeaderText.cs:9-23 DrawPainterHeaderText` — REPLACED to consume `ViewPaintArgs` (or simplified to use `args.ForegroundColor` + `args.HeaderFont`).
- `BeepCalendar.Core.Lifecycle.cs:28-47 OnMouseClick` — remove `_renderer.HandleClick(...)` + `new CalendarRenderContext(...)` creation; call `_viewPainter.HitTest(...)` instead.
- `BeepCalendar.Interactions.HitTesting.cs:7-33 ResolveInteractionTarget` — REPLACED by a direct `_viewPainter.HitTest(location, args)` call. The per-view resolver partials are deleted.
- `BeepCalendar.Interactions.HitTesting.MonthView.cs`, `WeekView.cs`, `DayView.cs`, `ListView.cs`, `AgendaView.cs`, `TimelineView.cs`, `TimedView.cs` — **DELETED**. Logic moved into the corresponding `*ViewPainter.HitTest`.
- `BeepCalendar.Interactions.HitTesting.Helpers.cs:10-28 ResolveResizeEdge` — **KEPT** as a `private static` helper called from view painters (or duplicated into a `Helpers/CalendarHitTestHelpers.cs` shared with painters — TBD).
- `BeepCalendar.Interactions.HitTesting.Views.cs` — **DELETED** (empty stub).
- `BeepCalendar.Painting.Views.cs`, `BeepCalendar.Painting.Helpers.cs` — **DELETED** (empty stubs).
- `BeepCalendar.Invalidation.cs`, `BeepCalendar.Commands.cs`, `BeepCalendar.EventOperations.Crud.cs`, `BeepCalendar.EventOperations.Public.cs`, `BeepCalendar.Core.cs`, `BeepCalendar.LayoutTheme.Controls.cs`, `BeepCalendar.LayoutTheme.ResponsiveLabels.cs`, `BeepCalendar.LayoutTheme.ResponsiveLabels.Assignments.cs`, `BeepCalendar.Interactions.Pointer.cs` — **DELETED** (empty / duplicate stubs).

**`ViewPaintArgs` extensions pending**:

- `ApplyTheme(IBeepTheme theme)` — re-resolve `BackgroundColor` / `ForegroundColor` / `BorderColor` / `PrimaryColor` / `SecondaryColor` from `theme.CalendarBackColor` / `CalendarForeColor` / `CalendarBorderColor` / `PrimaryColor` / `BeepTheme.SecondaryColor` when `UseThemeColors` is true; re-populate `Metrics = CalendarStyleMetrics.For(ControlStyle)`.
- `ApplyThemeFonts()` — resolve `HeaderFont` = `BeepThemesManager.ToFont(theme.CalendarTitleFont)`; `DayFont` = `BeepThemesManager.ToFont(theme.DateFont)`; `EventFont` = `BeepThemesManager.ToFont(theme.CalendarSelectedFont)`; `TimeFont` = `BeepThemesManager.ToFont(theme.CalendarUnSelectedFont)`; `DaysHeaderFont` = `BeepThemesManager.ToFont(theme.DaysHeaderFont)`. Fall back to existing fields if `theme` is null or a property is null.
- `CalendarStyleMetrics.For(BeepControlStyle style)` — extend to also resolve the default `IBeepTheme`: call `BeepThemesManager.GetThemeNameForFormStyle(BeepStyling.GetFormStyle(style))` (returns e.g. `"MaterialDesignTheme"` for `Material3`/`Material`/`MaterialYou`; `"MinimalTheme"` for `Minimal`/`NotionMinimal`/`VercelClean`), then `BeepThemesManager.GetTheme(name)`. Use that theme's `CalendarBackColor` etc. to seed the default color palette.

**Build verification**:

`dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" --nologo` must return 0 errors. Warning count should not regress (target ≤ 6996 pre-existing warnings).

### W3 - Editor Layer Infrastructure

New files:

- `Editor/CalendarEditorHost.cs` — owns active `HostedEditor` instances. Exposes `BeginEdit(CalendarEvent, Rectangle)`, `EndEdit(bool commit)`, `HitTest(Point)`, `AddEditor(CalendarEditorDescriptor)`, `RemoveEditor(string id)`, `RaiseEditStarted` / `EditCommitted` / `EditCancelled` events.
- `Editor/CalendarEditorLayer.cs` — `Panel` subclass. Constructor sets `BackColor = Color.Transparent`, calls `SetStyle(ControlStyles.SupportsTransparentBackColor, true)`, overrides `OnPaintBackground` to no-op.
- `Editor/CalendarEditorDescriptor.cs` — record describing a registered editor: `Id`, `DisplayName`, `SupportsInline`, `SupportsDialog`, `Func<HostedEditor>` factory.
- `Editor/HostedEditor.cs` — wraps a `Control` + its `CalendarEditorDescriptor` + bounds + visibility + `BeginEdit(event, rect)` / `EndEdit(commit)` / `IsDirty`.
- `Editor/CalendarEditorPool.cs` — reuses `BeepTextBox` / `BeepCheckBoxBool` / `BeepDateTimePicker` / `BeepComboBox` instances across editors to avoid allocation churn.

`BeepCalendar` new fields in `BeepCalendar.Fields.cs`:

```
private CalendarEditorLayer _editorLayer;
private CalendarEditorHost _editorHost;
```

`BeepCalendar.Core.Constructor.cs:9-39` instantiates both, calls `_editorHost.Initialize(this)`, and adds `_editorLayer` to `Controls` collection (single child, `DockStyle.None`, positioned absolutely via `OnResize`).

`BeepCalendar.cs` adds:

```
protected override bool AllowBaseControlClear => false;
protected override bool IsContainerControl => true;

protected override void OnPaintBackground(PaintEventArgs e)
{
    if (_editorLayer == null || !_editorLayer.Visible)
    {
        base.OnPaintBackground(e);
        return;
    }
    using var clip = new Region(ClientRectangle);
    clip.Exclude(_editorLayer.Bounds);
    var prev = e.Graphics.Clip;
    e.Graphics.Clip = clip;
    try { base.OnPaintBackground(e); }
    finally { e.Graphics.Clip = prev; }
}

protected override void OnPaint(PaintEventArgs e)
{
    if (_editorLayer == null || !_editorLayer.Visible)
    {
        base.OnPaint(e);
        return;
    }
    using var clip = new Region(ClientRectangle);
    clip.Exclude(_editorLayer.Bounds);
    var prev = e.Graphics.Clip;
    e.Graphics.Clip = clip;
    try { base.OnPaint(e); }
    finally { e.Graphics.Clip = prev; }
}
```

`BeepCalendar.Core.Lifecycle.cs:10 OnResize` syncs `_editorLayer.Bounds` from the layout-rect-derived area for the active editor binding (sidebar rect, header text bounds, or event-bar rect).

### W4 - Sample Editors

New files in `Editor/SampleEditors/`:

- `InlineEventTitleEditor.cs` — `BeepTextBox` editing `SelectedEvent.Title` in place; Enter commits, Esc cancels.
- `InlineEventDateRangeEditor.cs` — two `BeepDateTimePicker` instances for `StartTime` / `EndTime`; commit on focus leave, cancel on Esc.
- `InlineAllDayToggleEditor.cs` — `BeepCheckBoxBool` for `IsAllDay`; commit on check.

`BeepCalendar.BeginEdit(CalendarEvent, editorId = "title")` / `EndEdit(bool commit = true)` public API. `OnMouseDoubleClick` in `BeepCalendar.Core.Lifecycle.cs` routes through `TryBeginEditFromDoubleClick` -> `_editorHost.BeginEdit(event, "title")`. Esc on the editor cancels via `ProcessCmdKey`.

### W5 - Verification Demo

A WinForms sample (separate project) that drops a `BeepCalendar`, sets 3 events, and double-clicks one to show a real `BeepTextBox` editing the event title. Screenshot evidence captured for the `phase1-validation-checklist.md` matrix.

XUnit tests in `Beep.Winform.Controls.Tests/Calendar/BeepCalendarTests_EditorLayer.cs` (the directory was deleted during restore and must be recreated):

- `BeginEdit_AddsHostedEditorToLayer`
- `OnPaint_ClipsAroundEditorLayer`
- `OnPaintBackground_ClipsAroundEditorLayer`
- `EndEdit_RaisesEditCommitted`
- `EndEdit_Cancel_RaisesEditCancelled`
- `DoubleClick_SelectedEvent_OpensInlineTitleEditor`

### W6 - Validation Rows

See `phase1-validation-checklist.md` W5/W6 sections for new rows.

## Resolved Decisions (2026-06-04)

1. `BaseControl.ExcludeChildControlsFromClip` — **ignored**. We override `OnPaint` so the flag is irrelevant for our control.
2. `BaseControl.ExcludedPaintRectangles` — **ignored**. We use the new `OnPaint` / `OnPaintBackground` clip in this control.
3. Design-time behavior — **`_editorLayer.Site = null` and `[DesignerSerializationVisibility(Hidden)]`** so the layer + any hosted `BeepTextBox` are not serialized into the host form's `designer.cs`.
4. `BeepComboBox` dropdown — **`ToolStripDropDown` (top-level window) is acceptable** for combo editors hosted in the editor layer.
5. All-day toggle variant — **`BeepCheckBoxBool` (binary only)**. No tri-state needed for the all-day flag.
6. Legacy fate — **Option A: delete legacy entirely**. No `BEEPCALENDAR_LEGACY_RENDERING` compile-time flag; `_usePainterSystem`, `_renderer`, `UsePainterSystem`, and all `*ViewRenderer.cs` files are removed. **Plus**: `ICalendarStylePainter` + `MaterialCalendarPainter` + `MinimalCalendarPainter` are deleted too — they were the wrong abstraction. The per-view `ICalendarViewPainter` is the only painter abstraction.
7. Style variations inside a view painter — **switch on `ViewPaintArgs.ControlStyle`** (Material3 / Material / Minimal / etc.) inside the painter body. There is no separate style painter hierarchy.

## Risks

- **View painter size** — each painter file will be 100-300 lines (with TimelineViewPainter at the top end due to lane virtualization). Mitigation: keep per-view helpers in a `private static class` inside the painter file; share `CalendarDrawingPrimitives` (theme-agnostic helpers) and `CalendarStyleMetrics` across all painters.
- **Theme + font refresh** — every `ApplyTheme` call must rebuild the resolved color palette AND fonts on the next `Paint`. Mitigation: `ViewPaintArgs.ApplyTheme` + `ViewPaintArgs.ApplyThemeFonts` are called from `BeepCalendar.ApplyTheme`; `DrawWithPainter` re-runs `ResolveThemeColors()` + reads `args.HeaderFont` / `args.DayFont` / etc.
- **Icon cache contention** — `StyledImagePainter` is static + thread-safe but the per-call `Paint` may re-tint on every redraw. Mitigation: cache icon paths to painted bitmaps on the painter (e.g. `Dictionary<string, Image>` invalidated on theme change).
- **Region clip perf impact** on large event counts. Mitigation: cache `Region` per resize; only rebuild when `_editorLayer.Bounds` changes.
- **WinForms focus traversal** — `OnMouseDown` must check `_editorHost.HitTest(point)` first and forward to the editor control before invoking the calendar's selection logic. Captured in `BeepCalendar.Interactions.Pointer.DownUp.cs:9 OnMouseDown`.
- **Design-time serialization** of the layer + hosted controls. Mitigation: decision 3 above; if accepted, layer is invisible to the host form designer.
- **Combo dropdown focus** — `ToolStripDropDown` closes on outside click; we must make sure the calendar does not consume the close-click.

## Exit Criteria

- Single paint path: only `_viewPainter.Paint(g, args)` exists; all `ICalendarViewRenderer` / `ICalendarStylePainter` files deleted.
- Single hit-test path: `_viewPainter.HitTest(location, args)`; `_renderer.HandleClick` calls removed; `OnMouseClick` is a thin pass-through.
- Empty stub partial files removed from the project (count drops by 12+).
- `_editorLayer` is a single child of `BeepCalendar.Controls`; `AllowBaseControlClear` and `IsContainerControl` overridden.
- `OnPaint` AND `OnPaintBackground` both apply `Region.Exclude(_editorLayer.Bounds)` clip.
- Double-click on an event opens an inline `BeepTextBox` editing the title; Enter commits; Esc cancels.
- Build remains 0 errors; warning count does not regress.
- XUnit tests for editor layer all pass; manual demo screenshot captured.

## Definition Of Done

- Legacy `ICalendarViewRenderer` path and its callers / files / properties are gone.
- `ICalendarStylePainter` + Material/Minimal painters are gone; `CalendarPainterFactory` is gone.
- Per-view `ICalendarViewPainter` (Month / Week / WorkWeek / Day / Agenda / Timeline / List) consumes `IBeepTheme` + `BeepControlStyle` via `ViewPaintArgs` + `CalendarStyleMetrics.For(style)`.
- Hit-test and paint both consume `CalendarSurfaceModel`.
- Editor layer is a first-class child of `BeepCalendar` with `OnPaint` / `OnPaintBackground` clip.
- `CalendarEditorHost` lifecycle (`BeginEdit` / `EndEdit` / `HitTest`) is wired into mouse, keyboard, and command paths.
- All open questions resolved and the resulting decisions captured in this doc and `TODO-TRACKER.md`.
- Phase 1 validation checklist rows for W5-W6 are `Pass`.
