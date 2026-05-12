# Phase 2 Validation Matrix

Purpose: verify interaction parity and event-editing behavior for BeepCalendar Phase 2.

Status key:
- `Pass` = verified in manual run
- `Pass (Code)` = verified by implemented source path/contract
- `Fail` = observed defect
- `N/A` = not applicable for current view/flow
- `Pending` = not run yet

## Environment Baseline

- Build/profile: Pending
- Theme: Pending
- DPI scale: Pending
- View mode set: Month / Week / Day / List
- Conflict policy mode: AllowOverlap / WarnOnOverlap / PreventOverlap

## Interaction Parity Matrix

| Scenario | Month | Week | Day | List | Expected Result | Status | Notes |
|---|---|---|---|---|---|---|---|
| Click empty surface selects date | Pass (Code) | Pass (Code) | Pass (Code) | N/A | SelectedDate updates and DateSelected fires once | Pass (Code) | `OnMouseDown` selection + `DateSelected` invocation path is implemented for hit-test date targets. |
| Click event selects event | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | SelectedEvent updates and EventSelected fires once | Pass (Code) | Event hit targets update selection in `OnMouseDown` and emit `EventSelected`. |
| Drag to create event | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | New event created with snapped start/end | Pass (Code) | Day/Week derive range from drag anchor + current pointer; commit path persists snapped start/end. |
| Drag move event | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | Event time range shifts; duration preserved | Pass (Code) | Move mutation path updates event and enforces normalized duration. |
| Resize start handle | N/A | Pass (Code) | Pass (Code) | Pass (Code) | Start time changes, end remains stable, min duration enforced | Pass (Code) | Resize-start mutation kind resolves and duration is normalized. |
| Resize end handle | N/A | Pass (Code) | Pass (Code) | Pass (Code) | End time changes, start remains stable, min duration enforced | Pass (Code) | Resize-end mutation kind resolves and duration is normalized. |
| Copy with modifier on move | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | Cloned event created; source unchanged | Pass (Code) | Copy path clones event and assigns new ID when move+modifier is active. |
| Modifier on resize does not copy | N/A | Pass (Code) | Pass (Code) | Pass (Code) | Resize mutates source event only | Pass (Code) | Copy gate is restricted to `MoveEvent` mode only. |
| Snap-to-grid applied | N/A | Pass (Code) | Pass (Code) | N/A | Start/end align to InteractionSnapIntervalMinutes | Pass (Code) | Delta/date snapping flows through `SnapMinutes` and `InteractionSnapIntervalMinutes`. |
| Interaction completion commit flag accuracy | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | `InteractionCompleted.IsCommit` reflects actual mutation success | Pass (Code) | Completion event now uses commit result, not drag-threshold-only state. |
| Live conflict preview pointer fidelity | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | Conflict preview evaluates current pointer proposal while dragging | Pass (Code) | Interaction updates/completion pass current location+delta into conflict evaluation. |
| Escape cancels active interaction | Pass (Code) | Pass (Code) | Pass (Code) | Pass (Code) | Active drag/create interaction cancels without commit and emits cancel flow | Pass (Code) | `ProcessCmdKey` handles `Keys.Escape` with early return and routes to `CancelInteraction()`. |

## Conflict Policy Matrix

| Scenario | AllowOverlap | WarnOnOverlap | PreventOverlap | Expected Result | Status | Notes |
|---|---|---|---|---|---|---|
| Create overlapping event | Pass (Code) | Pass (Code) | Pass (Code) | Policy behavior respected; conflict notifications raised as configured | Pass (Code) | `CanSchedule` allows overlap for Allow/Warn and blocks on conflicts for Prevent. |
| Move into overlap | Pass (Code) | Pass (Code) | Pass (Code) | Policy behavior respected; mutation accepted or blocked accordingly | Pass (Code) | Move mutation checks policy before commit and raises `ConflictDetected` when blocked/conflicted. |
| Resize into overlap | Pass (Code) | Pass (Code) | Pass (Code) | Policy behavior respected; mutation accepted or blocked accordingly | Pass (Code) | Resize mutation uses same policy-check gate as move. |
| Copy into overlap | Pass (Code) | Pass (Code) | Pass (Code) | Policy behavior respected for copy path | Pass (Code) | Copy commit path policy-checks candidate clone before add. |

