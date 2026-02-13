# Theme Fix File - HolographicTheme

## Scope
- Folder-level audit for `ColorPalette`, `Buttons`, `ComboBox`, and `TextBox` theme parts.
- Cross-control consistency checks against `BeepButton`, `BeepComboBox`, and `BeepTextBox`.

## Theme Parts Status
- `BeepTheme.ColorPalette.cs`: OK (all required assignments present).
- `BeepTheme.Buttons.cs`: OK (all required assignments present).
- `BeepTheme.ComboBox.cs`: OK (all required assignments present).
- `BeepTheme.TextBox.cs`: OK (all required assignments present).

## Control-Level Findings (Affect All Themes)
- `BeepButton.cs`: hover rendering uses `ButtonSelectedHover*` instead of `ButtonHover*` in draw logic. This causes non-selected hover states to look like selected buttons.
- `BeepComboBoxHelper.cs`: placeholder color reads `TextBoxPlaceholderColor` (fallback `Color.Gray`) instead of a ComboBox-specific placeholder token; disabled/error fallbacks also use hardcoded colors.
- `BeepTextBox.Theme.cs`: applies `BorderColor = _currentTheme.BorderColor` instead of `TextBoxBorderColor`, so textbox border can drift from textbox theme tokens.

## Fix Plan
- Update `BeepButton` draw state mapping to use `ButtonHoverBackColor` and `ButtonHoverForeColor` for plain hover state.
- Add/use a dedicated ComboBox placeholder token (`ComboBoxPlaceholderColor`) and avoid hardcoded fallback grays/reds in helper methods.
- In `BeepTextBox.ApplyTheme()`, map primary border assignment to `TextBoxBorderColor` for consistent textbox visuals.
- Re-test this theme in forms containing mixed `BeepButton`, `BeepComboBox`, and `BeepTextBox` controls.
