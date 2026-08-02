# 06 — Keyboard, autocomplete and accessibility

## Scope

`FilterKeyboardHandler.cs` (353), `FilterAutocompletePopup.cs` (339),
`FilterSuggestionProvider.cs` (361), `FilterIconProvider.cs` (315).

## Work

- [ ] **Accessible tree.** `BeepFilter` paints its surface rather than hosting child controls, so
      MSAA has nothing to enumerate on its own — the same situation as `BeepDisplayContainer2`, where
      a screen reader saw one anonymous client area until `CreateAccessibilityInstance` was added.
      Expose the criteria as named children with roles, values and bounds.
- [ ] **Autocomplete popup semantics.** A suggestion list needs `Escape` to dismiss without
      committing, `Enter` to commit, arrows to move, and focus returned to the field on close. Check
      also that dismissing does not leave a partially-applied criterion.
- [ ] **Focus indicator.** Confirm the focused criterion is visibly marked (WCAG 2.4.7). Check
      whether one already exists before adding: in `DisplayContainers` a complete, correct focus ring
      was already implemented and never drew, because the flag driving it was read once and assigned
      nowhere. A second implementation was written before that was noticed.
- [ ] **Hit targets.** The painters draw pills and chips with remove/edit affordances; measure them
      against a 24 logical px minimum. In both the grid header and the container tab strip, the
      affordance rect was being used as *both* the glyph and the hit area, giving 13px targets.
- [ ] **High contrast.** Criteria and their affordances remain legible with
      `SystemInformation.HighContrast`.

## Verification

- Per-control accessible **names**, not a tree walk. A traversal via `GetChildCount`/`GetChild` on a
  control that has not overridden them returns nothing and proves nothing — a stock `Form` holding a
  `Label` and a `Button` measures 0 descendants, which is how that check produced a false "screen
  reader sees an empty dialog" verdict in the DialogsManagers program.
- Keyboard reachability asserted by **driving keys**, not by reading the handler.
- Hit targets measured from the rectangles the painter publishes, at 100% / 150% / 200%.