## Editor Contract Matrix

| Scenario | QuickEdit | DialogEdit | Expected Result | Status | Notes |
|---|---|---|---|---|---|
| Open from double-click selected event | Pass (Code) | Pass (Code) | Editor opens and returns edited event or cancel | Pass (Code) | `OnMouseDoubleClick` routes through `TryOpenEventEditor`. |
| Open from keyboard selected event (`F2`) | Pass (Code) | Pass (Code) | Editor opens for selected event using same mutation/edit path as mouse double-click | Pass (Code) | `ProcessCmdKey(Keys.F2)` routes through shared selected-event editor flow. |
| Open from command selected event (`EditSelectedEvent`) | Pass (Code) | Pass (Code) | Editor opens for selected event through command surface with deterministic command result | Pass (Code) | `CalendarCommandType.EditSelectedEvent` routes through shared selected-event editor flow. |
| Open create flow from keyboard (`Ctrl+N`) | Pass (Code) | Pass (Code) | Create-event flow opens from focused date via keyboard without mouse interaction | Pass (Code) | `ProcessCmdKey(Ctrl+N)` routes to `OnCreateEventRequested(_focusedDate.Date)`. |
| Open create flow from command (`CreateEventAtFocusedDate`) | Pass (Code) | Pass (Code) | Create-event flow opens from focused date via host command surface | Pass (Code) | `CalendarCommandType.CreateEventAtFocusedDate` routes through the same focused-date create pipeline. |
| Jump to Today from keyboard (`Ctrl+T`) | Pass (Code) | Pass (Code) | Today navigation occurs from keyboard and matches toolbar Today action | Pass (Code) | `ProcessCmdKey(Ctrl+T)` routes to `GoToToday()`. |
| Navigate period from keyboard (`Ctrl+Left`/`Ctrl+Right`) | Pass (Code) | Pass (Code) | Previous/next period navigation occurs from keyboard and matches toolbar arrows | Pass (Code) | `ProcessCmdKey(Ctrl+Left/Ctrl+Right)` routes to `NavigatePreviousPeriod()` / `NavigateNextPeriod()`. |
| Open from create-event entry point | Pass (Code) | Pass (Code) | Editor opens with proposed event seed values | Pass (Code) | `OnCreateEventRequested` routes through `TryOpenEventEditor`. |
| Cancel editor | Pass (Code) | Pass (Code) | No mutation committed; fallback event callbacks are not invoked | Pass (Code) | Editor returns handled-without-commit and caller suppresses fallback callbacks. |
| Save editor with invalid range | Pass (Code) | Pass (Code) | End is normalized to be after start | Pass (Code) | Editor and calendar mutation paths normalize end > start. |
| Save updates core fields | Pass (Code) | Pass (Code) | Title/start/end/location/status persist | Pass (Code) | Editor writes core fields and commit routes through add/update APIs. |

## Mutation Event Coverage Matrix

