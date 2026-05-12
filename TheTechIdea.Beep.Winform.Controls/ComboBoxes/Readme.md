# ComboBoxes — Beep WinForm Controls

Production-ready combo box controls with 9 visual variants, multi-select chips, inline editing, searchable popups, and full theme integration.

---

## Architecture

```
BeepComboBox (owner-drawn field)
  └── IComboBoxPainter (field chrome + text + chips)
        └── ComboBoxFieldPainterBase
              └── DesignSystemComboBoxFieldPainterBase
                    └── 9 variant painters (OutlineDefault, OutlineSearchable, FilledSoft, RoundedPill,
                          SegmentedTrigger, MultiChipCompact, MultiChipSearch, DenseList, MinimalBorderless)

  └── IComboBoxPopupHost (dropdown window)
        └── ComboBoxPopupHostForm
              └── BeepComboBoxPopupForm (hosts BeepListBox directly)
                    ├── BeepListBox (handles scrolling, keyboard nav, PageUp/PageDown)
                    ├── BeepTextBox (optional search)
                    └── ComboBoxPopupFooter (optional for multi-select)

  └── ComboBoxLayoutEngine (stateless rect calculator)
  └── ComboBoxChipLayoutEngine (chip wrap / overflow)
  └── ComboBoxSearchEngine (prefix / contains / fuzzy filtering)
  └── ComboBoxStateFactory (render state snapshot)
  └── ComboBoxPopupModelBuilder (SimpleItem → popup row model)

Note: All popup content is now handled by BeepListBox with automatic ListBoxType mapping
from ComboBoxType (via ComboBoxListBoxTypeMapper). This provides consistent behavior
across all variants with proper scrolling, keyboard navigation, and scrollbar visibility.
```

---

## Variant Catalog

| Variant | Painter | Popup | Best For |
|---------|---------|-------|----------|
| **OutlineDefault** | 1px border, 6px radius, hover border tint | ListBoxType.Outlined, clean rows | Standard single-select dropdown |
| **OutlineSearchable** | Same as Outline + magnifying-glass icon | ListBoxType.SearchableList, search box auto-focused | Typeahead / autocomplete |
| **FilledSoft** | Subtle tinted fill, bottom underline | ListBoxType.Filled, card-style rows | Material-style forms |
| **RoundedPill** | Full pill radius (height/2) | ListBoxType.Rounded, highly rounded popup | iOS / modern picker aesthetic |
| **SegmentedTrigger** | Body + accent trigger zone | ListBoxType.NavigationRail, accent category headers | Split-button / multi-level |
| **MultiChipCompact** | Chips in field, +N overflow | ListBoxType.ChipStyle, checkbox rows | Multi-select tags |
| **MultiChipSearch** | Chips + inline text cursor | ListBoxType.ChipStyle + search, chip summary strip | Searchable multi-select |
| **DenseList** | Tight padding, 4px radius, 32px button | ListBoxType.Compact, compact 30px rows | Data-dense tables |
| **MinimalBorderless** | Transparent chrome, ghost underline | ListBoxType.Borderless, shadow-only popup | Inline-edit in lists |

**Custom ListBoxType**: Use `DropdownListBoxType` property on BeepComboBox to override the default mapping.

---

## Key Behaviors

### Selection Modes
- **Single-select** — click row or press Enter to commit; Escape cancels.
- **Multi-select** — checkbox rows; chips appear in field; Apply/Cancel footer buffers changes.
- **Free-text** — type any value; delimiter key (comma/Enter) creates a chip token when `AllowFreeText` + multi-select.

### Search
- Prefix match (default), contains fallback, fuzzy subsequence scoring.
- Highlight matching substring in row text (`FocusBorderColor` accent).
- Empty / loading / no-results states with themed icons.

### Keyboard Navigation
| Key | Action |
|-----|--------|
| ↑ / ↓ | Move focus (or open popup) |
| Home / End | First / last row |
| PageUp / PageDown | Jump by visible page size (BeepListBox handles this correctly) |
| Enter | Commit focused row |
| Escape | Close without commit |
| Backspace / Delete | Remove last chip (multi-select) |
| Typeahead | Jump to first matching item (single-select, 700ms buffer) |

### Scrollbar Behavior
- BeepListBox's built-in BeepScrollBar handles scrolling correctly
- Scrollbar only appears when content exceeds viewport
- No "Page Up/Down" text is shown (custom BeepScrollBar is graphical only)
- PageUp/PageDown correctly moves by visible items

---

## Usage Examples

### Basic single-select
```csharp
var combo = new BeepComboBox
{
    ComboBoxType = ComboBoxType.OutlineDefault,
    ListItems = new BindingList<SimpleItem>
    {
        new SimpleItem { Text = "Apple",  Item = 1 },
        new SimpleItem { Text = "Banana", Item = 2 }
    }
};
combo.SelectedItemChanged += (s, e) => Console.WriteLine(e.SelectedItem?.Text);
```

### Searchable typeahead
```csharp
var combo = new BeepComboBox
{
    ComboBoxType = ComboBoxType.OutlineSearchable,
    AllowFreeText = true,
    AutoComplete = true,
    AutoCompleteMode = BeepAutoCompleteMode.Prefix
};
```

### Multi-select with chips
```csharp
var combo = new BeepComboBox
{
    ComboBoxType = ComboBoxType.MultiChipCompact,
    AllowMultipleSelection = true,
    UseApplyCancelFooter = true,
    ShowSelectAll = true
};
```

