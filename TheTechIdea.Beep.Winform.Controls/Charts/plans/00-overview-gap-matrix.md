# Charts Overview And Gap Matrix

Priority: High
Status: Planning Approved
Depends on: Existing BeepChart partial-class and chart painter/helper architecture

## Vision

Evolve the Charts family from a capable custom chart surface into a polished, commercial-grade visualization control set with:

- predictable rendering contracts across chart, axis, series, and legend painters
- modern, theme-aware chart presentation aligned with Figma-style tokens and surface variants
- clearer navigation, zoom, hover, selection, and comparison workflows
- stronger axis, label, legend, and annotation behavior for business data analysis
- documented guidance for performance, accessibility, design-time usage, and product rollout

## External Benchmark Inputs

### Commercial reference capabilities

- Telerik WinForms ChartView: broad series coverage, pan and zoom, annotations, drill-down, smart labels, multiple axes, trackball, selection, legend wrapping, and design-time configuration
- Syncfusion WinForms Chart: rich chart-type catalog, real-time updates, smart labels, zoom and panning, multiple axes, scale breaks, multiple legends, trackball, crosshair, export, print, and template serialization
- DevExpress-style commercial chart expectations: dense but readable axis/legend systems, palette consistency, design-time clarity, export readiness, and strong financial and dashboard presentation patterns

### Open-source and design-system reference capabilities

- ScottPlot and OxyPlot style patterns: strong axis abstractions, interaction tooling, annotation-oriented composition, and emphasis on rendering scalability
- Figma-style component systems: spacing, typography, color, elevation, and state tokens applied consistently across chart title, legend, tooltip, surface, and empty states
- Fluent and Material-inspired data visualization patterns: readable hierarchy, restrained accents, clear focus states, and interaction hints that do not overpower the data

## Current Architecture Snapshot

- BeepChart already uses a partial-class split for constructor shell, properties, methods, drawing, and events
- Painter abstractions already exist for chart surface, axes, series, and legend rendering
- SeriesPainterFactory currently routes bar, pie, bubble, area, and line-style rendering
- Theme-derived chart colors and default palettes already exist through ApplyTheme and chart color derivation helpers
- ChartInputHelper already supports wheel zoom, drag pan, and hover hit detection for data points
- Viewport helpers and data helpers already perform autoscaling and axis-type detection

## Key Gaps

1. The chart control has useful rendering infrastructure, but no documented product contract for supported chart scenarios, painter responsibilities, and interaction rules.
2. Axis capabilities are still basic compared with commercial chart suites: no multiple-axis strategy, scale breaks, advanced tick policies, or business-grade label management roadmap.
3. Hover and zoom exist, but there is no formal interaction plan for trackball, crosshair, selection, reset-zoom affordances, drill-down, or keyboard navigation.
4. Legend behavior is present, but wrapping, grouping, pinned placement rules, and richer runtime toggling states are not defined.
5. Export, print, serialization, design-time presets, and sample-driven productization are not yet planned as first-class deliverables.
6. Empty, loading, error, and high-volume data states are not documented as UX surfaces with explicit Figma-style rules.

## Phase Map

- Phase 1: Foundation, rendering contracts, and chart model normalization
- Phase 2: Interaction, navigation, and analytical tooling
- Phase 3: Visual variants, accessibility, and performance scaling
- Phase 4: Integration, documentation, and productization

## Definition Of Done For Program

- All phases complete with per-phase DoD satisfied.
- TODO tracker reflects verified status and remaining risks.
- Plans cover axes, legends, data labels, annotations, interactions, export, and host integration.
- Chart README and plans stay aligned with the implementation and rollout notes.