| Scenario | EventMutating | EventMutated | Expected Result | Status | Notes |
|---|---|---|---|---|---|
| Add event via API | Pass (Code) | Pass (Code) | Both events raised with create mutation payload | Pass (Code) | `TryAddEvent` invokes mutating/mutated around commit. |
| Update event via API | Pass (Code) | Pass (Code) | Both events raised with update mutation payload | Pass (Code) | `TryUpdateEvent` invokes mutating/mutated around commit. |
| Remove event via API | Pass (Code) | Pass (Code) | Both events raised with delete mutation payload | Pass (Code) | `RemoveEvent` invokes mutating/mutated around delete. |
| Move/resize interaction commit | Pass (Code) | Pass (Code) | Both events raised with move/resize mutation kind | Pass (Code) | Interaction commit paths raise mutation events for move/resize. |
| Copy interaction commit | Pass (Code) | Pass (Code) | Both events raised with copy mutation kind and copy flag | Pass (Code) | Copy path raises mutation events with copy semantics. |
| Cancel in EventMutating handler | Pass (Code) | N/A | Mutation blocked and EventMutated not raised | Pass (Code) | APIs short-circuit when mutating args set `Cancel = true`. |
| Undo/redo mutation replay | Pass (Code) | Pass (Code) | Undo and redo restore event snapshots without recursive history growth | Pass (Code) | `UndoLastMutation` / `RedoLastMutation` replay `CalendarMutationRecord` with history suspension. |
| Undo/redo command and keyboard access | Pass (Code) | Pass (Code) | History replay is reachable through command API and keyboard shortcuts | Pass (Code) | `CalendarCommandType.UndoMutation/RedoMutation` and `ProcessCmdKey` shortcuts are wired. |
| Undo/redo toolbar access | Pass (Code) | Pass (Code) | History replay is reachable through visible toolbar controls | Pass (Code) | Toolbar buttons call `UndoMutation` / `RedoMutation` and reflect `CanUndo`/`CanRedo`. |
| Undo/redo failed replay stack safety | Pass (Code) | Pass (Code) | Failed replay does not consume undo/redo stack entries | Pass (Code) | Undo/redo use peek-first replay and pop only after successful apply. |
| Delete selected event command/keyboard | Pass (Code) | Pass (Code) | Selected event can be deleted via command API and `Delete` key with deterministic result | Pass (Code) | `CalendarCommandType.DeleteSelectedEvent` and `ProcessCmdKey(Keys.Delete)` route through `RemoveEvent`. |
| Public delete by ID robustness | Pass (Code) | Pass (Code) | Deletion succeeds for matching event ID even when caller passes a different object instance | Pass (Code) | `RemoveEvent` resolves target event by ID before mutation/remove flow. |
| Duplicate selected event command/keyboard | Pass (Code) | Pass (Code) | Selected event can be duplicated through command API and `Ctrl+D`, producing a new shifted copy | Pass (Code) | `CalendarCommandType.DuplicateSelectedEvent` reuses copy mutation semantics with a one-day offset. |
| Duplicate selected event toolbar access | Pass (Code) | Pass (Code) | Duplicate action is visible in the toolbar and enabled only when a selection exists | Pass (Code) | `_duplicateEventButton` calls `DuplicateSelectedEvent()` and tracks `SelectedEvent` state. |
| Edit selected event toolbar access | Pass (Code) | Pass (Code) | Edit action is visible in the toolbar and enabled only when a selection exists | Pass (Code) | `_editEventButton` calls `EditSelectedEvent()` and tracks `SelectedEvent` state. |
| Delete selected event toolbar access | Pass (Code) | Pass (Code) | Delete action is visible in the toolbar and enabled only when a selection exists | Pass (Code) | `_deleteEventButton` calls `DeleteSelectedEvent()` and tracks `SelectedEvent` state. |
| Command success determinism | Pass (Code) | Pass (Code) | Command API returns true only when action succeeds and surfaces `Succeeded` flag | Pass (Code) | `ExecuteCommandCore` returns bool and updates `CalendarCommandEventArgs.Succeeded`. |
| Keyboard no-op consumption (`Delete`/`F2`) | Pass (Code) | Pass (Code) | Shortcut keys are consumed in no-selection states without fallback key-beep behavior | Pass (Code) | `ProcessCmdKey` always returns true for `Delete`/`F2` after invoking operation attempt. |

## Search/Filter Matrix

| Scenario | Status | Expected Result | Notes |
|---|---|---|---|
| Text filter on title/description | Pass (Code) | Matching events returned only | Implemented in `GetFilteredEvents` search predicate. |
| Category filter | Pass (Code) | Category-constrained result set | Implemented in `GetFilteredEvents` category clause. |
| Status filter | Pass (Code) | Status-constrained result set | Implemented in `GetFilteredEvents` status clause. |
| Date range filter | Pass (Code) | Overlap-aware range results | Implemented with overlap range predicate. |

## Code-Contract Execution Snapshot (2026-05-12)

- Scope executed: source-level parity verification (no runtime screenshot/manual interaction evidence in this pass).
- Result: all Phase 2 rows above are satisfied at implemented contract/code-path level.
- Remaining evidence work: manual UI walkthrough across Month/Week/Day/List with recorded outcomes and screenshots.

## Exit Criteria For Phase 2

- All interaction parity rows are `Pass` or explicitly `N/A`.
- Conflict policy rows are `Pass` for all three policy modes.
- Editor contract rows are `Pass` for both quick-edit and dialog-edit.
- Mutation event coverage rows are `Pass`, including cancellation behavior.
- Any `Fail` rows are linked to tracked fixes before Phase 3 kickoff.
