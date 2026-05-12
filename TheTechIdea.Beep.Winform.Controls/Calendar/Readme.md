# BeepCalendar

BeepCalendar is a BaseControl-derived scheduling surface with multiple views and style-painter rendering.

## Current Architecture


 Command surface (partial class): BeepCalendar.Commands.cs, BeepCalendar.Commands.Public.cs, BeepCalendar.Commands.Execution.cs, BeepCalendar.Commands.Execution.Core.cs, BeepCalendar.Commands.Helpers.cs
 Commands helpers duplicate (partial class): BeepCalendar.Commands.Helpers.Duplicate.cs
 Core constructor (partial class): BeepCalendar.Core.Constructor.cs
 Core public API appearance (partial class): BeepCalendar.Core.PublicApi.Appearance.cs
 Core lifecycle (partial class): BeepCalendar.Core.Lifecycle.cs
 Core public API events (partial class): BeepCalendar.Core.PublicApi.Events.cs
 Core public API (partial class): BeepCalendar.Core.PublicApi.cs
 Core style (partial class): BeepCalendar.Core.Style.cs
 Command type contracts (partial class): BeepCalendar.Types.Commands.cs
 Event args and filter contracts (partial class): BeepCalendar.Types.EventArgs.cs
 Event type contracts (partial class): BeepCalendar.Types.Events.cs
 Event operations CRUD add (partial class): BeepCalendar.EventOperations.Crud.Add.cs
 Event operations CRUD (partial class): BeepCalendar.EventOperations.Crud.cs
 Event operations CRUD remove (partial class): BeepCalendar.EventOperations.Crud.Remove.cs
 Event operations CRUD upsert (partial class): BeepCalendar.EventOperations.Crud.Upsert.cs
 Event operations editor (partial class): BeepCalendar.EventOperations.Editor.cs
 Event operations editor commit helper (partial class): BeepCalendar.EventOperations.Editor.Commit.cs
 Event operations history apply internals (partial class): BeepCalendar.EventOperations.History.Apply.cs
 Event operations history apply remove helper (partial class): BeepCalendar.EventOperations.History.Apply.Remove.cs
 Event operations history apply upsert helper (partial class): BeepCalendar.EventOperations.History.Apply.Upsert.cs
 Event operations history (partial class): BeepCalendar.EventOperations.History.cs
 Event operations history internals (partial class): BeepCalendar.EventOperations.History.Internal.cs
 Event operations navigation helpers (partial class): BeepCalendar.EventOperations.Navigation.cs
 Event operations (partial class): BeepCalendar.EventOperations.cs, BeepCalendar.EventOperations.Public.cs, BeepCalendar.EventOperations.Queries.cs
 Painting day view (partial class): BeepCalendar.Painting.DayView.cs
 Painting design-time (partial class): BeepCalendar.Painting.DesignTime.cs
 Painting helpers (partial class): BeepCalendar.Painting.Helpers.cs
 Painting list view (partial class): BeepCalendar.Painting.ListView.cs
 Painting month view (partial class): BeepCalendar.Painting.MonthView.cs
 Painting month view events helper (partial class): BeepCalendar.Painting.MonthView.Events.cs
 Painting month view headers helper (partial class): BeepCalendar.Painting.MonthView.Headers.cs
 Painting pipeline legacy (partial class): BeepCalendar.Painting.Pipeline.Legacy.cs
 Painting pipeline (partial class): BeepCalendar.Painting.Pipeline.cs
 Painting pipeline telemetry (partial class): BeepCalendar.Painting.Pipeline.Telemetry.cs
 Painting pipeline header formatting (partial class): BeepCalendar.Painting.Pipeline.HeaderFormatting.cs
 Painting pipeline view dispatch (partial class): BeepCalendar.Painting.Pipeline.Views.cs
 Painting sidebar (partial class): BeepCalendar.Painting.Sidebar.cs
 Painting views (partial class): BeepCalendar.Painting.Views.cs
 Painting week view (partial class): BeepCalendar.Painting.WeekView.cs
 Painting week view events helper (partial class): BeepCalendar.Painting.WeekView.Events.cs
 Layout/theme apply theme (partial class): BeepCalendar.LayoutTheme.ApplyTheme.cs
 Layout/theme header text (partial class): BeepCalendar.LayoutTheme.HeaderText.cs
 Layout/theme helpers (partial class): BeepCalendar.LayoutTheme.Helpers.cs
 Layout/theme month cells (partial class): BeepCalendar.LayoutTheme.MonthCells.cs
 Layout/theme controls (partial class): BeepCalendar.LayoutTheme.Controls.cs
 Layout/theme controls categories helper (partial class): BeepCalendar.LayoutTheme.Controls.Categories.cs
 Layout/theme layout (partial class): BeepCalendar.LayoutTheme.Layout.cs
 Layout/theme responsive labels (partial class): BeepCalendar.LayoutTheme.ResponsiveLabels.cs
 Layout/theme responsive labels assignments helper (partial class): BeepCalendar.LayoutTheme.ResponsiveLabels.Assignments.cs
 Interaction commit helpers (partial class): BeepCalendar.Interactions.Commit.cs
 Interaction commit copy helper (partial class): BeepCalendar.Interactions.Commit.Copy.cs
 Interaction commit new-event helper (partial class): BeepCalendar.Interactions.Commit.NewEvent.cs
 Interaction hit-testing helpers (partial class): BeepCalendar.Interactions.HitTesting.Helpers.cs
 Interaction hit-testing timed view (partial class): BeepCalendar.Interactions.HitTesting.TimedView.cs
 Interaction hit-testing helpers (partial class): BeepCalendar.Interactions.HitTesting.cs
 Interaction hit-testing day view (partial class): BeepCalendar.Interactions.HitTesting.DayView.cs
 Interaction hit-testing list view (partial class): BeepCalendar.Interactions.HitTesting.ListView.cs
 Interaction hit-testing month view (partial class): BeepCalendar.Interactions.HitTesting.MonthView.cs
 Interaction hit-testing week view (partial class): BeepCalendar.Interactions.HitTesting.WeekView.cs
 Interaction hit-testing views (partial class): BeepCalendar.Interactions.HitTesting.Views.cs
 Interaction pointer down/up (partial class): BeepCalendar.Interactions.Pointer.DownUp.cs
 Interaction pointer up handler (partial class): BeepCalendar.Interactions.Pointer.Up.cs
 Interaction pointer move/cancel (partial class): BeepCalendar.Interactions.Pointer.MoveCancel.cs
 Interaction pointer handlers (partial class): BeepCalendar.Interactions.Pointer.cs
 Interaction proposal creation range helper (partial class): BeepCalendar.Interactions.Proposals.CreationRange.cs
 Interaction proposal helpers (partial class): BeepCalendar.Interactions.Proposals.cs
 Interaction timing helpers (partial class): BeepCalendar.Interactions.Timing.cs
 Interaction timing snap location helper (partial class): BeepCalendar.Interactions.Timing.Snap.Location.cs
 Interaction timing snap helpers (partial class): BeepCalendar.Interactions.Timing.Snap.cs
 Visual update scope helper (partial class): BeepCalendar.VisualUpdates.Scope.cs
 Visual update flush helper (partial class): BeepCalendar.VisualUpdates.Flush.cs

