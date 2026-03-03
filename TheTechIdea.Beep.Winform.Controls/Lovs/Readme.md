# BeepListofValuesBox

A full-featured **List of Values (LOV)** input control for WinForms .NET, inspired by Oracle Forms LOV, Material Design 3 Select, and Fluent 2 Combobox patterns.

---

## Quick Start

```csharp
var lov = new BeepListofValuesBox
{
    LovTitle    = "Select Employee",
    LabelText   = "Employee",
    HelperText  = "Press F9 to browse all employees.",
    ShowKeyBadge = true
};

lov.ListItems = new List<SimpleItem>
{
    new SimpleItem { Value = "1001", Text = "Alice Brown",  Description = "Finance" },
    new SimpleItem { Value = "1002", Text = "Bob Chen",     Description = "IT" },
    new SimpleItem { Value = "1003", Text = "Carol Davies", Description = "HR" }
};

lov.SelectionChanged += (s, e) =>
  //  Console.WriteLine($"Selected: {lov.SelectedKey} — {lov.SelectedDisplayValue}");

panel.Controls.Add(lov);
```

---

## Properties

### Data

| Property | Type | Default | Description |
|---|---|---|---|
| `ListItems` | `List<SimpleItem>` | `[]` | In-memory item list (used when `ItemsLoader` is null). |
| `SelectedKey` | `string` | `""` | The raw key of the currently selected item (from `SimpleItem.Value`). |
| `SelectedDisplayValue` | `string` | `""` | The display label of the current selection (from `SimpleItem.Text`). Read-only. |
| `SelectedItem` | `SimpleItem?` | `null` | The full `SimpleItem` for the current selection. |
| `ItemsLoader` | `Func<CancellationToken, Task<List<SimpleItem>>>?` | `null` | Async factory. When set, the popup opens with a spinner and calls this delegate in the background. |

### LOV Popup

| Property | Type | Default | Description |
|---|---|---|---|
| `LovTitle` | `string` | `"Select Value"` | Title shown in the popup header bar. |
| `MaxPopupHeight` | `int` | `360` | Maximum height of the popup form (pixels). |
| `LovColumns` | `List<BeepColumnConfig>` | `[]` | Optional explicit column definitions for the popup grid. Leave empty for auto Key + Display-Value columns. |

### Visual

| Property | Type | Default | Description |
|---|---|---|---|
| `ShowKeyBadge` | `bool` | `true` | Paints a coloured pill badge showing the raw key next to the display value. |
| `LabelText` | `string` | `""` | Label rendered above the field. Setting a non-empty value auto-enables `LabelTextOn`. |
| `HelperText` | `string` | `""` | Hint text rendered below the field. Setting a non-empty value auto-enables `HelperTextOn`. |
| `ErrorText` | `string` | `""` | Validation error shown below the field (overrides helper text). Setting this also sets `HasError`. |
| `HasError` | `bool` | `false` | Drives error-state border and `ErrorText` visibility. |

### History

| Property | Type | Default | Description |
|---|---|---|---|
| `RecentSelections` | `List<SimpleItem>` | `[]` | Most-recent 5 selections (oldest-first). Readable after each `SelectionChanged`. Can be set to restore history between sessions. |

---

## Events

| Event | Signature | When |
|---|---|---|
| `SelectionChanged` | `EventHandler` | Fires after each confirmed value change (popup accept or programmatic set). |

---

## Keyboard Navigation

| Key | Action |
|---|---|
| **F9** | Open popup (Oracle Forms standard). Pre-seeds search with current key field text. |
| **Alt + ↓** | Open popup (Windows combobox standard). |
| **Escape** | Close popup without changing selection. |
| **Del / Backspace** | Clear selection (when key field is not focused). |

---

## Async Loading

Set `ItemsLoader` to defer the data fetch until the popup opens:

```csharp
lov.ItemsLoader = async (ct) =>
{
    await Task.Delay(300, ct);   // simulate network call
    return await myService.GetEmployeesAsync(ct);
};
```

The popup appears immediately with a spinning overlay while loading. If the user dismisses
the popup before loading finishes the `CancellationToken` is cancelled automatically.

---

## Recent Selections (Phase 13)

The control tracks the last 5 items the user has picked. Each time the popup opens, those
items appear as clickable chip buttons in the popup header — a single click re-selects without
searching.

**Persist across sessions:**

```csharp
// Save (e.g. on form close)
Properties.Settings.Default.LovHistory =
    System.Text.Json.JsonSerializer.Serialize(lov.RecentSelections);

// Restore (e.g. on form load)
var json = Properties.Settings.Default.LovHistory;
if (!string.IsNullOrEmpty(json))
    lov.RecentSelections =
        System.Text.Json.JsonSerializer.Deserialize<List<SimpleItem>>(json);
```

---

## Multi-Column Popup

By default the popup shows two columns: **Key** and **Display Value**.  
Supply `LovColumns` to override:

```csharp
lov.LovColumns = new List<BeepColumnConfig>
{
    new BeepColumnConfig { ColumnName = "Value",       ColumnCaption = "ID",         Width = 70  },
    new BeepColumnConfig { ColumnName = "Text",        ColumnCaption = "Name",       Width = 160 },
    new BeepColumnConfig { ColumnName = "Description", ColumnCaption = "Department", Width = 100 }
};
```

---

## Design-Time Smart Tags

Open the smart-tag panel (▶ arrow) on the control to access:

- **Title** — popup header title
- **Label Text** — above-field label
- **Helper Text** — below-field hint
- **Show Key Badge** — toggle pill badge
- **Max Popup Height** — popup height cap
- **Employee Preset** — one-click configuration for a standard employee LOV
- **Compact Preset** — minimal LOV with no label or badge

---

## Architecture

| File | Role |
|---|---|
| `BeepListofValuesBox.cs` | Main control — inherits `BaseControl`. Handles layout, painting, keyboard, async popup opening. |
| `BeepLovPopup.cs` | Popup form — inherits `Form` (no border, no taskbar). Hosts `BeepGridPro` for multi-column display, search bar, recent chips, loading overlay. |
| `Helpers/LovFontHelpers.cs` | Font resolution via `BeepThemesManager.ToFont()`. |
| `Helpers/LovIconHelpers.cs` | Icon path helpers (`SvgsUI` lookups). |
| `Helpers/LovThemeHelpers.cs` | Theme color accessors (background, border, focus colors). |
| `Helpers/LovStyleHelpers.cs` | Layout ratios and spacing per `BeepControlStyle`. |
| `Models/LovColorConfig.cs` | Color configuration value object. |
| `Models/LovStyleConfig.cs` | Style configuration value object. |

---

## Theming

All colors are pulled from the active `IBeepTheme`:

| Usage | Theme property |
|---|---|
| Field background | `PanelBackColor` |
| Field text | `ForeColor` |
| Key badge background | `AccentColor` |
| Key badge text | Auto-contrast (black/white) via ITU-R BT.601 luminance |
| Placeholder text | `SecondaryTextColor` |
| Popup header | `GridHeaderBackColor` |
| Error border / text | `ErrorColor` |
| Helper text | `SecondaryTextColor` |

Set `Theme = "MyThemeName"` or `UseThemeColors = true` on the control; changes are forwarded to the popup automatically.

---

## Inheritance

```
System.Windows.Forms.Control
  └── BaseControl          (Beep base — theme, icons, DPI, border, label/helper system)
        └── BeepListofValuesBox
```
