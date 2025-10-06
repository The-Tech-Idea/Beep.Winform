# Styling and Theming

GridStyle presets (set `BeepGridPro.GridStyle`)
- Default: theme defaults; grid lines, sort indicators, hover effects.
- Clean: subtle borders, minimal padding.
- Bootstrap: striped rows.
- Material: gradient header, elevation, larger header/row heights.
- Flat: no grid lines, no hover.
- Dark: dotted grid lines, bold headers, stripes.
- Compact: smaller padding and heights.
- Corporate: professional look with gradient headers, bold text.
- Minimal: no lines, no sort indicators; focus on content.
- Card: card-like look with elevation and rounded rectangles.
- Borderless: no grid lines.

Theme integration
- The current theme is resolved via `BeepThemesManager.GetTheme(grid.Theme)`.
- `GridThemeHelper.ApplyTheme()` sets control back/fore colors and can align fonts to theme typography.
- `GridRenderHelper.Theme` property is used during paint for colors and fonts.

Appearance knobs
- Row height: `RowHeight` (min 18)
- Header height: `ColumnHeaderHeight` (min 22)
- Show column headers: `ShowColumnHeaders`
- Show navigator footer: `ShowNavigator`
- GridRenderHelper options: `ShowGridLines`, `ShowRowStripes`, `GridLineStyle`, `UseHeaderGradient`, `UseHeaderHoverEffects`, `UseBoldHeaderText`, `HeaderCellPadding`.

Sticky columns
- Set `BeepColumnConfig.Sticked = true` to pin a column to the left portion of the grid.
- Sticky columns are rendered on top and excluded from the horizontal scroll region.