- Converted BeepCalendar to a partial class to support cleaner feature separation.
- Added CalendarTokens as the shared source for layout and spacing constants.
- Mapped CalendarLayoutMetrics constants to CalendarTokens to keep existing call sites stable.
- Added a command-driven API:
  - GoToToday()
  - NavigatePreviousPeriod()
  - NavigateNextPeriod()
  - SwitchView(CalendarViewMode)
  - SetVisibleRange(DateTime start, DateTime end)
- Added command lifecycle events:
  - CommandInvoking (cancelable)
  - CommandInvoked
- Routed toolbar and keyboard page navigation through the command API.
- Normalized render-context interaction state (hover, focus, visible range) through CalendarState/CalendarRenderContext.
- Added shared day-cell state builder helper used by both painter and legacy month rendering paths.
- Added visual update coalescing (`BeginVisualUpdate` / `EndVisualUpdate` / `DeferVisualUpdate`) to reduce redundant layout + repaint churn.

## Phase 2 Foundations (2026-05-12)

- Expanded `CalendarEvent` contract with recurrence, exception, reminder, status, timezone, and metadata fields.
- Added conflict-policy hooks through a pluggable strategy:
  - `CalendarConflictPolicyMode` (`AllowOverlap`, `WarnOnOverlap`, `PreventOverlap`)
  - `ConflictDetected` event surfaced by `BeepCalendar`
