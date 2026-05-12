# Phase 2 - Interactions, Navigation, And Analytical Tooling

Priority: High
Status: Planned
Depends on: [01-phase1-foundation-chart-contracts.md](01-phase1-foundation-chart-contracts.md)

## Objectives

- Expand the chart from basic hover and viewport changes into a clear analytical interaction surface.
- Define commercial-grade navigation patterns for zooming, panning, hover inspection, and legend-assisted comparison.
- Add a roadmap for point or series selection, drill-down, and context actions.

## Scope

### Navigation and viewport UX

- Define zoom-in, zoom-out, reset, fit-to-data, and pan behaviors
- Document mouse, wheel, drag, and optional keyboard navigation patterns
- Define visible affordances for the current zoom level and viewport reset entry points

### Hover, tooltip, and analysis aids

- Define standard tooltip content rules for title, series name, X/Y values, formatted value, and optional metadata
- Add a roadmap for trackball and crosshair modes for comparative reading across series
- Define how hovered points, hovered series, and hovered legend items visually respond

### Legend interactions

- Document single-series toggle, isolate-series, show-all, and grouped legend scenarios
- Define legend placement behavior for compact cards, standard charts, and dense dashboards
- Add runtime policy for legend wrapping, truncation, and overflow

### Selection and drill-down

- Define selection states for point, segment, bar, or series interactions
- Add a drill-down roadmap for bar, pie, and categorical charts where deeper navigation is useful
- Define context-menu and command-surface opportunities for filtering, exporting, and focus operations

## Deliverables

- A unified interaction model for zoom, pan, hover, legend actions, and reset flows
- Tooltip and trackball/crosshair planning guidance
- A selection and drill-down roadmap suitable for business dashboards
- Runtime interaction notes for compact and full-size chart hosts

## Risks To Resolve

- Interaction density can overwhelm embedded charts without clear mode boundaries
- Hover detection and future selection support will need a stronger hit-testing contract than the current point-distance approach
- Drill-down and legend isolate behaviors require event and state models that do not conflict with current painter abstractions

## Definition Of Done

- Interaction rules are documented for hover, zoom, pan, legend actions, and reset.
- Selection and drill-down are defined as supported roadmap scenarios.
- Compact versus full-size chart UX rules are specified.
- Manual QA expectations are listed for interaction regressions.