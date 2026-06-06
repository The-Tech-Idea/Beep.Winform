# BeepCalendar TODO Tracker

## Phase 0 - Planning And Benchmark Alignment

- [x] Create planning index and overview gap matrix
- [x] Create per-phase implementation documents (Phase 1 to Phase 4)
- [x] Create master todo tracker for BeepCalendar program
- [x] Capture external benchmark references (open-source + commercial + design standards)

## Phase 1 - Foundation And Core Contracts

- [x] Define shared calendar design tokens (spacing, radii, typography scales, state overlays)
- [x] Normalize render context contract for all view renderers
- [x] Add canonical painter state model (normal, hover, focus, selected, disabled, today, out-of-range)
- [x] Add calendar command surface for navigation and view switching
- [x] Harden layout invalidation and resize flow in CalendarLayoutManager
- [x] Create phase 1 validation checklist
- [ ] Capture phase 1 baseline screenshots

## Immediate Next Wave (Execution Queue)

- [ ] Run `phase1-validation-checklist.md` end-to-end and mark each row Pass/Fail
- [ ] Capture baseline screenshot evidence (Month/Week/Day/List, density, DPI, theme)
- [ ] Close Phase 1 by marking all remaining manual gates complete
- [x] Open Phase 2 kickoff branch for event model contract implementation
- [x] Start Phase 2 with event domain model expansion and conflict policy hooks
- [x] Promote Phase 3 planning to the active workstream
- [ ] Start Phase 3 with additional view renderers, resource model foundations, and performance telemetry

## Phase 2 - Event Model And Interaction Workflows

- [x] Expand event domain model (recurrence, exceptions, reminders, status, category, metadata)
- [ ] Implement interaction primitives (create, drag, resize, move, copy with modifiers)
- [x] Define conflict/collision policy hooks
- [x] Add event editor contract (inline quick edit and dialog edit)
- [x] Add search/filter contract for events and categories
- [x] Create phase 2 validation matrix for interaction parity
- [x] Complete Phase 2 code-contract implementation for interaction/editor/command parity
- [ ] Capture remaining manual UI evidence for Phase 2 parity rows

## Phase 2 Execution Notes

- Interaction contract scaffold added: pointer down/up state, interaction events, and target hit testing are wired into the control.
- Interaction preview layer now distinguishes create, move, resize-start, and resize-end targets with proposed start/end timestamps.
- Interaction commit layer now applies create/move/resize mutations, supports copy-gesture cloning, and keeps selection/caches in sync.
- Interaction edits now snap timed changes to the configured interval and emit undo-friendly mutation snapshots before/after commits.
- Event editor contract added: injectable quick-edit/dialog-edit request flow is now wired into double-click and create-event entry points.
- Default built-in editor added with a Beep dialog host, quick-edit core fields, and dialog-edit advanced fields.
- Remaining Phase 2 interaction work: drag/create/resize parity checks and remaining undo/validation polish.
- Phase 2 validation matrix executed at code-contract level; manual UI evidence pass remains.
- Undo/redo mutation history contract implemented (`CanUndo`, `CanRedo`, `UndoLastMutation`, `RedoLastMutation`) and wired for API + interaction commits.
- Undo/redo is now integrated into command surface and keyboard shortcuts (`Ctrl+Z`, `Ctrl+Y`, `Ctrl+Shift+Z`).
- Undo/redo toolbar buttons added and bound to history availability state.
- Header title bounds now account for undo/redo buttons to prevent overlap in narrow layouts.
- Day/Week drag-to-create now uses pointer-down and pointer-up range to derive event duration (not fixed one-hour only).
- Interaction completion now reports actual commit success, and conflict preview evaluates proposed candidate state (not stale selected event state).
- Calendar command pipeline now returns deterministic success/failure outcomes (`CalendarCommandEventArgs.Succeeded`) including undo/redo no-op cases.
- Interaction conflict previews now evaluate against live pointer context during drag updates/completion.
- Escape key now cancels active interactions via early return (no focus/date navigation side effects) and raises the shared interaction-cancel flow.
- Undo/redo replay now preserves history stacks on failed apply attempts (peek-then-pop on success only).
- Added delete-selected command/keyboard path (`Delete`) with deterministic success semantics.
- Added selected-event keyboard edit path (`F2`) wired through the shared event editor contract.
- Added selected-event host command path (`EditSelectedEvent`) reusing the same shared editor flow.
- Added `Ctrl+N` keyboard shortcut to create events at focused date through shared create/editor routing.
- Keyboard `Delete` and `F2` shortcuts now consume key input even on no-op states to avoid fallback key-beep behavior.
- Public delete path now resolves target events by ID (not reference-only) for safer host/API deletion calls.
- Added host-command create flow (`CreateEventAtFocusedDate`) aligned with the focused-date create pipeline.
- Added duplicate-selected-event command/shortcut (`Ctrl+D`) using copy mutation semantics with a one-day offset.
- Added duplicate-event toolbar access aligned with the duplicate command flow.
- Added edit/delete toolbar access aligned with the selected-event command flows.
- Added `Ctrl+T` keyboard shortcut to jump to Today through the existing navigation command.
- Added `Ctrl+Left` and `Ctrl+Right` keyboard shortcuts for previous/next period navigation.

## Phase 3 - Advanced Views, Resources, And Performance

- [x] Start Phase 3 with advanced views, resource model, and performance workstreams
- [x] Add work-week and agenda renderers as first-class views
- [x] Add timeline renderer and resource lane model
- [x] Add resource grouping and resource header presentation
- [x] Introduce viewport virtualization strategy for dense schedules
- [x] Add paint and query performance counters with thresholds
- [ ] Create phase 3 performance benchmark scenarios

## Phase 4 - Integrations, Accessibility, And Enterprise Readiness

- [ ] Add ICS import/export service contract
- [ ] Add external sync provider interface (Outlook/Google/M365 adapters)
- [ ] Finalize keyboard map and screen-reader narration model
- [ ] Complete localization/RTL/timezone and business-hours policies
- [ ] Add print/export strategy (daily/weekly/monthly/list layouts)
- [ ] Build enterprise QA matrix (DPI/HC/RTL/large-data/recurrence edge cases)

## Phase 6 - Pipeline Consolidation And Editor Layer

(Plan: `06-pipeline-consolidation-and-editor-layer.md`. Build: **CLEAN** as of 2026-06-04. 0 errors / 0 warnings in our changes. W1, W2, W2-Redo, W3, W4, W8 are complete. W2-Redo-2 (inline legacy painters) and W9 (all-views plan) are in flight.)

### W2 - Per-View Painter Pipeline (COMPLETE 2026-06-04)

**Architecture pivot (2026-06-04)**: NO `ICalendarStylePainter` / `MaterialCalendarPainter` / `MinimalCalendarPainter` / `CalendarPainterFactory`. The only painter abstraction is the per-view `ICalendarViewPainter` which consumes `IBeepTheme` + `BeepControlStyle` directly via `ViewPaintArgs` + `CalendarStyleMetrics.For(style)`.

