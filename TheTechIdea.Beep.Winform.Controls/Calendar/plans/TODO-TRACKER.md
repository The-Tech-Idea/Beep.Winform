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

(Plan: `06-pipeline-consolidation-and-editor-layer.md`. Build: **BROKEN** during W2 view painter installation. 0 errors / 6996 warnings before W2 deletions. Working tree has uncommitted W2 partial changes.)

### W1 - CalendarSurfaceModel

- [x] Create `Helpers/CalendarSurfaceModel.cs` (immutable, built per `UpdateLayout`)
- [x] Migrate geometry math from `BeepCalendar.LayoutTheme.*` and `Helpers/CalendarLayoutGeometry.cs:8-150` into the model
- [x] Promote `CalendarState` + `CalendarRects` to `public` so the model factory is callable
- [x] Promote `CalendarInteractionHitTestResult` to `public` so `ICalendarViewPainter.HitTest` can return it
- [x] Update all painters / view-painter methods to consume the model (deferred to W2 view painter files)
- [x] Update all hit-test helpers to consume the model (deferred to W2 view painter files)

### W2 - Per-View Painter Pipeline (in progress, build broken)

**Architecture pivot (2026-06-04)**: NO `ICalendarStylePainter` / `MaterialCalendarPainter` / `MinimalCalendarPainter` / `CalendarPainterFactory`. The only painter abstraction is the per-view `ICalendarViewPainter` which consumes `IBeepTheme` + `BeepControlStyle` directly via `ViewPaintArgs` + `CalendarStyleMetrics.For(style)`.

