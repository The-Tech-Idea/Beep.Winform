# BeepListofValuesBox — Enhancement Plan

**Reference**: Oracle Forms LOV, Material Design 3 Select, Ant Design Select,  
Oracle JET InputSearch, Fluent 2 Combobox, Figma Auto Layout patterns.

---

## Current-State Audit

| Area | Current | Problem |
|---|---|---|
| Layout | 3 separate `BeepTextBox` + `BeepButton` | Not unified; looks fragmented, 20/70/10 split is poor |
| Popup | `BeepContextMenu` | No column headers, no record count, limited for LOV |
| Label | None | No above-field label, no required indicator |
| Helper/Error text | None | No below-field helper text, error messages use tooltip only |
| Keyboard | None | No F9/Enter/Escape/Arrow navigation (Oracle standard) |
| Clear button | None | No ✕ to reset selection |
| Loading state | None | No spinner for async item loading |
| Key badge | Always in its own textbox | Wastes space, looks broken for long keys |
| Font API | `LovFontHelpers` (partially wired) | `ApplyFontTheme()` is empty no-op; should use `BeepThemesManager.ToFont()` |
| GDI | `new SolidBrush`, `new Font` in sub-controls | Handled by sub-controls but no explicit control over leak prevention |
| Validation | Tooltip + text reset | No visual indicator (red border, error text) |
| Multi-column popup | Not supported | Oracle LOV shows a multi-column dialog |
| Design-time | No designer registration | Not easily discoverable |

---

## Target Design (UI/UX Reference)

### Layout — Single Unified Field

```
┌─────────────────────────────────────────────────────────┐
│ Display Value Text                          [✕]  [▼]   │  ← outer field (painted)
└─────────────────────────────────────────────────────────┘
  KEY: 1042                                              ← key badge (small, below-left, optional)
  * Employee Name                                        ← label (above, optional)
  ↳ Search for an employee by name or ID                 ← helper text (below, optional)
```

**When value is selected:**  
```
┌─────────────────────────────────────────────────────────┐
│ [1042]  John Smith – Engineering           [✕]  [▼]   │
└─────────────────────────────────────────────────────────┘
```

- `[1042]` = KEY BADGE pill (rounded, accent background, white text)  
- Display text = main value  
- `[✕]` = clear button, only shown when value is set (`ShowClearButton`)  
- `[▼]` = dropdown opener, always visible, rotates 180° when open  

---

### Popup (BeepLovPopup)

```
┌─────────────────────────────────────────────────────────┐
│  Select Employee                    🔍 [____________]  │  ← title + search
├──────┬──────────────────┬──────────┬────────────────────┤
│  ID  │  Name            │ Dept.    │  Email             │  ← column headers
├──────┼──────────────────┼──────────┼────────────────────┤
│ 📌 Recent: John Smith, Jane Doe                         │  ← recent section
├──────┼──────────────────┼──────────┼────────────────────┤
│ 1001 │  Alice Brown     │ Finance  │  alice@corp.com    │
│ 1002 │  Bob Chen        │ IT       │  bob@corp.com      │
│ [...]│  ...             │  ...     │  ...               │
└──────┴──────────────────┴──────────┴────────────────────┘
│  12 of 248 results    [  Cancel  ]  [    OK    ]        │  ← footer
└────────────────────────────────────────────────────────-┘
```

---

## Enhancement Phases

---

### Phase 6 — Unified Visual Field (self-drawn)

**Goal**: Replace the 3-sub-control approach with a single custom-drawn outer container.  
The key textbox is eliminated from the visual layout; the key badge is painted inline.  
The value textbox becomes a single transparent hosted `RichTextBox` or `TextBox` overlay.