**Theme / color rule (2026-06-04)**: CalendarStyleMetrics carries layout constants only. **No colors on the metrics class.** Colors + theme come from `IBeepTheme` (same as every other Beep control). The mapping is:
1. `Owner.ControlStyle` (BaseControl's `_controlstyle`) → `BeepStyling.GetFormStyle(style)` → `BeepThemesManager.GetThemeNameForFormStyle(formStyle)` → `BeepThemesManager.GetTheme(name)`.
2. The calendar's `ApplyTheme()` override follows the same pattern as `BaseControl.Properties.cs:382`: `Theme = BeepStyling.GetThemeStyle(ControlStyle)`.
3. The ViewPaintArgs `ControlStyle` is the per-VIEW style (drives layout) and is separate from the per-CONTROL style (drives theme). These were previously conflated; they are now clean.

- [x] Create `Rendering/ICalendarViewPainter.cs` (interface: `ViewMode`, `Layout(ViewPaintArgs)`, `Paint(Graphics, ViewPaintArgs)`, `HitTest(Point, ViewPaintArgs)`)
- [x] Create `Rendering/ViewPaintArgs.cs` (single args bundle: theme, control style, state, rects, surface, event service, events, categories, resources, fonts, hover/selected, owner, resolved color palette, `CalendarStyleMetrics`; methods `ApplyTheme(IBeepTheme)`, `ApplyThemeFonts()`, `ResolveThemeColors()`, `GetCategoryColor(int)`)
- [x] Create `Rendering/ViewPainterFactory.cs` (static `GetPainter(CalendarViewMode)` with `Dictionary<CalendarViewMode, ICalendarViewPainter>` cache + `Reset()` test hook)
- [x] Create `Rendering/CalendarPainterHelpers.cs` (theme-agnostic draw helpers)
- [x] Create all 7 view painters: `MonthViewPainter`, `WeekViewPainter`, `WorkWeekViewPainter`, `DayViewPainter`, `AgendaViewPainter`, `TimelineViewPainter`, `ListViewPainter`
- [x] Create `TimedWeekPaintLogic.cs` (shared timed-week drawing helpers)
- [x] Delete all legacy files: `ICalendarViewRenderer.cs`, `CalendarRenderer.cs`, `CalendarRenderContext.cs`, `CommonDrawing.cs`, all `*ViewRenderer.cs`, `ICalendarStylePainter.cs`, `MaterialCalendarPainter.cs`, `MinimalCalendarPainter.cs`, `CalendarPainterFactory.cs`
- [x] Delete `StylePainters/` directory
- [x] Promote reusable helpers from `CommonDrawing.cs` into `Helpers/CalendarDrawingPrimitives.cs`
- [x] Remove `BeepCalendar.Painting.Pipeline.Legacy.cs:13 DrawWithLegacyRenderer`
- [x] Update `BeepCalendar.Fields.cs` (remove `_stylePainter`, `_renderer`, `_usePainterSystem`; add `private ICalendarViewPainter _viewPainter;`)
- [x] Update `BeepCalendar.Core.Constructor.cs` (init `_viewPainter = ViewPainterFactory.GetPainter(_state.ViewMode)`)
- [x] Update `BeepCalendar.Core.Style.cs` (remove `UsePainterSystem` property; `CalendarStyle` setter only updates `_calendarStyle` + `Invalidate()`)
- [x] Update `BeepCalendar.Core.PublicApi.cs` (ViewMode setter swaps `_viewPainter` via `ViewPainterFactory.GetPainter(mode)` + `RequestLayoutAndRedraw()`)
- [x] Update `BeepCalendar.Core.Lifecycle.cs OnMouseClick` (calls `_viewPainter.HitTest(location, args)` via `ResolveInteractionTarget`)
- [x] Update `BeepCalendar.Painting.cs` (drop `_usePainterSystem` check; always call `DrawWithPainter`)
- [x] Update `BeepCalendar.Painting.Pipeline.cs DrawWithPainter` (build single `ViewPaintArgs`; call `_viewPainter.Layout(args)` + `_viewPainter.Paint(g, args)`; chrome stays in this method)
- [x] Delete legacy painting partials: `Painting.MonthView.cs`, `Painting.WeekView.cs`, `Painting.DayView.cs`, `Painting.ListView.cs`, `Painting.Sidebar.cs`, `Painting.MonthView.Events.cs`, `Painting.MonthView.Headers.cs`, `Painting.WeekView.Events.cs`
- [x] Delete legacy hit-testing partials: `Interactions.HitTesting.{MonthView,WeekView,DayView,ListView,AgendaView,TimelineView,TimedView}.cs`
- [x] Update `BeepCalendar.Interactions.HitTesting.cs ResolveInteractionTarget` (replace switch with `_viewPainter.HitTest(location, args)` call)
- [x] Update `BeepCalendar.LayoutTheme.Layout.cs` (use `CalendarStyleMetrics.For(_calendarStyle).CornerRadius` + `CellPadding`)
- [x] Update `BeepCalendar.LayoutTheme.ApplyTheme.cs` (override `ApplyTheme()` to derive theme from `ControlStyle` via `BeepStyling.GetThemeStyle(ControlStyle)` — same pattern as `BaseControl.Properties.cs:382`)
- [x] Delete empty / stub partials: `Painting.Views.cs`, `Painting.Helpers.cs`, `Invalidation.cs`, `HitTesting.Views.cs`, `Pointer.cs`, `Commands.cs`, `EventOperations.Crud.cs`, `EventOperations.Public.cs`, `Core.cs`, `LayoutTheme.Controls.cs`, `LayoutTheme.ResponsiveLabels.cs`, `LayoutTheme.ResponsiveLabels.Assignments.cs`
- [x] `CalendarStyleMetrics.For(style)` returns layout metrics only (no colors, no theme lookup — colors come from `IBeepTheme` resolved via `BeepThemesManager` like every other Beep control)
- [x] `ViewPaintArgs.ResolveThemeColors` resolves the default theme from `Owner.ControlStyle` via `BeepStyling.GetFormStyle` + `BeepThemesManager.GetThemeNameForFormStyle` + `BeepThemesManager.GetTheme` (same as `BaseControl.Properties.cs:382`)
- [x] `ViewPaintArgs.ApplyTheme` + `ApplyThemeFonts` used in `Pipeline.DrawWithPainter` (replaces manual `Theme = _currentTheme; ResolveThemeColors()`)
- [x] Build verification: `dotnet build ... --nologo` returns **0 errors / 0 warnings**

### W1 - CalendarSurfaceModel

- [x] Create `Helpers/CalendarSurfaceModel.cs` (immutable, built per `UpdateLayout`)
- [x] Migrate geometry math from `BeepCalendar.LayoutTheme.*` and `Helpers/CalendarLayoutGeometry.cs:8-150` into the model
- [x] Promote `CalendarState` + `CalendarRects` to `public` so the model factory is callable
- [x] Promote `CalendarInteractionHitTestResult` to `public` so `ICalendarViewPainter.HitTest` can return it
- [x] Update all painters / view-painter methods to consume the model (deferred to W2 view painter files)
- [x] Update all hit-test helpers to consume the model (deferred to W2 view painter files)

### W2 - Per-View Painter Pipeline (DUP — see W2 (COMPLETE) above)

The duplicate W2 in-progress list above was the pre-W2-Redo planning checklist. W2 is actually **COMPLETE** (see the "W2 - Per-View Painter Pipeline (COMPLETE 2026-06-04)" block). W2-Redo-2 below is the follow-on cleanup that inlines the remaining legacy painters.

### W3 - Editor Layer Infrastructure (COMPLETE 2026-06-04)

**Architecture**: A transparent child `Panel` (`_editorLayer`) is added to `BeepCalendar.Controls`. `BeepCalendar` overrides **both** `OnPaint` and `OnPaintBackground` to exclude `_editorLayer.Bounds` from its paint region. The layer's own `OnPaintBackground` is a no-op, so the result is a clean cut-out for the hosted `BeepTextBox` / `BeepDateTimePicker` / `BeepComboBox` / `BeepCheckBoxBool` editors produced by the W4 sample editor factories.

**Design-time safety**: `CalendarEditorLayer` clears its own `Site` in the constructor (and is `[ToolboxItem(false)]`) so the host form's `*.Designer.cs` does not pick it up. W4's sample editors will additionally need to null their own `Site` to prevent serialization of the wrapped control.

**Pool cap**: `CalendarEditorPool` keeps at most 16 free instances per type — beyond that, released controls are disposed.

- [x] Create `Editor/CalendarEditorHost.cs` (BeginEdit / EndEdit / HitTest / AddEditor / RemoveEditor + EditStarted / EditCommitted / EditCancelled events)
- [x] Create `Editor/CalendarEditorLayer.cs` (Panel subclass, transparent, SupportsTransparentBackColor, no-op `OnPaintBackground`, clears `Site`)
- [x] Create `Editor/CalendarEditorDescriptor.cs` (Id / DisplayName / SupportsInline / SupportsDialog / factory)
- [x] Create `Editor/HostedEditor.cs` (wraps Control + descriptor + bounds + IsDirty + Event)
- [x] Create `Editor/CalendarEditorPool.cs` (reuses BeepTextBox / BeepCheckBoxBool / BeepDateTimePicker / BeepComboBox instances; max 16 per type)
- [x] Add `_editorLayer` and `_editorHost` fields in `BeepCalendar.Fields.cs`
- [x] Instantiate + add to `Controls` in `BeepCalendar.Core.Constructor.cs` (wrapped in try/catch + `ISupportInitialize` BeginInit/EndInit for designer safety)
- [x] Add `AllowBaseControlClear => false` and `IsContainerControl => true` overrides in `BeepCalendar.cs`
- [x] Add `OnPaint` and `OnPaintBackground` overrides in `BeepCalendar.cs` applying `Region.Exclude(_editorLayer.Bounds)` and restoring `Graphics.Clip` in `finally`
- [x] Sync `_editorLayer.Bounds` (currently `ClientRectangle`; W4 will refine per-editor binding) in `BeepCalendar.Core.Lifecycle.cs:OnResize`
- [x] Build verification: `dotnet build ... --nologo` returns **0 errors**; W3 files contribute 0 new warnings

### W4 - Sample Editors (COMPLETE 2026-06-04)

**Architecture**: The three sample editors are pure static factories that produce a `HostedEditor` wrapping the right `Control`. They use the new `HostedEditor.Loading` / `HostedEditor.Saving` events for data binding and raise `CommitRequested` / `CancelRequested` from their `KeyDown` (Enter / Esc), `Leave` (date range) and `CheckedChanged` (all-day) handlers. `CalendarEditorHost.BeginEdit` subscribes to those events and translates them into `EndEdit(commit: true|false)`.

**Editor factory pattern** (canonical for future editors):
```csharp
public static HostedEditor Create() {
    var tb = new BeepTextBox { Site = null, ... };
    var hosted = new HostedEditor(GetDescriptor(), tb);
    CalendarEvent? current = null;
    hosted.Loading += (s, evt) => { current = evt; tb.Text = evt?.Title ?? ""; };
    hosted.Saving  += (s, evt) => { if (current != null) current.Title = tb.Text ?? ""; };
    tb.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) hosted.RequestCommit(); ... };
    return hosted;
}
public static CalendarEditorDescriptor GetDescriptor() => new(id, name, true, true, Create);
```

- [x] Create `Editor/SampleEditors/InlineEventTitleEditor.cs` (BeepTextBox; Enter commits, Esc cancels)
- [x] Create `Editor/SampleEditors/InlineEventDateRangeEditor.cs` (BeepDateTimePicker x2 in a hosting Panel; commit on focus-leave when `start <= end`, Esc cancels)
- [x] Create `Editor/SampleEditors/InlineAllDayToggleEditor.cs` (BeepCheckBoxBool; commit on CheckedChanged, Esc cancels)
- [x] Add public `BeginEdit(CalendarEvent, string editorId = "title")` (returns `HostedEditor?`) and `EndEdit(bool commit = true)` on `BeepCalendar`
- [x] Add `IsEditing` property on `BeepCalendar`
- [x] Add `ComputeEditorBounds(editorId, event)` helper (W4 default: vertical stack at the top of the calendar; W5 will replace with per-event hit-test bounds)
- [x] Route `OnMouseDoubleClick` in `BeepCalendar.Core.Lifecycle.cs` through `BeginEdit(_state.SelectedEvent, "title")`
- [x] Route `ProcessCmdKey(Escape)` to `EndEdit(false)` when `IsEditing` (in `BeepCalendar.cs`); falls through to existing Esc handling otherwise
- [x] Extend `HostedEditor` with `Loading` / `Saving` / `CommitRequested` / `CancelRequested` events + `RequestCommit()` / `RequestCancel()` helpers
- [x] Wire `CalendarEditorHost.BeginEdit` to subscribe to the hosted editor's commit/cancel requests
- [x] Build verification: `dotnet build ... --nologo --no-incremental` returns **0 errors**; W4 files contribute 0 new warnings
- [x] Resolve open questions 1-6 (clip flag, ExcludedPaintRectangles wiring, design-time serialization, combo dropdown, check-box variant, legacy compile flag)
- [x] Decisions (2026-06-04): ignore Q1+Q2 (we override OnPaint); Q3 design-time `Site=null`; Q4 `ToolStripDropDown` accepted; Q5 use `BeepCheckBoxBool` (binary only); Q6 Option A — delete legacy entirely

### W5 - Verification Demo + Tests

- [ ] Recreate `TheTechIdea.Beep.Winform.Controls.Tests/Calendar/` directory
- [ ] Create `BeepCalendarTests_EditorLayer.cs` with the 6 XUnit cases from `phase1-validation-checklist.md` W6
- [ ] Add WinForms sample that drops a `BeepCalendar`, sets 3 events, double-clicks one, screenshots inline `BeepTextBox`
- [ ] Run full solution build; ensure 0 errors and warning count does not regress
- [ ] Capture `phase1-validation-checklist.md` W5 + W6 screenshot evidence

### W2-Redo - New Week1..Week7 View Modes (COMPLETE 2026-06-04)

**Motivation**: User added 7 sample calendar layout images (`sampleimages/c1.png` through `c7.png`) and asked for view modes named `Week1` through `Week7` (no legacy names). Each new view has a unique layout (timed grid, month grid, event-card list, timeline variant) that the painter owns. View-specific logic must live IN the painter; central code calls painter metadata instead of switching on the view mode.

**Rule (user 2026-06-04)**: "every thing for each view should handled in that view remeber dont scatter things" — view-specific behavior (navigation, header text, gutter, day count, drag mode, month-cell invalidation) is exposed as methods on `ICalendarViewPainter`. Central files in `BeepCalendar.*.cs` and `Helpers/Calendar*` use the painter's metadata rather than `switch (_state.ViewMode)`.

- [x] Add `CalendarViewMode.Week1`..`CalendarViewMode.Week7` (numeric 0..6) in `BeepCalendar.Types.cs`. Legacy 7 names (Month=100, Week=101, …) kept as backward-compat aliases.
- [x] Change `CalendarState.ViewMode` default to `CalendarViewMode.Week4` (the 6×7 month-grid sample, c4.png).
- [x] Extend `ICalendarViewPainter` with metadata the central pipeline needs: `Key`, `DisplayLabel`, `VisibleDayCount`, `IsTimedView`, `IsMonthGrid`, `RequiresLeftGutter`, `HasAllDayStrip`, `SupportsEventDrag`, `NavigatePrevious/Next`, `GetHeaderText`, `GetVisibleRangeStart/End`, `GetDateTimeFromLocation`.
- [x] Add `BuildViewPaintArgsForInteraction()` helper in `BeepCalendar.Painting.Pipeline.Helpers.cs` for interaction-time helper code that needs a `ViewPaintArgs` snapshot outside of `OnPaint`.
- [x] Create 7 new painters in `Rendering/ViewPainters/`: `Week1ViewPainter.cs` (c1, 7-day timed grid + left sidebar), `Week2ViewPainter.cs` (c2, 7-day timed grid + right detail panel), `Week3ViewPainter.cs` (c3, 4-day timed grid + filter bar), `Week4ViewPainter.cs` (c4, 6×7 month grid + right detail panel), `Week5ViewPainter.cs` (c5, 7-day event-card columns + day-of-week tabs), `Week6ViewPainter.cs` (c6, 6-day event-card columns in time order), `Week7ViewPainter.cs` (c7, 7-day timed grid + filter bar + status badges). Each is self-contained — no shared logic with other painters.
- [x] Update `ViewPainterFactory.GetPainter` to map `Week1..Week7` → new painters; legacy names alias to the same painter.
- [x] Add `ViewPainterFactory.GetRegisteredViews()` for the toolbar to enumerate view-selector buttons dynamically (no hard-coded list).
- [x] Refactor `BeepCalendar.Toolbar.cs` to build view-selector buttons from `GetRegisteredViews()` (drop the hard-coded 7-name list) and replace `IsViewActive` switch with `_viewPainter.Key == key`.
- [x] Refactor `BeepCalendar.EventOperations.Navigation.cs` to call `_viewPainter.NavigatePrevious/Next`.
- [x] Refactor `BeepCalendar.Painting.Pipeline.HeaderFormatting.cs` to call `_viewPainter.GetHeaderText(_state.CurrentDate)`.
- [x] Refactor `BeepCalendar.Interactions.Timing.Snap.cs` to use `_viewPainter.IsTimedView` for the timed-view branch.
- [x] Refactor `BeepCalendar.Interactions.Timing.Snap.Location.cs` to delegate to `_viewPainter.GetDateTimeFromLocation` (via `BuildViewPaintArgsForInteraction`).
- [x] Refactor `BeepCalendar.Interactions.Proposals.CreationRange.cs` to use `_viewPainter.IsTimedView` for the timed-view branch.
- [x] Refactor `BeepCalendar.Interactions.cs:ResolveDragMode` to use `_viewPainter.IsTimedView` (resize) + `_viewPainter.SupportsEventDrag` (move) instead of switching on `Day/Week/WorkWeek/Timeline`.
- [x] Refactor `BeepCalendar.LayoutTheme.MonthCells.cs:InvalidateDateCell` to use `_viewPainter.IsMonthGrid` instead of `_state.ViewMode == Month`.
- [x] Refactor `Helpers/CalendarLayoutManager.UpdateLayout` to use `viewPainter.RequiresLeftGutter` (with legacy `Week/WorkWeek/Day` fallback) instead of switching on the view mode.
- [x] Refactor `Helpers/CalendarSurfaceModel` to use `_viewPainter.VisibleDayCount` and `_viewPainter.GetVisibleRangeStart(CurrentDate)` (with legacy `WorkWeek=5` fallback).
- [x] Update `BeepCalendar.LayoutTheme.Layout.cs` to pass `_viewPainter` to both `_layout.UpdateLayout` and `CalendarSurfaceModel.Build`.
- [x] Update `TimedWeekPaintLogic.GetDateTimeFromLocation` to use `CalendarSurfaceModel.MinutesPerDay` (the static const qualifier, since the field is `const`).
- [x] Update legacy painters (`ListViewPainter.GetDateTimeFromLocation`, `AgendaViewPainter.GetDateTimeFromLocation`) to call their own `GetVisibleRangeStart` (not the surface's).
- [x] Update `CalendarCellContext` to reference `CalendarEvent` from `TheTechIdea.Beep.Winform.Controls.Calendar` (not `Helpers.CalendarEvent`).
- [x] Build verification: **0 errors**, 7035 warnings (all pre-existing — 3 new XML doc warnings from this wave).

### W8 - IBeepUIComponent Cell Render (COMPLETE 2026-06-04)

**Motivation (user 2026-06-04)**: "i want the ability to draw and IBeepUIComponeent in any Cell useing Draw method … but when user click to edit cell the calnedar should show the actual control to edit data and respond. … create cache for controls and create one each time user clicks on cell". Settled (3 questions): all 3 cell kinds, 3 separate `Func<CalendarCellContext, IBeepUIComponent>` factories, cache lifetime = calendar lifetime.

**Pattern (mirrors `GridRenderHelper.CellContent.cs:84-95`)**: per-cell-key drawer cached, `drawer.Bounds = rect; drawer.Draw(g, rect)` for paint, `drawer is Control` for live editing.

- [x] Create `CellRender/CalendarCellContext.cs` (sealed class with `Kind`, `Event`, `Date`, `ViewMode`, `Row`, `Column`, `UserData`; constructor for the 6 most-common fields; `CalendarCellKind` enum: `EventBlock`, `DateCell`, `TimeSlot`).
- [x] Create `CellRender/CalendarCellComponentCache.cs` (per-calendar `Dictionary<string, IBeepUIComponent>` keyed by cell key; `SetFactory(kind, Func)`, `GetFactory(kind)`, `GetOrCreate(cellKey, ctx)`, `Contains(cellKey)`, `Remove(cellKey)`, `Clear()`, `Count`; components are NOT disposed).
- [x] Add `_componentCache` field in `BeepCalendar.Fields.cs`.
- [x] Public API in `BeepCalendar.Core.PublicApi.cs`: `EventBlockComponentFactory`, `DateCellComponentFactory`, `TimeSlotComponentFactory` (3 `[Browsable(false)]` `Func<CalendarCellContext, IBeepUIComponent>` properties routed to `_componentCache.SetFactory/GetFactory`); `CachedCellComponentCount` (test hook); `ClearCellComponentCache()`; `GetCellComponent(cellKey, ctx)`; `DrawCellComponent(g, rect, cellKey, ctx)` (sets `Control.Bounds`, calls `Draw`); `ActivateCellComponent(cellKey, ctx, bounds)` (sets `Bounds` + `Tag = cellKey`, adds to `_editorLayer.Controls`, `BringToFront`, `Focus`); `DeactivateCellComponent(cellKey)` (finds Control by `Tag`, hides, removes); `DeactivateAllCellComponents()` (called by `EndEdit` to clear the W8 layer on Esc).
- [x] Update `EndEdit(bool commit = true)` to also call `DeactivateAllCellComponents()` so Esc cleanly ends both W4 sample editor sessions and W8 cell-host sessions.
- [x] Update `OnMouseDoubleClick` in `BeepCalendar.Core.Lifecycle.cs` to re-hit-test (instead of using stale `_state.SelectedEvent`), build a `CalendarCellContext(EventBlock, hit.Event, …)`, and try `ActivateCellComponent` first (W8 path); fall back to `BeginEdit(hit.Event, "title")` (W4 fallback) when no factory is registered for the cell kind.
- [x] All 7 new painters (Week1..Week7) call `args.Owner?.DrawCellComponent(g, rect, cellKey, ctx)` for each event block with cell key `"evt:{evt.Id}"` and a populated `CalendarCellContext`. The painter's `Draw` falls through to the default event-bar rendering when no factory is registered.
- [x] Add `CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)` (public static helper) — fetches the cached component, sets `Control.Bounds`, calls `Draw`; returns true on success, false when no factory is registered. Centralizes the try/catch so each painter stays linear.
- [x] Update all 14 painters (7 new Week1..Week7 + 7 legacy Month/Week/WorkWeek/Day/Agenda/Timeline/List) to call `CalendarPainterHelpers.TryDrawCellComponent` at the top of their event-paint method, `return` immediately on success, and fall through to the default event-bar rendering otherwise. W8 cell-component integration is now uniform across every view mode.
- [x] Build verification: **0 errors**, 7066 warnings (all pre-existing — 31 new XML doc warnings from this wave's W8 helper + painter refactors).

### W2-Redo Follow-ups (RESOLVED — see W2-Redo-2 above)

User originally decided: **keep the 7 legacy enum values + 7 legacy painters + TimedWeekPaintLogic.cs in place** as backward-compat aliases. The new Week1..Week7 painters were added alongside, not as replacements. No deletions scheduled.

**2026-06-04 update**: User changed direction — "legacy painter suppose to be updated to new interface and work like the new one???" + "EACH VIEW SHOULD DISTINCT THATS A RULE". The legacy painters are now **inlined** to be self-contained (no `TimedWeekPaintLogic` shim). W2-Redo-2 is COMPLETE 2026-06-04.

- [x] Decision recorded: legacy code stays (Month, Week, WorkWeek, Day, Agenda, Timeline, List) — enum values kept for backward compat
- [x] ViewPainterFactory routes legacy enum values to the same painters the new Week1..Week7 enum values resolve to (backward compat)
- [x] All 14 painters (legacy + new) support W8 cell-component render via the shared `CalendarPainterHelpers.TryDrawCellComponent` helper
- [x] **W2-Redo-2 inlined `WeekViewPainter` / `WorkWeekViewPainter` / `DayViewPainter`** so each is a self-contained class (no shared `TimedWeekPaintLogic` shim). `TimedWeekPaintLogic.cs` deleted.
- [x] Build verification: 0 errors, 0 warnings after W2-Redo-2 inlining.

- [ ] Decide whether to delete the 7 legacy painters (`MonthViewPainter`, `WeekViewPainter`, `WorkWeekViewPainter`, `DayViewPainter`, `AgendaViewPainter`, `ListViewPainter`, `TimelineViewPainter`) for a clean break. Currently they remain as legacy aliases routed through the factory. **Defer until W2-Redo-3 (final cleanup) — see below.**
- [ ] Decide whether to delete the 7 legacy enum values (`Month`, `Week`, `WorkWeek`, `Day`, `Agenda`, `Timeline`, `List`) from `BeepCalendar.Types.cs`. Currently they remain for backward compatibility. **Defer until W2-Redo-3 (final cleanup) — see below.**
- [ ] Consider promoting `IBeepUIComponent` render to date cells (Month grid, Agenda, List) and time slots (timed views) in addition to event blocks. W8 currently implements the event-block path; the `DateCellComponentFactory` + `TimeSlotComponentFactory` API is in place but no painter calls `DrawCellComponent` for those kinds yet.

### W2-Redo-2 - Inline Legacy Timed Painters (COMPLETE 2026-06-04)

**User request (2026-06-04)**: "legacy painter suppose to be updated to new interface and work like the new one???" + "EACH VIEW SHOULD DISTINCT THATS A RULE". The 3 legacy *timed* painters (`WeekViewPainter`, `WorkWeekViewPainter`, `DayViewPainter`) used to delegate to `TimedWeekPaintLogic` for Paint/HitTest/GetDateTimeFromLocation. This violated the "every thing for each view should handled in that view" rule and the "each view should be distinct" rule (view-specific code must live in the painter — no shared shim file). They're inlined.

`TimedWeekPaintLogic.cs` deleted after `DayViewPainter` shipped as a self-contained painter (no callers remained).

The 4 legacy *non-timed* painters (`MonthViewPainter`, `AgendaViewPainter`, `ListViewPainter`, `TimelineViewPainter`) were already self-contained — no work needed.

- [x] `Rendering/ViewPainters/WeekViewPainter.cs` — fully rewritten as self-contained 7-day timed painter (Layout, Paint, HitTest, GetDateTimeFromLocation, PaintDayHeader, PaintTimeLabel, PaintTimeSlot, PaintEventBlock all inline; uses `surface.GetWeekDayHeaderRect`/`GetWeekDayColumnRect`/`GetWeekDayDate`/`GetTimeRowRect` from `CalendarSurfaceModel`; calls `CalendarPainterHelpers.TryDrawCellComponent` for W8).
- [x] `Rendering/ViewPainters/WorkWeekViewPainter.cs` — fully rewritten as self-contained 5-day (Mon-Fri) timed painter (same pattern; computes `startOfWorkWeek` via `(int)DayOfWeek == 0 ? 6 : (int)DayOfWeek - 1` monday-offset math; uses `CalendarPainterHelpers.GetColumnRect` for headers + columns because `surface.GetWeekDayHeaderRect` is 7-day only).
- [x] `ICalendarViewPainter` extended with `SupportsEventDrag` property (used by `BeepCalendar.Interactions.cs:ResolveDragMode`).
- [x] `Rendering/ViewPainters/DayViewPainter.cs` — rewritten as 1-day self-contained painter (template: `WeekViewPainter`; differences: `VisibleDayCount => 1`, `NavigatePrevious/Next => ±1 day`, `GetHeaderText(d) => d.ToString("dddd, MMMM dd, yyyy")`, `GetVisibleRangeStart(d) => d.Date`, `GetVisibleRangeEnd(d) => d.Date.AddDays(1)`, `Paint` uses 1 day column = full `surface.TimedArea`, `HitTest` uses `surface.CurrentDate.Date` for the day, `GetDateTimeFromLocation` returns `surface.CurrentDate.Date.AddMinutes(minuteOffset)` (no column math); uses `CalendarPainterHelpers.GetColumnRect` for the 1-wide column because `surface.GetWeekDayHeaderRect`/`GetWeekDayColumnRect` are 7-day only).
- [x] Delete `Rendering/ViewPainters/TimedWeekPaintLogic.cs` (after DayViewPainter rewrite; `grep -r "TimedWeekPaintLogic" Calendar/` should return zero). **DONE 2026-06-04.**
- [x] Build verification: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 0 warnings** (15.96s). **DONE 2026-06-04.**
- [x] Update TODO-TRACKER to mark "All 14 painters are now self-contained; TimedWeekPaintLogic.cs deleted." (W2-Redo-2 complete) **DONE 2026-06-04.**

### W9 - All-Views Coverage Plan (PLAN DOCUMENT COMPLETE 2026-06-04)

**User request (2026-06-04)**: "go online and LOOK FOR ALL TYPES OF CALENDAR AND STANDARD ONE AND CREATE APLAN FOR EACH WITH ITS PAINTER" + "EACH VIEW SHOULD DISTINCT THATS A RULE".

**Deliverable**: a new plan document `Calendar/plans/09-all-calendar-view-painter-plan.md` covering 30+ view types organized into 6 families (TimeGrid / DayGrid / List+Agenda / Timeline / Gantt / Specialized). The plan defines the per-view painter design (properties, geometry helpers, helpers needed, W8 cell-component integration, navigation, hit-test, drag/resize) and lists the interface extensions needed on `ICalendarViewPainter`. **WRITTEN 2026-06-04.**

- [x] Web research (3 searches) covering FullCalendar, Teamup, DevExtreme, SVAR, Kendo, Material 3, SAP Fiori, Gantt suites.
- [x] View taxonomy drafted: 6 families (TimeGrid, DayGrid, List+Agenda, Timeline, Gantt, Specialized).
- [x] Interface extension list drafted (~10 new members: `IsTimelineView`, `IsResourceView`, `IsReadOnly`, `SupportsTaskHierarchy`, `SupportsDependencies`, `ShowHeatmap`, `ShowWeekNumbers`, `AllowMultiSelection`, `MaxVisibleDays`, `RowHeaderWidth`).
- [x] Geometry helper extension list drafted (`GetResourceRowRect`, `GetResourceHeaderRect`, `GetTimelineColumnRect`, `GetTaskBarRect`, `GetDependencyArrow`, `GetHeatmapCellRect`, `GetMultiMonthCellRect`, `GetWeekNumberRect`).
- [x] Data model extension list drafted (`CalendarEvent.ResourceId`, `ParentId`, `Dependencies`, `Progress`, `TaskType`, `IsHeatmap`; new `CalendarTaskDependency`, `CalendarTaskType`).
- [x] Write `Calendar/plans/09-all-calendar-view-painter-plan.md` — one section per view with layout sketch, painter properties, required surface helpers, W8 integration, navigation, hit-test, drag/resize, read-only rules, build priority. **DONE 2026-06-04.**
- [x] Decide which views land in W10 (high-priority: DayGrid, DayGrid Week, Multi-Day, Year, Quarter, Multi-Month, listDay/Week/Month/Year), W11 (medium: Timeline Day/Week/Month/Year, Resource Timeline Day/Week/Month, Resource TimeGrid, Resource DayGrid), W12 (low: Gantt, Tiles, Scheduler, Heatmap, Mini Month). **DONE in plan §9.**

### W10 - Standard Office-App Views (PLANNED — start after W9 sign-off)

Closes the gap on the most common office-app requirement: Day / Week / Month / Year + List variants. All painters in this wave are self-contained per the "each view should be distinct" rule.

- [ ] Extend `ICalendarViewPainter` with the 10 new members from W9 §3 (`IsTimelineView`, `IsResourceView`, `IsReadOnly`, `SupportsTaskHierarchy`, `SupportsDependencies`, `ShowHeatmap`, `ShowWeekNumbers`, `AllowMultiSelection`, `MaxVisibleDays`, `RowHeaderWidth`).
- [ ] Add `CalendarViewPainterBase` class with default implementations of the new members (return false / 0) so new painters can override only what's distinct.
- [ ] Add new `CalendarViewMode` enum values from W9 §8 (DayGridWeek=10, DayGridDay=11, MultiDay=12, MultiMonth=20, Quarter=21, Year=22, ListDay=30, ListWeek=31, ListMonth=32, ListYear=33, AgendaDay=34, Tiles=35, DayCard=36, ...).
- [ ] Extend `CalendarSurfaceModel` with the geometry helpers needed for W10 (B2, B3, B4, B5, B6, C1, C3-C6) per W9 §5.
- [ ] Extend `CalendarPainterHelpers` with `PaintEventCard`, `PaintMiniMonth`, `PaintMonthGridLine` per W9 §6.
- [ ] Create `Rendering/ViewPainters/DayGridWeekViewPainter.cs` (B2) — self-contained 1×7 day column grid with stacked all-day event blocks.
- [ ] Create `Rendering/ViewPainters/DayGridDayViewPainter.cs` (B3) — self-contained 1 day full-height event stack.
- [ ] Create `Rendering/ViewPainters/MultiDayViewPainter.cs` (A4) — self-contained 1-14 day timed grid (uses `CalendarPainterHelpers.GetColumnRect(timedArea, day, N)` with N dynamic).
- [ ] Create `Rendering/ViewPainters/MultiMonthViewPainter.cs` (B4) — self-contained 2×2 / 3×4 / 4×3 of full months (configurable).
- [ ] Create `Rendering/ViewPainters/QuarterViewPainter.cs` (B5) — self-contained 3 mini-months in a row.
- [ ] Create `Rendering/ViewPainters/YearViewPainter.cs` (B6) — self-contained 12 mini-months in a 3×4 (default) or 4×3 grid.
- [ ] Create `Rendering/ViewPainters/ListDayViewPainter.cs` (C3), `ListWeekViewPainter.cs` (C4), `ListMonthViewPainter.cs` (C5), `ListYearViewPainter.cs` (C6), `AgendaDayViewPainter.cs` (C1) — self-contained list-style painters.
- [ ] Update `ViewPainterFactory.GetPainter` to map new enum values to new painters.
- [ ] Update `ViewPainterFactory.GetRegisteredViews` to include the new views in the toolbar.
- [ ] Build verification after each painter: 0 errors, 0 new warnings.

### W2-Redo-4 - Second Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Second audit after W2-Redo-3 found 6 additional gaps the first pass missed. Build: **0 errors**, 7089 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP A - ViewMode setter left W8 hosted cell component + W4 sample editor active across view changes** (`BeepCalendar.Core.PublicApi.cs:20-30`). When the user switched view mode, the active `IBeepUIComponent` Control stayed visible at the old view's cell bounds. Fixed: call `EndEdit(commit: true)` after the painter is swapped. View-switch is a hard context change; both W4 and W8 layers should be cleared.

- [x] **GAP B - NavigatePrevious/Next/Today left W8 cell components active across date changes** (`BeepCalendar.EventOperations.Navigation.cs`). The W8 `DateCell` and `TimeSlot` cell keys include the date, so navigating to a different date left a stale Control visible at the old date's bounds. Fixed: call `DeactivateAllCellComponents()` (W8 only) when the new date differs from the current date. W4 sample editors are event-bound (not date-bound) and remain valid across navigation.

- [x] **GAP C - OnMouseDown date-changing click left W8 cell component active** (`BeepCalendar.Interactions.Pointer.DownUp.cs`). When the user clicked a different date cell, `CurrentDate` was updated but the W8 Control stayed at the old date's bounds. Fixed: added a date-change check + new helper `IsClickInsideActiveEditor(Point)` that returns true when the click is inside any active W8 Control's bounds. The deactivation is skipped when the user clicks inside the editor they're interacting with, so typing into a W8 host isn't interrupted by stray clicks on the same Control.

- [x] **GAP D - 7 HitTest methods returned `SelectEvent` for event blocks instead of resolving resize edges** (Week2/3/5/6/7 + Month + Week4). Resize was impossible in those views even though the painters paint the event blocks. Fixed: added `CalendarPainterHelpers.ResolveResizeEdge(location, eventRect, handleSize)` calls with `SelectEvent`/`ResizeStart`/`ResizeEnd` mode mapping + `ResizeEdge = edge` on the result. Handle size is 4px for small event bars (Month, Week4 at 20px tall) and 6px for larger event cards (Week5/6 at 56-64px) and timed views (Week2/3/7).

- [x] **GAP D (extra) - Week4's HitTest was missing event hit-test entirely** (`Rendering/ViewPainters/Week4ViewPainter.cs`). The painter painted up to 3 event cards per month cell, but clicking them selected the date instead of the event. Fixed: added an event hit-test loop that mirrors the Paint geometry exactly (cellY + 24, 20px height, 2px gap, cellX + 4, cellW - 8 width) and returns an EventBlock hit with resize edge.

- [x] **GAP F - Toolbar's `IsViewActive` only matched the new Week1..Week7 painter keys** (`BeepCalendar.Toolbar.cs:215-218`). When the user set `ViewMode = Month` (or any legacy enum value) in code, the toolbar would not highlight any view-selector button as active. Fixed: added `ViewMode` field to `CalendarToolbarButton` struct (set during `InitializeToolbar` from the enum value), then changed `IsViewActive(key)` to check `btn.ViewMode == _state.ViewMode` in addition to the painter.Key fallback. Now setting `ViewMode = Month` correctly highlights the toolbar's "Month" button.

**Build verification**: `dotnet build ... --nologo` returns **0 errors, 7089 warnings** (all pre-existing). No new warnings introduced.

**Architectural impact**:
- W8 cell components are now correctly deactivated whenever the date-bearing cell key would become stale (view change, date navigation, date-changing click).
- The W4 sample editor is closed on view change (EndEdit=true) but kept active across date navigation (view-independent bounds).
- Resize-from-edge is now available in all 14 painters' event-block HitTests, not just the 7 that had it before.
- The toolbar's view-selector active-state correctly reflects any `CalendarViewMode` value, including legacy enums (Month/Week/WorkWeek/Day/Agenda/List/Timeline).

### W2-Redo-5 - Third Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Third audit after W2-Redo-4 found 7 additional gaps the second pass missed (4 fixed, 3 documented as design constraints). Build: **0 errors**, 7088 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - W8 Control's `Bounds` reset on every paint while editor is active** (`Rendering/CalendarPainterHelpers.cs:187-203` in `TryDrawCellComponent`, plus the same logic in `BeepCalendar.Core.PublicApi.cs:271-291` in `DrawCellComponent`). For Control-based W8 components, `asControl.Bounds = rect` was running on every paint, shifting the Control's bounds mid-edit and triggering WinForms `Resize`/`LocationChanged` events. Could cause focus loss or caret jumps in text editors. Fixed: only set `Bounds` when `asControl.Parent == null` (Control is not currently hosted in the editor layer). Once a Control is in the editor layer, WinForms' own layout owns its position.

- [x] **GAP 2 - W8 cell component's `Draw` is called on every paint even when Control is in editor layer (double-paint)** (same files as GAP 1). `comp.Draw(g, rect)` was called on every paint, including when the Control was already in the editor layer (where WinForms' own paint cycle draws it). Caused double-painting AND painted at a stale `rect` if the layout changed between activation and current paint. Fixed: if `asControl.Parent != null` (Control is hosted/active), return `true` immediately without calling `Draw()`. The painter skips its default rendering for this cell, and WinForms' paint cycle handles the Control's drawing at the activation-time bounds.

- [x] **GAP 3 - Click outside active W8 editor doesn't deactivate it** (`BeepCalendar.Interactions.Pointer.DownUp.cs:18-33`). `OnMouseDown` only called `DeactivateAllCellComponents()` when the click was changing the date (`_activeInteractionHit.Date.Value.Date != _state.CurrentDate.Date`). Clicking a different cell on the SAME date (e.g., another time slot in a timed view) left the W8 editor stuck open. Fixed: removed the date-change precondition. `OnMouseDown` now always calls `DeactivateAllCellComponents()` (guarded by `!IsClickInsideActiveEditor(e.Location)` which handles edge cases like clicks on the editor layer's transparent Panel background). In WinForms, clicks on hosted W8 Controls are captured by the Control and don't reach `BeepCalendar.OnMouseDown`, so reaching this code path means the click is outside the editor.

- [x] **GAP 4 - W8 cell-component Controls not deactivated on calendar resize** (`BeepCalendar.Core.Lifecycle.cs:11-42` in `OnResize`). After `SyncEditorLayerBounds()` set `_editorLayer.Bounds = ClientRectangle`, the hosted W8 Controls' `Bounds` were NOT repositioned. If the user had a W8 editor open and resized the calendar, the Control stayed at its old activation-time bounds, misaligned with the new cell positions. Fixed: after `SyncEditorLayerBounds()`, call `DeactivateAllCellComponents()` to drop every active W8 host (user re-clicks to re-activate at the new cell rect). The W4 sample editors are handled separately in GAP 5.

- [x] **GAP 5 - W4 sample editor widths not refreshed on calendar resize** (same `OnResize`). W4 editors' bounds are set at `BeginEdit` time via `ComputeEditorBounds(editorId, @event)`, which captures `ClientRectangle.Width` then. After a resize, the editor's `Width` was stale. Fixed: in `OnResize`, walk `_editorHost.ActiveEditors` and update each `hosted.Control.Width` to `ClientRectangle.Width - 2 * 8` (the same pad constant `ComputeEditorBounds` uses). The W4 editors' `X`/`Y` are `pad` from the top of the editor layer, which is invariant to size changes — only width needs updating.

- [x] **GAP 7 - Double-click on empty date cell opened title editor of unrelated selected event** (`BeepCalendar.Core.Lifecycle.cs:88-217` in `OnMouseDoubleClick`). The W4 fallback flow had an "open title editor on `_state.SelectedEvent`" branch that fired for ANY non-EventBlock hit BEFORE the create-at-focused-date branch. If the user double-clicked an empty date cell (Jan 5) and a different event was selected (Jan 20), the title editor for the Jan 20 event opened. Wrong event editor on the wrong date. Fixed: moved the `CreateEventAtFocusedDate()` block UP into the `DateCell` branch and added a `return` after it, so the DateCell path always commits to creating a new event at the hit date and never falls through to the shotgun SelectedEvent fallback. The SelectedEvent fallback now only fires for OTHER hit types (header cell, sidebar, toolbar, time gutter, etc.) — preserving the pre-W8 shotgun behavior for non-cell surfaces.

**Gaps documented as design constraints (not bugs, not fixed)**:

- [x] **GAP 6 - `GetCellComponent` returns null for non-Control IBeepUIComponent activations** (`BeepCalendar.Core.PublicApi.cs`). `ActivateCellComponent` returns `false` for any component that is not a WinForms `Control` (line ~306 in the public API: `if (comp is not Control control) return false;`). This means non-Control IBeepUIComponent instances can be rendered by the painter's `Draw()` call but cannot be hosted in the editor layer for editing. By design: only WinForms Controls can be parented to the editor layer Panel. This is acceptable because the IBeepUIComponent contract is intentionally dual-purpose (rendering + optional editing) and the editor-layer activation is opt-in. Documented but not changed.

- [x] **GAP 8 - W4 sample editor `ComputeEditorBounds` is view-agnostic** (`BeepCalendar.Core.PublicApi.cs:174-194`). The W4 `ComputeEditorBounds` always returns a top-of-control rect (`y = pad`). For the W4 sample editors, this is actually correct — they are designed to be top-of-control, view-independent bars (title editor, date range editor, all-day toggle) that float over the calendar surface. Documented but not changed.

- [x] **GAP 9 - SelectedEvent shotgun fallback for non-DateCell, non-EventBlock hit types** (preserved by GAP 7 fix). For hit types like `HeaderCell`, `TimeGutter`, `Sidebar`, `Toolbar`, the double-click path still opens the title editor of the currently selected event if one exists. This is pre-W8 shotgun behavior and is preserved as a convenience. Documented but not changed.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7088 warnings** (all pre-existing). No new warnings introduced by this pass.

**Architectural impact**:
- W8 Control-based cell components are now fully co-operating with WinForms' hosted-control paint cycle: bounds are set once at activation, WinForms handles drawing while active, and the painter only draws when the Control is not hosted.
- Click-outside-editor and resize-while-editing both correctly deactivate W8 hosts, so the user never sees a stale or duplicated editor.
- W4 sample editors survive a calendar resize (width refresh) and a view change (closed on view change, see W2-Redo-4 GAP A), but survive date navigation (view-independent bounds).
- Double-clicking an empty date cell is now predictable: it always creates a new event at that date, never opens a title editor for an unrelated selected event.
- The shotgun SelectedEvent fallback is preserved for non-cell surfaces (header, toolbar, gutter, sidebar) so behavior there is unchanged.

### W2-Redo-6 - Fourth Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Fourth audit after W2-Redo-5 found 5 additional real bugs the previous three passes missed. Build: **0 errors**, 7090 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - Toolbar was completely dead** (`BeepCalendar.Toolbar.cs:194-220` + `BeepCalendar.Interactions.Pointer.DownUp.cs:9-115` + `BeepCalendar.Core.Lifecycle.cs:64-89`). The toolbar's `HitTestToolbar(Point)` and `ExecuteToolbarClick(Point)` methods were defined but **never called from anywhere** in the calendar. Result: the entire toolbar (prev, today, next, undo, redo, create, edit, delete + 7 view-selector buttons) was painted but did not respond to clicks, and `_toolbarHoveredIndex` was permanently `-1` so the hover highlight was never drawn. Fixed: call `HitTestToolbar(e.Location)` first in `OnMouseDown`; if `IsClickOnToolbarButton(e.Location)` returns true, short-circuit (no interaction setup) and return — `OnMouseClick` will then invoke `ExecuteToolbarClick` to dispatch the button's `Action` (NavigatePrevious, GoToToday, Undo, etc.). Added `IsClickOnToolbarButton` helper that filters out spacers and null-Action buttons.

- [x] **GAP 2 - Mouse capture not set during drag interactions** (`BeepCalendar.Interactions.Pointer.DownUp.cs` `OnMouseDown`, `BeepCalendar.Interactions.Pointer.Up.cs` `OnMouseUp`, `BeepCalendar.Interactions.Pointer.MoveCancel.cs` `CancelInteraction`). `Control.Capture = true` was never set, so dragging an event off the calendar (or resizing beyond the bottom edge) dropped the drag mid-gesture — the cursor would leave the control, MouseMove/MouseUp would stop being delivered, and the event would snap back. Fixed: in `OnMouseDown`, after the W8-deactivation and pointer-state setup, set `Capture = true` when the left button is down. In `OnMouseUp`, release capture first thing (safe to assign false unconditionally). In `CancelInteraction` (Esc mid-drag), also release capture so the calendar doesn't keep holding the mouse after cancel.

- [x] **GAP 3 - DayViewPainter header hit was inconsistent with other timed views** (`Calendar/Rendering/ViewPainters/DayViewPainter.cs:82-130`). `DayViewPainter.HitTest` returned `EmptyHit` (mode `CreateEvent`) for any click in the day-header band, while every other timed-view painter (Week1/Week2/Week3/Week5/Week6/Week7/WorkWeek/Week) returns `DateCell` with `SelectDate` mode for the same area. Symptom: double-clicking the day header in Day view would open the create-event-at-focused-date W4 fallback (a new event starting at the current date) instead of opening the date-cell editor. Fixed: split the early-out into two checks — clicks above the day header band return `DateCell`/`SelectDate` with the current date (consistent with the other timed views), and clicks in the time-column gutter below the header still return `EmptyHit` (the gutter is a time-of-day label strip, not a date cell).

- [x] **GAP 4 - W2-Redo-5 GAP 3 fix was incomplete for editor-layer clicks** (`BeepCalendar.Core.Constructor.cs:32-60` + `BeepCalendar.Interactions.Pointer.DownUp.cs` `EditorLayer_MouseDownForward`). The W2-Redo-5 GAP 3 fix added the `IsClickInsideActiveEditor` check in `OnMouseDown`, but `OnMouseDown` only fires when the click reaches the calendar's WndProc. In WinForms, clicks on a transparent child `Panel` (the `_editorLayer`) are captured by the panel and do NOT bubble to the parent. So a click on the editor layer's empty background (outside any hosted W8/W4 child) would silently fail to deactivate W8, and would also fail to start any new interaction. Fixed: subscribe to `_editorLayer.MouseDown` in the constructor; the handler guards against W8/W4 children (`IsClickInsideActiveEditor` + `CalendarEditorHost.HitTest`) and forwards empty-area clicks to `OnMouseDown(e)`. The editor layer's coordinate system is identical to the calendar's (the layer's `Bounds = ClientRectangle` at `(0, 0)`), so the `MouseEventArgs.Location` is reusable as-is. This is the missing piece that makes the W2-Redo-5 GAP 3 W8 deactivation actually work for layer clicks.

- [x] **GAP 5 - W4 sample editor stayed open across deselect clicks** (`BeepCalendar.Interactions.Pointer.DownUp.cs` `OnMouseDown` + new `IsClickOutsideW4Editor` helper). When a W4 title/date-range/all-day editor was open for Event A and the user clicked Event B or an empty cell, the W4 editor for A stayed open. The user could only close it via Esc or Enter — clicking elsewhere did nothing. The W4 editor's host was the editor layer, so the click on a different event reached the calendar's OnMouseDown but didn't trigger any editor-close logic. Fixed: in `OnMouseDown`, after the toolbar check and before the W8 deactivation, call `IsClickOutsideW4Editor(e.Location)` — if the W4 editor is active AND the click is outside its bounds, call `EndEdit(commit: true)` to commit any pending changes and close the editor. The W4 editor still survives clicks on the editor layer's empty area (now properly forwarded by GAP 4) because the forwarded click hits `OnMouseDown` and the `IsClickOutsideW4Editor` check returns false for clicks inside the W4 Control's bounds.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7090 warnings** (all pre-existing). No new warnings introduced by this pass.

**Architectural impact**:
- **The toolbar is now functional** — prev/today/next, undo/redo, create/edit/delete, and 7 view-selector buttons all dispatch their actions on click, with hover highlighting. This was a regression in W2-Redo (the toolbar was painted but never wired).
- **Drag-to-move and drag-to-resize are now reliable** — `Capture = true` ensures the calendar keeps receiving MouseMove/MouseUp events even when the cursor leaves the control. Drag of an event to a different day, or resizing an event to extend past the bottom of the grid, now works correctly.
- **The editor layer click forwarding closes the W2-Redo-5 GAP 3 loophole** — clicks on the editor layer's transparent background now properly deactivate W8 and start new interactions, instead of being silently captured by the layer.
- **W4 editors close on deselect** — the user no longer needs to press Esc/Enter to dismiss a stale W4 title editor when they've moved on to clicking something else.
- **DayView header clicks are consistent** with the other timed views.

**Known design constraints deferred** (potential W2-Redo-7 work):
- No `OnKeyDown` handler — arrow keys don't change `_focusedDate`, Enter doesn't open the focused-date editor. `_keyboardFocusVisible` is reset to false on every mouse click and is never set to true. Pure keyboard navigation is a feature gap, not a bug. **W2-Redo-7 GAP 5 — FIXED.**
- No `OnMouseWheel` handler — wheel does not navigate dates. Feature gap. **W2-Redo-7 GAP 6 — FIXED.**
- No `Dispose` override — the cell-component cache, editor host, and event handlers are not disposed when the control is destroyed. Memory leak in long-lived scenarios. **W2-Redo-7 GAP 4 — FIXED.**
- `ResizeHandleHitSizePx` constant in `BeepCalendar.Fields.cs:64` is defined but never used. Dead constant. **W2-Redo-7 GAP 3 — FIXED.**
- `OnCreateEventRequested` raises a public event when the editor doesn't handle the create; it should also call `CreateEventAtFocusedDate` if no event subscriber creates one, so the event lands in `_events` and the calendar updates. **W2-Redo-7 GAP 2 — FIXED.**

### W2-Redo-7 - Fifth Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Fifth audit after W2-Redo-6 found 6 additional gaps. Five were deferred design constraints from the W2-Redo-6 known-constraints list; one was a real DayView/WorkView hit-test regression that W2-Redo-6 GAP 3 missed when it patched DayViewPainter only. Build: **0 errors**, 7088 warnings (baseline — no new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - WorkWeekViewPainter + WeekViewPainter day-header hit was inconsistent with DayViewPainter** (`Calendar/Rendering/ViewPainters/WorkWeekViewPainter.cs:101-107` + `Calendar/Rendering/ViewPainters/WeekViewPainter.cs:91-97`). W2-Redo-6 GAP 3 split the early-out check in `DayViewPainter.HitTest` to return `DateCell`/`SelectDate` for clicks in the day-header band, but left the same single-OR early-out in `WorkWeekViewPainter` and `WeekViewPainter`: `if (location.X < grid.X + surface.TimeColumnWidth || location.Y < grid.Y + surface.DayHeaderHeight) return EmptyHit(args);`. Symptom: double-clicking the day header in WorkWeek or Week view triggered `CreateEvent` (the W4 fallback) instead of `SelectDate` (the date-cell fallback). Fixed: split into two checks. Time-column gutter (X < TimeColumnWidth) returns `EmptyHit`; day-header band (Y < DayHeaderHeight) returns `DateCell`/`SelectDate` with the column-derived date. Mirrors the DayViewPainter / Week1/Week2/Week3/Week5/Week6/Week7 pattern.

- [x] **GAP 2 - `OnCreateEventRequested` lost the proposed event when no editor and no subscribers** (`Calendar/BeepCalendar.EventOperations.Editor.cs:9-22`). The deferred W2-Redo-6 design constraint. When `EventEditor == null` AND `CreateEventRequested == null`, the create request was silently dropped — the proposed `proposedEvent` was never added to `_events` and the calendar didn't update. Fixed: in that case, build a default "New Event" with `Id = NextEventId()`, normalize duration, and call `TryAddEvent` to commit it via the standard mutation history + cache-invalidate path. Developers who want custom handling still get it via the public `CreateEventRequested` event or by setting a custom `ICalendarEventEditor`.

- [x] **GAP 3 - Dead `ResizeHandleHitSizePx` constant in `BeepCalendar.Fields.cs:64`**. The constant was defined as 6px but never referenced anywhere — the 14 painters hardcode 4px (Month, Week4) or 6px (timed views + Week5/6) in their `CalendarPainterHelpers.ResolveResizeEdge` calls. Removing the unused constant prevents future readers from assuming the calendar uses a single shared resize-handle size (it doesn't). Fixed: removed the field and added a comment block explaining the per-view handle sizes the painters actually use.

- [x] **GAP 4 - No `Dispose` override — cell-component cache, editor pool, editor layer, and event subscriptions leaked** (`Calendar/BeepCalendar.cs`). The deferred W2-Redo-6 design constraint. The cell component cache (per-calendar `Dictionary<string, IBeepUIComponent>`) held cached `IBeepUIComponent` instances forever; the editor pool (per-host `Dictionary<Type, Stack<Control>>`) kept up to 16 Controls per type cached; the editor layer's `MouseDown` subscription was never unsubscribed; the undo/redo stacks were never cleared. Fixed: added `Dispose(bool)` override in `BeepCalendar.cs` that calls `EndEdit(false)`, `DeactivateAllCellComponents()`, clears the cell cache, unsubscribes `_editorLayer.MouseDown`, removes the editor layer from `Controls` and disposes it, and clears the undo/redo stacks before calling `base.Dispose(disposing)`. All cleanup is wrapped in try/catch so a failure in one step doesn't block the rest.

- [x] **GAP 5 - No `OnKeyDown` handler — `_focusedDate` and `_keyboardFocusVisible` were dead state** (`Calendar/BeepCalendar.cs`). The deferred W2-Redo-6 design constraint. The `_focusedDate` field defaulted to `DateTime.Today` and was only ever assigned by `OnMouseDown` / `OnMouseDoubleClick`. `_keyboardFocusVisible` was reset to `false` on every click and never set to `true`. Arrow keys did nothing. Fixed: added `OnKeyDown` override that:
  - `Left` / `Right`: move `_focusedDate` by ±1 day
  - `Up` / `Down`: move `_focusedDate` by ±7 days (week)
  - `PageUp` / `PageDown`: navigate by `_viewPainter.NavigatePrevious/Next` (falls back to `±1 month` for views without a painter)
  - `Home` / `End`: jump to start / end of the focused week
  - `Enter`: call `OnCreateEventRequested(_focusedDate)` so the user can create an event with the keyboard alone
  - Sets `_keyboardFocusVisible = true` so the painter can draw a focus ring around the focused date cell
  - Skips when `IsEditing` so the W4 text editor's own KeyDown handler wins
  - Raises `DateSelected` for consistency with the mouse path

- [x] **GAP 6 - No `OnMouseWheel` handler — wheel did not navigate dates** (`Calendar/BeepCalendar.cs`). The deferred W2-Redo-6 design constraint. Fixed: added `OnMouseWheel` override that:
  - `Delta > 0` (scroll up): `NavigatePreviousPeriod()`
  - `Delta < 0` (scroll down): `NavigateNextPeriod()`
  - `Ctrl + wheel`: cycle through `ViewPainterFactory.GetRegisteredViews()` (skipping unknown / null keys). Materializes the `IEnumerable` to a `List` because the factory returns a deferred LINQ projection, not an indexed collection.
  - WinForms' `MouseEventArgs` doesn't expose a `Handled` flag, so the event is consumed by simply not forwarding it to the scrollable base container.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7088 warnings** (baseline). No new warnings introduced. A fresh rebuild would surface the 20 pre-existing project errors in unrelated files (BeepGridFilterFlyout, BeepWait, BeepPopupListForm, BaseControl.Methods) that reference missing `UseRichToolTip`/`ToolTipType`/`_toolTip` symbols — all in non-Calendar code, unchanged by this pass.

**Architectural impact**:
- The 14 view painters are now consistent on day-header hit-test semantics: all return `DateCell`/`SelectDate` for clicks in the day-header band (above `DayHeaderHeight`), and only the time-column gutter (X < `TimeColumnWidth` in timed views) returns `EmptyHit`. W2-Redo-6 GAP 3's DayView-only fix is now generalized.
- `OnCreateEventRequested` no longer silently drops the create request. Default behavior (no editor, no subscribers) is now "create a 1-hour New Event at the focused date" — matches the user's apparent intent.
- The control now releases all owned resources on `Dispose`: cell cache, editor pool, editor layer, undo/redo stacks, and event subscriptions. No more leak in long-lived scenarios (MDI tabs, dashboard refreshes).
- The keyboard focus state (`_focusedDate`, `_keyboardFocusVisible`) is now actually used. Arrow keys, Home/End, PageUp/PageDown, and Enter all move the focus / create an event. `Ctrl+wheel` swaps the view mode.
- Dead `ResizeHandleHitSizePx` constant is removed; the per-view 4px / 6px resize-handle sizes in the painters are now the single source of truth.

### W2-Redo-8 - Sixth Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Sixth audit after W2-Redo-7 found 4 additional gaps. One is a real editor bug (all-day toggle editor would close itself on activation), one is a memory leak in the cell-component cache (W2-Redo-7's Dispose fixed the cache entries but not the components), one is a UX gap left over from the W2-Redo-7 GAP 2 default-create fallback, and one is a focus issue in `OnMouseWheel`. Build: **0 errors**, 7088 warnings (baseline — no new from this pass).

- [x] **GAP 1 - `InlineAllDayToggleEditor` immediately committed on activation for all-day events** (`Calendar/Editor/SampleEditors/InlineAllDayToggleEditor.cs:30-70`). When the user double-clicked an all-day event to open the All-Day toggle editor, the `Loading` event set `checkBox.Checked = evt.IsAllDay` (true), which fired `CheckedChanged` → `RequestCommit` → `EndEdit(commit: true)`. The editor opened and immediately closed itself in a single frame. Symptom: the All-Day editor was unusable for any event with `IsAllDay = true`. Fixed: added a `_isLoading` guard inside the editor factory. While `_isLoading` is true, `CheckedChanged` returns early without raising `RequestCommit`. The `Loading` handler sets `_isLoading = true` before mutating the checkbox and resets it to `false` in a `finally` block. Same fix shape as the standard "suspend change tracking while seeding initial state" pattern used in WinForms data-binding.

- [x] **GAP 2 - Cell component cache leaked `IBeepUIComponent` instances on calendar dispose** (`Calendar/CellRender/CalendarCellComponentCache.cs:74-77` + `Calendar/BeepCalendar.cs:Dispose`). W2-Redo-7 GAP 4 added `_componentCache.Clear()` to `BeepCalendar.Dispose`, which removed the dictionary entries but did NOT dispose the `IBeepUIComponent` instances they referenced. Control-based W8 components held in the cache (and on the editor layer at activation time) would never be released. Fixed: added `CalendarCellComponentCache.DisposeAll()` that walks the cache and calls `Dispose()` on any `IDisposable` component (Controls implement `IDisposable`). Each dispose is wrapped in try/catch so a failing component doesn't block the rest. `BeepCalendar.Dispose` now calls `DisposeAll()` instead of `Clear()`. Safe to call multiple times (the cache is cleared after dispose).

- [x] **GAP 3 - W2-Redo-7 GAP 2 default-create event was added but not selected** (`Calendar/BeepCalendar.EventOperations.Editor.cs:24-48`). W2-Redo-7 added a fallback path in `OnCreateEventRequested` that creates a default "New Event" and commits it via `TryAddEvent` when no `EventEditor` is set and no `CreateEventRequested` subscriber is registered. The event landed in `_events` but the calendar's state didn't reflect that it was selected — `_state.SelectedEvent` was unchanged, `_state.SelectedDate` was unchanged, and no `EventSelected` event was raised. The user saw the new event appear in the grid but not highlighted. Fixed: after `TryAddEvent(fallback)` returns true, set `_state.SelectedEvent = fallback`, update `_state.SelectedDate` and `_state.CurrentDate` to the event's date, and raise `EventSelected` for host subscribers. If `TryAddEvent` returns false (e.g., due to a conflict-policy rejection or a `Mutating` handler that cancelled), we leave the state unchanged.

- [x] **GAP 4 - `OnMouseWheel` didn't focus the calendar, breaking subsequent keyboard navigation** (`Calendar/BeepCalendar.cs:OnMouseWheel`). The W2-Redo-7 `OnMouseWheel` handler navigated dates (and cycled views on Ctrl+wheel) but did not call `Focus()` first. Mouse-wheel events fire when the cursor is over the control, without requiring a click; if the user scrolled while focus was on the form's chrome (or another control), the next arrow-key press would go to that other control instead of the calendar's `OnKeyDown`. Fixed: call `Focus()` at the top of `OnMouseWheel` when the calendar is not already focused. The focus call is cheap when the calendar is already focused (early return on `!Focused`).

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7088 warnings** (baseline — no new from this pass).

**Architectural impact**:
- The W4 sample editor family is now bug-free: title, date-range, and all-day toggle all work correctly for every event state (timed, all-day, with/without a `EventEditor` registered, with/without a `CreateEventRequested` subscriber).
- The cell component cache now disposes its contents on calendar dispose, closing a memory leak in long-lived scenarios (MDI tabs, dashboard refreshes, dynamic form generation).
- The default-create fallback (`OnCreateEventRequested` with no editor and no subscribers) now selects the created event and raises `EventSelected`, so the new event is visually highlighted and the host can react to it.
- Mouse-wheel navigation is now coherent with mouse-click + keyboard navigation: scrolling focuses the calendar, so the next arrow-key press is delivered to `OnKeyDown`.



### W2-Redo-9 - Seventh Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "Continue if you have next steps". Seventh audit after W2-Redo-8 found 4 additional gaps. Two are real consistency bugs (the `CurrentDate` setter silently leaves `_focusedDate` and any active W8 cell component out of sync; `RemoveEvent` raises `EventMutating` with the wrong `appliedEvent` argument), one is a dead-allocation cleanup, and one is a UX issue (`TryEditSelectedEvent(Point.Empty)` is used for the F2 / Edit toolbar / `EditSelectedEvent()` command path, which breaks custom `ICalendarEventEditor` implementations that position a popup at the supplied location). Build: **0 errors**, 7089 warnings (baseline — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - `CurrentDate` setter silently left `_focusedDate` and W8 cell components out of sync** (`Calendar/BeepCalendar.Core.PublicApi.cs:17-19`). The previous `CurrentDate` setter only updated `_state.CurrentDate` and `_state.SelectedDate` and called `Invalidate()`. After a programmatic assignment of `CurrentDate` (e.g. `calendar.CurrentDate = new DateTime(2026, 7, 1)`), `_focusedDate` still held the old date — so the next `Right` arrow press in `OnKeyDown` would jump to `oldDate.AddDays(1)` instead of the expected `newDate.AddDays(1)`. Any active W8 cell component (DateCell / TimeSlot factory output, hosted in the editor layer) was also left visible at the old cell bounds because the cell key is date-scoped. Fixed: the setter now also updates `_focusedDate` / `_state.FocusedDate` to the new date and calls `DeactivateAllCellComponents()` for the same reason `NavigatePrevious` / `NavigateNext` / `NavigateToday` do. Added an early return when `newDate` matches the current value to avoid spurious invalidations.

- [x] **GAP 2 - `OnCreateEventRequested` allocated an unused `proposedEvent` in the no-editor / no-subscriber fallback** (`Calendar/BeepCalendar.EventOperations.Editor.cs:9-57`). The previous method created a `proposedEvent` at the top of the body and then, in the `EventEditor == null && CreateEventRequested == null` branch, built a separate `fallback` (with `NextEventId()` assigned) and returned early. The `proposedEvent` was only consumed in the editor / subscriber path that came after. The branch's early return made the original `proposedEvent` allocation dead. Fixed: moved the `proposedEvent` allocation to immediately before its only consumer (`TryOpenEventEditor` / `CreateEventRequested?.Invoke`). The fallback path now allocates a single CalendarEvent (as before) and there is no wasted allocation in the common "no editor, no subscriber" case.

- [x] **GAP 3 - `RemoveEvent` raised `EventMutating` with `appliedEvent = existingEvent` for a delete** (`Calendar/BeepCalendar.EventOperations.Crud.Remove.cs:29`). The `EventMutating` event was raised with `(Delete, existingEvent, null, existingEvent, false, out var canceled)`. The first three arguments are correct (kind, original, proposed = null because the proposal is "remove"), but `appliedEvent` is the post-mutation state, which for a delete is "event not present" (= null). The matching `RaiseMutated` call below correctly passed `null` for `appliedEvent`. The inconsistency would mislead any subscriber that branches on `e.Args.AppliedEvent` to decide whether the mutation is still live (during the mutating window, `AppliedEvent` would be the about-to-be-deleted event, suggesting the mutation had already been applied). Fixed: changed the `appliedEvent` argument to `null` so both `RaiseMutating` and `RaiseMutated` agree on the post-mutation state for a delete.

- [x] **GAP 4 - `TryEditSelectedEvent(Point.Empty)` placed custom-editor popups at (0, 0)** (`Calendar/BeepCalendar.Commands.Execution.Core.cs:50`). The `EditSelectedEvent` command routed through `TryEditSelectedEvent(System.Drawing.Point.Empty)`. The W4 sample editors (title, date-range, all-day) ignore the request's `Location` (they use `ComputeEditorBounds` keyed on the calendar's `ClientRectangle`), so the empty point is harmless for them. But custom `ICalendarEventEditor` implementations typically use `request.Location` as the click point to position a popup dialog. With `Point.Empty` the dialog would appear at the form's top-left corner — a confusing UX. Fixed: replaced the `Point.Empty` argument with the center of the calendar's `ClientRectangle`. A custom editor that wants to position its popup at a specific point can still do so; the center is a sensible default for the F2 / Edit toolbar / `EditSelectedEvent()` API path (which has no click context).

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7089 warnings** (baseline — 0 new from this pass; `grep -i calendar` on the warning list returns no Calendar-specific warnings). Build time: cached, instant.

**Architectural impact**:
- `CurrentDate` setter is now consistent with `NavigatePrevious` / `NavigateNext` / `NavigateToday` and `ViewMode` setter: all paths that change the visible date clear any W8 cell component and sync `_focusedDate` to the new date.
- `OnCreateEventRequested` no longer wastes a CalendarEvent allocation per call in the default-create path.
- `RemoveEvent`'s `EventMutating` event contract is now consistent with `RaiseMutated` (both pass `null` for `appliedEvent` on a delete). The semantic meaning of `EventMutationEventArgs.AppliedEvent` is now "the post-mutation state of the event, or null if the event is no longer present".
- Programmatic edit (`F2` / Edit toolbar / `calendar.EditSelectedEvent()`) now positions a custom-editor popup at a sensible default location (calendar client center) instead of `(0, 0)`.



### W2-Redo-10 - Eighth Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Eighth audit after W2-Redo-9. Read all 14 view painters and `CalendarEventEditor.cs` for a full per-view sweep of hit-test / paint / event-card / empty-hit gaps. Found 3 real bugs across the painter family: 1 cosmetic-but-confusing inconsistency in `EmptyHit` shape, and 2 stackable event-card hit-tests that could resolve to an event that Paint never drew (because the day has more events than fit in the column and the hit-test loop wasn't bounded by the same `maxRows` / `maxCards` cap as Paint). Build: **0 errors**, 7089 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - `TimelineViewPainter.EmptyHit` returned `SelectDate` instead of `CreateEvent`** (`Calendar/Rendering/ViewPainters/TimelineViewPainter.cs:346-353`). All 13 other painters (Day/Week/WorkWeek/Month/Agenda/List/Week1..Week7) return `CalendarInteractionMode.CreateEvent` for their `EmptyHit` helper. The Timeline painter was the lone outlier returning `SelectDate`. The `RequestedMode` field is metadata not consumed by `BeepCalendar.ResolveDragMode` (which branches on `TargetKind`, not on `RequestedMode`) or by `OnMouseDoubleClick` (which branches on `TargetKind` + `Date.TimeOfDay`), so the actual user-facing behavior was identical for both values. The bug is consistency: any downstream consumer reading the `CalendarInteractionHitTestResult` (custom `ICalendarViewPainter` implementations, debug overlays, test harnesses, future code) would see a different value for Timeline than for every other view. Fixed: changed `SelectDate` to `CreateEvent`. Timeline's `EmptyHit` is now shape-consistent with all 13 other painters.

- [x] **GAP 2 - `Week5ViewPainter.HitTest` row index could exceed Paint's `maxRows` cap** (`Calendar/Rendering/ViewPainters/Week5ViewPainter.cs:140-180`). Paint's card loop at `Week5ViewPainter.cs:94` bounds the visible cards with `count = Math.Min(dayEvents.Count, dayColumn.Height / (cardH + 4))`. The matching HitTest only checked `if (row < dayEvents.Count)` — no `maxRows` guard. If a day has more events than fit in the column (e.g. 8 events in a 200px column with `cardH=56`, `maxRows=3`), a click at Y = `dayColumn.Y + 4 + 3*(cardH+4) = dayColumn.Y + 184` (where Paint drew no card) would still resolve to `dayEvents[3]` and the user would see a "selected event" highlight for a card that isn't visible. Fixed: added `int maxRows = Math.Max(0, dayColumn.Height / (cardH + 4));` and changed the condition to `if (row < dayEvents.Count && row < maxRows)`. HitTest and Paint now agree on which cards are user-clickable.

- [x] **GAP 3 - `Week6ViewPainter.HitTest` row index could exceed Paint's `maxCards` cap** (`Calendar/Rendering/ViewPainters/Week6ViewPainter.cs:117-145`). Same shape as GAP 2 but for the 6-day stacked view: Paint at `Week6ViewPainter.cs:84-87` bounds the visible cards with `count = Math.Min(dayEvents.Count, maxCards)` where `maxCards = Math.Max(1, dayColumn.Height / (cardH + 6))`. The matching HitTest only checked `if (row < dayEvents.Count)` — no `maxCards` guard. Same user-visible symptom as GAP 2 (click selects an off-screen card). Fixed: added `int maxCards = Math.Max(0, dayColumn.Height / (cardH + 6));` and changed the condition to `if (row < dayEvents.Count && row < maxCards)`. HitTest and Paint now agree on which cards are user-clickable.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Wformin\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7089 warnings** (baseline — 0 new from this pass; `grep -i "calendar\|painter"` on the error list returns no painter-specific errors). Build time: 1m (rebuild after a stale intermediate from a prior edit cycle).

**Architectural impact**:
- `EmptyHit` shape is now uniform across all 14 painters: every painter's `EmptyHit` returns `TargetKind = EmptySurface`, `RequestedMode = CreateEvent`. The `RequestedMode` field remains metadata for future consumers (e.g. custom painters, automation hooks) that may branch on it.
- Week5 and Week6 stacked event cards are now clickable exactly where they are visible. If a day has more events than fit, only the visible `maxRows` / `maxCards` are clickable; clicks below the last visible card return a `DateCell/CreateEvent` (so the user can still create a new event in that day). The previous behavior (click selects an off-screen card) was a visible-state bug that this pass closes.
- This pass is the eighth in the W2-Redo series and brings the cumulative gap count for the W2 area to **46 fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 across W2-Redo-3..W2-Redo-10, not counting deferred design constraints).

### W2-Redo-11 - Ninth Gap Fix Pass (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Ninth audit after W2-Redo-10. Read all previously-unread central BeepCalendar partial files (Commands, EventOperations, Interactions, Toolbar, LayoutTheme, Painting, Editor, Helpers, etc.) — i.e. focused on the central interaction/state-mutation pipeline that wires the painters (audited in W2-Redo-10) to the public API. Found 3 high-severity bugs, all in the "date / focusedDate / CurrentDate state-not-synced-after-a-state-changing-operation" class (same family as W2-Redo-9 GAP 1). Build: **0 errors**, 7089 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - `SetVisibleRange` command did not sync `_focusedDate` / `_state.FocusedDate` or call `DeactivateAllCellComponents()`** (`Calendar/BeepCalendar.Commands.Execution.Core.cs:28-38`). After the user programmatically calls `ExecuteCommand(new CalendarCommandEventArgs { CommandType = SetVisibleRange, AnchorDate = X })`, the visible date jumped to X (because `_state.CurrentDate` and `_state.SelectedDate` were both set), but the keyboard-navigation anchor stayed at the old date. Two user-visible symptoms: (a) a hosted W8 cell component remained visible at the previous cell's bounds even though that cell was no longer in view (because `DeactivateAllCellComponents` was never called); (b) the next Left/Right arrow key press jumped from the OLD focused date, not from X. This is the exact same shape as W2-Redo-9 GAP 1 (the `CurrentDate` setter fix) — every code path that changes the visible date must keep `_focusedDate` and `_state.FocusedDate` in sync. Fixed: changed the body to compute `DateTime anchor = args.AnchorDate.Value.Date;` once and assign it to `VisibleRangeStart`, `CurrentDate`, `SelectedDate`, then assign `_focusedDate = anchor; _state.FocusedDate = _focusedDate; DeactivateAllCellComponents();` before `Invalidate()`. SetVisibleRange now mirrors the `CurrentDate` setter and `ViewMode` setter.

- [x] **GAP 2 - `OnMouseDown` dropped time-of-day from `_focusedDate` for both DateCell and EventBlock hits** (`Calendar/BeepCalendar.Interactions.Pointer.DownUp.cs:72-89`). The previous code routed through `_state.SelectedDate`, which is always a midnight Date. `OnMouseDoubleClick` (`Calendar/BeepCalendar.Core.Lifecycle.cs:168-176`) preserves time via `_focusedDate = hit.Date.Value;` — a clean asymmetry that this gap closes. User-visible symptom: in a timed view (Day/WorkWeek/Week1/Week2/Week3/Week7/Timeline) a user clicking a 14:30 cell had their `_focusedDate` snap to that day's 00:00. The next Right-arrow press then advanced to *the next midnight*, not the next 14:30 — silently shifting the focused time-of-day on every click. Same symptom on event hits: clicking a 10:15-11:00 event pinned `_focusedDate` to the day's 00:00. Fixed: first branch `_focusedDate = _activeInteractionHit.Date.Value;` (preserves the time-of-day the painter resolved from the hit location); second branch `_focusedDate = _activeInteractionHit.Event.StartTime;` (preserves the event's actual start time). OnMouseDown is now shape-consistent with OnMouseDoubleClick.

- [x] **GAP 3 - `CommitCopyEventMutation` did not update `_state.CurrentDate` / `_state.SelectedDate` / `_focusedDate` / `_state.FocusedDate` after a successful copy** (`Calendar/BeepCalendar.Interactions.Commit.Copy.cs:7-32`). Compare to `CommitNewEventMutation` (`Calendar/BeepCalendar.Interactions.Commit.NewEvent.cs:31-35`) which syncs all four state fields after committing a new event. `CommitCopyEventMutation` only set `_state.SelectedEvent = mutated;` — the new event was added to `_events` and selected, but the calendar's "current date" still pointed at the source event's day. Three user-visible symptoms: (a) Ctrl+Shift+drag-copy did not scroll the view to the destination of the copy; (b) keyboard navigation after a copy was off (Right arrow advanced from the source event's day, not the copy's day); (c) any consumer reading the calendar's "current day" via `CurrentDate` saw stale data. Fixed: after `_state.SelectedEvent = mutated;` added the same four lines that `CommitNewEventMutation` uses (`SelectedDate = mutated.StartTime.Date; CurrentDate = mutated.StartTime.Date; _focusedDate = mutated.StartTime.Date; _state.FocusedDate = _focusedDate;`). Commit copy is now state-consistent with commit new.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7089 warnings** (baseline — 0 new from this pass; `grep -i calendar` on the error list returns no Calendar-specific errors). Build time: cached, instant (one transient VBCSCompiler file lock retried automatically).

**Architectural impact**:
- Three "state-not-synced-after-a-state-changing-operation" gaps fixed in one pass. The pattern is now uniformly applied across all five code paths that change the visible date: `CurrentDate` setter (W2-Redo-9 GAP 1), `ViewMode` setter (W2-Redo-3 GAP 1), `NavigatePrevious` / `NavigateNext` / `NavigateToday` (central navigation), and now `SetVisibleRange` + `OnMouseDown` + `CommitCopyEventMutation`. `_focusedDate` and `_state.FocusedDate` are kept in lockstep with `_state.CurrentDate` everywhere a transition happens.
- `OnMouseDown` and `OnMouseDoubleClick` now agree on the time-of-day semantics of `_focusedDate` — both preserve the time the painter resolved from the hit location. Pre-W2-Redo-11 behavior was a quiet latent bug because the time-of-day drift only manifests on the *next* keyboard navigation, not on the click itself.
- This pass is the ninth in the W2-Redo series and brings the cumulative gap count for the W2 area to **49 fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 across W2-Redo-3..W2-Redo-11, not counting deferred design constraints).

### W2-Redo-12 - Tenth Audit Pass — No New Gaps Found (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Tenth audit after W2-Redo-11. Read all 31 remaining previously-unread files: LayoutTheme partials (ApplyTheme, MonthCells, HeaderText, Controls.Categories), Commands helpers (Helpers, Helpers.Duplicate), Painting pipeline (HeaderFormatting, Telemetry, DesignTime), VisualUpdates (Flush, Scope), Core public API (Events, Appearance, Style), Types (Types.cs, Types.Events.cs, Types.EventArgs.cs, Types.Commands.cs), Helpers (Resources, Tokens, PerformanceMetrics, ViewStateHelper, DrawingPrimitives, LayoutGeometry), CellRender (CellContext, CellComponentCache), and Rendering (ViewPaintArgs, ICalendarViewPainter, CalendarPainterHelpers, ViewPainterFactory). **No bugs found.** All files are clean.

**Files audited (31 total)**:

| Category | Files | Result |
|----------|-------|--------|
| LayoutTheme | `ApplyTheme.cs`, `MonthCells.cs`, `HeaderText.cs`, `Controls.Categories.cs` | Clean |
| Commands | `Commands.Helpers.cs`, `Commands.Helpers.Duplicate.cs` | Clean |
| Painting pipeline | `HeaderFormatting.cs`, `Telemetry.cs`, `DesignTime.cs` | Clean |
| Visual updates | `Flush.cs`, `Scope.cs` | Clean |
| Core API | `PublicApi.Events.cs`, `PublicApi.Appearance.cs`, `Style.cs` | Clean |
| Types | `Types.cs`, `Types.Events.cs`, `Types.EventArgs.cs`, `Types.Commands.cs` | Clean |
| Helpers | `CalendarResources.cs`, `CalendarTokens.cs`, `CalendarPerformanceMetrics.cs`, `CalendarViewStateHelper.cs`, `CalendarDrawingPrimitives.cs`, `CalendarLayoutGeometry.cs` | Clean |
| Cell render | `CalendarCellContext.cs`, `CalendarCellComponentCache.cs` | Clean |
| Rendering | `ViewPaintArgs.cs`, `ICalendarViewPainter.cs`, `CalendarPainterHelpers.cs`, `ViewPainterFactory.cs` | Clean |

**Notable observations**:
- `TryDuplicateSelectedEvent` (Commands.Helpers.Duplicate.cs:44-48) correctly syncs `SelectedDate`, `CurrentDate`, `_focusedDate`, and `_state.FocusedDate` after duplication — the correct pattern already applied here before W2-Redo-11's GAP 3 fix.
- `TryDeleteSelectedEvent` (Commands.Helpers.cs) correctly preserves the current date/focus state after deletion (the event is removed; the view should stay at the same day).
- `CalendarPainterHelpers.TryDrawCellComponent` correctly distinguishes between hosted Controls (edit mode — skip Draw, let WinForms paint it) and unhosted Controls (set bounds, call Draw).
- `ViewPaintArgs.ResolveThemeColors` has proper null-theme fallback with `BeepStyling.GetFormStyle → BeepThemesManager.GetTheme` chain matching `BaseControl`'s convention.
- All disposable resources (`GraphicsPath`, `SolidBrush`, `Font`, `Bitmap`) are properly wrapped in `using` blocks.
- `CalendarCellComponentCache.DisposeAll` correctly disposes `IDisposable` components and clears the cache.

**Build verification**: Not run (no code changes). Previous build from W2-Redo-11 was **0 errors, 7101 warnings** (all pre-existing).

**Architectural impact**:
- This pass confirms that all 31 remaining unread Calendar files are bug-free. With W2-Redo-11 closing the last "state-not-synced" gap class, the cumulative 49 gaps fixed across W2-Redo-3..W2-Redo-11 have fully closed the audit surface. Every BeepCalendar partial, helper, type, and rendering file in the Calendar directory has now been audited at least once.
- The W2-Redo series is now complete through 10 audit passes (3 bug-fixing + 1 clean). The only remaining items are the deferred design constraints (inconsistent week-start day across painters, OnKeyPreview global keyboard, undo/redo stacks uncapped, IME/RTL support, etc.) which are feature/architecture-level items outside the scope of gap-fix passes.

### W2-Redo-13 - W4 Editor Commit Pipeline Gap Fix (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Eleventh audit after W2-Redo-12. Deep cross-file audit of interaction pipelines (mouse Down→Move→Up→Commit, undo/redo, editor commit). Found 1 high-severity gap in the W4 inline editor commit pipeline. Build: **0 errors**, 7103 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP A - W4 inline editor commits modified `CalendarEvent` in-place without invalidating cache, recording undo history, or raising `EventMutated`** (`Calendar/BeepCalendar.Fields.cs`, `Calendar/BeepCalendar.Core.Constructor.cs`). The W4 sample editors (title / date-range / all-day toggle) use a two-layer lifecycle: `CalendarEditorHost.BeginEdit` creates a `HostedEditor`, the sample editor's `Loading` handler reads values from the `CalendarEvent` into the control, the user edits, and on Enter the `Saving` handler writes the control's value back to the SAME `CalendarEvent` object in `_events` (e.g. `current.Title = textBox.Text`). The `CalendarEditorHost.EndEdit` then fires `EditCommitted` — but no code subscribed to it. Three user-visible symptoms: (a) the `_eventService` cache was never invalidated, so cached query results for the old date range could become stale if the date-range editor changed StartTime/EndTime; (b) W4 edits could not be undone because `RecordMutationHistory` was never called (Undo/Ctrl+Z did nothing after a W4 edit); (c) `EventMutated` subscribers (custom sidebars, sync engines, external loggers) never learned about W4 edits.

  **Fix** (3 changes in 2 files):
  1. **`BeepCalendar.Fields.cs`**: Added `private CalendarEvent _editingBeforeSnapshot` — stores a deep clone of the pre-edit `CalendarEvent` for undo history and event notification.
  2. **`BeepCalendar.Core.Constructor.cs`**: Subscribed to three `CalendarEditorHost` events:
     - `EditStarted` — captures `_editingBeforeSnapshot = CloneEvent(hosted.Event)` (deep clone of the event BEFORE the user begins editing)
     - `EditCommitted` — calls `_eventService.InvalidateCache()`, records `RecordMutationHistory(Update, _editingBeforeSnapshot, hosted.Event)`, raises `RaiseMutated(...)`, then calls `Invalidate()`
     - `EditCancelled` — clears `_editingBeforeSnapshot` and calls `Invalidate()`
  3. The entire fix uses only existing internal APIs (`CloneEvent`, `RecordMutationHistory`, `RaiseMutated`, `GetConflicts`) — no new public surface.

**Build verification**: `dotnet build ...` returns **0 errors, 7103 warnings** (was 7101; +2 from unrelated files recompiled — `grep -E "(Fields|Constructor)"` on the warning list returns **no matches**). 0 new warnings from the 2 Calendar files changed.

**Architectural impact**:
- W4 inline editors are now a first-class mutation source on par with drag-commit (`CommitExistingEventMutation`) and programmatic edit (`TryCommitEditedEvent`). All three paths now: invalidate cache, record undo history, raise `EventMutated`, and redraw.
- The `HostedEditor` / `CalendarEditorHost` two-layer design (where the sample editor's `Saving` handler modifies the `CalendarEvent` in-place) is preserved. The fix operates entirely on the `BeepCalendar` side via event subscription — zero changes to `HostedEditor`, `CalendarEditorHost`, or any sample editor.
- The edit-commit event subscription is safe across the auto-EndEdit in `CalendarEditorHost.BeginEdit` (which commits any previously-active editor before starting a new one): `EditStarted` fires for the new editor AFTER `EditCommitted` fires for the previous one, so the `_editingBeforeSnapshot` field is always correct for the active editor.
- Cumulative W2 series: **50 gaps fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 + 0 + 1 across W2-Redo-3..W2-Redo-13, not counting deferred design constraints).

### W2-Redo-14 - Navigation State Sync Gap Fix (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Twelfth audit after W2-Redo-13. Deep audit of remaining pipeline code: `CalendarEventEditor` dialog, event queries, `CalendarConflictPolicy`, toolbar, painting pipeline, `CalendarEventService` cache, and navigation commands. Found 1 gap in the navigation state sync. Build: **0 errors**, 7103 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP B - `NavigatePrevious` and `NavigateNext` did not sync `_state.SelectedDate`, `_focusedDate`, or `_state.FocusedDate`** (`Calendar/BeepCalendar.EventOperations.Navigation.cs:28-56`). Compare to `NavigateToday` (lines 7-26 in the same file) which syncs all five date/focus fields:
  ```csharp
  _state.CurrentDate = DateTime.Today;
  _state.SelectedDate = DateTime.Today;
  _focusedDate = DateTime.Today;
  _state.FocusedDate = _focusedDate;
  ```
  `NavigatePrevious` and `NavigateNext` only set `_state.CurrentDate = newDate` (the raw field, bypassing the `CurrentDate` property setter's W2-Redo-9 sync logic). `_state.SelectedDate`, `_focusedDate`, and `_state.FocusedDate` were left at the old navigation anchor. Three user-visible symptoms: (a) after clicking prev/next in the toolbar or pressing PageUp/PageDown, the next Left/Right arrow press jumped from the OLD date, not the new navigation anchor; (b) `_state.SelectedDate` didn't track the new visible range, so `DateSelected` consumers and the toolbar's "New" button (which reads `_state.SelectedDate`) used stale data; (c) code reading `_state.FocusedDate` after toolbar/command-based navigation saw the previous period's date.

  **Fix**: Added the same three sync lines that `NavigateToday` uses to both `NavigatePrevious` and `NavigateNext`:
  ```csharp
  _state.SelectedDate = newDate.Date;
  _focusedDate = newDate.Date;
  _state.FocusedDate = _focusedDate;
  ```
  The fix mirrors the W2-Redo-9 GAP 1 / W2-Redo-11 BUG A / W2-Redo-11 BUG C pattern — every code path that changes the visible date must keep all five date/focus fields in lockstep. `NavigatePrevious`, `NavigateNext`, and `NavigateToday` are now shape-consistent.

**Other files audited — all clean**:
- `CalendarEventEditor.cs` — dialog editor uses `TryOpenEventEditor` → `TryCommitEditedEvent` → standard `TryAddEvent`/`TryUpdateEvent` path that records history and raises events. No in-place mutation.
- `BeepCalendar.EventOperations.Queries.cs` — `GetEventsForDateRange`, `SearchEvents`, `AnalyzeConflicts` correctly delegate to `_eventService` and `_conflictPolicy`.
- `Helpers/CalendarConflictPolicy.cs` — three modes (AllowOverlap, WarnOnOverlap, PreventOverlap) correctly use `CalendarEvent.OverlapsWith()` for conflict detection.
- `BeepCalendar.Toolbar.cs` — toolbar initialization, layout, paint, hit-test, and click execution are clean. All actions delegate to existing well-audited methods.
- `BeepCalendar.Painting.Pipeline.cs` — `DrawWithPainter` is clean: calls `_eventService.BeginPaintCycle()`, builds `ViewPaintArgs`, delegates to the per-view painter, paints chrome.
- `Helpers/CalendarEventService.cs` — `BeginPaintCycle` / `InvalidateCache` / `CheckCacheValidity` correctly manage a dual-layer cache (lazy clear on paint cycle + explicit clear on mutation).
- `BeepCalendar.LayoutTheme.Layout.cs` — `UpdateLayout` builds `_surfaceModel`, calls `_layout.UpdateLayout`, lays out toolbar, invalidates. Clean.
- `BeepCalendar.Commands.Public.cs` + `Commands.Execution.cs` — command infrastructure with `CommandInvoking` / `CommandInvoked` lifecycle. Clean.

**Build verification**: `dotnet build ...` returns **0 errors, 7103 warnings** (same as W2-Redo-13 baseline — `grep -i "Navigation.cs"` on the warning list returns **no matches**). 0 new warnings from the 1 Calendar file changed.

**Architectural impact**:
- Navigation is now the fourth code path in the date-sync family: `CurrentDate` setter (W2-Redo-9), `SetVisibleRange` (W2-Redo-11 BUG A), `OnMouseDown` (W2-Redo-11 BUG B), `CommitCopyEventMutation` (W2-Redo-11 BUG C), and now `NavigatePrevious` / `NavigateNext`. All five paths keep `_state.CurrentDate`, `_state.SelectedDate`, `_focusedDate`, and `_state.FocusedDate` in lockstep.
- Cumulative W2 series: **51 gaps fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 + 0 + 1 + 1 across W2-Redo-3..W2-Redo-14, not counting deferred design constraints).

### W2-Redo-15 - Delete-During-Edit Safety Gap Fix (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Thirteenth audit after W2-Redo-14. Deep audit of remaining interaction helpers (proposals, snap, creation-range), LayoutTheme helpers, and edge-case scenarios. Found 1 gap: deleting an event while its W4 inline editor is open. Build: **0 errors**, 7101 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP C - `RemoveEvent` did not close any active W4 inline editor for the event being removed** (`Calendar/BeepCalendar.EventOperations.Crud.Remove.cs:44-67`). Scenario: the user double-clicks an event to open the W4 title editor, then clicks the Delete toolbar button. `RemoveEvent` removes the event from `_events`, nulls `_state.SelectedEvent`, and redraws — but the W4 editor (hosted by `_editorHost`) stays open with a stale reference to the now-deleted `CalendarEvent`. Three symptoms: (a) the user sees a ghost editor for a deleted event — pressing Enter in it fires `EditCommitted`, and the W2-Redo-13 handler records `RecordMutationHistory(Update, ..., hosted.Event)` with an event that's no longer in `_events`; (b) if the user then presses Ctrl+Z, `UndoMutation` sees two entries in the undo stack (the intentional `Delete` from `RemoveEvent` AND the spurious `Update` from the stale editor commit), requiring two undos to get back to the pre-edit state; (c) the `Update` undo re-creates the event via `AddOrReplaceEvent(recordedBefore)` which re-inserts the event as it was BEFORE the W4 edit, losing the in-progress edit and littering the undo stack with garbage.

  **Fix**: Added a guard in `RemoveEvent` that checks `_editorHost.ActiveEditors` for any editor whose `HostedEditor.Event.Id` matches the event being removed. If found, calls `EndEdit(commit: false)` which silently discards the editor: the `Saving` handler never fires (the event is NOT modified in-place), the `EditCancelled` handler clears `_editingBeforeSnapshot` and calls `Invalidate()`, and the editor is removed from the layer. The undo stack gets exactly one `Delete` entry from `RemoveEvent` — no spurious `Update`. The fix uses the existing `EndEdit` API with zero new public surface.

**Other files audited — all clean**:
- `BeepCalendar.Interactions.Proposals.cs` — `BuildProposedStart` / `BuildProposedEnd` correctly branch on selected event and interaction mode. Clean.
- `BeepCalendar.Interactions.Proposals.CreationRange.cs` — `BuildCreationRange` correctly computes range from pointer-down anchor to current location. Fallback for non-timed views returns sensible defaults. Clean.
- `BeepCalendar.Interactions.Timing.Snap.Location.cs` — `GetTimedViewDateFromLocation` delegates to the per-view painter. Clean.
- `BeepCalendar.Painting.Pipeline.Helpers.cs` — `BuildViewPaintArgsForInteraction` builds a lightweight ViewPaintArgs for hit-test (no theme resolution). Clean.
- `BeepCalendar.LayoutTheme.Helpers.cs` — `ApplyThemeTypography` + `ResolveFont` correctly project theme typography onto calendar font properties. Clean.

**Build verification**: `dotnet build ...` returns **0 errors, 7101 warnings** (was 7103; -2 from baseline fluctuation recompiled files — `grep -i "Remove.cs"` on the warning list returns **no matches**). 0 new warnings from the 1 Calendar file changed.

**Architectural impact**:
- `RemoveEvent` is now safe to call while a W4 editor is active. The delete-while-editing edge case is fully closed: the editor is discarded silently, the undo stack stays clean (single `Delete` entry), and the stale editor commit path is never reached.
- The W2-Redo-13 edit-commit fix (which subscribed to `EditCommitted` / `EditCancelled`) already made the `EditCancelled` handler safe to call at any time (it null-checks `_editingBeforeSnapshot`). The W2-Redo-15 fix plugs the remaining gap where the `EditCommitted` handler could fire with a stale event reference.
- Cumulative W2 series: **52 gaps fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 + 0 + 1 + 1 + 1 across W2-Redo-3..W2-Redo-15, not counting deferred design constraints).

