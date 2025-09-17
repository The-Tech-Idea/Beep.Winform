# BeepGridPro Layout Presets – Main Plan

Goal
- Provide a set of professional, ready-to-use layout presets for `BeepGridPro` under `GridX/Layouts`.
- Presets are applied programmatically to reconfigure sizing, spacing, and rendering options for specific use?cases.

Reference Source
- Requested first source: comparison/price table design (reference link provided by user).
- Due to offline constraints, we derive common, well-known comparison table patterns and document them clearly.

Initial Layout Candidates (derived from comparison tables)
1) ComparisonPricing
   - A feature matrix: first column lists features, subsequent columns are pricing tiers/plans.
   - Bold, larger column headers, striped rows, subdued gridlines or none, centered plan values.
2) FeatureMatrixSimple
   - Minimal feature vs. plan columns, thin separators, neutral headers.
3) FeatureMatrixStriped
   - Same as simple but with stronger alternating row stripes and clearer header separation.
4) BorderlessMinimal
   - No gridlines, subtle hover, extra whitespace; minimalistic look.
5) CardPricingGrid (optional follow-up)
   - Card-like header rows; slightly elevated headers; more padding; visually grouped columns.

Architecture
- Define a tiny preset contract: `IGridLayoutPreset { void Apply(BeepGridPro grid); }`.
- Provide an extension: `grid.ApplyLayoutPreset(preset)`.
- Each preset is a small helper class that:
  - Tweaks grid?level layout: `RowHeight`, `ColumnHeaderHeight`, `ShowColumnHeaders`.
  - Tweaks render helper flags: `ShowGridLines`, `ShowRowStripes`, `UseHeaderGradient`, `UseBoldHeaderText`, etc.
  - Optionally updates sizing via `Layout.Recalculate()`.

Data Assumptions for ComparisonPricing
- Grid should have at least: a first "Feature" column and 2+ plan columns (Basic/Pro/Enterprise style).
- Values are typically boolean/tick, checkmarks, or text; centering plan values usually works well.
- We do NOT create columns or data here; we adapt the presentation.

Implementation Order
- Phase 1: Implement `ComparisonPricingLayoutHelper`.
- Phase 2+: Add remaining presets one-by-one, each with its own plan file and helper.

Usage Example
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

// Once you have your grid and columns bound
var preset = new ComparisonPricingLayoutHelper();
grid.ApplyLayoutPreset(preset); // applies layout + recalculates + invalidates
```

Notes
- We intentionally avoid assumptions about per-column flags (e.g., Frozen) to keep compatibility.
- If advanced styling is needed later (e.g., custom cell drawing), we’ll extend `GridRenderHelper` where appropriate.