**Changes in `BeepListofValuesBox.cs`:**
- Remove public `BeepTextBox _keyTextBox` as visible child
- Keep a hidden `_keyTextBox` for keyboard input or use a single `_editBox` (a hosted native TextBox or BeepTextBox)
- Override `OnPaint` (via `Draw`) to paint:
  - Field background + border (theme-aware, state-aware)
  - KEY BADGE pill (rounded rect, accent color, white text) — left side, only if `ShowKeyBadge && SelectedKey != ""`
  - Display value text — centered vertically, respects left offset from badge
  - Placeholder text — when no value selected, drawn in muted color
  - Clear button `[✕]` rectangle — right side, only when `ShowClearButton && SelectedKey != ""`
  - Chevron `[▼]` — right side always, animated rotation when popup open
- Add state properties: `_isPopupOpen`, `_isHovered`, `_isFocused`
- `AdjustLayout()` computes hit areas: badge rect, clear rect, chevron rect

**New properties:**
```csharp
public string Label { get; set; }           // label above field
public bool IsRequired { get; set; }        // adds * to label
public string HelperText { get; set; }      // gray text below field
public string ErrorText { get; set; }       // red text below field (overrides HelperText)
public bool ShowKeyBadge { get; set; } = true;
public bool ShowClearButton { get; set; } = true;
public string LovTitle { get; set; }        // popup title
public string PlaceholderText { get; set; } = "Select a value...";
public LovValidationState ValidationState { get; set; }
public enum LovValidationState { None, Valid, Invalid, Warning }
```

**Layout calculation:**
```
totalHeight = (Label != null ? labelHeight : 0)
            + fieldHeight          // the main field
            + (HasHelperText ? helperHeight : 0)

fieldHeight = max(MinHeight, PreferredHeight)
```

---

### Phase 7 — Label, Helper Text & Error Visualization

**Goal**: Full field-level label + annotation system, matching Material Design 3 and Oracle JET.

**Drawing order in `Draw()`:**
1. Draw label text above field (if `Label != null`)  
   - Font: `BeepThemesManager.ToFont(Theme?.LabelSmall)` in small weight  
   - Color: muted ForeColor when normal, AccentColor when focused  
   - Add `*` (asterisk) in ErrorColor if `IsRequired`  
2. Draw field outline with state-aware border
   - Normal: 1px `BorderColor`
   - Hover: 1.5px `TextBoxHoverBorderColor`
   - Focus: 2px `AccentColor` (bottom-only for Material3 style, full for Fluent2/iOS)
   - Error: 2px `ErrorColor`
3. Draw helper or error text below field (if present)  
   - Font: `BeepThemesManager.ToFont(Theme?.CaptionStyle)` 
   - Color: `SecondaryTextColor` for HelperText, `ErrorColor` for ErrorText  
   - Preceded by ⚠ icon when `ValidationState == Invalid`  
4. Resize the outer `Height` to accommodate label + field + helper text  

**`BeepThemesManager.ToFont` usage:**
```csharp
// In RebuildFonts():
_labelFont  = BeepThemesManager.ToFont(Theme?.LabelSmall  ?? new TypographyStyle { FontSize = 11f }, true);
_fieldFont  = BeepThemesManager.ToFont(Theme?.LabelSmall  ?? new TypographyStyle { FontSize = 9f  }, true);
_helperFont = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f  }, true);
_badgeFont  = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f, FontWeight = FontWeight.Bold }, true);
```

---

### Phase 8 — Keyboard & Accessibility

**Goal**: Full Oracle LOV keyboard behavior + Windows combobox standards.

| Key | Action |
|---|---|
| `F9` | Open popup (Oracle standard) |
| `Alt+↓` | Open popup (Windows combobox) |
| `Enter` | Open popup if empty; confirm selected item if popup open |
| `Escape` | Close popup, restore previous value |
| `↑` / `↓` | Navigate popup rows when popup visible |
| `Page Up/Down` | Scroll popup list |
| `Home` / `End` | Jump to first/last item in popup |
| `Delete` / `Backspace` | Clear selection (shows confirmation tooltip) |
| `Tab` | Close popup, move focus to next control |
| Any printable char | Open popup and prefill search box with typed char |