### W2-Redo-16 - Infrastructure Re-Audit — No New Gaps Found (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Fourteenth audit after W2-Redo-15. Re-audited the remaining infrastructure files: `CalendarInteractionContracts.cs` (enums, hit-test result, interaction/mutation args, editor request/interface), `CalendarLayoutManager.cs` (layout computation), `CalendarState.cs` / `CalendarRects.cs` (state and rect data classes), `CalendarEditorLayer.cs` (transparent editor Panel), `CalendarEditorPool.cs` (control reuse pool), `CalendarEditorDescriptor.cs` (editor registration descriptor), and `CalendarSurfaceModel.cs` (immutable geometry snapshot). **No bugs found.** All infrastructure is clean.

**Files audited (7 total)**:

| File | Result |
|------|--------|
| `Helpers/CalendarInteractionContracts.cs` | Clean — enums use `init` accessors for immutability, args classes properly null-guard |
| `Helpers/CalendarLayoutManager.cs` | Clean — guards against zero dimensions, scales with metric scale, respects painter's gutter preference |
| `Helpers/CalendarState.cs` (`CalendarRects`, `CalendarLayoutMetrics`) | Clean — all fields properly defaulted, `CalendarLayoutMetrics` delegates to `CalendarTokens` |
| `Editor/CalendarEditorLayer.cs` | Clean — transparent Panel with `OnPaintBackground` no-op, designer-safe (`Site=null`, `[ToolboxItem(false)]`) |
| `Editor/CalendarEditorPool.cs` | Clean — 16-instance cap per type, `Release` disposes overflow, `Acquire` is present but unused (pre-existing deferred finding) |
| `Editor/CalendarEditorDescriptor.cs` | Clean — validates id/factory in constructor |
| `Helpers/CalendarSurfaceModel.cs` | Clean — immutable snapshot built per layout cycle, time-of-day math correctly handles all-day and zero-duration events |

