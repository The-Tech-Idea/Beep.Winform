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

## Cross-Phase Governance

- [ ] Keep plans and tracker in sync after each implementation wave
- [ ] Update Calendar readme with feature and contract changes per phase
- [ ] Keep sample/demo coverage aligned with newly added features
