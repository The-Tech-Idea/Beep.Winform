# Phase 1 - Foundation, Rendering Contracts, And Chart Model Normalization

Priority: High
Status: Planned
Depends on: [00-overview-gap-matrix.md](00-overview-gap-matrix.md)

## Objectives

- Define the core chart contract so painters, helpers, and host controls operate against stable responsibilities.
- Normalize the chart model surface for series, points, axes, legend, and viewport state.
- Establish theme, palette, spacing, and surface rules that make future variants predictable.
- Document current supported chart types and the extension path for new ones.

## Scope

### Rendering and painter contracts

- Document the responsibilities of `IChartPainter`, `IChartAxisPainter`, `IChartSeriesPainter`, and `IChartLegendPainter`
- Clarify layout ownership between chart surface painter, title section, axis layout, plot rect, legend, and hit areas
- Define when painter initialization occurs and which properties must trigger reinitialization versus simple invalidation

### Chart model normalization

- Define the public contract for `DataSeries`, legend labels, default series colors, viewport bounds, and axis types
- Document supported axis modes and how numeric, date, and categorical data are detected and mapped
- Establish the chart-type compatibility matrix for line, bar, area, pie, bubble, and future additions such as scatter, radar, donut, stacked variants, and financial types

### Theme and UX tokens

- Document chart tokens for title, subtitle, value, axis labels, grid lines, legend surfaces, series palette, and accent surfaces
- Define default surface variants for `Classic`, `Card`, `Outline`, and `Glass` with visual intent and recommended use cases
- Define density and spacing recommendations for dashboard, detail, and embedded-widget use cases

### State and invalidation rules

- Define when autoscale runs, when viewport changes are preserved, and when theme changes should regenerate painter state
- Document hover, active, selected, hidden-series, and empty/loading/error states as first-class chart states
- Add a reset policy for drawing rect and plot rect recalculation on resize and data updates

## Deliverables

- A stable chart foundation contract for the BeepChart surface
- A chart-type capability matrix and extension rules for additional series painters
- Theme-aware token guidance for chart surfaces and text hierarchy
- A documented invalidation and initialization policy for chart rendering

## Risks To Resolve

- The current public surface may mix product-facing properties with implementation details that should remain internal
- Chart type expansion may require additional data shape rules before new painters can be safely introduced
- Plot and legend layout ownership can drift unless responsibilities are documented early

## Definition Of Done

- Painter roles are documented and non-overlapping.
- Chart-type support and extension rules are written down.
- Surface, spacing, and theme token defaults are specified.
- The phase is reviewed against existing BeepChart partials and helpers.