# ComboBox Overhaul Manual QA Matrix

## Scope
- `TheTechIdea.Beep.Winform.Controls/ComboBoxes/*`
- `TheTechIdea.Beep.Winform.Controls/ComboBoxes/Popup/*`
- All `ComboBoxType` variants:
  - `OutlineDefault`
  - `OutlineSearchable`
  - `FilledSoft`
  - `RoundedPill`
  - `SegmentedTrigger`
  - `MultiChipCompact`
  - `MultiChipSearch`
  - `DenseList`
  - `MinimalBorderless`
  - `CommandMenu`
  - `VisualDisplay`

## Environment Matrix
- DPI: `100%`, `125%`, `150%`, `200%`
- Direction: `LTR`, `RTL`
- Theme: at least one light + one dark

## Test Data Sets
- Small list: 10 items (mixed enabled/disabled)
- Grouped list: group headers + separators
- Rich list: image path + subtext + trailing text/value
- Large list: 1,000+ items for stress
- Empty list

## Core Field State Matrix (Per ComboBoxType)
- [ ] Normal
- [ ] Hover (control)
- [ ] Hover (dropdown button)
- [ ] Focused
- [ ] Open
- [ ] Disabled
- [ ] Loading
- [ ] Validation `Error/Warning/Success`
- [ ] Clear button visible + hover + click
- [ ] Leading icon/image present + absent

Expected:
- No clipping, icon overlap, or border drift
- Loading indicator rendered once (no flicker/double-spinner)
- Clear/validation/dropdown button geometry remains stable across DPI

## Popup Behavior Matrix (Per Popup Variant)
- [ ] Open popup from mouse click
- [ ] Open popup from keyboard
- [ ] Close on Escape
- [ ] Close on outside click
- [ ] Close on commit in single-select
- [ ] Keep open on multi-select toggles (unless configured otherwise)
- [ ] Search filtering updates rows correctly
- [ ] Focus row visible while navigating

Expected:
- Keyboard focus index is deterministic
- Non-selectable rows (headers/separators/state rows) never commit
- Selectable rows commit correctly by Enter/click

## Row Kind Contract Matrix
- [ ] `Normal`
- [ ] `Selected`
- [ ] `Disabled`
- [ ] `GroupHeader`
- [ ] `Separator`
- [ ] `WithSubText`
- [ ] `CheckRow`
- [ ] `EmptyState`
- [ ] `LoadingState`
- [ ] `NoResults`

Expected:
- State rows are informational only and non-interactive
- Group header/separator are non-interactive in all variants
- Check rows toggle and repaint consistently

## Multi-Select Workflow Matrix
- [ ] Toggle one item repeatedly (rapid)
- [ ] Select-all on filtered list
- [ ] Clear-all on filtered list
- [ ] Apply/Cancel footer path
- [ ] Primary-action footer path
- [ ] Chip remove (header variants)

Expected:
- No UI freeze/event storm on large lists
- Selection snapshot restored on cancel (when configured)
- Footer selected-count stays correct

## Property Contract Matrix
- [ ] `ShowOptionDescription`
- [ ] `ShowStatusIcons`
- [ ] `EmptyStateText`
- [ ] `AutoFlip`
- [ ] `MinDropdownWidth`
- [ ] `DropdownButtonWidth`
- [ ] `InnerPadding`

Expected:
- Property overrides take precedence over token defaults
- Popup placement respects `AutoFlip` and min width
- Description/icons visibility follows property toggles

## RTL Matrix
- [ ] Dropdown button mirrored correctly
- [ ] Field text/icon alignment mirrored
- [ ] Popup content aligned and navigable in RTL
- [ ] Search/footer interaction unchanged in RTL

Expected:
- No hit-test mismatch after mirroring
- Keyboard navigation behavior unchanged semantically

## Pass/Fail Summary Template
- Date:
- Tester:
- Build:
- Theme(s):
- DPI(s):
- Failures:
  - File/variant:
  - Repro steps:
  - Expected vs actual:
  - Screenshot/video:
