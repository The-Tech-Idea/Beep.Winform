# Phase 8 - ListBox Painter Modernization Plan

Priority: High  
Status: Manual Validation In Progress (provisional code passes recorded; visual evidence pending)  
Depends on: Phases 1-7 completion

## Progress

- [x] Core contract pass started.
- [x] Added cache-aware layout recalculation in base painter path.
- [x] Added hierarchy-aware accessibility role sync (List vs Outline).
- [x] Added hierarchy expanded/collapsed accessibility states.
- [x] Updated accessibility child enumeration to visible items.
- [x] Enforced shared background pipeline in `ChipStyleListBoxPainter`.
- [x] Normalized `CompactListPainter` height to token-based dense row metric.
- [x] Tokenized `SimpleListPainter` corner radius.
- [x] Aligned `SearchableListPainter` with shared search control rendering contract.
- [x] Updated `OutlinedListBoxPainter` overlays and normal fill for theme/token consistency.
- [x] Normalized `WithIconsListBoxPainter` padding and row height to tokenized metrics.
- [x] Updated `MaterialOutlinedListBoxPainter` hover/selection visuals to theme-aware token usage.
- [x] Refactored `AvatarListBoxPainter` to reuse shared circular-avatar fallback helper.
- [x] Corrected secondary text hierarchy in `ContactListBoxPainter` and `ThreeLineListBoxPainter`.
- [x] Tokenized badge sizing in `ChatListBoxPainter` and `NotificationListBoxPainter`.
- [x] Updated `CardListPainter` surfaces/border to use theme colors instead of hardcoded light values.
- [x] Fixed `NeumorphicListBoxPainter` theme initialization in overridden `Paint` flow.
- [x] Updated `GradientCardListBoxPainter` and `TimelineListBoxPainter` to use visible-list indexing under filter/group/hierarchy modes.
- [x] Hardened `HeroUIListBoxPainter` null-theme safety and tokenized minimum touch-target height.
- [x] Normalized `CompactListPainter` hover/normal surfaces and spacing to theme/token rules.
- [x] Tokenized `CheckboxListPainter` padding/height/border metrics and divider inset.
- [x] Normalized shortcut-chip metrics in `CommandListBoxPainter` using list tokens.
- [x] Improved `ErrorStatesPainter` theme-safe foreground/background behavior for normal and hover states.
- [x] Reduced hardcoded fallback color usage in `ChipStyleListBoxPainter`.
- [x] Added robust themed status-color fallback in `AvatarListBoxPainter`.
- [x] Added resource-disposal and theme-safe text/background fixes in `ColoredSelectionPainter`.
- [x] Added resource-disposal and tokenized badge metrics/path reuse in `ErrorStatesPainter`.
- [x] Improved `CategoryChipsPainter` chip fallback colors and close-icon contrast behavior.
- [x] Normalized `SimpleListPainter`, `RoundedListBoxPainter`, `ThreeLineListBoxPainter`, `RaisedCheckboxesPainter`, and `TimelineListBoxPainter` for remaining theme/token/resource gaps.
- [x] Continue Batch B/C painter normalization (tokenization + parity).
- [x] Finalized base painter owner-context synchronization (`_helper`/`_layout`) and tokenized search-spacing fallback.
- [x] Reduced base painter scroll-path allocations by caching trailing-metadata font in `BaseListBoxPainter.DrawItem`.
- [x] Normalized `ChakraUIListBoxPainter` text/badge/selection colors to theme-safe fallbacks and tokenized compact row height.
- [x] Normalized `NeumorphicListBoxPainter` to theme-safe fallback colors and tokenized preferred row height.
- [x] Normalized `GlassmorphismListBoxPainter` selection/text/check colors to theme-safe fallbacks and tokenized preferred row height.
- [x] Normalized `HeroUIListBoxPainter` overlay/text/badge colors to theme-safe fallbacks and tokenized state alphas.
- [x] Normalized `GradientCardListBoxPainter` selected/hover/default color fallbacks to theme-safe paths and tokenized overlay alphas.
- [x] Normalized `FilterStatusPainter` hover/checkbox/text fallbacks and tokenized minimum touch-target height.
- [x] Normalized `ChipStyleListBoxPainter` chip/icon/check/close color fallbacks to theme-safe chains and tokenized overlay alphas.
- [x] Normalized `AvatarListBoxPainter` text/checkbox/avatar/status fallbacks to theme-safe chains and tokenized alpha values.

## Goal

Bring all ListBox painters and shared feature behavior to a production-grade baseline aligned with:

- Material Design 3 list patterns
- Fluent 2 list and navigation patterns
- Figma Auto Layout spacing discipline
- WCAG 2.2 AA interaction and contrast expectations

This phase is no longer only a "scale and token cleanup" pass. It is now a full painter-platform hardening pass covering consistency, accessibility, performance, and modern interaction quality.

## Audit Snapshot (Current)

- Painter files under review: 47
- `GetPreferredItemHeight()` overrides: 43
- Files calling `DrawItemBackgroundEx()`: 35
- Shared rendering contract source: `BaseListBoxPainter`

### Verified Structural Findings

1. `BaseListBoxPainter.Paint()` currently reassigns `_theme` from `_owner._currentTheme` and recalculates layout each paint pass. This increases coupling and repaint cost.
2. `BaseListBoxPainter` contains strong default behavior, but several painters still carry local spacing styles and visual math not fully tokenized.
3. Accessibility is present in the control layer, but hierarchy accessibility semantics are still list-centric (role stays List/ListItem).
4. Search, grouping, and hierarchy exist, but visual consistency across all variants is not uniform.

## Success Criteria

- All painters obey a single draw contract and layout pipeline.
- All dimensions are DPI-safe and token-driven.
- All variants support keyboard focus visibility, disabled states, and high-contrast behavior.
- Hierarchy and group visuals are consistent across style families.
- Visual polish aligns with modern list UI patterns (leading/content/trailing zones, compact/comfortable density, predictable hover/selection layers).

## Design Baseline (Apply To All Painters)

### Row Composition

Every painter should treat a row as three zones:

- Leading: checkbox, avatar, icon, hierarchy chevron
- Content: title, subtext, optional secondary line
- Trailing: metadata, badge, shortcut, status, disclosure affordance

### State Layers

Use consistent state overlays from tokens/theme:

- Hover
- Pressed
- Selected
- Focused (keyboard)
- Disabled

### Density Modes

All painters must respect density mode consistently:

- Dense
- Compact
- Comfortable

### Accessibility

- Minimum 44px effective touch target for interactive elements
- High contrast mode support for text, selection, focus ring
- Keyboard-first parity with mouse interactions
- Tooltip and accessible-name parity for text truncation

## Workstreams

## W1 - Contract Hardening (Critical)

Scope:

- Make `BaseListBoxPainter` the enforced source of painter lifecycle behavior.
- Minimize direct dependence on private owner internals in painters.
- Ensure all painter overrides start from the same background/state pipeline.

Tasks:

- Keep one sanctioned path for theme resolution.
- Guard layout recalculation to invalidation-driven flow where possible.
- Enforce `DrawItemBackgroundEx()` usage in all custom `DrawItem()` overrides.

Done when:

- All painter classes pass contract checks without exceptions.

## W2 - Tokenization and DPI Consistency (Critical)

Scope:

- Remove hardcoded spacing/radius/icon sizing values where token equivalents exist.
- Normalize row heights and paddings by density and painter type.

Tasks:

- Replace remaining raw layout values with `Scale()` and token constants.
- Standardize `GetPreferredItemHeight()` to token-first returns.
- Ensure icon/avatar/text vertical alignment is centered and deterministic.

Done when:

- No non-essential magic layout numbers remain in painter draw paths.

## W3 - Accessibility and Input Parity (High)

Scope:

- Close gaps between visual painter behavior and accessibility behavior.

Tasks:

- Add hierarchy semantic path (`Tree` / `TreeItem`) when hierarchy mode is enabled.
- Validate focus ring behavior for keyboard navigation in all variants.
- Ensure disabled items are visually dimmed and non-interactive.
- Validate tooltips and text truncation behavior in dense modes.

Done when:

- Keyboard-only navigation and AT behavior are equivalent to mouse behavior.

## W4 - Performance and Layout Stability (High)

Scope:

- Reduce repaint and per-frame work in shared painter path.

Tasks:

- Avoid unnecessary per-frame layout work where cache is valid.
- Ensure virtualization and clipping remain stable with large data sets.
- Keep rendering allocations low in hot loops (fonts, brushes, gradients).

Done when:

- Scroll remains smooth for 1000+ items at 125% and 150% DPI.

## W5 - Modern UI/UX Visual Upgrade (High)

Scope:

- Align visual quality with modern design systems while preserving Beep identity.

Tasks:

- Harmonize elevation, border, and fill treatment across painter families.
- Ensure selection and hover affordances are clear and consistent.
- Align typography hierarchy for title/subtext/meta rows.
- Standardize badge and trailing metadata visuals.

Done when:

- Cross-painter visual language feels consistent and intentional.

## W6 - Variant Rationalization (Medium)

Scope:

- Reduce duplicate logic and improve maintainability for style variants.

Tasks:

- Identify near-duplicate painter pairs and extract shared helper methods.
- Move repeated avatar/chip/timeline primitives to shared painter helpers.
- Keep specialized variants focused on style deltas only.

Done when:

- Variant-specific files mostly contain style decisions, not core row logic.

## W7 - Validation Matrix and Regression Gates (High)

Scope:

- Add repeatable verification checklist per painter class family.

Tasks:

- Validate each painter against: DPI, density, selection, keyboard, tooltip, disabled, search, hierarchy.
- Add screenshot comparison baseline for representative styles.
- Document pass/fail status in this phase doc.

Done when:

- Every painter has explicit verification status and no untracked regressions.

## Execution Batches

### Batch A - Shared Core First

- `BaseListBoxPainter`
- `BeepListBoxLayoutHelper`
- `BeepListBoxHelper`
- Accessibility adjustments in `BeepListBox.Accessibility`

Outcome:

- Stable contract, state layers, and semantics before touching all variants.

### Batch B - Structural or Legacy Outliers

- Any painter with custom search/header or custom state background flow
- Any painter still diverging from normalized row zone layout

Outcome:

- Eliminate fragile one-off behavior before broad token cleanup.

### Batch C - Mainline Variant Families

- Standard/Outlined/Minimal/Compact derivatives
- Avatar/Card/Profile/Contact/Chat families
- Checkbox/Radio/Multi-select families

Outcome:

- Full design consistency across most-used styles.

### Batch D - Complex Visual Effect Painters

- Neumorphic/Glassmorphism/Gradient/Hero/Timeline variants

Outcome:

- Effects remain performant and accessible without breaking shared contracts.

## Definition Of Done

- All painter files compile and conform to contract checks.
- All layout-critical values are tokenized and DPI-safe.
- Hierarchy and grouped modes remain stable across all painter families.
- Accessibility role/state behavior is correct in both list and hierarchy modes.
- Performance and UX checks pass in dense, compact, and comfortable layouts.

## Deliverables

1. Updated painter code across all variant families.
2. Updated helper/accessibility core for list and hierarchy parity.
3. Updated verification report section in this phase plan.
4. Updated painter enhancement summary file after implementation.

## Verification Report (2026-05-12)

- Static diagnostics for recently touched painters and plan/readme files: no reported errors.
- Shared contract parity: 30/30 `BaseListBoxPainter` derivatives audited; all 30 now contain explicit `DrawItemBackgroundEx()` contract coverage.
- Base pipeline hardening validated: owner context now refreshes helper/layout references per paint pass.
- Tokenization pass validated for final outlier set (`Simple`, `Rounded`, `ThreeLine`, `RaisedCheckboxes`, `Timeline`).
- Performance-path hardening validated: trailing metadata text no longer allocates a new font per row draw in base painter path.
- Neumorphic pass validated: hardcoded fallback colors removed from selected/text/accent/check paths and row height aligned to list tokens.
- Glassmorphism pass validated: selected/text/subtext/checkbox paths now prefer theme-safe colors and tokenized row-height baseline.
- HeroUI pass validated: accent scope consistency fixed and hover/badge/icon overlays now use list token alpha values.
- GradientCard pass validated: text/checkbox/icon overlays now use theme-safe fallbacks and tokenized alpha values, with tokenized preferred height.
- FilterStatus pass validated: hover overlays and checkbox/text defaults now resolve through theme-safe fallback chains and tokenized row height.
- ChipStyle pass validated: selected/hover/icon/check/close visuals now use theme-safe fallback chains with list-token alpha overlays.
- Avatar pass validated: primary/secondary text, avatar border, checkbox states, and status ring now use theme-safe fallback chains and list-token alphas.

### W7 Family Matrix (Code Verification)

