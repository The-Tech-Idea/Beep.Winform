# Phase 1 - Foundation And Core Contracts

Priority: High
Status: In Progress (implementation-complete for core contracts; manual visual baseline pending)
Depends on: Existing Calendar render pipeline

## Goal

Establish a stable, token-driven, view-agnostic foundation so all subsequent features can be added without architectural churn.

## Why This Phase First

Current architecture already has strong seams (state, layout manager, renderer strategy, style painter strategy). This phase formalizes contracts and consistency rules so advanced scheduler features do not fragment behavior.

## Scope

- Core rendering and style contracts
- Layout and invalidation discipline
- Calendar design tokens and density policies
- Navigation command model and state transitions

## Current Completion Snapshot (2026-05-12)

- Completed:
  - Shared calendar tokens introduced and mapped for core layout metrics.
  - Render context normalized with interaction and visible-range state accessors.
  - Canonical day-cell state helper introduced and wired into month rendering paths.
  - Command surface added (`GoToToday`, previous/next navigation, `SwitchView`, `SetVisibleRange`) with command lifecycle events.
  - Layout/repaint coalescing added via deferred visual update scope to reduce redundant invalidation churn.
  - Phase 1 validation checklist document created.
- Pending:
  - Baseline screenshot capture for density, view, DPI, and theme matrix.

## Planned Workstreams

### W1 - Token System And UI Consistency

- Introduce CalendarTokens class for:
  - Header and toolbar heights
  - Grid and event paddings
  - Corner radii
  - State overlay alpha values
  - Typography size tiers
- Align MaterialCalendarPainter and MinimalCalendarPainter to tokens first, fallbacks second.
- Add density modes (dense, compact, comfortable) with deterministic metrics.

### W2 - Rendering Contract Hardening

- Refine CalendarRenderContext to include:
  - Effective density and zoom/DPI metrics
  - Active interaction state
  - Visible range and clipping bounds
- Ensure all ICalendarViewRenderer implementations consume the same state contract.
- Eliminate duplicated metric math inside individual view renderers.

### W3 - Navigation And Command Surface

- Add canonical command methods in BeepCalendar for:
  - GoToToday
  - NavigatePrevious/NavigateNext
  - SwitchView(mode)
  - SetVisibleRange(start, end)
- Add lightweight command event routing so host apps can intercept/override behavior.

### W4 - Invalidation And Layout Reliability

- Define invalidation policy (full, partial, data-only, style-only).
- Improve layout manager resilience on resize and sidebar toggles.
- Prevent unnecessary recompute/repaint churn during multi-property updates.

### W5 - Pipeline Consolidation (cross-ref Phase 6)

- Delete the legacy `ICalendarViewRenderer` / `CalendarRenderer` / `*ViewRenderer.cs` path AND the `ICalendarStylePainter` / `MaterialCalendarPainter` / `MinimalCalendarPainter` / `CalendarPainterFactory` stack. **No** style painter hierarchy remains. The replacement is one `ICalendarViewPainter` per `CalendarViewMode` (Month / Week / WorkWeek / Day / Agenda / Timeline / List) under `Rendering/ViewPainters/`, each consuming `IBeepTheme` + `BeepControlStyle` directly via `ViewPaintArgs` + `CalendarStyleMetrics.For(style)`.
- Promote reusable geometry into a new `Helpers/CalendarSurfaceModel.cs` (immutable, built per `UpdateLayout`).
- Delete 12+ empty / stub partial files.
- Remove `_usePainterSystem` flag, `UsePainterSystem` property, `_renderer` field, and `_renderer.HandleClick(...)` call.
- Full workstream: `06-pipeline-consolidation-and-editor-layer.md` W1-W2.

### W6 - Editor Layer (cross-ref Phase 6)

- New `Editor/CalendarEditorHost.cs`, `Editor/CalendarEditorLayer.cs`, `Editor/CalendarEditorDescriptor.cs`, `Editor/HostedEditor.cs`, `Editor/CalendarEditorPool.cs`.
- `BeepCalendar` overrides `OnPaint` AND `OnPaintBackground` to apply `Region.Exclude(_editorLayer.Bounds)` clip.
- `BeepCalendar` overrides `AllowBaseControlClear => false` and `IsContainerControl => true`.
- Sample editors: `InlineEventTitleEditor` (BeepTextBox), `InlineEventDateRangeEditor` (BeepDateTimePicker x2), `InlineAllDayToggleEditor` (BeepCheckBoxBool).
- XUnit tests in `Beep.Winform.Controls.Tests/Calendar/BeepCalendarTests_EditorLayer.cs`.
- Full workstream: `06-pipeline-consolidation-and-editor-layer.md` W3-W5.

## UX/Visual Standards Applied

- Figma-style component structure: header, selector, grid, sidebar as reusable layout regions.
- State clarity: hover/focus/selected/today/disabled states visually distinct and tokenized.
- Minimum interaction target aligned with modern standards for clickable date and event surfaces.

## Deliverables

1. Calendar token file and usage rollout.
2. Updated render context and renderer contract documentation.
3. Command surface API for navigation/view changes.
4. Phase 1 validation checklist and screenshot baseline set.

## Exit Criteria For Phase 1 Close

- Complete all rows in `phase1-validation-checklist.md`.
- Attach baseline screenshot evidence for required scenarios.
- Update `TODO-TRACKER.md` to mark Phase 1 complete and open Phase 2 execution.

## Definition Of Done

- Tokens drive all major layout and state visuals in existing views.
- Renderer contract is consistent across month/week/day/list implementations.
- Command-driven navigation is deterministic and unit-testable.
- No regressions in current views after token and layout normalization.
- **W5:** single paint path; per-view `ICalendarViewPainter` (Month / Week / WorkWeek / Day / Agenda / Timeline / List) consumes `IBeepTheme` + `BeepControlStyle` via `ViewPaintArgs` + `CalendarStyleMetrics.For(style)`; legacy `ICalendarViewRenderer` files AND `ICalendarStylePainter` / Material / Minimal painters all deleted; `_usePainterSystem` / `_renderer` / `UsePainterSystem` / `_stylePainter` removed; `CalendarSurfaceModel` consumed by every painter and hit-test helper.
- **W6:** `BeepCalendar` overrides `OnPaint` and `OnPaintBackground`; `_editorLayer` is a single child of `Controls`; double-clicking an event opens an inline `BeepTextBox` for the title; Esc cancels, Enter commits; XUnit tests pass.

## Risks

- Hidden view-specific assumptions may resist contract normalization.
- Mitigation: keep adapter shims for one iteration and remove after verification.