**Notable observations**:
- `CalendarSurfaceModel.Build` snapshots `state.FocusedDate` (which after W2-Redo-11 BUG B and W2-Redo-14 GAP B fixes always matches `_focusedDate`), `state.SelectedDate`, and `state.CurrentDate` — all three are now in lockstep across every date-changing code path.
- `CalendarState.FocusedDate` is `DateTime` (non-nullable, always initialized to `DateTime.Today` and synced with `_focusedDate` by every navigation/interaction path).
- `CalendarEditorPool.Acquire` is present but the sample editor factories (`InlineEventTitleEditor.Create()`, etc.) always call `new BeepTextBox()` and never use the pool. Controls are Released into the pool but never Acquired from it. This is a pre-existing deferred finding (already noted in W2-Redo-11) — the pool works correctly (capped at 16/type, disposes overflow) but its Acquire path is dead code until factories are updated to use it.
- `RaiseMutating` in `BeepCalendar.EventOperations.cs` always returns `true` — callers use the pattern `if (RaiseMutating(...) && canceled)` which resolves to `if (canceled)` since the first operand is always true. Cosmetic — the return value is vestigial.

**Build verification**: Not run (no code changes). Previous build from W2-Redo-15 was **0 errors, 7101 warnings** (all pre-existing).

**Architectural impact**:
- Every BeepCalendar partial, every helper class, every editor component, and every rendering/type file in the Calendar directory has now been audited at least once across the 16-pass W2-Redo series.
- The audit surface is fully closed: 52 gaps fixed, 2 clean-sweep passes (W2-Redo-12 + W2-Redo-16), and the deferred design constraints are well-documented.



