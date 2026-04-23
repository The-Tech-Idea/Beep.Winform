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
              └── IPopupContentPanel (rows + search + footer)
                    └── ComboBoxPopupContent (default)
                    └── CardRowPopupContent, PillGridPopupContent, GroupedSectionsPopupContent,
                        ChipHeaderPopupContent, DenseAvatarPopupContent, MinimalCleanPopupContent

  └── ComboBoxLayoutEngine (stateless rect calculator)
  └── ComboBoxChipLayoutEngine (chip wrap / overflow)
  └── ComboBoxSearchEngine (prefix / contains / fuzzy filtering)
  └── ComboBoxStateFactory (render state snapshot)
  └── ComboBoxPopupModelBuilder (SimpleItem → popup row model)
```

---

## Variant Catalog

| Variant | Painter | Popup | Best For |
|---------|---------|-------|----------|
| **OutlineDefault** | 1px border, 6px radius, hover border tint | Clean rows, checkmark selected | Standard single-select dropdown |
| **OutlineSearchable** | Same as Outline + magnifying-glass icon | Search box auto-focused, highlight matches | Typeahead / autocomplete |
| **FilledSoft** | Subtle tinted fill, bottom underline | Card-style rows, warm hover | Material-style forms |
| **RoundedPill** | Full pill radius (height/2) | Highly rounded popup, search at bottom | iOS / modern picker aesthetic |
| **SegmentedTrigger** | Body + accent trigger zone | Accent category headers, row separators | Split-button / multi-level |
| **MultiChipCompact** | Chips in field, +N overflow | Checkbox rows, select-all/clear-all footer | Multi-select tags |
| **MultiChipSearch** | Chips + inline text cursor | Search + chip summary strip + checkbox rows | Searchable multi-select |
| **DenseList** | Tight padding, 4px radius, 32px button | Compact 28px rows, keyboard bar focus | Data-dense tables |
| **MinimalBorderless** | Transparent chrome, ghost underline | Shadow-only popup, no checkmark | Inline-edit in lists |

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
| PageUp / PageDown | Jump 6 rows |
| Enter | Commit focused row |
| Escape | Close without commit |
| Backspace / Delete | Remove last chip (multi-select) |
| Typeahead | Jump to first matching item (single-select, 700ms buffer) |

### Accessibility
- `AccessibleRole = ComboBox`
- `AccessibleName` / `AccessibleDescription` set on init
- `AccessibilityNotifyClients` on open/close state change

### Theme & DPI
- All colors/fonts resolved via `ComboBoxThemeTokens` from active `BeepTheme`.
- `ScaleLogicalX/Y` helpers for 125%, 150%, 200% DPI.
- RTL support via `MirrorRect` in layout and paint pipelines.

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

### Programmatically scroll to an item (chip click)
```csharp
// Clicking a chip body (not the ×) automatically opens the popup
// and calls _popupHost.FocusItem(item), scrolling the row into view.
// This is wired internally; no manual code required.
```

---

## Popup Quality Features

- **Shadow** — configurable depth (light/medium/heavy) per profile.
- **Corner radius** — matches field variant (6–16px).
- **Fade animation** — 150ms ease-out cubic open/close via `BeepPopupForm.Opacity`.
- **Row hover transition** — 100ms smooth background color blend per row.
- **Footer** — themed Apply/Cancel + Select-all/Clear-all + selected count badge.

---

## File Organization

```
ComboBoxes/
  BeepComboBox.cs            — public API + properties
  BeepComboBox.Core.cs       — fields, initialization, layout, dispose
  BeepComboBox.Drawing.cs    — DrawContent, Draw(g, rect), painter factory
  BeepComboBox.Events.cs     — mouse, keyboard, process dialog key
  BeepComboBox.Methods.cs    — Show/Close/ToggleDropdown, inline editor, search
  BeepComboBox.Properties.cs — designer-visible properties
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
    BeepComboBoxHelper.cs         — text measurement, color helpers, item lookup
  Painters/
    ComboBoxFieldPainterBase.cs              — base field painting (bg, border, text, image, chips, spinner, skeleton)
    DesignSystemComboBoxFieldPainterBase.cs  — adds filled/segmented/borderless/search overrides
    ComboBoxChipPainter.cs                 — chip bg, text, close button, +N badge
    OutlineDefaultComboBoxPainter.cs         — 15-line variant overrides
    ... (8 more variant painters)
  Popup/
    IComboBoxPopupHost.cs            — host interface
    IPopupContentPanel.cs            — content panel interface
    ComboBoxPopupHostForm.cs         — default host wiring form + content + placement
    ComboBoxPopupContent.cs          — default rows + search + footer + keyboard nav
    ComboBoxPopupRow.cs              — per-row paint + hover animation + commit
    ComboBoxPopupFooter.cs           — Apply/Cancel/SelectAll/ClearAll buttons
    ComboBoxPopupPlacementHelper.cs  — below/above flip + viewport clamp
    ComboBoxPopupHostProfile.cs      — per-variant popup settings
    *PopupHostForm.cs (9 files)      — one per variant, overrides CreateProfile/CreateContentPanel
    *PopupContent.cs (6 files)       — specialized content panels
```

---

## Recent Changes (2026-04-23)

- Added `ChipBodyRects` + chip-click → popup scroll-to-item.
- Migrated `UpdateLayout()` from `BeepComboBoxHelper.CalculateLayout` to `ComboBoxLayoutEngine.Compute` for single-source-of-truth.
- Removed dead `CalculateLayout` from `BeepComboBoxHelper`.
- Verified `BeepDropDownCheckBoxSelect` has no legacy popup code.
- Updated `BeepPopupForm` open/close fade animation (150ms ease-out cubic) and `ComboBoxPopupRow` hover transition (100ms blend).
