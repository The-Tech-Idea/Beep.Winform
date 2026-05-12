# Charts

The Charts folder contains the Beep WinForms charting surface centered on `BeepChart` and its painter/helper infrastructure.

## Included Components

### BeepChart

`BeepChart` is the primary chart control. It currently provides:

- title, value, and subtitle header composition
- theme-derived chart colors and default series palette generation
- painter-based rendering for chart surfaces, axes, series, and legends
- viewport autoscaling and axis-type detection helpers
- runtime wheel zoom, drag pan, hover detection, and tooltip support
- typed interaction events including point/axis/legend hits and viewport-change notifications (`ViewportChanged`)
- trackball mode with vertical crosshair and multi-series tooltip (`EnableTrackball`, `TrackballShowCrosshair`, `TrackballShowMultiSeriesValues`)
- legend isolate: double-click legend item to show only that series, double-click again to restore all (`EnableLegendIsolate`, `IsolateSeriesAt()`, `RestoreAllSeries()`, `SeriesIsolationChanged` event)
- point selection with visual highlighting and multi-select support (`EnablePointSelection`, `SelectionMode`, `SelectPoint()`, `SelectSeries()`, `ClearSelection()`, `SelectionChanged` event)
- context menu on right-click for copy/export of selected data (`EnableContextMenu`, `ContextMenuRequested` event, built-in CSV/JSON export)
- visual preset system for dashboard, analytical, high-contrast, print, and presentation modes (`CurrentVisualPreset`, `ApplyVisualPreset()`)
- full keyboard navigation for accessibility: Tab (cycle series), Arrow keys (navigate points), Enter/Space (select), Ctrl+A (select all), dashed focus indicator (`EnableKeyboardNavigation`, `KeyboardFocusChanged` event)
- performance optimization for large datasets (1000+ points): point culling, vertex simplification (Ramer-Douglas-Peucker), grid line decimation (`EnablePerformanceMode`, `LargeDatasetThreshold`, `GetPerformanceReport()`, `PerformanceOptimized` event)
- non-color accessibility communication using per-series fill patterns (`EnableFillPatterns`, `SetSeriesFillPattern()`, `ApplyAutoPatterns()`, `ChartSeriesFillPattern`, `NonColorStateCommunicationChanged` event)
- dense-data label optimization with dynamic tick/label skipping (`EnableDenseLabelOptimization`, `MaxVisibleAxisLabels`, `LabelOptimizationApplied` event)
- real-time data streaming with append APIs and throttled incremental redraw (`AppendDataPoint()`, `AppendDataPoints()`, `FlushStreamingRender()`, `EnableRealTimeStreaming`, `StreamRenderThrottleMs`, `IncrementalRenderCompleted` event)
- host integration export APIs for full-series data and chart snapshots (`ExportAllDataAsCSV()`, `ExportAllDataAsJSON()`, `CaptureChartBitmap()`, `SaveChartImage()`)

Key implementation files:

- `BeepChart.cs`
- `BeepChart.Properties.cs`
- `BeepChart.Methods.cs`
- `BeepChart.Drawing.cs`
- `BeepChart.Events.cs`

### Helpers And Painters

The `Helpers` folder contains the current chart rendering and interaction infrastructure, including:

- series painter selection and concrete series painters
- axis and legend painters
- input, viewport, and data helpers
- chart surface painters and shared render option models

## Design Intent

This chart family is intended to provide:

- business-ready chart surfaces for dashboards and analytical views
- theme-aware visuals with clear hierarchy and readable defaults
- extensible painter contracts for future chart-type growth
- interaction patterns that support zoom, hover inspection, legend-driven comparison, and future drill-down flows

## Plans

The phased enhancement plan for this control family is in:

- [plans/README.md](plans/README.md)

That plan set includes:

- overview and gap matrix
- per-phase roadmap documents
- benchmark and UX standards notes
- TODO tracker

## Rollout Status

- Phase 2 implementation track is complete (interaction and analysis features delivered).
- Phase 3 implementation track is complete (visual presets, accessibility, performance, and streaming).
- Phase 4 integration/documentation track is complete for host usage, export/print/state persistence, and usage scenarios.
- Remaining roadmap items in `plans/TODO-TRACKER.md` are foundation-plan tasks for the earlier phase planning stream and do not block current chart host integration usage.

## Notes

- `BeepChart` inherits from `BaseControl` and should follow the shared Beep control authoring rules.
- The current planning package is intended to guide phased enhancement work before broad rollout of advanced chart capabilities.

## Host Integration Patterns

Common host integration patterns can be implemented directly with the public API surface:

```csharp
// 1) Real-time feed append (throttled incremental render)
beepChart.EnableRealTimeStreaming = true;
beepChart.StreamRenderThrottleMs = 33;
beepChart.AppendDataPoint("CPU", new ChartDataPoint(DateTime.Now.ToString("O"), usage.ToString("0.##"), usage));

// 2) Export all visible series
string csv = beepChart.ExportAllDataAsCSV(onlyVisibleSeries: true);
string json = beepChart.ExportAllDataAsJSON(onlyVisibleSeries: true, indented: true);

// 3) Snapshot and image export
using var bmp = beepChart.CaptureChartBitmap();
beepChart.SaveChartImage(@"C:\\Temp\\chart.png");

// 4) Print integration
beepChart.PrintChart(showPrintDialog: true, fitToPage: true);
beepChart.ShowPrintPreview(fitToPage: true);

// 5) Persist and restore chart state
string state = beepChart.ExportChartStateAsJSON(indented: true);
beepChart.ImportChartStateFromJSON(state);
beepChart.SaveChartStateToFile(@"C:\\Temp\\chart-state.json", indented: true);
beepChart.LoadChartStateFromFile(@"C:\\Temp\\chart-state.json");
```

## Design-Time Usage Notes

- Keep `EnableRealTimeStreaming` disabled in designer scenarios unless previewing live behavior.
- For clean designer previews, use `CurrentVisualPreset = ChartVisualPreset.Dashboard` and keep `ShowLegend = true`.
- Use `ApplyAutoPatterns()` in high-contrast designer previews to validate non-color series differentiation.
- Use `MaxVisibleAxisLabels` and `EnableDenseLabelOptimization` to avoid crowded axis labels at design time.