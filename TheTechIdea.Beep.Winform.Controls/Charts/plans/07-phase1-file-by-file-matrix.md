# Phase 1 - File-By-File Implementation Matrix

This matrix converts the Phase 1 checklist into concrete work by file.

## Execution Order

1. [BeepChart.Properties.cs](../BeepChart.Properties.cs)
2. [BeepChart.Methods.cs](../BeepChart.Methods.cs)
3. [BeepChart.Drawing.cs](../BeepChart.Drawing.cs)
4. [BeepChart.Events.cs](../BeepChart.Events.cs)
5. [Helpers/BeepChartViewportHelper.cs](../Helpers/BeepChartViewportHelper.cs)
6. [Helpers/ChartInputHelper.cs](../Helpers/ChartInputHelper.cs)
7. [Helpers/CartesianAxisPainter.cs](../Helpers/CartesianAxisPainter.cs)
8. [Helpers/RightSideLegendPainter.cs](../Helpers/RightSideLegendPainter.cs)
9. [Helpers/SeriesPainterFactory.cs](../Helpers/SeriesPainterFactory.cs)

## Matrix

| File | Phase 1 responsibility | Specific work | Notes |
|---|---|---|---|
| [BeepChart.Properties.cs](../BeepChart.Properties.cs) | Public chart contract | Review `DataSeries`, `LegendLabels`, `ChartType`, `LegendPlacement`, axis properties, viewport properties, and theme-facing appearance properties. Document which properties are part of the stable product surface versus internal support state. | This file defines the core chart API and should anchor the Phase 1 contract. |
| [BeepChart.Methods.cs](../BeepChart.Methods.cs) | Initialization and chart behavior helpers | Review `InitializeDefaultSettings`, `InitializeDesignTimeSampleData`, `InitializePainter`, tooltip behavior, animation start, and series toggling. Clarify when initialization updates theme, viewport, and painter state. | This file governs default runtime behavior and painter setup sequencing. |
| [BeepChart.Drawing.cs](../BeepChart.Drawing.cs) | Layout and rendering pipeline | Verify paint order, title composition, axis context creation, series drawing dispatch, legend drawing, and hit-area registration. Document ownership boundaries between chart painter, axis painter, and legend painter. | This is the main rendering pipeline and should be the source of truth for layout flow. |
| [BeepChart.Events.cs](../BeepChart.Events.cs) | Resize and input lifecycle | Review resize handling, handle-created behavior, mouse-wheel zoom entry, drag-pan lifecycle, hover invalidation throttling, and extension events such as `CustomDrawSeries`. | This file defines the lifecycle boundaries for interaction and redraw behavior. |
| [Helpers/BeepChartViewportHelper.cs](../Helpers/BeepChartViewportHelper.cs) | Viewport normalization | Review autoscale logic, padding rules, bounds enforcement, and chart drawing rect initialization. Decide which rules are canonical Phase 1 defaults and which need future extension points. | Viewport rules should be stable before Phase 2 expands navigation behavior. |
| [Helpers/ChartInputHelper.cs](../Helpers/ChartInputHelper.cs) | Basic navigation contract | Audit wheel zoom, drag pan, and hovered-point detection to document the existing interaction baseline. Identify where future reset-zoom, trackball, and selection contracts should plug in. | Phase 1 should document the baseline rather than widen the interaction surface yet. |
| [Helpers/CartesianAxisPainter.cs](../Helpers/CartesianAxisPainter.cs) | Axis layout and tick policy | Review default plot padding, axis title placement, numeric/date/text tick rules, label rotation, and axis hit-area registration. Document current limitations and the Phase 1 axis contract. | This file is the right place to define what the first stable axis layer really owns. |
| [Helpers/RightSideLegendPainter.cs](../Helpers/RightSideLegendPainter.cs) | Legend layout baseline | Review legend rectangle calculation, item sizing, placement modes, text rendering, and legend-item toggling hit areas. Document the current legend baseline for future wrapping and grouping work. | Phase 1 should stabilize legend rules before Phase 2 adds richer legend interactions. |
| [Helpers/SeriesPainterFactory.cs](../Helpers/SeriesPainterFactory.cs) | Supported chart-type registry | Validate the current painter mapping and document the extension policy for new series painters. | This is the canonical chart-type registration point for the current architecture. |

## Phase 1 Deliverables By File

- `BeepChart.Properties.cs`: stable chart API and appearance contract.
- `BeepChart.Methods.cs`: documented initialization and painter setup rules.
- `BeepChart.Drawing.cs`: clear rendering and layout ownership.
- `BeepChart.Events.cs`: stable redraw and input lifecycle rules.
- `BeepChartViewportHelper.cs`: canonical autoscale and viewport normalization guidance.
- `ChartInputHelper.cs`: documented baseline interaction contract.
- `CartesianAxisPainter.cs`: stable axis layout and tick policy baseline.
- `RightSideLegendPainter.cs`: stable legend layout and toggle baseline.
- `SeriesPainterFactory.cs`: canonical chart-type mapping authority.

## Phase 1 Acceptance Notes

- The chart API should feel predictable enough that future chart types do not force a redesign of the rendering contract.
- Viewport and paint ownership should be explainable without reverse-engineering the entire chart pipeline.
- Phase 2 should be able to focus on richer interactions without reopening Phase 1 foundation decisions.