### Inline free-text tokenization
```csharp
var combo = new BeepComboBox
{
    ComboBoxType = ComboBoxType.MultiChipSearch,
    AllowMultipleSelection = true,
    AllowFreeText = true,
    TokenDelimiters = new[] { ',', ';', '\n' }
};
```

### Custom ListBoxType for popup
```csharp
var combo = new BeepComboBox
{
    ComboBoxType = ComboBoxType.OutlineDefault,
    DropdownListBoxType = ListBoxType.CommandList  // Override default ListBoxType.Outlined
};
```

### Programmatically scroll to an item (chip click)
```csharp
// Clicking a chip body (not the ×) automatically opens the popup
// and calls _popupHost.FocusItem(item), scrolling the row into view.
// This is wired internally; no manual code required.
```

---

## Popup Quality Features

- **BeepListBox Integration** — Uses BeepListBox for all popup content, providing consistent scrolling and keyboard navigation
- **Shadow** — configurable depth (light/medium/heavy) per profile.
- **Corner radius** — matches field variant (6–16px).
- **Fade animation** — 100ms ease-out cubic open/close via `BeepPopupForm.Opacity`.
- **Row hover transition** — 100ms smooth background color blend per row (handled by BeepListBox painter).
- **Footer** — themed Apply/Cancel + Select-all/Clear-all + selected count badge.
- **Correct PageUp/PageDown** — Uses BeepListBox's viewport height for accurate page sizing.
- **No scrollbar text issues** — BeepScrollBar is purely graphical with no text labels.

---

## File Organization

```
ComboBoxes/
  BeepComboBox.cs            — public API + properties
  BeepComboBox.Core.cs       — fields, initialization, layout, dispose
  BeepComboBox.Drawing.cs    — DrawContent, Draw(g, rect), painter factory
  BeepComboBox.Events.cs     — mouse, keyboard, process dialog key
  BeepComboBox.Methods.cs    — Show/Close/ToggleDropdown, inline editor, search
  BeepComboBox.Properties.cs — designer-visible properties (includes DropdownListBoxType)
  BeepDropDownCheckBoxSelect.cs — standalone multi-select control
  Helpers/
    ComboBoxLayoutEngine.cs       — stateless rect calculator
    ComboBoxChipLayoutEngine.cs   — chip wrap + overflow
    ComboBoxSearchEngine.cs       — filter + highlight ranges
    ComboBoxStateFactory.cs       — render state snapshot
    ComboBoxPopupModelBuilder.cs  — SimpleItem → popup row model
    ComboBoxRenderState.cs        — 22-field immutable state
    ComboBoxPopupModel.cs         — row kinds + model
    ComboBoxVisualTokens.cs       — per-variant geometry tokens
    ComboBoxListBoxTypeMapper.cs  — maps ComboBoxType → ListBoxType for popup
    BeepComboBoxHelper.cs         — text measurement, color helpers, item lookup
  Painters/
    ComboBoxFieldPainterBase.cs              — base field painting (bg, border, text, image, chips, spinner, skeleton)
    DesignSystemComboBoxFieldPainterBase.cs  — adds filled/segmented/borderless/search overrides
    ComboBoxChipPainter.cs                   — chip bg, text, close button, +N badge
    OutlineDefaultComboBoxPainter.cs         — 15-line variant overrides
    ... (8 more variant painters)
  Popup/
    IComboBoxPopupHost.cs            — host interface
    ComboBoxPopupHostForm.cs         — default host using BeepComboBoxPopupForm
    BeepComboBoxPopupForm.cs         — NEW: simplified popup form hosting BeepListBox directly
    ComboBoxPopupRow.cs              — per-row paint + hover animation + commit
    ComboBoxPopupFooter.cs           — Apply/Cancel/SelectAll/ClearAll buttons
    ComboBoxPopupPlacementHelper.cs  — below/above flip + viewport clamp
    ComboBoxPopupHostProfile.cs      — per-variant popup settings
    ComboBoxThemeTokens.cs           — theme colors for popup rows
```

**Note**: Legacy popup content files (ComboBoxPopupContent, DenseAvatarPopupContent, ChipHeaderPopupContent, etc.) 
have been removed. All popup content is now handled by BeepListBox with automatic ListBoxType mapping.

---

## Recent Changes (2026-05-11)

- **Rebuilt popup system** — Replaced legacy multi-content-panel system with single BeepComboBoxPopupForm hosting BeepListBox
- **Fixed PageUp/PageDown** — Now correctly uses BeepListBox viewport height for accurate page sizing
- **Fixed scrollbar issues** — No more "Page Up/Down" text appearing; BeepScrollBar is graphical only
- **Added DropdownListBoxType property** — Allows overriding the default ListBoxType mapping for any ComboBoxType
- **Deleted legacy popup content files** — DenseAvatarPopupContent, ChipHeaderPopupContent, MinimalCleanPopupContent, CardRowPopupContent, GroupedSectionsPopupContent, PillGridPopupContent, ComboBoxPopupContent, ComboBoxListBoxPopupContent, IPopupContentPanel
- **Simplified architecture** — Single BeepComboBoxPopupForm handles all popup content via BeepListBox
- **Fade animation** — Reduced to 100ms for faster popup open/close