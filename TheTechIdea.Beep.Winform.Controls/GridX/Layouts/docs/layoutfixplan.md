# BeepGridPro Layouts – Fix/Enhancement Plan

Purpose
Domain-specific layout helpers crept into GridX (e.g., DepartmentsRecordBoldLayoutHelper). Layouts must be generic visual presets that define how the grid looks (spacing, alignments, chrome), not what data it shows. This plan tracks the refactor: what to add, rename, deprecate, and how to evolve the API.

Goals
- Provide a small, coherent set of domain?agnostic layout presets.
- Make presets idempotent: applying one fully defines layout knobs (row/header sizes, paddings, alignments, stripes, sticky behavior), without hidden dependencies.
- Separate color/theme (BeepGridStyle) from layout presets.
- Keep a migration path: older, domain-named helpers kept as thin wrappers or marked obsolete.
- Document and test presets for consistent UX.

Non?Goals
- Changing themes or colors (that’s handled by Theme and GridStyle).
- Data modeling or per?column business logic.

Planned API changes (to be implemented after this plan)
- Add enum `GridLayoutPreset` with generic items:
  - Default, Clean, Dense, Striped, Borderless, HeaderBold, MaterialHeader, Card, ComparisonTable, MatrixSimple, MatrixStriped
- Add property `LayoutPreset` on `BeepGridPro`:
  - Setting it applies the corresponding `IGridLayoutPreset` implementation (via factory) and calls `Layout.Recalculate()` and `Invalidate()`.
- Keep `BeepGridStyle` as-is (colors/visual effects); `LayoutPreset` controls structure (spacing/alignments/stripes).

Technical approach
1) Introduce new generic preset helpers (new files):
   - DefaultTableLayoutHelper ? balanced defaults
   - CleanTableLayoutHelper ? increased padding, clear chrome
   - DenseTableLayoutHelper ? compact rows/headers
   - StripedTableLayoutHelper ? alternating row stripes
   - BorderlessTableLayoutHelper ? no grid lines, padding tuned for readability
   - HeaderBoldTableLayoutHelper ? bold headers, larger header height
   - MaterialHeaderTableLayoutHelper ? subtle gradient header, elevated feel
   - CardTableLayoutHelper ? card-like rows (no line grid)
   - ComparisonTableLayoutHelper ? tidy, centered numerics/action columns
   - MatrixSimpleTableLayoutHelper / MatrixStripedTableLayoutHelper ? feature matrix variants (generic, not domain bound)

2) Back-compat wrappers (existing domain helpers ? thin calls):
   - DepartmentsRecordBoldLayoutHelper ? [DONE] delegates to HeaderBoldTableLayoutHelper and marked [Obsolete]
   - TeamMembersCleanLayoutHelper ? [PLANNED] delegate to CleanTableLayoutHelper
   - ComparisonPricingLayoutHelper ? [PLANNED] delegate to ComparisonTableLayoutHelper
   - DesignSystemTableLayoutHelper ? [KEEP] for now; candidate to align to Default/Clean tuning later
   - FeatureMatrixSimpleLayoutHelper ? [PLANNED] delegate to MatrixSimpleTableLayoutHelper
   - FeatureMatrixStripedLayoutHelper ? [PLANNED] delegate to MatrixStripedTableLayoutHelper

3) BeepGridPro changes:
   - Add GridLayoutPreset enum and LayoutPreset property with mapping. [DONE]

4) Consistency rules each preset must set:
   - Dimensions: RowHeight, ColumnHeaderHeight, ShowColumnHeaders
   - Render flags: Render.ShowGridLines, Render.GridLineStyle, Render.ShowRowStripes, Render.UseHeaderGradient, Render.UseHeaderHoverEffects, Render.UseBoldHeaderText, Render.HeaderCellPadding, Render.UseElevation, Render.CardStyle
   - Alignment heuristics: first visible column centered only if checkbox/row number; otherwise left; obvious names (Status/Action) centered.
   - Do not refer to domain nouns.

5) Documentation/testing
   - Update Docs to mention LayoutPreset vs GridStyle. [PLANNED]

Tracking table
- 2025?10?06 — Plan created.
- 2025?10?06 — [DONE] Added enum GridLayoutPreset and BeepGridPro.LayoutPreset property.
- 2025?10?06 — [DONE] Added helpers: DefaultTable, CleanTable, DenseTable, StripedTable, BorderlessTable, HeaderBoldTable, MaterialHeaderTable, CardTable, ComparisonTable, MatrixSimpleTable, MatrixStripedTable.
- 2025?10?06 — [DONE] DepartmentsRecordBoldLayoutHelper marked obsolete and delegated to HeaderBold.
- 2025?10?06 — [PLANNED] Add wrappers/obsolete for TeamMembersClean, ComparisonPricing, FeatureMatrixSimple, FeatureMatrixStriped.
