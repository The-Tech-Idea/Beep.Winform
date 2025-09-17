# ComparisonPricing Layout Plan

Intent
- Emulate a professional comparison/pricing matrix.
- Prioritize readability of plan tiers across columns with a clear feature list in the first column.

Grid Tweaks
- RowHeight: 28–32px
- ColumnHeaderHeight: 36–44px (bigger header)
- ShowGridLines: subtle (solid or disabled)
- ShowRowStripes: enabled
- UseHeaderGradient: optional; off by default
- UseBoldHeaderText: true
- HeaderCellPadding: 4–6px

Column/Cell Styling Guidelines
- First column (feature names): left aligned.
- Other columns (plans): center aligned.
- Header text slightly larger if theme allows.
- Optional: highlight preferred plan column later (follow-up).

Algorithm
1) Increase header height and row height.
2) Enable stripes; use solid thin gridlines or disabled gridlines for a cleaner look.
3) Make headers bold; increase header padding.
4) Apply alignment heuristics: first visible column left, others center (if API allows in helpers).
5) Recalculate layout and invalidate.

Out of Scope (for first pass)
- No custom icons (checkmarks) yet; use string values or existing cell content.
- No column freezing or reordering.

Test Checklist
- With 3–5 plan columns + feature column, verify header spacing and row striping.
- Resize grid; layout should remain stable.