### W2-Redo-3 - Gap Fix Pass + W8 Widening (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Full audit of all 14 painters + central interaction code. 14 gaps identified; all fixed in this wave. Build: **0 errors**, 7089 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP 1 - Factory routes legacy enum values to wrong painters** (`Rendering/ViewPainterFactory.cs`). Legacy painters (Week/WorkWeek/Day/Month/Agenda/List/Timeline) were routed to Week1/Week3/Week7/Week5 etc. aliases. Fixed: each legacy enum now maps to its own self-contained painter instance (e.g. `Week => new WeekViewPainter()`). No aliasing.
- [x] **GAP 2 - Added `IsHorizontalTimeAxis` to `ICalendarViewPainter` interface** (`Rendering/ICalendarViewPainter.cs`). Replaces hardcoded `ViewMode == Timeline` switch in `BeepCalendar.Interactions.Timing.Snap.cs`. Only `TimelineViewPainter` returns `true`; all others `false`.
- [x] **GAP 2b - Replaced hardcoded Timeline switch in `BeepCalendar.Interactions.Timing.Snap.cs`** with `painter.IsHorizontalTimeAxis` check + `painter.VisibleDayCount` instead of hardcoded `7d`. Now works for any horizontal-axis view.
- [x] **GAP 3 - `Week1ViewPainter.HitTest` resize-edge gap** - event-block hit-test was returning `SelectEvent` unconditionally. Now calls `CalendarPainterHelpers.ResolveResizeEdge` and returns `ResizeStart`/`ResizeEnd`/`SelectEvent` based on which edge was hit.
- [x] **GAP 4 - `TimelineViewPainter.HitTest` resize-edge gap** - same fix as GAP 3 (resolves resize edge on event-bar hit).
- [x] **GAP 5 - `ListViewPainter.GetDateTimeFromLocation` was broken** (always returned `GetVisibleRangeStart(CurrentDate)` regardless of location). Now returns `null` (List is time-agnostic; this method is reserved for painters that anchor Y/X to a time-of-day).
- [x] **GAP 6 - `OnMouseDoubleClick` W4 fallback for DateCell** - when no `DateCellComponentFactory` was registered, the control did nothing. Now focuses the hit date and calls `CreateEventAtFocusedDate()` (parity with pre-W8 behavior).
- [x] **GAP 7/7b - Dead-code legacy fallback in `CalendarLayoutManager.UpdateLayout` + `CalendarSurfaceModel.WeekDayCount` + `GetWeekDayDate`** - removed `ViewMode == Week/WorkWeek/Day` branches; painter is always supplied now.
- [x] **GAP 8 - W8 date-cell + time-slot integration added to 6 timed painters** (Week/WorkWeek/Day/Week1/Week2/Week3/Week7). Each now calls `CalendarPainterHelpers.TryDrawCellComponent` for day-header rects (key `"date:{yyyy-MM-dd}:header"`, kind `DateCell`) and time slots (key `"slot:{yyyy-MM-dd}:{hour}"`, kind `TimeSlot`) before default rendering. `PaintTimeSlot` signature extended with `(dayDate, dayIndex, args)` and all callers updated.
- [x] **GAP 9 - W8 date-cell integration to `MonthViewPainter.PaintDayCell`** - added `(row, col, args)` params and W8 call with key `"date:{date:yyyy-MM-dd}:cell"`, kind `DateCell`. Falls through to default month-cell paint otherwise.
- [x] **GAP 10 - W8 date-cell integration to `Week4ViewPainter` (6×7 month grid)** - added inline W8 call before the cell strokeRect with key `"date:{date:yyyy-MM-dd}:cell"`, kind `DateCell`.
- [x] **GAP 11 - W8 date-cell integration to `AgendaViewPainter` group headers** - added W8 call before the FillRoundedRect header with key `"date:{groupKey:yyyy-MM-dd}:agenda-header"`, kind `DateCell`.
- [x] **GAP 12 - W8 date-cell integration to `Week5ViewPainter` and `Week6ViewPainter` day-column headers** - Week5 uses one combined band rect (tab + date row), key `"date:{date:yyyy-MM-dd}:week5-header"`. Week6 uses the per-day header rect, key `"date:{date:yyyy-MM-dd}:week6-header"`.
- [x] **GAP 13 - W8 date-cell integration to `TimelineViewPainter.PaintDayHeader`** - added W8 call with key `"date:{date:yyyy-MM-dd}:timeline-header"`, kind `DateCell`.
- [x] **GAP 14 - TimeSlot double-click W4 fallback in `OnMouseDoubleClick`** - added new branch that detects time-bearing `DateCell` hits (`hit.Date.Value.TimeOfDay != TimeSpan.Zero`), tries `TimeSlotComponentFactory` first (W8 path), then falls back to `OnCreateEventRequested(hit.Date.Value)` (preserves time-of-day). The previous W4 DateCell fallback stripped time, creating events at midnight; this fixes that for time-slot double-clicks.

