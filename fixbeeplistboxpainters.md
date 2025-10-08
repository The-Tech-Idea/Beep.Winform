# BeepListBox Painters Modernization Log

This log tracks migrating painters to the centralized `BeepListBoxLayoutHelper` (ListItemInfo) and hit-test model for consistent UX.

## Contract

- Use `LayoutHelper.GetCachedLayout()`; don’t recompute per-item geometry.
- Paint:
  - Background within `RowRect`.
  - Checkbox in `CheckRect` when `ShowCheckBox`.
  - Icon in `IconRect` (StyledImagePainter) when `ShowImage` and item has ImagePath.
  - Text in `TextRect` with ellipsis.
- Hover/selected state based on `RowRect.Contains(mouse)` and `owner.SelectedItem`.

## Updated painters

- BaseListBoxPainter
  - Now pulls cached layout and iterates `ListItemInfo`.
  - DrawItems uses `RowRect`; hover detection is viewport-aware.
- StandardListBoxPainter
  - Uses `CheckRect`, `IconRect`, `TextRect` from layout.
  - Improves alignment and spacing; no per-item recompute.
- MinimalListBoxPainter
  - Inherits Standard behavior; background kept minimal.
- SimpleListPainter
  - Adds left selection indicator and defers to Minimal/Standard drawing for rects.
- RoundedListBoxPainter
  - Uses layout rects for content (checkbox, icon, text) while keeping rounded row background.
- CheckboxListPainter
  - Confirms checkbox support, height tuned for click targets; uses Standard rect behavior.
- WithIconsListBoxPainter
  - Taller item height for icon clarity; inherits rect-driven rendering from Outlined/Standard.
- FilledListBoxPainter
  - Uses layout rects for icon/text; keeps elevated rounded backgrounds and shadow.
- RadioSelectionPainter
  - Uses TextRect/IconRect from layout; reserves right-aligned radio area; two-line text with ellipsis.
- OutlinedCheckboxesPainter
  - Uses CheckRect and TextRect from layout; outlined checkbox styling preserved.

## Next to update

1) WithIconsListBoxPainter
2) OutlinedListBoxPainter (inherits Standard rects; divider only)
3) BorderlessListBoxPainter (inherits Minimal/Standard rects)
4) MaterialOutlinedListBoxPainter (inherits Outlined)
5) CompactListPainter
6) CategoryChipsPainter
7) SearchableListPainter
8) LanguageSelectorPainter
9) CardListPainter
13) GroupedListPainter
14) TeamMembersPainter
15) FilledStylePainter
16) FilterStatusPainter
17) OutlinedCheckboxesPainter
18) RaisedCheckboxesPainter
19) MultiSelectionTealPainter
20) ColoredSelectionPainter
21) RadioSelectionPainter
22) ErrorStatesPainter
23) CustomListPainter

We’ll proceed in the above order, validating hover/selection visuals and checkbox/icon/text alignment against the rects.
