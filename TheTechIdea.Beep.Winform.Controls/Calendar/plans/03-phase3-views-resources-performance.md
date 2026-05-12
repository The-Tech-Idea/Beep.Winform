# Phase 3 - Advanced Views, Resources, And Performance

Priority: High
Status: In Progress
Depends on: Phase 2 completion

## Current Completion Snapshot (2026-05-12)

- Phase 3 kickoff is active and implementation has started.
- Initial scope focus: additional view renderers and resource model foundations are now implemented for work week, agenda, and timeline lanes.
- Initial viewport strategy is now implemented for timeline lanes by rendering only visible lane ranges and pre-indexing lane events per draw cycle.
- Paint and query performance counters with thresholds are now available through calendar performance metrics.
- Remaining scope focus: broader virtualization coverage and benchmark scenarios.

## Goal

Expand BeepCalendar into a multi-scenario scheduler with resource-aware views and robust performance under enterprise event volumes.

## Scope

- New view renderers
- Resource model and grouping
- Virtualization and rendering optimization
- Performance instrumentation

## Planned Workstreams

### W1 - Additional View Renderers

Add first-class support for:

- Work week view
- Agenda/list timeline view (date-grouped chronological list)
- Timeline view optimized for horizontal scheduling ranges
- Optional year-overview mode for planning scenarios

Current implementation status:

- Work week view renderer: complete
- Agenda view renderer: complete
- Timeline view renderer: complete
- Year-overview mode: planned

### W2 - Resource Scheduling Model

- Introduce resource entity model (people, rooms, assets, teams).
- Allow event-to-resource assignment (single and multi-resource support policy).
- Add resource grouping presentation (lanes, tabs, or grouped headers).
- Add resource color and legend contracts.

Current implementation status:

- Resource contract: complete (lightweight lane DTO + event resource ID support)
- Resource lane presentation: complete in timeline renderer
- Multi-resource policy: planned
- Resource legend/color refinement: planned

### W3 - Viewport Virtualization And Caching

- Add renderer-level virtualization for dense timelines/lists.
- Introduce visible-range-only event layout evaluation.
- Expand CalendarEventService caching for resource/date slices.
- Add batched invalidation to reduce redraw storms.

Current implementation status:

- Timeline lane virtualization and lane-event pre-indexing: complete
- List/agenda virtualization coverage: planned
- Resource/date slice caching expansion: planned

### W4 - Performance Telemetry

- Add optional perf counters:
  - Query time
  - Layout time
  - Paint time
  - FPS-like redraw cadence metric
- Add debug overlay toggle for internal diagnostics.

Current implementation status:

- Query-time and paint-time counters with thresholds: complete
- Layout-time and redraw cadence counters: planned
- Debug overlay: planned

## UX/Visual Standards Applied

- Preserve low visual noise in dense views.
- Keep lane and time ruler readability at 100/125/150 DPI.
- Maintain consistent gestures and keyboard behavior across all views.

## Deliverables

1. New renderer implementations and view mode wiring.
2. Resource model, grouping contract, and renderer support.
3. Virtualization and perf optimization pass.
4. Performance benchmark document and thresholds.

## Definition Of Done

- Work week, agenda, and timeline renderers are production-ready.
- Resource grouping works in at least one time-grid and one timeline-like view.
- Performance remains responsive with large datasets in benchmark scenarios.
- Telemetry can pinpoint bottlenecks without altering production behavior.

## Risks

- Complex overlap layout in resource timelines.
- Mitigation: start with deterministic lane allocation and evolve to smarter packing behind feature flags.