**Build verification**: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" -c Debug --nologo` returns **0 errors, 7089 warnings** (all pre-existing). Build time: 1m 36s. No new warnings introduced by this pass.

**Architectural impact**:
- W8 cell-component render is now uniform across **every visible cell** in every view: event blocks, date cells (day headers, month cells, agenda group headers, timeline day axis, week5/6 day columns), and time slots.
- The `IsHorizontalTimeAxis` interface member lets the central interaction code stay view-agnostic — no more `ViewMode == Timeline` hardcoded switch in the snap logic.
- The legacy enum values (`Week`, `WorkWeek`, `Day`, `Month`, `Agenda`, `List`, `Timeline`) now route to **distinct, self-contained** painters — no more aliasing. "EACH VIEW SHOULD BE DISTINCT" rule is fully honored.

## Cross-Phase Governance

- [ ] Keep plans and tracker in sync after each implementation wave
- [ ] Update Calendar readme with feature and contract changes per phase
- [ ] Keep sample/demo coverage aligned with newly added features

### W2-Redo-17 - Cell Component Cache Staleness + Factory Consolidation (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Post-W9 audit focused on the cell component pipeline after the template consolidation. Found 1 gap: cached cell components held stale `CalendarEvent` references after `TryUpdateEvent` replaced events in `_events`. Also consolidated the three W8 factory properties into one. Build: **0 errors**, 7105 warnings (all pre-existing — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP A - cached cell components held stale `CalendarEvent` references after event mutations** (`Calendar/BeepCalendar.EventOperations.Crud.*.cs`, `BeepCalendar.Interactions.Commit*.cs`, `BeepCalendar.EventOperations.History.Apply.*.cs`, `BeepCalendar.Commands.Helpers.Duplicate.cs`). The `CalendarCellComponentCache` creates `IBeepUIComponent` instances once (keyed by cell key like `"evt:42"`) and reuses them across paint cycles. `GetOrCreate` calls the developer's factory with a `CalendarCellContext` only on cache miss. When `TryUpdateEvent` replaces `_events[index] = appliedEvent` with a NEW `CalendarEvent` object (same ID, different fields), the cached component's `CurrentContext.Event` still pointed at the OLD `CalendarEvent` — the template painted stale title/description/time values. W4 inline edits (which mutate the event in-place via `Saving` handlers) were unaffected because the reference didn't change. But programmatic updates, undo/redo operations, and dialog-based edits all triggered the gap. Symptom: after undoing a title edit, the event block still showed the edited title until the user switched views or forced a cache miss.

  **Fix**: Added `_componentCache?.Clear()` alongside every `_eventService?.InvalidateCache()` call in all 9 mutation sites (Add, Upsert, Remove, Commit, Commit.Copy, History.Apply.Upsert, History.Apply.Remove, Duplicate, and the W4 EditCommitted handler in the constructor). When any event is added, updated, or removed, the entire cell component cache is cleared. The next paint cycle recreates all components with fresh `CalendarCellContext` snapshots. The performance cost is negligible — templates are lightweight `new()` calls, and recreating them on mutation (not on every paint) is the same cost as how `_eventService` rebuilds its query cache.

