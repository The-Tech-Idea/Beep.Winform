# Charts External Benchmark And UX Standards

## Commercial Reference Summary

### Telerik WinForms ChartView

Observed benchmark themes:

- broad series catalog including area, line, bar, pie, scatter, bubble, radar, funnel, waterfall, and financial series
- analytical tooling such as pan and zoom, lasso-style zoom, trackball, tooltips, scale breaks, annotations, and drill-down
- chart structure features such as multiple axes, smart labels, legend wrapping, predefined palettes, and strong design-time support
- productization signals such as print support, themes, and integration with adjacent control surfaces

Implications for BeepChart:

- interaction planning should explicitly include trackball, crosshair, selection, and reset-zoom UX
- multiple-axis and smart-label strategy should be considered a roadmap requirement, not an optional stretch goal
- design-time clarity and themed visual presets matter as much as raw series count

### Syncfusion WinForms Chart

Observed benchmark themes:

- large chart-type breadth plus real-time data handling and high-volume rendering narratives
- strong axis and legend capabilities including scale breaks, multiple axes, label rotation, label tooltips, multiple legends, and legend checkboxes
- extensive runtime interaction such as zooming, panning, highlight, tooltips, crosshair, trackball, and draggable or interactive cursors
- export and productization breadth including image, PDF, SVG, print, XML serialization, and template support

Implications for BeepChart:

- export, persistence, and sample scenarios need to be part of the roadmap early enough to shape API design
- chart readability should include label-density management, threshold or strip-line planning, and legend runtime behavior
- performance guidance should distinguish dashboard cards from heavy analytical views

### DevExpress-Style Commercial Expectations

Observed benchmark themes from the broader WinForms chart category:

- consistent surface quality across dashboards, reports, and drill-down workflows
- strong palette and theme coordination with readable defaults
- emphasis on financial and business-ready visualizations, export readiness, and designer discoverability

Implications for BeepChart:

- default presets should feel intentional and business-ready out of the box
- chart contracts should keep painter composition stable enough for future financial and reporting scenarios

## Open-Source Reference Summary

### ScottPlot and OxyPlot style lessons

- axis management and render-state ownership should be explicit
- interaction features such as pan, zoom, trackers, and legends benefit from dedicated abstractions instead of being folded into general paint paths
- high-volume charting usually succeeds when rendering contracts stay narrow and predictable

Implications for BeepChart:

- hit testing, viewport changes, and annotation models should be documented before feature breadth expands
- painter APIs should stay focused on layout and drawing rather than mixed mutation responsibilities

## Figma-Style UX Standards For Charts

### Visual hierarchy

- Titles, values, subtitles, and legends should use a clear typographic ladder with stable spacing.
- The plot area should remain the primary visual surface; header chrome must not dominate the data.
- Accent colors should highlight data intent, not simply decorate the container.

### Tokenization

- Define chart tokens for spacing, radius, typography, label opacity, grid strength, and legend surfaces.
- Keep surface styles intentional: card charts, outline charts, and glass charts should each have a documented use case.
- Use consistent state colors for hover, selection, hidden series, alerts, and thresholds.

### Interaction clarity

- Zoom, pan, drill-down, and reset actions should always provide visible confirmation.
- Hover and tooltips should reinforce comparison, not obstruct the plot.
- Legends should act as controls only when their interactive behavior is obvious.

### Accessibility and readability

- Do not rely on color alone to communicate active or hidden states.
- Dense labels need graceful fallback rules: rotate, wrap, abbreviate, summarize, or move to tooltip.
- Empty, loading, and error states should be designed as chart-specific surfaces, not generic blank rectangles.

## Roadmap Guidance

- Prioritize contract clarity before expanding chart-type count.
- Treat interaction tooling and readability as equal in importance to visual variants.
- Do not copy commercial suites feature-for-feature; extract the behaviors that most improve BeepChart usability, consistency, and product readiness.