- [x] Create `Rendering/ICalendarViewPainter.cs` (interface: `ViewMode`, `Layout(ViewPaintArgs)`, `Paint(Graphics, ViewPaintArgs)`, `HitTest(Point, ViewPaintArgs)`)
- [x] Create `Rendering/ViewPaintArgs.cs` (single args bundle: theme, control style, state, rects, surface, event service, events, categories, resources, fonts, hover/selected, owner, resolved color palette, `CalendarStyleMetrics`; methods `ApplyTheme(IBeepTheme)`, `ApplyThemeFonts()`, `ResolveThemeColors()`, `GetCategoryColor(int)`)
- [x] Create `Rendering/ViewPainterFactory.cs` (static `GetPainter(CalendarViewMode)` with `Dictionary<CalendarViewMode, ICalendarViewPainter>` cache + `Reset()` test hook)
- [x] Delete `Rendering/ICalendarViewRenderer.cs`, `CalendarRenderer.cs`, `CalendarRenderContext.cs`, `CommonDrawing.cs`, all `*ViewRenderer.cs`
- [x] Delete `Rendering/ICalendarStylePainter.cs`, `StylePainters/MaterialCalendarPainter.cs`, `StylePainters/MinimalCalendarPainter.cs`, `StylePainters/` directory, `CalendarPainterFactory.cs`
- [x] Promote reusable helpers from `CommonDrawing.cs` (e.g. `RoundedRect`) into `Helpers/CalendarDrawingPrimitives.cs`
- [x] Remove `BeepCalendar.Painting.Pipeline.Legacy.cs:13 DrawWithLegacyRenderer`
- [ ] Create `Rendering/ViewPainters/MonthViewPainter.cs` (Layout + Paint + HitTest; switches on `ViewPaintArgs.ControlStyle` for Material3 / Minimal variants; uses `StyledImagePainter` + `SvgsUIcons` for icon painting)
- [ ] Create `Rendering/ViewPainters/WeekViewPainter.cs`
- [ ] Create `Rendering/ViewPainters/WorkWeekViewPainter.cs`
- [ ] Create `Rendering/ViewPainters/DayViewPainter.cs`
- [ ] Create `Rendering/ViewPainters/AgendaViewPainter.cs` (month events grouped by day)
- [ ] Create `Rendering/ViewPainters/TimelineViewPainter.cs` (resource lanes × day axis; carries lane virtualization from deleted `TimelineViewRenderer.cs`)
- [ ] Create `Rendering/ViewPainters/ListViewPainter.cs`
- [ ] Update `BeepCalendar.Fields.cs` (remove `_stylePainter`, `_renderer`, `_usePainterSystem`; add `_viewPainter`)
- [ ] Update `BeepCalendar.Core.Constructor.cs` (remove `_renderer` + `_stylePainter` initialization; add `_viewPainter = ViewPainterFactory.GetPainter(_state.ViewMode)`; call `ViewPaintArgs.ApplyTheme(_currentTheme)`)
- [ ] Update `BeepCalendar.Core.Style.cs` (remove `UsePainterSystem` property; `CalendarStyle` setter only updates `_calendarStyle` + `Invalidate()`)
- [ ] Update `BeepCalendar.Core.PublicApi.cs` (ViewMode setter swaps `_viewPainter` via `ViewPainterFactory.GetPainter(mode)` + `RequestLayoutAndRedraw()`)
- [ ] Update `BeepCalendar.Core.Lifecycle.cs:OnMouseClick` (remove `_renderer.HandleClick(...)` + `new CalendarRenderContext(...)`; call `_viewPainter.HitTest(location, args)`)
- [ ] Update `BeepCalendar.Painting.cs` (drop `_usePainterSystem` check; always call `DrawWithPainter`)
- [ ] Update `BeepCalendar.Painting.Pipeline.cs:DrawWithPainter` (resolve `ViewPaintArgs` once; call `_viewPainter.Layout(args)` + `_viewPainter.Paint(g, args)`; chrome stays here)
- [ ] Update `BeepCalendar.Painting.Pipeline.Views.cs` (replace switch dispatch with direct `_viewPainter.Paint(g, args)` call or fold into `DrawWithPainter`)
- [ ] Delete `BeepCalendar.Painting.MonthView.cs`, `Painting.WeekView.cs`, `Painting.DayView.cs`, `Painting.ListView.cs`, `Painting.Sidebar.cs`, `Painting.MonthView.Events.cs`, `Painting.MonthView.Headers.cs`, `Painting.WeekView.Events.cs`
- [ ] Delete `BeepCalendar.Interactions.HitTesting.MonthView.cs`, `HitTesting.WeekView.cs`, `HitTesting.DayView.cs`, `HitTesting.ListView.cs`, `HitTesting.AgendaView.cs`, `HitTesting.TimelineView.cs`, `HitTesting.TimedView.cs`
- [ ] Update `BeepCalendar.Interactions.HitTesting.cs:ResolveInteractionTarget` (replace switch with `_viewPainter.HitTest(location, args)`)
- [ ] Update `BeepCalendar.LayoutTheme.Layout.cs` (remove `_stylePainter?.GetMetrics()?.CornerRadius`; use `CalendarStyleMetrics.For(_calendarStyle).CornerRadius`)
- [ ] Update `BeepCalendar.LayoutTheme.ApplyTheme.cs` (call `ViewPaintArgs.ApplyTheme(_currentTheme)` + `ViewPaintArgs.ApplyThemeFonts()`)
- [ ] Update `BeepCalendar.LayoutTheme.HeaderText.cs:DrawPainterHeaderText` (use `ViewPaintArgs.ForegroundColor` + `ViewPaintArgs.HeaderFont`)
- [ ] Extend `ViewPaintArgs.ApplyTheme(IBeepTheme)` to project theme colors into the resolved color palette
- [ ] Extend `ViewPaintArgs.ApplyThemeFonts()` to re-resolve `HeaderFont` / `DayFont` / `EventFont` / `TimeFont` / `DaysHeaderFont` from `theme.CalendarTitleFont` / `DaysHeaderFont` / `DateFont` / `CalendarSelectedFont` / `CalendarUnSelectedFont` via `BeepThemesManager.ToFont(theme.X)`
- [ ] Extend `CalendarStyleMetrics.For(BeepControlStyle)` to also resolve the default `IBeepTheme` via `BeepThemesManager.GetThemeNameForFormStyle(FormStyle)` + `BeepThemesManager.GetTheme(name)` and seed the default color palette
- [ ] Delete empty / stub partials: `Painting.Views.cs`, `Painting.Helpers.cs`, `Invalidation.cs`, `HitTesting.Views.cs`, `Pointer.cs`, `Commands.cs`, `EventOperations.Crud.cs`, `EventOperations.Public.cs`, `Core.cs`, `LayoutTheme.Controls.cs`, `LayoutTheme.ResponsiveLabels.cs`, `LayoutTheme.ResponsiveLabels.Assignments.cs`
- [ ] Build verification: `dotnet build "C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj" --nologo` returns 0 errors