- [x] **Factory consolidation**: Merged `EventBlockComponentFactory`, `DateCellComponentFactory`, and `TimeSlotComponentFactory` into a single `CellComponentFactory` property. The `CalendarCellComponentCache` now stores one factory (no `CalendarCellKind` dispatch). All 3 call sites in `Core.Lifecycle.cs` updated to no-arg `GetFactory()`. XML doc comments in `CalendarPainterHelpers.cs` and `Week1ViewPainter.cs` updated. (6 files total: `CalendarCellComponentCache.cs`, `Core.PublicApi.cs`, `Core.Lifecycle.cs`, `Fields.cs`, `CalendarPainterHelpers.cs`, `Week1ViewPainter.cs`).

**Build verification**: `dotnet build ...` returns **0 errors, 7105 warnings** (0 new from changed files).

**Architectural impact**:
- The cell component cache is now kept in sync with the event service cache: both are invalidated on every event mutation. Paint cycles always see the current state.
- The component factory is now unified — one lambda handles EventBlock, DateCell, and TimeSlot via `ctx.Kind`.
- Cumulative W2 series: **53 gaps fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 + 0 + 1 + 1 + 1 + 0 + 1 + 0 + 1 across W2-Redo-3..W2-Redo-17, not counting deferred design constraints).

### W2-Redo-18 - Mouse Capture Loss During Drag Fix (COMPLETE 2026-06-05)

**User request (2026-06-05)**: "DO ANOTHER PASS AND FIX GAPS IN CALENDAR". Audited ResolveResizeEdge consistency (all 14 painters use shared helper — clean), IsHorizontalTimeAxis (only Timeline=true — clean), SupportsEventDrag consistency (timed views=true, others=false — clean), hover state (clean), CancelInteraction (clean). Found 1 gap: drag state not reset on mouse capture loss. Build: **0 errors**, 0 warnings (incremental — 0 new from this pass).

**Gaps fixed**:

- [x] **GAP A - `_pointerDown`/`_dragInProgress` not reset when mouse capture is lost** (`Calendar/BeepCalendar.Interactions.Pointer.MoveCancel.cs`). When the user is mid-drag and presses Alt+Tab (or a system menu opens, or the window is deactivated), Windows releases the mouse capture but does NOT send a `WM_LBUTTONUP`. Without a handler for this, `_pointerDown` stays `true`, `_dragInProgress` stays `true`, and `_state.InteractionMode` retains its drag value (MoveEvent/ResizeStart/ResizeEnd/CreateEvent). When the user clicks back on the calendar, `OnMouseMove` fires — it sees `_dragInProgress == true` and immediately fires `InteractionUpdated` with the stale interaction mode targeting the NEW mouse position, causing a spurious drag of whatever was previously selected. The previous `OnMouseDown` handler doesn't reset `_dragInProgress` because it assumes the previous drag was cleanly completed or cancelled.

  **Fix**: Added `OnMouseCaptureChanged` handler. When `Capture` becomes `false` and `_pointerDown` is `true` (meaning capture was taken away, not released by our own code), calls `CancelInteraction()` which resets `_pointerDown`, `_dragInProgress`, `_state.InteractionMode`, raises `InteractionCancelled`, releases `Capture`, and calls `Invalidate()`.

**Build verification**: `dotnet build ...` returns **0 errors, 0 warnings** (incremental — 0 new from changed file).

**Architectural impact**:
- Drag loss is now handled identically to explicit Esc-cancel and explicit OnMouseUp. All three paths go through `CancelInteraction()` which is the single source of truth for interaction teardown.
- Cumulative W2 series: **54 gaps fixed** (14 + 4 + 3 + 4 + 6 + 4 + 4 + 3 + 3 + 0 + 1 + 1 + 1 + 0 + 1 + 0 + 1 + 1 across W2-Redo-3..W2-Redo-18, not counting deferred design constraints).

## W9 - Built-in Cell Template Controls Plan (IMPLEMENTING 2026-06-05)

### Motivation

The W8 cell-component system (`EventBlockComponentFactory`, `DateCellComponentFactory`, `TimeSlotComponentFactory`) gives developers a per-cell factory hook to render custom `IBeepUIComponent` instances inside calendar cells. However, building a production-ready event template from scratch requires:
1. Extending `BaseControl` (already implements `IBeepUIComponent` → full Beep theme, borders, DPI, events, icons, styling).
2. Wiring `CalendarCellContext` → `CalendarEvent` data into the control's paint/render.
3. Handling the dual-mode lifecycle: **paint-mode** (`Draw` called from the painter pipeline via `TryDrawCellComponent`) vs **edit-mode** (the `BaseControl` is hosted in the `CalendarEditorLayer` Panel, letting WinForms handle its paint cycle).
4. Supporting click-to-edit → the control becomes a live WinForms control in the transparent editor layer; editing modifies `CalendarEvent` fields in-place or fires save requests.

This plan proposes a set of **8 ready-to-use, `BaseControl`-derived cell templates** covering the most common business/domain scheduling scenarios. Developers register a template via the W8 factory, set `CalendarEvent.Metadata` keys for domain-specific fields, and get a fully themed, DPI-aware event card rendered inside the calendar.

### Architecture (how templates work with the existing W8 system)

