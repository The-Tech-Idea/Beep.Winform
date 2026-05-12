# Phase 2 - Event Model And Interaction Workflows

Priority: High
Status: In Progress (code-contract complete; manual evidence remaining)
Depends on: Phase 1 completion

## Current Completion Snapshot (2026-05-12)

- Completed in this wave:
  - Event domain contract expansion on `CalendarEvent` (recurrence, exceptions, reminders, status, metadata, timezone).
  - Conflict policy hook contract (`AllowOverlap`, `WarnOnOverlap`, `PreventOverlap`) with pluggable policy implementation.
  - Conflict analysis and notification hook on `BeepCalendar` (`ConflictDetected`).
  - Filter/search API contract (`CalendarEventFilter`) and service-backed filtered queries.
  - Date/range query behavior updated to overlap-aware matching for multi-day events.
  - Interaction contract scaffold added (`CalendarInteractionMode`, hit-test result/event args, pointer state, and mouse-down/up wiring).
  - Interaction preview layer now differentiates create, move, resize-start, and resize-end targets with tentative start/end values.
  - Interaction commit layer now applies create/move/resize mutations, supports copy-gesture cloning, and preserves selected-event state.
  - Timed interaction edits now snap to the configured interval and raise undo-friendly mutation events with before/after snapshots.
  - Event editor contract added (`ICalendarEventEditor`, `CalendarEventEditorRequest`, `CalendarEventEditorMode`) and wired into double-click/create entry points.
  - Default built-in editor implementation added with a Beep dialog host, quick-edit core fields, and dialog-edit advanced fields.
  - Drag modifier semantics refined so copy applies to move operations only and resize mutations stay on the original event.
  - Undo/redo mutation history contract added (`CanUndo`, `CanRedo`, `UndoLastMutation`, `RedoLastMutation`) with interaction/API history recording.
  - Undo/redo wired into command surface and keyboard shortcuts (`Ctrl+Z`, `Ctrl+Y`, `Ctrl+Shift+Z`).
  - Undo/redo toolbar buttons added and synchronized with history state.
  - Header text bounds updated to account for undo/redo controls and avoid title overlap.
  - Day/Week create-range interactions now derive start/end from drag anchor and current pointer to produce variable-duration event creation.
  - Interaction completion now reports real mutation commit success and conflict previews evaluate proposed candidate state.
  - Command pipeline now reports deterministic success/failure (`CalendarCommandEventArgs.Succeeded`) for host integration and keyboard/no-op handling.
  - Conflict preview evaluation now uses live pointer location/delta during interaction updates and completion events.
  - Escape key now cancels active interactions through an early-return path that avoids focus/date navigation side effects.
  - Undo/redo now applies mutation records with peek-first semantics so failed replay does not drop history entries.
  - Delete-selected command surface added and mapped to `Delete` keyboard shortcut with deterministic success/failure reporting.
  - Selected-event keyboard edit shortcut (`F2`) now routes through the same editor/fallback flow as mouse double-click.
  - Selected-event command surface now includes `EditSelectedEvent`, aligned with keyboard/mouse editor routing.
  - `Ctrl+N` keyboard shortcut now routes create-event requests through the same create/editor pipeline using focused date context.
  - `Delete` and `F2` keyboard handlers now consume key input even when no selected event exists, preventing fallback key-beep side effects.
  - Public delete API now resolves and removes events by ID, avoiding reference-equality-only deletion failures.
  - Host command surface now includes `CreateEventAtFocusedDate`, aligned with `Ctrl+N` and the focused-date create pipeline.
  - Duplicate selected-event flow now exists as a copy mutation command (`DuplicateSelectedEvent`) with `Ctrl+D` shortcut.
  - Duplicate selected-event flow now has toolbar access and enable-state wiring tied to the current selection.
  - Edit/delete selected-event flows now have toolbar access and enable-state wiring tied to the current selection.
  - `Ctrl+T` keyboard shortcut now routes to the Today navigation command.
  - `Ctrl+Left` and `Ctrl+Right` keyboard shortcuts now route to previous/next period navigation.
- Remaining in Phase 2:
  - Direct-manipulation interaction parity and runtime evidence coverage.
  - Execute and complete `phase2-validation-matrix.md` parity checks.

## Phase Transition Note

- Phase 3 has been promoted to the active workstream and will take priority for new implementation work.

## Goal

Bring BeepCalendar event handling and editing UX to modern scheduler expectations with robust event semantics and rich direct-manipulation interactions.

## Scope

- Event domain model expansion
- Drag/drop and resize interactions
- Conflict handling and editing policies
- Search/filter workflows

## Planned Workstreams

### W1 - Event Domain Expansion

Add/extend event model fields and contracts for:

- Recurrence rule and series identity
- Recurrence exceptions and detached instances
- Reminder metadata
- Event status and category semantics
- Custom metadata bag for line-of-business fields
- Optional timezone on event level

### W2 - Interaction Primitives

Implement shared interaction engine for:

- Click to create
- Drag to create range
- Drag to move
- Resize start/end
- Keyboard nudging and range extension
- Modifier behavior (copy/move semantics)

### W3 - Conflict And Policy Hooks

- Introduce conflict detection strategy interface.
- Add host-configurable policies:
  - Allow overlap
  - Warn on overlap
  - Prevent overlap
- Add visual conflict indicators in day/week/timeline-like surfaces.

### W4 - Editing UX Contracts

- Define quick-edit popover contract.
- Define full editor dialog contract.
- Provide validation callback points before save/update/delete.
- Add undo-friendly event mutation events.

### W5 - Search And Filtering

- Add event filtering by text, category, status, and date range.
- Add quick search surface with highlighted results.
- Add event source filtering hooks for external provider scenarios.

## UX/Visual Standards Applied

- Progressive disclosure: inline quick actions for simple edits, dialog for advanced fields.
- Strong interaction affordance: clear drag handles, snap indicators, drop targets.
- Figma-style interaction states: idle, hover, active-drag, invalid-drop, selected.

## Deliverables

1. Extended CalendarEvent model and compatibility adapters.
2. Shared interaction controller for move/resize/create flows.
3. Conflict policy interfaces and default implementation.
4. Search and filter API plus basic UI hooks.

## Definition Of Done

- Recurrence and exceptions can be represented and edited through contracts.
- Drag and resize behavior is consistent across supported views.
- Conflict policy can be swapped without renderer rewrite.
- Search/filter results are stable and correctly synchronized with renderers.

## Risks

- Recurrence edge cases may be complex.
- Mitigation: adopt ICS-compatible recurrence concepts and keep conversion helpers explicit.
