# Phase 1 Validation Checklist

Status: Ready for execution
Scope: Foundation and core contracts

## Contract And Architecture

- [ ] CalendarTokens are the canonical source for layout constants.
- [ ] CalendarLayoutMetrics maps to CalendarTokens without behavior regressions.
- [ ] BeepCalendar command API routes toolbar and keyboard navigation paths.
- [ ] CommandInvoking cancellation prevents command execution.
- [ ] CommandInvoked fires after successful command execution.
- [ ] Render context exposes interaction state (hover/focus/visible-range).
- [ ] Month rendering uses shared day-cell state helper in both legacy and painter paths.

## Layout And Invalidation

- [ ] BeginVisualUpdate/EndVisualUpdate coalesces multiple layout/invalidate calls.
- [ ] DeferVisualUpdate scope flushes pending layout/redraw once on dispose.
- [ ] Resize path remains stable with no visible jitter.
- [ ] Theme apply path remains stable with no control overlap.

## Core Interaction

- [ ] Month view date select works by click.
- [ ] Keyboard navigation updates focused/selected date correctly.
- [ ] PageUp/PageDown uses command-driven period navigation.
- [ ] View switches (Month/Week/Day/List) keep layout consistent.

## Pipeline Consolidation (W5 - Phase 6)

- [ ] Only one paint path executes: `_viewPainter.Paint(g, args)` from `BeepCalendar.Painting.Pipeline.cs:12 DrawWithPainter`; no call sites to `DrawWithLegacyRenderer`.
- [ ] Per-view `ICalendarViewPainter` files exist in `Rendering/ViewPainters/`: `MonthViewPainter.cs`, `WeekViewPainter.cs`, `WorkWeekViewPainter.cs`, `DayViewPainter.cs`, `AgendaViewPainter.cs`, `TimelineViewPainter.cs`, `ListViewPainter.cs`.
- [ ] `Rendering/ICalendarViewRenderer.cs`, `CalendarRenderer.cs`, `CalendarRenderContext.cs`, `CommonDrawing.cs`, `*ViewRenderer.cs` files are deleted from the project.
- [ ] `Rendering/ICalendarStylePainter.cs`, `StylePainters/MaterialCalendarPainter.cs`, `StylePainters/MinimalCalendarPainter.cs`, `StylePainters/` directory, `CalendarPainterFactory.cs` are deleted from the project (style painter hierarchy is gone).
- [ ] `_usePainterSystem` field, `UsePainterSystem` property, `_stylePainter` field are removed from `BeepCalendar`.
- [ ] `_renderer` field and `_renderer.HandleClick(...)` call in `OnMouseClick` are removed.
- [ ] 12+ empty / stub partial files are deleted (Painting.Views, Painting.Helpers, Invalidation, HitTesting.Views, Pointer, Commands, EventOperations.Crud, EventOperations.Public, Core, LayoutTheme.Controls, LayoutTheme.ResponsiveLabels, LayoutTheme.ResponsiveLabels.Assignments, Painting.DesignTime).
- [ ] `CalendarSurfaceModel` is built once per `UpdateLayout` and consumed by all view painters and view-painter hit-test methods.
- [ ] `ViewPaintArgs` exposes `ApplyTheme(IBeepTheme)` and `ApplyThemeFonts()`; `BeepCalendar.ApplyTheme` calls both; `CalendarStyleMetrics.For(BeepControlStyle)` resolves the default `IBeepTheme` for the style via `BeepThemesManager.GetThemeNameForFormStyle(FormStyle)`.
- [ ] View painters use `StyledImagePainter` + `SvgsUIcons` for icon painting.
- [ ] Month / Week / WorkWeek / Day / Agenda / Timeline / List views produce identical screenshots before and after the consolidation (delta within 1 px).

## Editor Layer (W6 - Phase 6)

- [ ] `Editor/CalendarEditorHost.cs`, `Editor/CalendarEditorLayer.cs`, `Editor/CalendarEditorDescriptor.cs`, `Editor/HostedEditor.cs`, `Editor/CalendarEditorPool.cs` exist in the project.
- [ ] `BeepCalendar` overrides `AllowBaseControlClear => false` and `IsContainerControl => true`.
- [ ] `BeepCalendar` overrides BOTH `OnPaint` and `OnPaintBackground`; each applies `Region.Exclude(_editorLayer.Bounds)` and restores the previous `Graphics.Clip` in a `finally`.
- [ ] `_editorLayer` is a single child of `BeepCalendar.Controls`; its `BackColor` is `Color.Transparent` and `SupportsTransparentBackColor` is enabled.
- [ ] `BeepCalendar.BeginEdit(event, "title")` activates an inline `BeepTextBox` in the editor layer; the painted calendar surface is not over-drawn under the editor.
- [ ] `BeepCalendar.EndEdit(true)` raises `EditCommitted`; `EndEdit(false)` raises `EditCancelled`.
- [ ] Double-clicking an event opens the inline title editor; Enter commits; Esc cancels.
- [ ] The editor does not appear in the host form's `designer.cs` (Site = null + `[DesignerSerializationVisibility(Hidden)]`, or equivalent).
- [ ] XUnit tests in `Beep.Winform.Controls.Tests/Calendar/BeepCalendarTests_EditorLayer.cs` all pass:
  - `BeginEdit_AddsHostedEditorToLayer`
  - `OnPaint_ClipsAroundEditorLayer`
  - `OnPaintBackground_ClipsAroundEditorLayer`
  - `EndEdit_RaisesEditCommitted`
  - `EndEdit_Cancel_RaisesEditCancelled`
  - `DoubleClick_SelectedEvent_OpensInlineTitleEditor`
- [ ] Manual demo screenshot captured: `BeepCalendar` with one event in week view, double-clicked, `BeepTextBox` visible over the event block, typed text appears in the editor.

## Baseline Screenshot Matrix

Capture screenshots for each row and record file paths.

| Scenario | Required Shots | File Path | Status |
|---|---|---|---|
| Month view (Comfortable) | Default + selected date + today |  | Pending |
| Week view (Comfortable) | Empty grid + with events |  | Pending |
| Day view (Comfortable) | Time slots + event blocks |  | Pending |
| List view (Comfortable) | Multiple events list |  | Pending |
| Toolbar compact mode | Width < 720 behavior |  | Pending |
| Theme switch pass | Light/default theme + alternate theme |  | Pending |

## Notes

- Execute this checklist before starting Phase 2 implementation.
- For failed checks, link issue IDs in this file.