**Implementation:**
- Override `OnKeyDown` in `BeepListofValuesBox`
- Track `_preOpenKey` when popup is opened by typing
- Pass key to popup search field when opening via typed character
- Popup raises `ItemAccepted(item)` event → `SetSelectedItem(item)`
- `AccessibleName` property set to `Label ?? "List of Values"`
- `AccessibleRole = AccessibleRole.ComboBox`

---

### Phase 9 — BeepLovPopup (New File)

**File**: `Lovs/BeepLovPopup.cs`

**Goal**: Replace `BeepContextMenu` with a purpose-built LOV selection popup.

**Structure** (inherits `Form` with `ShowInTaskbar = false`, `FormBorderStyle = None`):
```
┌── BeepLovPopup ───────────────────────────────────────────┐
│  [Title]                           [🔍 Search input]      │  ← header bar
│  ┌── Recent ─────────────────────────────────────────┐    │
│  │  📌 [John Smith]  [Jane Doe]                      │    │  ← recent chips (optional)
│  └───────────────────────────────────────────────────┘    │
│  ┌── Grid ────────────────────────────────────────────┐   │
│  │  Col1 Header │ Col2 Header │ Col3 Header            │   │  ← column headers
│  │  ─────────────────────────────────────────────     │   │
│  │  row data...                                       │   │  ← rows (virtual)
│  └───────────────────────────────────────────────────-┘   │
│  [12 of 248]                    [Cancel]  [      OK      │  ← footer
└───────────────────────────────────────────────────────────┘
```

**Properties:**
- `Items`: `List<SimpleItem>` — source data
- `Columns`: `List<LovColumn>` — column definitions for multi-column display  
- `Title`: popup header title  
- `MaxHeight`: limits popup panel height (default 320px)  
- `ShowRecentItems`: if true, shows last 5 selected items at top  
- `IsLoading`: shows spinner while data loads  

**Events:**
- `ItemAccepted(SimpleItem)` — user confirmed a selection  
- `SearchChanged(string)` — search text changed (for server-side filtering)  
- `Cancelled()` — user dismissed without selecting  

**Drawing:**
- All fonts from `BeepThemesManager.ToFont()`  
- All GDI from `PaintersFactory`  
- Icons from `StyledImagePainter.PaintWithTint`  
- Close button: `SvgsUI.X`; Search icon: `SvgsUI.Search`  
- Virtual scrolling for >200 items  

---

### Phase 10 — Multi-Column Support

**New model in `Lovs/Models/LovColumn.cs`:**
```csharp
public class LovColumn
{
    public string FieldName   { get; set; }   // property name on SimpleItem or a delegate
    public string HeaderText  { get; set; }
    public int    Width        { get; set; } = 100;
    public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
    public bool   IsSortable   { get; set; } = true;
    public Func<SimpleItem, string>? ValueSelector { get; set; }
}
```

**Usage on control:**
```csharp
lov.Columns.Add(new LovColumn { FieldName = "Value", HeaderText = "ID",   Width = 60 });
lov.Columns.Add(new LovColumn { FieldName = "Text",  HeaderText = "Name", Width = 180 });
lov.Columns.Add(new LovColumn { ValueSelector = i => i.Description, HeaderText = "Dept.", Width = 100 });
```

When `Columns` is empty → single-column fallback (current behavior, show `item.Text`).

---

### Phase 11 — Font & GDI Cleanup

**Goal**: Remove all `LovFontHelpers.GetLovFont()` → use `BeepThemesManager.ToFont()` throughout.  
Fix the no-op `LovFontHelpers.ApplyFontTheme()`.

**Changes:**
1. In `BeepListofValuesBox`:  
   - Add `private Font? _labelFont, _fieldFont, _helperFont, _badgeFont;`  
   - Add `protected override void RebuildFonts()` with `BeepThemesManager.ToFont` calls  
   - Dispose fonts in `Dispose(bool)`  