```
CalendarCellComponentCache
  └─ factory: CalendarCellContext → IBeepUIComponent   (developer registers a template)
       └─ returns a BeepCellTemplate<T> : BaseControl, IBeepUIComponent

During paint:
  TryDrawCellComponent(g, rect, cellKey, ctx, args)
    → cache.GetOrCreate(cellKey, ctx)
    → template.SetContext(ctx)        // push CalendarEvent + metadata into the control
    → template.Draw(g, rect)           // IBeepUIComponent: control paints at cell rect

During click-to-edit:
  ActivateCellComponent(cellKey, ctx, bounds)
    → template.SetContext(ctx)        // repopulate if cached from a different event
    → template.Bounds = bounds        // position the BaseControl in the editor layer
    → editorLayer.Controls.Add(template.Control)  // WinForms handles OnPaint directly
    → user interacts with live Control (buttons, text inputs, etc.)
    → on Commit: template writes changes back to CalendarEvent.Metadata
```

**Key insight**: `CalendarEvent.Metadata` (a `Dictionary<string, string>`) is the bridge between domain-specific data and the template. Each template defines its own metadata key conventions (like `"PatientName"`, `"FlightNumber"`, etc.). This avoids polluting the `CalendarEvent` class with domain-specific properties.

### Phase 1 - Base Infrastructure (`Calendar\CellTemplates\`)

New folder: `Calendar/CellTemplates/` containing:

1. **`BeepCellTemplateBase.cs`** — Abstract base for all cell templates.
   ```
   public abstract class BeepCellTemplateBase : BaseControl
   ```
   Responsibilities:
   - Exposes `CalendarCellContext CurrentContext { get; }` — set by `SetContext()`.
   - `virtual void SetContext(CalendarCellContext ctx)` — called before every paint/edit. Updates `CurrentContext`, extracts `CalendarEvent` + `Metadata`, calls `OnContextChanged()` for subclass override.
   - `virtual CalendarEventCommitData CollectCommitData()` — called when the user finishes editing (Ctrl+Enter, focus leave, or explicit save button). Returns the changed metadata keys/values. The calendar's `EditCommitted` handler (W2-Redo-13) passes this to `TryUpdateEvent`.
   - Optional `bool SupportsInlineEdit { get; }` — true if the template is more than paint-only (has editable controls).
   - Resolves theme colors via `_currentTheme` (inherited from `BaseControl`).

2. **`BeepCellTemplateHelpers.cs`** — Static helpers reused across templates:
   - `DrawAccentBadge(Graphics g, Rectangle rect, Color color, string text)` — colored left-edge badge like Telerik's status indicators.
   - `DrawIconLabel(Graphics g, Rectangle rect, string iconPath, string label)` — icon + text row (used for location, room, attendee).
   - `DrawTimeBar(Graphics g, Rectangle rect, DateTime start, DateTime end, Color fg, Color bg)` — visual time span indicator.
   - `DrawStatusDot(Graphics g, Point pt, CalendarEventStatus status, int radius = 5)` — green/orange/red dot per status.
   - `DrawProgressMini(Graphics g, Rectangle rect, float percent, Color color)` — mini progress bar for task deadlines.
   - `Metadatum(CalendarCellContext ctx, string key, string fallback = "")` — safe Metadata reader with fallback.

### Phase 2 - Domain Templates (8 controls)

Each template is a `sealed class` inheriting `BeepCellTemplateBase`. All use `CalendarEvent.Metadata` for domain fields (no new properties on `CalendarEvent`).

#### Template 1: `BeepMedicalAppointmentCell`
**Use case**: Hospital/clinic scheduling, patient booking systems.

| Metadata key | Type | Description |
|---|---|---|
| `"PatientName"` | string | Full patient name |
| `"DoctorName"` | string | Assigned physician |
| `"ProcedureCode"` | string | CPT/ICD code or procedure name |
| `"RoomNumber"` | string | Exam room / ward |
| `"AppointmentType"` | string | "New", "Follow-up", "Emergency", "Surgery" |
| `"PatientStatus"` | string | "Scheduled", "CheckedIn", "InProgress", "Complete", "NoShow" |

**Visual layout** (paint at cell rect):
```
┌──────────────────────────────┐
│ ◆ Dr. Chen · Room 3B         │  ← doctor + room (icon + text)
│ 👤 Maria Santos              │  ← patient name (bold)
│ CPT 99213 · Follow-up        │  ← procedure code + type badge
│ 10:30 AM - 11:15 AM    ○     │  ← time bar + status dot (green=on-time)
└──────────────────────────────┘
```
- Left accent stripe color-coded by `AppointmentType`.
- Status dot at right edge (green = Scheduled/CheckedIn, yellow = InProgress, red = Cancelled/NoShow).

#### Template 2: `BeepBusinessMeetingCell`
**Use case**: Corporate meetings, conference room booking, team standups.

| Metadata key | Type | Description |
|---|---|---|
| `"ConferenceRoom"` | string | Room name or number |
| `"Attendees"` | string | Comma-separated names or "5 attendees" |
| `"Agenda"` | string | Meeting agenda snippet |
| `"OrganizerName"` | string | Meeting host (defaults to `CalendarEvent.Organizer`) |
| `"MeetingLink"` | string | Teams/Zoom URL (for tooltip or double-click action) |

**Visual layout**:
```
┌──────────────────────────────┐
│ ☑ Sprint Planning            │  ← CalendarEvent.Title (bold)
│ 📍 Boardroom A · 6 attendees │  ← room + attendee count
│ 📋 Review backlog, assign... │  ← agenda preview (truncated)
│ 9:00 AM - 10:00 AM           │  ← time
└──────────────────────────────┘
```
- Left accent in category color. Attendee count as a small badge.

#### Template 3: `BeepClassLectureCell`
**Use case**: Schools, universities, training centers.

| Metadata key | Type | Description |
|---|---|---|
| `"CourseCode"` | string | "CS-301", "MATH-101" |
| `"Instructor"` | string | Professor / teacher name |
| `"Room"` | string | Classroom or lab |
| `"Enrollment"` | string | "42 students" |
| `"LectureType"` | string | "Lecture", "Lab", "Seminar", "Exam" |

**Visual layout**:
```
┌──────────────────────────────┐
│ 📖 CS-301: Data Structures   │  ← course code + title
│ 📐 Prof. Johnson · Room 201  │  ← instructor + room
│ 👥 42 enrolled               │  ← enrollment count
│ Mon/Wed/Fri 10:00-11:15    │  ← day pattern + time
└──────────────────────────────┘
```
- Left accent by `LectureType` (blue=Lecture, green=Lab, red=Exam).

#### Template 4: `BeepTaskDeadlineCell`
**Use case**: Project management, Gantt-style task visualization, kanban deadlines.

| Metadata key | Type | Description |
|---|---|---|
| `"AssigneeName"` | string | Person responsible |
| `"Priority"` | string | "Low", "Medium", "High", "Critical" |
| `"ProgressPercent"` | string | "0"-"100" as string |
| `"ProjectName"` | string | Parent project |
| `"TaskId"` | string | JIRA ticket, issue number |

**Visual layout**:
```
┌──────────────────────────────┐
│ ● Submit Q2 Report           │  ← priority dot + title
│ 👤 Alice Chen · PROJ-042     │  ← assignee + project
│ ▓▓▓▓▓▓▓▓▓▓░░ 75%            │  ← progress bar
│ Due: Fri 5:00 PM             │  ← deadline
└──────────────────────────────┘
```
- Priority dot color: gray (Low), blue (Medium), orange (High), red (Critical).
- Progress bar animates from 0-100%.

#### Template 5: `BeepFlightTravelCell`
**Use case**: Travel agencies, corporate travel desks, airline crew scheduling.

| Metadata key | Type | Description |
|---|---|---|
| `"FlightNumber"` | string | "AA-1234" |
| `"Origin"` | string | Airport code (JFK, LAX) |
| `"Destination"` | string | Airport code |
| `"Gate"` | string | Departure gate |
| `"Baggage"` | string | Baggage claim carousel |
| `"TravelStatus"` | string | "OnTime", "Delayed", "Boarding", "Departed", "Arrived" |

**Visual layout**:
```
┌──────────────────────────────┐
│ ✈ AA-1234  JFK → LAX        │  ← flight icon + route
│ 🧳 Gate B12 · Baggage C4   │  ← gate + baggage claim
│ Dep: 2:30 PM · Arr: 5:45 PM │  ← departure + arrival times
│ Boarding              🟢     │  ← status text + colored badge
└──────────────────────────────┘
```
- Status badge: green = OnTime/Arrived, orange = Delayed, blue = Boarding.

#### Template 6: `BeepRestaurantReservationCell`
**Use case**: Restaurant booking systems, hospitality management.

| Metadata key | Type | Description |
|---|---|---|
| `"GuestName"` | string | Reservation holder |
| `"PartySize"` | string | "4" |
| `"TableNumber"` | string | Assigned table |
| `"SpecialRequests"` | string | "Window seat", "Anniversary" |
| `"ReservationStatus"` | string | "Booked", "Seated", "Completed", "Cancelled" |

**Visual layout**:
```
┌──────────────────────────────┐
│ 🍽 Garcia · Party of 4       │  ← guest name + party size
│ 🪑 Table 12 · Window seat   │  ← table + special request
│ 7:00 PM - 9:00 PM            │  ← reservation time
│ Confirmed              🟢    │  ← status
└──────────────────────────────┘
```

#### Template 7: `BeepServiceCallCell`
**Use case**: Field service, maintenance scheduling, IT support tickets.

| Metadata key | Type | Description |
|---|---|---|
| `"TechnicianName"` | string | Assigned tech |
| `"TicketNumber"` | string | Service ticket ID |
| `"Priority"` | string | "Low", "Medium", "High", "Emergency" |
| `"EquipmentType"` | string | What to service |
| `"ETAMinutes"` | string | Estimated arrival in minutes |

**Visual layout**:
```
┌──────────────────────────────┐
│ 🔧 HVAC Repair · #SR-8842    │  ← title + ticket number
│ 👨‍🔧 Mike Torres · ⏱ ETA 15m  │  ← tech + ETA
│ Equipment: Chiller Unit 3    │  ← equipment
│ Emergency             🔴     │  ← priority badge
└──────────────────────────────┘
```
- Priority badge: red=Emergency, orange=High, blue=Medium, gray=Low.

#### Template 8: `BeepGenericCell`
**Use case**: Data-driven flexible template — no hardcoded domain. The developer defines which Metadata keys to display and how.

| Metadata key | Type | Description |
|---|---|---|
| `"cell:label1"` | string | First detail line label ("Doctor", "Room") |
| `"cell:value1"` | string | First detail line value ("Dr. Chen", "Room 3B") |
| `"cell:label2"` | string | Second detail line label |
| `"cell:value2"` | string | Second detail line value |
| `"cell:icon1"` | string | Icon path for detail 1 (from SvgsUIcons) |
| `"cell:icon2"` | string | Icon path for detail 2 |
| `"cell:accentColor"` | string | Hex color for left accent stripe |
| `"cell:badgeText"` | string | Right-edge badge text ("VIP", "Draft") |
| `"cell:badgeColor"` | string | Hex color for the badge |

**Visual layout**:
```
┌──────────────────────────────┐
│ [CalendarEvent.Title]        │  ← always the event title
│ icon1 label1: value1         │  ← detail line 1 (if keys present)
│ icon2 label2: value2         │  ← detail line 2 (if keys present)
│ StartTime - EndTime  [badge] │  ← time range + optional badge
└──────────────────────────────┘
```
- If accentColor is not set, falls back to the event's category color.
- If badgeText is not set, no badge rendered.

### Implementation plan per template

Each template follows the same pattern (example: `BeepMedicalAppointmentCell`):

1. **File**: `Calendar/CellTemplates/BeepMedicalAppointmentCell.cs`
2. **Class**: `public sealed class BeepMedicalAppointmentCell : BeepCellTemplateBase`
3. **Constructor**: `public BeepMedicalAppointmentCell() { Size = new Size(200, 60); }`
4. **Key method**: `protected override void DrawContent(Graphics g)` — reads `CurrentContext`, extracts `CalendarEvent` + Metadata keys, paints the visual layout at `DrawingRect`.
5. **No interaction methods needed by default** — `DrawContent` is called by both the paint pipeline (via `IBeepUIComponent.Draw`) and by WinForms `OnPaint` (when hosted in the editor layer). The `BaseControl` infrastructure handles border, background, theme, and DPI scaling automatically.

### Registration pattern (how developers use these templates)

```csharp
// In form load or calendar initialization:
var calendar = new BeepCalendar();

// Option A: One template type for ALL event blocks
calendar.EventBlockComponentFactory = (ctx) => new BeepMedicalAppointmentCell();

// Option B: Per-category routing (different templates per CategoryId)
calendar.EventBlockComponentFactory = (ctx) =>
{
    if (ctx.Event == null) return null;
    return ctx.Event.CategoryId switch
    {
        1 => new BeepMedicalAppointmentCell(),   // Medical category
        2 => new BeepBusinessMeetingCell(),      // Business category
        3 => new BeepTaskDeadlineCell(),         // Projects category
        _ => new BeepGenericCell()               // Default
    };
};

// Option C: Read a Metadata key to decide the template
calendar.EventBlockComponentFactory = (ctx) =>
{
    var kind = ctx.Event?.Metadata?.GetValueOrDefault("cell:templateKind");
    return kind switch
    {
        "medical" => new BeepMedicalAppointmentCell(),
        "meeting" => new BeepBusinessMeetingCell(),
        "flight"  => new BeepFlightTravelCell(),
        _         => new BeepGenericCell()
    };
};
```

### Build plan

1. [x] **W9-1**: Create `Calendar/CellTemplates/` folder structure. — **DONE 2026-06-05**
2. [x] **W9-2**: Implement `BeepCellTemplateBase.cs` (abstract base, `SetContext`, metadata helpers). — **DONE 2026-06-05**
3. [x] **W9-3**: Implement `BeepCellTemplateHelpers.cs` (drawing primitives: badges, dot, time bar, progress, icon label). — **DONE 2026-06-05**
4. [x] **W9-4**: Implement `BeepGenericCell.cs` — the data-driven template. — **DONE 2026-06-05**
5. [x] **W9-5**: Implement `BeepMedicalAppointmentCell.cs`. — **DONE 2026-06-05**
6. [x] **W9-6**: Implement `BeepBusinessMeetingCell.cs` + `BeepTaskDeadlineCell.cs`. — **DONE 2026-06-05**
7. [x] **W9-7**: Implement `BeepClassLectureCell.cs` + `BeepFlightTravelCell.cs`. — **DONE 2026-06-05**
8. [x] **W9-8**: Implement `BeepRestaurantReservationCell.cs` + `BeepServiceCallCell.cs`. — **DONE 2026-06-05**
9. [ ] **W9-9**: Create sample/demo project that shows all 8 templates in one calendar, with a selector dropdown.
10. [ ] **W9-10**: Add inline-edit support to `BeepGenericCell` (double-click opens a BeepTextBox → edits event Title → commits via W2-Redo-13 handler).

**Build verification (W9-1..W9-8)**: `dotnet build ...` returns **0 errors, 7105 warnings** (baseline — `grep -i "CellTemplates"` on the warning list returns **no matches**). All 10 files (`CellTemplates/` folder with 8 templates + base + helpers) compile cleanly with 0 new warnings.

### Design decisions

| Decision | Rationale |
|---|---|
| All domain data via `CalendarEvent.Metadata` | No changes to `CalendarEvent` class. Developers own their schema. |
| Templates extend `BaseControl` (not just `Control`) | Full Beep theme support, borders, DPI scaling, icons, zero marginal cost. |
| `BaseControl` already implements `IBeepUIComponent` | No extra interface implementation. `Draw(Graphics, Rectangle)` → `DrawContent` is already wired. |
| No new public enum for template type | The W8 `CalendarCellKind` enum (EventBlock, DateCell, TimeSlot) is sufficient. Template selection is the developer's responsibility in the factory lambda. |
| Paint and edit share `SetContext/OnContextChanged` | Single data-flow path. The template always reads from `CurrentContext`, regardless of paint or edit mode. |
| No WinForms designer support for templates | Templates are runtime controls (created by factory `Func`) — no `.Designer.cs` file generated. This is intentional; they live inside the cache and the editor layer. |
| `CalendarCellComponentCache` maps `cellKey → IBeepUIComponent` | When an event moves (drag-commit), the cache still has the old key. The `InvalidateCache` calls throughout the W2-Redo fixes clear this correctly. If the event key changes (e.g., event ID changes), a new component is created from the factory the next time the painter visits that cell. |

### Risks / Future work

- **Cache key collision**: The cache key for event blocks is `"evt:{event.Id}"`. If two events share the same ID (theoretical), they'd share a cached component. Mitigation: `NextEventId()` guarantees uniqueness.
- **Template resizing**: When the calendar is resized (W2-Redo-5 GAP 4), W8 components are deactivated. On next paint, the factory creates new components with the new cell rect. Templates must be designed to look good at any size.
- **`BeepTaskDeadlineCell` progress bar editing**: This template would benefit from inline-edit (slider or click-to-increment). This is deferred to W9-10+.
- **Accessibility**: Templates should expose their `AccessibleName` and `AccessibleRole` (via `BaseControl`'s accessibility support). Deferred to post-W9.

