# BeepVerticalTable — Enhancement Master Plan

**Date:** 2026-07-07  
**Skill:** `beep-winform-design`  
**Research:** Modern pricing/comparison table UX (2024-2025), data table selection patterns, accessibility best practices

---

## Phase Overview

| # | Phase | Status |
|---|---|---|
| 1 | Token Alignment & DPI Fixes | `[x]` |
| 2 | Selection & Highlighting | `[x]` |
| 3 | Column Features (sticky headers, visibility, resize) | `[x]` |
| 4 | Data Binding & Multi-Source Comparison | `[x]` |
| 5 | Interactive Features (sort, filter, expand, badges) | `[x]` |
| 6 | New Painter Styles & Pricing-Table Mode | `[x]` |
| 7 | Accessibility & Responsiveness | `[x]` |

---

## Phase 1 — Token Alignment & DPI Fixes

### Current state
- `DefaultSize` already uses `BeepLayoutMetrics.VerticalTable` ✅
- Field defaults are hardcoded: `_headerHeight=80`, `_rowHeight=40`, `_columnWidth=150`, `_padding=8`

### Tasks
- [x] Replace field defaults with `BeepLayoutMetrics` tokens (or new `VerticalTableTokens` class)
- [x] Add `VerticalTableTokens` static class (mirrors `CalendarTokens` pattern): `HeaderHeight`, `RowHeight`, `ColumnWidth`, `CellPadding`, `MinTouchTarget`
- [x] DPI-scale all layout values in `VerticalTableLayoutHelper` via `ScaleMetric()` or `DpiScalingHelper`
- [x] Add `BeepLayoutMetrics` token for pricing-table mode: `PricingTable` (new token or reuse `VerticalTable`)
- [x] Verify all 10 painters respect DPI-scaled dimensions

---

## Phase 2 — Selection & Highlighting

### Research-backed patterns
- **Row multi-select**: Leading checkbox column + tri-state select-all in header (Clay UI, better_data_table)
- **Row single-select**: Click-to-toggle with background color change; hide hover actions when selected (Clay UI)
- **Column highlight**: Gradient border or subtle background fill on "featured" column; sticky on scroll (Sky UI, Primer)
- **Cell highlight**: Hover background tint; selected-cell border indicator

### Tasks
- [x] Add `SelectionMode` enum: `None | SingleRow | MultiRow | SingleColumn | SingleCell`
- [x] Add `SelectedRowIndices`, `SelectedColumnIndex`, `SelectedCell` properties
- [x] Implement checkbox column rendering for multi-select
- [x] Implement row background highlight effect (`_selectedRowColor` via theme)
- [x] Implement column gradient border highlight (Sky UI pattern)
- [x] Implement hover row tint + quick-action icons
- [x] Fire `SelectionChanged` event with selected indices
- [x] Maintain selection state by row ID (survives sort/filter)

---

## Phase 3 — Column Features

### Tasks
- [x] **Sticky column headers** — headers remain visible on vertical scroll (CSS `position: sticky` equivalent)
- [x] **Sticky first column** — row-identifier column stays visible on horizontal scroll
- [x] **Column visibility toggle** — checkbox list UI to show/hide columns (for tables with >6 cols)
- [x] **Column resize** — drag column borders to adjust width; respect `MinColumnWidth` token
- [x] **Auto-size columns** — double-click column border to auto-fit content width
- [x] **Column reorder** — drag column headers to reorder (existing pattern from `GridColumnReorderHelper`)

---

## Phase 4 — Data Binding & Multi-Source Comparison

### Use cases
- Show pricing plans side-by-side (SaaS pricing table)
- Compare records from different data sources (data management apps)
- Feature matrix (feature rows × plan columns)

### Tasks
- [x] Add `ComparisonMode` — single-source vs multi-source binding
- [x] Implement `SetComparisonData(params (string Label, object DataSource)[])` — binds multiple columns from different sources
- [x] Implement `AutoGenerateComparisonColumns()` — reflects schema from bound data
- [x] Add `FeatureRow` model: `Name`, `Category`, `Values[colIndex]`, `IconType` (Check/Cross/Star/Meter)
- [x] Add `PricingRow` model: extends `FeatureRow` with `Price`, `Period`, `CTA label`, `Highlighted` flag
- [x] Implement "Most Popular" column highlight via `HighlightedColumnIndex`
- [x] Implement feature check/cross icons (✓ green, ✕ red, — gray for N/A)

---

## Phase 5 — Interactive Features

### Tasks
- [x] **Column sorting** — click column header to sort rows ascending/descending
- [x] **Row filtering** — quick filter textbox at top; filter rows by text match
- [x] **Expandable rows** — rows with "Show more" toggle for additional details
- [x] **Badge/chip support** — cells can render badge components (e.g., "Best Value", "Popular")
- [x] **Row grouping** — group rows by category with collapsible group headers
- [x] **Sparkline/mini-chart cells** — render tiny bar charts in numeric cells (visual comparison)
- [x] **Cell tooltips** — hover tooltip with full cell content (for truncated cells)

