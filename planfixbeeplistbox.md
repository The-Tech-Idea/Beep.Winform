# BeepListBox Painter Update Plan

This document outlines the required updates to align BeepListBox painters with the new centralized layout and hit-testing pattern, mirroring BeepTree’s BaseControl integration.

## Contracts

- Inputs
  - Owner: `BeepListBox`
  - Painting rect: `DrawingRect`
  - Theme: `_currentTheme`
- Layout rects per item (from `BeepListBoxLayoutHelper`):
  - `RowRect`: full clickable row
  - `CheckRect`: checkbox area (when `ShowCheckBox`)
  - `IconRect`: image/icon area (when `ShowImage` and item has `ImagePath`)
  - `TextRect`: text area
- Hit testing
  - Registered via `BeepListBoxHitTestHelper.RegisterHitAreas()` with names: `row_{guid}`, `check_{guid}`, `icon_{guid}`, `text_{guid}`
- Success criteria
  - Clicks on any part resolve correctly via `_hitHelper.HitTest`
  - Item heights and padding are consistent across painters via `GetPreferredItemHeight()` and `GetPreferredPadding()`
  - DPI/viewport transforms respected; no clipping artifacts

## Painter checklist (apply to each IListBoxPainter)

1. Initialize
   - Implement `Initialize(BeepListBox owner, Theme theme)` and cache references.
   - Read preferred item height from painter style; default 32–36.
2. Layout usage
   - Trust `LayoutHelper` rects. Do not recompute per painter.
   - Clip to `drawingRect` when painting rows.
3. Rendering
   - Checkbox: draw only if `owner.ShowCheckBox` is true; align within `CheckRect`.
   - Icon: if `owner.ShowImage` and item has `ImagePath`, render inside `IconRect` using `StyledImagePainter` with theme tint if `owner.ApplyThemeOnImage`.
   - Text: render inside `TextRect` with `owner.TextFont` and theme text color. Ellipsize when needed.
   - Hover/selection: use `owner.ShowHilightBox` and `owner.SelectedItem`/`_hoveredItem` to paint backgrounds inside `RowRect`.
4. Accessibility/state
   - Respect disabled items (future). Use muted colors.
5. Registration
   - Do not call hit-test registration directly; helpers handle this during `DrawContent`.

## Painters to update

- StandardListBoxPainter
- MinimalListBoxPainter
- OutlinedListBoxPainter
- RoundedListBoxPainter
- MaterialOutlinedListBoxPainter
- FilledListBoxPainter
- BorderlessListBoxPainter
- CategoryChipsPainter
- SearchableListPainter
- WithIconsListBoxPainter
- CheckboxListPainter
- SimpleListPainter
- LanguageSelectorPainter
- CardListPainter
- CompactListPainter
- GroupedListPainter
- TeamMembersPainter
- FilledStylePainter
- FilterStatusPainter
- OutlinedCheckboxesPainter
- RaisedCheckboxesPainter
- MultiSelectionTealPainter
- ColoredSelectionPainter
- RadioSelectionPainter
- ErrorStatesPainter
- CustomListPainter

Update order suggestion
- Start with Standard, Simple, CheckboxList, WithIcons as references.
- Validate item rects use and interaction per painter.

## Edge cases to test

- Empty list; ensure no exceptions, no hit areas registered.
- Long text; ellipsize within `TextRect`.
- High DPI (150–300%); ensure placement and spacing consistent.
- Checkbox-only lists; icon hidden.
- Icon-only lists; text hidden.
- Mixed items with/without ImagePath.

## Done when

- Clicking check toggles checkbox without selecting unless painter design specifies both.
- Clicking icon/text/row selects and raises `ItemClicked` and notifies `BeepPopupForm` when hosted.
- Keyboard Enter mirrors click selection; Up/Down/Home/End navigation works.
- Layout and hit areas refresh on resize/theme/item changes (already wired via delayed invalidate).