### W3 - Editor Layer Infrastructure

- [ ] Create `Editor/CalendarEditorHost.cs` (BeginEdit / EndEdit / HitTest / AddEditor / RemoveEditor + EditStarted / EditCommitted / EditCancelled events)
- [ ] Create `Editor/CalendarEditorLayer.cs` (Panel subclass, transparent, SupportsTransparentBackColor)
- [ ] Create `Editor/CalendarEditorDescriptor.cs` (Id / DisplayName / SupportsInline / SupportsDialog / factory)
- [ ] Create `Editor/HostedEditor.cs` (wraps Control + descriptor + bounds + IsDirty)
- [ ] Create `Editor/CalendarEditorPool.cs` (reuses BeepTextBox / BeepCheckBoxBool / BeepDateTimePicker / BeepComboBox instances)
- [ ] Add `_editorLayer` and `_editorHost` fields in `BeepCalendar.Fields.cs`
- [ ] Instantiate + add to `Controls` in `BeepCalendar.Core.Constructor.cs:9-39`
- [ ] Add `AllowBaseControlClear => false` and `IsContainerControl => true` overrides in `BeepCalendar.cs`
- [ ] Add `OnPaint` and `OnPaintBackground` overrides in `BeepCalendar.cs` applying `Region.Exclude(_editorLayer.Bounds)` and restoring `Graphics.Clip` in `finally`
- [ ] Sync `_editorLayer.Bounds` in `BeepCalendar.Core.Lifecycle.cs:10 OnResize`

### W4 - Sample Editors

- [ ] Create `Editor/SampleEditors/InlineEventTitleEditor.cs` (BeepTextBox; Enter commits, Esc cancels)
- [ ] Create `Editor/SampleEditors/InlineEventDateRangeEditor.cs` (BeepDateTimePicker x2)
- [ ] Create `Editor/SampleEditors/InlineAllDayToggleEditor.cs` (BeepCheckBoxBool)
- [ ] Add public `BeginEdit(CalendarEvent, string editorId = "title")` and `EndEdit(bool commit = true)` on `BeepCalendar`
- [ ] Route `OnMouseDoubleClick` in `BeepCalendar.Core.Lifecycle.cs` through `TryBeginEditFromDoubleClick`
- [ ] Route `ProcessCmdKey(Escape)` to `EndEdit(false)` when an editor is active
- [x] Resolve open questions 1-6 (clip flag, ExcludedPaintRectangles wiring, design-time serialization, combo dropdown, check-box variant, legacy compile flag)
- [x] Decisions (2026-06-04): ignore Q1+Q2 (we override OnPaint); Q3 design-time `Site=null` + `[DesignerSerializationVisibility(Hidden)]`; Q4 `ToolStripDropDown` accepted; Q5 use `BeepCheckBoxBool` (binary only); Q6 Option A — delete legacy entirely

### W5 - Verification Demo + Tests

- [ ] Recreate `TheTechIdea.Beep.Winform.Controls.Tests/Calendar/` directory
- [ ] Create `BeepCalendarTests_EditorLayer.cs` with the 6 XUnit cases from `phase1-validation-checklist.md` W6
- [ ] Add WinForms sample that drops a `BeepCalendar`, sets 3 events, double-clicks one, screenshots inline `BeepTextBox`
- [ ] Run full solution build; ensure 0 errors and warning count does not regress
- [ ] Capture `phase1-validation-checklist.md` W5 + W6 screenshot evidence

## Cross-Phase Governance

- [ ] Keep plans and tracker in sync after each implementation wave
- [ ] Update Calendar readme with feature and contract changes per phase
- [ ] Keep sample/demo coverage aligned with newly added features