2. In `LovFontHelpers.cs`:  
   - Delete or deprecate `ApplyFontTheme()` (was no-op)  
   - Keep utility helpers but mark them as fallback-only  
3. All `new SolidBrush` / `new Pen` in draw code → `PaintersFactory.GetSolidBrush/GetPen`  
4. `StyledImagePainter.PaintWithTint` for chevron and clear-button icons  

---

### Phase 12 — Async Loading & Loading State

**Goal**: Allow items to be loaded on demand (server-side LOV).

**New members:**
```csharp
// Fired when popup opens and items haven't been loaded yet
public event EventHandler<LovLoadEventArgs> LoadItemsRequested;

// Call this to provide items after async load
public void SetItemsAsync(IEnumerable<SimpleItem> items);

// Shows spinner inside field when true
public bool IsLoading { get; private set; }
```

**`LovLoadEventArgs`:**
```csharp
public class LovLoadEventArgs : EventArgs
{
    public string SearchText { get; }      // current search filter
    public int PageIndex { get; }          // for server-side paging
    public int PageSize { get; }
}
```

**Visual:**
- While `IsLoading = true`: replace chevron with a spinner animation (BeepSpinner or animated arc)  
- After `SetItemsAsync()`: spinner stops, items appear in popup  

---

### Phase 13 — Recent Selection History

**Goal**: Show last 5 used items at top of popup (like Oracle Smart LOV).

**Implementation:**
- `private readonly Queue<SimpleItem> _recentItems = new(5);`  
- On `SetSelectedItem()`: push to `_recentItems`, cap at 5  
- Popup header shows recent as small chip buttons  
- Clicking a chip directly selects it  
- `ShowRecentItems` property to enable/disable (default `true`)  

---

### Phase 14 — Design-Time & Readme Update

- Register control in `DesignRegistration.cs`  
- Update `Lovs/Readme.md` with all new properties and usage examples  
- Update `LOV_ENHANCEMENT_SUMMARY.md` to reflect completed phases  

---

## File Change Summary

| File | Action |
|---|---|
| `Lovs/BeepListofValuesBox.cs` | Major rewrite: unified paint, label, helper text, keyboard, async |
| `Lovs/BeepLovPopup.cs` | **NEW** — multi-column popup replaces BeepContextMenu use |
| `Lovs/Models/LovColumn.cs` | **NEW** — column definition model |
| `Lovs/Models/LovColorConfig.cs` | Add error/warning/focus color variants |
| `Lovs/Models/LovStyleConfig.cs` | Add `ShowKeyBadge`, `ShowClearButton`, `ShowRecentItems` |
| `Lovs/Helpers/LovFontHelpers.cs` | Deprecate `ApplyFontTheme()`, keep utils as fallback |
| `Lovs/Helpers/LovThemeHelpers.cs` | Add `GetFocusIndicatorColor`, `GetHelperTextColor` |
| `Lovs/Helpers/LovIconHelpers.cs` | Add `GetClearIconPath()`, `GetSearchIconPath()` |
| `Lovs/Readme.md` | **NEW** — control documentation |
| `Lovs/LOV_ENHANCEMENT_SUMMARY.md` | Update with Phase 6–14 status |

---

## Priority Order (Implementation Sequence)

| # | Phase | Impact | Effort |
|---|---|---|---|
| 1 | Phase 11 — Font/GDI cleanup | High | Low |
| 2 | Phase 6 — Unified visual field | Very High | High |
| 3 | Phase 7 — Label + helper text | High | Medium |
| 4 | Phase 8 — Keyboard nav | High | Medium |
| 5 | Phase 10 — LovColumn model | Medium | Low |
| 6 | Phase 9 — BeepLovPopup | Very High | Very High |
| 7 | Phase 13 — Recent history | Medium | Low |
| 8 | Phase 12 — Async loading | Medium | Medium |
| 9 | Phase 14 — Design-time | Low | Low |
