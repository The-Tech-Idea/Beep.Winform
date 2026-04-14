# Widget Designer Sweep

## Completed

- `BeepCalendarWidgetDesigner`: added subtitle, view mode, weekend/today/event toggles, multi-select, and working-hours properties; presets now set the supporting behavior too
- `BeepChartWidgetDesigner`: added legend, grid, and min/max scale properties plus a sparkline preset and stronger chart-style presets
- `BeepDashboardWidgetDesigner`: added `ShowTitle` and a `Status Overview` preset with layout defaults
- `BeepFinanceWidgetDesigner`: added subtitle, primary value, percentage, currency, and visibility toggles plus balance/budget presets
- `BeepFormWidgetDesigner`: added subtitle, description, validation, required, read-only, progress, step, and layout properties plus a step-form preset
- `BeepMapWidgetDesigner`: added subtitle, address, coordinates, zoom, route/traffic toggles, provider, and view-type properties plus live-tracking/travel presets
- `BeepMetricWidgetDesigner`: added trend direction, trend percentage, show-trend, and show-icon properties plus comparison/card presets and stronger metric presets
- `BeepNavigationWidgetDesigner`: added icon, orientation, and current-index properties plus a breadcrumb preset
- `BeepControlWidgetDesigner`: added enabled, label, selected-option, and search-text properties plus a dropdown-filter preset
- `BeepListWidgetDesigner`: added header, selection, selected-index, and visible-item controls plus a compact status-list preset
- `BeepMediaWidgetDesigner`: added subtitle, image-path, overlay settings, and fixed the avatar-list preset to use `MediaWidgetStyle.AvatarList`
- `BeepNotificationWidgetDesigner`: added action/icon/dismiss/progress controls plus a success-banner preset
- `BeepSocialWidgetDesigner`: added subtitle, user identity/status properties, avatar/status toggles, and a profile-card preset while correcting the chat preset to target `SocialWidgetStyle.ChatWidget`

## Follow-Up

- If new widget runtime controls land later, keep smart tags limited to scalar, runtime-backed properties that materially change the preview and strengthen presets to configure those properties together instead of only flipping `Style`