# Phase 1 - Task Breakdown

This document converts the Phase 1 file-by-file matrix into a practical execution backlog.

## Step 1 - Normalize The Public Chart Contract

- Review the product-facing properties in `BeepChart.Properties.cs`.
- Define which properties are stable public inputs for chart data, appearance, axes, viewport, and legend behavior.
- Identify which fields and internal properties should remain implementation support rather than being treated as user-facing contract.
- Capture the supported chart-type baseline and any naming clarifications needed for future extension.

## Step 2 - Stabilize Initialization And Painter Setup

- Review `InitializeDefaultSettings`, `InitializeDesignTimeSampleData`, and `InitializePainter` in `BeepChart.Methods.cs`.
- Confirm when theme colors, fonts, and palettes are derived.
- Confirm when viewport autoscaling occurs and whether it should be preserved after user interaction.
- Document which state changes require painter recreation versus standard invalidation.

## Step 3 - Lock Down Rendering And Layout Ownership

- Review the paint flow in `BeepChart.Drawing.cs`.
- Confirm title, plot, axis, series, and legend layout sequencing.
- Decide which layer owns hit-area registration and future extensibility points for annotations or overlays.
- Record any rendering assumptions that later phases must preserve.

## Step 4 - Stabilize Resize And Input Lifecycle

- Review `BeepChart.Events.cs`.
- Confirm resize, handle-created, hover, drag-pan, and mouse-wheel entry points.
- Document the throttling and invalidation policy used to avoid over-redraw.
- Record any lifecycle assumptions that Phase 2 interaction work must preserve.

## Step 5 - Normalize Viewport Rules

- Review `BeepChartViewportHelper.cs`.
- Confirm autoscale, padding, bounds enforcement, and chart drawing rect bootstrap rules.
- Decide which rules are canonical defaults for Phase 1 and which should become later extension points.
- Capture any edge cases for empty data, categorical axes, or highly imbalanced ranges.

## Step 6 - Document The Baseline Interaction Contract

- Review `ChartInputHelper.cs`.
- Confirm the current baseline for wheel zoom, drag pan, and hovered-point lookup.
- Identify where future reset, selection, trackball, and crosshair behavior should connect without breaking Phase 1.
- Note any current limitations in hit-testing accuracy or scalability.

## Step 7 - Stabilize Axis, Legend, And Chart-Type Mapping

- Review `CartesianAxisPainter.cs`, `RightSideLegendPainter.cs`, and `SeriesPainterFactory.cs`.
- Confirm axis padding, title placement, tick policies, legend sizing, placement, and toggle behavior.
- Decide which legend and axis behaviors are canonical defaults for the phase.
- Confirm the current chart-type registry and capture the extension rules for future painters.

## Step 8 - Review And Handoff

- Record discrepancies between the current implementation and the new Phase 1 contract.
- Mark anything that should be deferred into Phase 2 or Phase 3.
- Update the Charts tracker with the implementation-status outcomes.

## Phase 1 Review Gate

- The chart surface should feel like a single coherent product, not a loose collection of helpers.
- Painter, viewport, and input ownership should be predictable from the documented contract.
- Interaction work in Phase 2 should be able to start without reopening basic rendering and state decisions.