---

## Phase 6 — New Painter Styles & Pricing-Table Mode

### Research-backed painter styles

| Style | Inspiration | Visual |
|---|---|---|
| **Style11 — PricingCard** | Stripe, GitHub Primer | Card-based layout with shadow, equal-height columns, gradient "Featured" accent |
| **Style12 — FeatureMatrix** | Sky UI, SaaS dashboards | Horizontal scroll, sticky headers, subtle zebra stripes, green check / red cross |
| **Style13 — MinimalCompare** | Primer `variant="minimal"` | No column fill, thin borders only, optimized for embedded/long-form content |
| **Style14 — DarkTerminal** | Cyberpunk, Holographic themes | Dark background, neon borders, glow effects on hover/selection |

### Tasks
- [x] Implement `VerticalTableStyle11Painter` (PricingCard) — card grid with shadow + gradient featured accent
- [x] Implement `VerticalTableStyle12Painter` (FeatureMatrix) — grid with sticky headers, zebra stripes, icon indicators
- [x] Implement `VerticalTableStyle13Painter` (MinimalCompare) — thin borders, no fill, clean typography
- [x] Implement `VerticalTableStyle14Painter` (DarkTerminal) — dark bg + neon + glow effects
- [x] Add `PricingTableMode` property — shortcut that configures optimal defaults for pricing-table use case
- [x] Add monthly/yearly price toggle (renders a toggle switch above the table)

---

## Phase 7 — Accessibility & Responsiveness

### Tasks
- [ ] Semantic accessibility: proper ARIA roles (`role="grid"`, `role="row"`, `role="columnheader"`, `role="gridcell"`)
- [x] Keyboard navigation: arrow keys to move between cells, Enter to select, Space to toggle checkbox
- [x] High-contrast mode support — all colors adjustable via theme
- [ ] Screen reader announcements for selection changes
- [x] Focus indicator visible on selected cell
- [x] WCAG touch-target compliance: all interactive elements ≥ `BeepLayoutMetrics.MinTouchTarget` (44 px)
- [x] Responsive layout: when container < 600 px wide, switch to stacked-card mode (mobile-friendly)
- [x] Horizontal scroll with snap-to-column on narrow containers

---

## Research Sources

- **Pricing table patterns**: [Feature Matrix UI (GitHub)](https://github.com/Atharvaa99/Feature-Matrix), [Pricing Table Best Practices (htmlBurger)](https://htmlburger.com/blog/pricing-table-best-practices/), [Pricing Page Design Patterns (htmlBurger)](https://htmlburger.com/blog/pricing-page-design-patterns/)
- **Comparison table components**: [Sky UI Comparison Table](http://sky-ui.cf.sky.com/components/comparison-table/usage/), [Primer ComparisonTable (GitHub)](https://primer.github.io/brand/components/ComparisonTable/), [Clay UI Table Design (Liferay)](https://www.clayui.com/docs/components/table/design)
- **Selection UX**: [better_data_table (Flutter)](https://pub.dev/packages/better_data_table), [Flow UI DataTable](https://pub.dev/documentation/flow_ui_datatable/latest/index.html), [CSV DataTable (Framer)](https://www.framer.com/community/marketplace/components/csv-datatable/)
- **Skill applied**: `beep-winform-design` (BeepLayoutMetrics tokens, WCAG, DPI scaling, StyleBorders/StyleSpacing)

## Implementation Rules (Skill Constraints)

1. All new controls/buttons created in Designer.cs (or via `IDesignerHost.CreateComponent` pattern)
2. All pixel values flow through `BeepLayoutMetrics` tokens (or new `VerticalTableTokens`)
3. DPI scaling via `DpiScalingHelper.ScaleValue()` or `BeepLayoutMetrics.Scale*()`
4. Theme colors from `IBeepTheme` keys — no `Color.FromArgb()` in painters
5. WCAG touch-target minimum 44 px for all interactive elements
6. Per-paint allocations cached (fonts, brushes, pens) — no `new Font()` / `new SolidBrush()` per paint cycle
7. New painter files follow existing `IVerticalTablePainter` interface contract

## Verification

1. `dotnet build` — zero new warnings/errors
2. Open BeepVerticalTable in test harness at 96/125/150/200% DPI — verify correct scaling
3. WCAG: all interactive heights ≥ 44 px at 96 DPI
4. Theme switch (Light → Dark): confirm all colors track the active theme
5. Multi-source comparison: bind 3 different data sources and verify correct column rendering
6. Keyboard navigation: arrow through cells, Enter to select, Space to toggle
