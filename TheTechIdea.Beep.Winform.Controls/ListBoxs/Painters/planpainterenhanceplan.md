# ListBox Painter Enhancement Plan

This document tracks the enhancement of ListBox painters in `ListBoxs\Painters`. Each painter will be updated to:
- Inherit from `BaseListBoxPainter`.
- Use shared icon drawing logic from `BaseListBoxPainter` (ensure icons are painted if present).
- Draw checkbox if `ShowCheckBox` is true, using a consistent method.
- Implement distinct visual styles per painter, matching their `ListBoxType` intent.
- Use `BeepStyling` for painting item background, shadow, and border for each item, ensuring consistency with the overall theme and style system.

## Why Use BeepStyling?
- Centralizes all painting logic for backgrounds, borders, shadows, and images.
- Ensures consistent look and feel across all controls and painters.
- Makes it easier to update themes and styles globally.
- Reduces code duplication and improves maintainability.
- All ListBox styles must paint icons if they are present, using the shared `DrawItemImage` method from `BaseListBoxPainter`.

## Enhancement Steps
1. List all painter classes in the folder.
2. For each painter:
   - Refactor to inherit from `BaseListBoxPainter` (if not already).
   - In the `Paint` method or `DrawItem`, ensure icons are drawn using `DrawItemImage` from `BaseListBoxPainter`.
   - Draw checkbox if `ShowCheckBox` is true, using `DrawCheckbox` from `BaseListBoxPainter`.
   - In `DrawItemBackground`, use `BeepStyling` methods:
     - `BeepStyling.PaintControlBackground(g, path, style, theme, useThemeColors, state)` for background.
     - `BeepStyling.PaintControlBorder(g, path, style, theme, useThemeColors, state)` for border.
     - Shadow is handled within the background painter if applicable.
   - Apply unique styling for background, borders, text, etc., using `BeepStyling` helpers where possible.
   - Mark as completed in this plan.
3. Update this document after each painter enhancement, specifying which painters use `BeepStyling` and how.

## Progress Table
| Painter Class                | Status      | Notes |
|-----------------------------|-------------|-------|
| StandardListBoxPainter      | Completed   | Uses BaseListBoxPainter, draws icon & checkbox, distinct style |
| MinimalListBoxPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| OutlinedListBoxPainter      | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| RoundedListBoxPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| MaterialOutlinedListBoxPainter | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| FilledListBoxPainter        | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| BorderlessListBoxPainter    | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| CategoryChipsPainter        | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| SearchableListPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| WithIconsListBoxPainter     | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| CheckboxListPainter         | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| SimpleListPainter           | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| LanguageSelectorPainter     | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| CardListPainter             | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| CompactListPainter          | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| GroupedListPainter          | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| TeamMembersPainter          | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| FilledStylePainter          | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| FilterStatusPainter         | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| OutlinedCheckboxesPainter   | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| RaisedCheckboxesPainter     | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| MultiSelectionTealPainter   | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| ColoredSelectionPainter     | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| RadioSelectionPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| ErrorStatesPainter          | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| ChakraUIListBoxPainter     | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| HeroUIListBoxPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
| RekaUIListBoxPainter       | Completed   | Uses BeepStyling for background, border, shadow; ensures icons painted |
