# Fixed and Updated Painters

This workspace is being modernized so all widgets look and feel like a modern web dashboard: clean surfaces, subtle elevation, rounded corners, soft shadows, hover/active state layers, responsive layouts, and interactive hit areas. The goal is consistent, theme-aware visuals and interactions across all painters.

Scope and requirements
- Unify visual language across widgets to match modern web dashboards (Material/Fluent inspired):
  - Rounded corners, soft shadows, subtle gradients where appropriate.
  - Theme-aware colors (Primary/Accent/Border/Foreground/Background).
  - Hover, focus, and active state layers with translucency.
  - Minimal borders; use elevation and state layers instead.
  - Clear typography hierarchy for titles, values, labels.
- Make widgets interactive with consistent hit area patterns:
  - Register hit areas for key parts (titles, icons, buttons, rows, cells, charts).
  - Provide hover cues and selection highlights.
  - Write back to `ctx.CustomData` on clicks (e.g., SelectedIndex, toggles, dismissed flags).
  - Call `Owner?.Invalidate()` after state changes.
- Keep layouts resilient and DPI-aware via `Owner?.DrawingRect` and painter `AdjustLayout`.
- Avoid duplication: track each painter updated here.

Completed painters and interactions
- Media
  - MediaViewerPainter: control buttons (prev/pause/play/next/refresh) hit areas; hover outlines; info overlay; placeholder.
  - AvatarGroupPainter: per-avatar hit areas; hover outline accents; initials fallback.
  - AvatarListPainter: per-item, avatar, and status hit areas; hover cues; placeholder avatar rendering.
  - MediaGalleryPainter: per-item hit areas; hover overlays.
  - ImageCardPainter, IconCardPainter: card and badge hit areas; hover cues.
  - ImageOverlayPainter: close and action buttons hit areas; hover cues.
  - IconGridPainter, PhotoGridPainter: per-cell hit areas; hover accents.
- Notification
  - AlertBannerPainter: banner/icon/dismiss hit areas; hover cues.
  - StatusCardPainter: card click and dismiss action; hover outline; optional loading progress.
  - ValidationMessagePainter: dismiss and copy actions; hover cues; layout for actions.
  - SuccessBannerPainter: dismiss action; hover cue.
  - InfoPanelPainter: expand/collapse toggle; hover cue.
  - WarningBadgePainter: baseline visuals.
- Navigation
  - BreadcrumbPainter: home and item hit areas; home hover accent.
  - PaginationPainter: prev/next and per-page hit areas; hover cues; updates `CurrentPage`.
- Chart
  - BarChartPainter: chart hit area; hover cue; simple legend.
  - PieChartPainter: chart hit area; hover cue.
  - LineChartPainter: chart hit area; hover cue; point accents.
  - HeatmapPainter: chart and per-cell hit areas; hover cues.
  - GaugeChartPainter: chart hit area; hover cue; value display.
  - CombinationChartPainter: chart hit area; hover cue.
- Dashboard
  - AnalyticsPanelPainter: chart and per-metric hit areas; hover cues; title icon.
  - ChartGridPainter: title/cell/expand hit areas; hover cues.
  - ComparisonGridPainter: left/right panels, VS icon, title hit areas; hover cues.
  - StatusOverviewPainter: header and per-row hit areas; hover highlights.
  - MultiMetricPainter: title and per-cell hit areas; hover cues; trend accents.
- List
  - ActivityFeedPainter: per-item hit areas; hover background; timeline visuals.
  - DataTablePainter: header and per-row hit areas; hover cues; alt rows.
  - TaskListPainter: item and checkbox hit areas; hover cues; toggle intent.
  - ProfileListPainter: avatar/name hit areas; hover accents.
  - RankingListPainter: per-row hit areas; hover background; rank badges.
- Metric
  - CardMetricPainter: clickable icon; hover accent; trend display.
- Control
  - ColorPickerPainter: palette cell hit areas; hover/selection accents; selected swatch text.
  - CheckboxGroupPainter: checkbox item hit areas; hover accents.

Pending/next steps
- Navigation tabs/menu/sidebar painters: per-tab/menu hit areas; active/hover/disabled states.
- Keyboard accessibility for key widgets (Pagination, MediaViewer: arrows, space).
- DataTable sorting indicators on header hit; optional sort state in `CustomData`.
- Consistent tooltip hints via `Owner._toolTip` where applicable.
- Review DPI scaling helpers and ensure painter rectangles use `Owner.GetScaleFactor` where needed.

Conventions
- Hit area names use `Painter_Element_{index}` where relevant.
- All hit callbacks update `ctx.CustomData[...]` and invalidate the owner.
- Hover cues use theme Primary/Accent with translucent overlays or outline pens.
- Layout uses `Owner?.DrawingRect` in `AdjustLayout` when available.

This ledger is the source of truth. Update it whenever a painter is modified to prevent duplicate work.