- Added event filtering/search contract via `CalendarEventFilter` and service-backed filtering.
- Updated date and date-range event queries to overlap-aware matching so multi-day events are included correctly.
- Added interaction scaffold partial with pointer state, interaction events, and initial hit testing for date/event selection.
- Added interaction preview support for create/move/resize targets with proposed start/end timestamps.
- Added interaction commit support for create/move/resize and copy-gesture cloning, with selection/cache refresh.
- Added configurable snap-to-grid timing for interaction edits and mutation events that expose before/after snapshots for undo integration.
- Added injectable event editor contract for quick-edit/dialog-edit flows, wired into double-click and create-event entry points.
- Added a default built-in editor using a Beep dialog host with quick-edit core fields and dialog-edit advanced fields.
- Refined drag modifier semantics so copy only applies when moving events, not when resizing them.
- Added undo/redo mutation history support via `CanUndo`, `CanRedo`, `UndoLastMutation`, and `RedoLastMutation`.
- Added command and keyboard entry points for undo/redo (`UndoMutation`, `RedoMutation`, `Ctrl+Z`, `Ctrl+Y`, `Ctrl+Shift+Z`).
- Added toolbar undo/redo buttons with enabled-state binding to history availability.
- Updated header title layout bounds so toolbar controls (including undo/redo) do not overlap the title region.
- Updated Day/Week drag-to-create behavior to use pointer-down/pointer-up range for variable-duration event creation.
- Updated interaction completion semantics to report actual commit success and conflict previews to evaluate proposed candidate state.
- Updated command pipeline to return deterministic success/failure and expose command outcome via `CalendarCommandEventArgs.Succeeded`.
- Updated interaction conflict preview to evaluate live pointer proposals during drag updates/completion.
- Added Escape keyboard handling to cancel active interaction flows via early return through the shared cancel pathway.
- Updated undo/redo replay to preserve history stacks when a replay apply attempt fails.
- Added delete-selected command support and `Delete` keyboard shortcut with deterministic success outcomes.
- Added selected-event `F2` keyboard shortcut to invoke the shared event editor flow.
- Added `EditSelectedEvent` command API for host-level selected-event edit invocation.
- Added `Ctrl+N` keyboard shortcut to start create-event flow at the focused date.
- Updated `Delete` and `F2` keyboard handling to consume no-op key presses and avoid fallback key-beep behavior.
- Updated public delete behavior to remove events by ID (not reference-only) for safer host/API integration.
- Added `CreateEventAtFocusedDate` command API for host-level create flow at the focused date.
- Added `DuplicateSelectedEvent` command API and `Ctrl+D` shortcut for quick one-day event duplication.
- Added duplicate-event toolbar access that follows selection state.
- Added edit/delete toolbar access that follows selection state.
- Added `Ctrl+T` keyboard shortcut to jump to Today.
- Added `Ctrl+Left` and `Ctrl+Right` shortcuts for previous/next period navigation.

## Phase 3 Focus (2026-05-12)

- Phase 3 is now the active implementation stream and work-week/agenda view wiring is underway.
- Work week, agenda, and timeline renderers now exist in the calendar renderer pipeline.
- Resource lane foundation is now present through `CalendarResource` and event resource assignment fields.
- Timeline lane virtualization is now in place (visible-lane rendering + lane event pre-indexing per draw cycle).
- Query and paint performance counters with warning thresholds are now available through `PerformanceMetrics` on `BeepCalendar`.
- Next work will center on broader virtualization coverage, deeper telemetry, and multi-resource behavior.

## Design Principles

- Keep rendering contracts stable and view-agnostic.
- Use helper classes for reusable behavior and metrics.
- Keep BeepCalendar public API explicit and host-friendly.
- Maintain theme-aware, DPI-aware layout and visuals.

## Next Planned Work

- Add work-week and agenda renderers as first-class views.
- Add timeline renderer and resource lane model.
- Add resource grouping and resource header presentation.
- Introduce viewport virtualization strategy for dense schedules.
- Add paint and query performance counters with thresholds.
- Create phase 3 performance benchmark scenarios.