- Core/shared layer: Complete (`BaseListBoxPainter`, `BeepListBoxLayoutHelper`, accessibility hierarchy role/state sync).
- Standard/simple/outlined family: Complete (tokenized heights/padding and theme-safe hover/selection baselines).
- Selection controls family (checkbox/radio/multi): Complete (`CheckboxList`, `OutlinedCheckboxes`, `RaisedCheckboxes`, `RadioSelection`, `MultiSelectionTeal`).
- Rich content family: Complete (`Avatar`, `Contact`, `ThreeLine`, `Notification`, `ProfileCard`, `TeamMembers`, `Chat`).
- Card/effect family: Complete (`Card`, `GradientCard`, `Glassmorphism`, `Neumorphic`, `HeroUI`, `Timeline`).
- Command/navigation family: Complete (`CommandList`, `NavigationRail`, `FilterStatus`, `CategoryChips`, `ChipStyle`).
- Special pipeline family: Complete with noted behavior (`InfiniteScroll` uses base row pipeline and custom sentinel rendering).
- Special pipeline family: Complete (`InfiniteScroll` keeps sentinel-row customization and now declares explicit background contract override).

### Remaining Manual Matrix (W7)

- Visual regression sweep per density mode (Dense/Compact/Comfortable).
- Keyboard and focus parity checks under grouped and hierarchy modes.
- High-contrast interactive-state verification.
- 125%/150% DPI screenshot baseline comparison across representative painter families.

### Manual Closeout Checklist

- [ ] Standard/simple/outlined representative screenshots captured in Dense, Compact, Comfortable.
- [ ] Rich-content representative screenshots captured (`Contact`, `ThreeLine`, `Notification`, `Chat`).
- [ ] Effect painter representative screenshots captured (`Neumorphic`, `GradientCard`, `Timeline`, `HeroUI`).
- [ ] Grouped mode keyboard traversal validated (Up/Down, PageUp/PageDown, Home/End).
- [ ] Hierarchy mode expand/collapse keyboard parity validated (selection + focus visibility).
- [ ] High-contrast mode validated for selection border, hover layer, and focus ring.
- [ ] Disabled item non-interactive behavior validated across at least one painter per family.

### Manual Execution Matrix (Assigned Representatives)

Use this matrix to capture pass/fail evidence in one sweep.

| Validation Gate | Representative Painters | Status | Evidence Notes |
|---|---|---|---|
| Dense/Compact/Comfortable visual parity | `StandardListBoxPainter`, `SimpleListBoxPainter`, `OutlinedListBoxPainter` | Provisional Pass (Code) | Density tokens and tokenized item heights/padding are implemented in shared and representative painter paths. |
| Rich row spacing + hierarchy of text | `ContactListBoxPainter`, `ThreeLineListBoxPainter`, `NotificationListBoxPainter`, `ChatListBoxPainter` | Provisional Pass (Code) | Rich row variants use dedicated list tokens for row heights, spacing, and subtext hierarchy. |
| Effect readability + contrast | `NeumorphicListBoxPainter`, `GradientCardListBoxPainter`, `TimelineListBoxPainter`, `HeroUIListBoxPainter` | Provisional Pass (Code) | Effect painters are aligned to shared background/state pipeline and current theme fallbacks. |
| Grouped keyboard traversal | `GroupedListPainter`, `StandardListBoxPainter` | Provisional Pass (Code) | Keyboard handling includes Up/Down/PageUp/PageDown/Home/End and grouped collapse APIs are present. |
| Hierarchy expand/collapse keyboard parity | `StandardListBoxPainter`, `NavigationRailListBoxPainter` | Provisional Pass (Code) | Hierarchy-aware keyboard branches and chevron/expand state handling are present in control + painter pipeline. |
| High-contrast interaction states | `OutlinedListBoxPainter`, `CheckboxListPainter`, `RadioSelectionPainter` | Provisional Pass (Code) | High-contrast helpers and focus/selection HC paint overrides are implemented in control and base painter. |
| Disabled-item behavior across families | `ErrorStatesPainter`, `RaisedCheckboxesPainter`, `CommandListBoxPainter`, `ProfileCardListBoxPainter` | Provisional Pass (Code) | Disabled alpha/token dimming and non-interactive visual states are present across representative families. |
| 125% and 150% DPI screenshot baseline | `StandardListBoxPainter`, `ContactListBoxPainter`, `TimelineListBoxPainter`, `NeumorphicListBoxPainter` | Pending (Manual Capture) | DPI scaling hooks and tokenized sizing are present; screenshot evidence still required. |

### Manual Run Notes

- For each gate marked `Provisional Pass (Code)`, confirm visually and then set to `Pass` or `Fail`.
- For `Pending (Manual Capture)` rows, attach screenshot evidence first, then set `Pass` or `Fail`.
- For failures, include a one-line defect note and owning painter.
- After all gates are `Pass`, phase status can be set to `Completed`.

## Notes

- This phase is implementation-ready and can be executed incrementally by batch.
- Use this file as the source of truth for painter modernization sequencing and acceptance.