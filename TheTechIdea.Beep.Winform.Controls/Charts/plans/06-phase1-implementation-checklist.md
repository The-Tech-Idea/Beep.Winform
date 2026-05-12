# Phase 1 - Implementation Checklist

This checklist turns the Phase 1 plan into an execution-ready set of verification steps for the Charts family.

## 1. Painter And Layout Contracts

- [ ] Confirm the responsibility boundaries for `IChartPainter`, `IChartAxisPainter`, `IChartSeriesPainter`, and `IChartLegendPainter`.
- [ ] Confirm which layer owns `DrawingRect`, `PlotRect`, title region, legend bounds, and hit areas.
- [ ] Verify which property changes should call `InitializePainter()` versus a plain `Invalidate()`.
- [ ] Document the current fallback behavior when no chart painter or plot rect is available.

## 2. Chart Model And Type Rules

- [ ] Confirm the public contract for `DataSeries`, legend labels, default series palette, and viewport properties.
- [ ] Validate the currently supported chart-type matrix in `SeriesPainterFactory`.
- [ ] Document the extension rules for adding new series painters such as scatter, donut, radar, stacked variants, or financial series.
- [ ] Verify axis detection and value-conversion expectations for numeric, date, and categorical data.

## 3. Viewport And State Ownership

- [ ] Confirm when autoscale should run and when user-driven viewport changes should be preserved.
- [ ] Review viewport padding and bounds normalization rules in the viewport helper.
- [ ] Confirm resize behavior, plot rect recalculation, and invalidation throttling expectations.
- [ ] Document hover, hidden-series, empty-data, and future selected-state ownership.

## 4. Theme, Palette, And Surface Defaults

- [ ] Review default theme-derived colors for chart surface, axes, legend, grid, and series.
- [ ] Confirm visual intent and recommended usage for `Classic`, `Card`, `Outline`, and `Glass` surface styles.
- [ ] Document token candidates for chart spacing, title spacing, axis padding, legend sizing, and marker visibility.
- [ ] Confirm default header composition for title, value, and subtitle in dashboard and analytic contexts.

## 5. Documentation And Readiness

- [ ] Cross-check the checklist against the Phase 1 plan and overview gap matrix.
- [ ] Update the Charts plans README if Phase 1 terminology changes.
- [ ] Capture any implementation risks that must roll into Phase 2.

## Exit Criteria

- [ ] Painter and layout ownership are documented.
- [ ] Chart-type and axis-contract rules are stable and explainable.
- [ ] Viewport and theme defaults are documented.
- [ ] The chart family is ready for Phase 2 interaction work.