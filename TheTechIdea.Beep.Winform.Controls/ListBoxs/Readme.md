# ListBoxes — Beep WinForm Controls

Production-ready list box control with 30+ visual variants, multi-select, checkbox mode, grouped rows, hierarchy, search, drag-to-reorder, skeleton loading states, and full theme integration.

---

## Architecture

```
BeepListBox (owner-drawn, inherits BaseControl)
  └── IListBoxPainter (per-variant row chrome + text + icons + checkboxes)
        └── BaseListBoxPainter
              └── 30+ variant painters (Standard, Minimal, Outlined, Rounded, Filled,
                    Borderless, Card, Avatar, Glassmorphism, Neumorphic, Timeline, etc.)

  └── BeepListBoxHelper
        ├── GetVisibleItems()          — search filter + hierarchy flatten + group header injection
        ├── FlattenHierarchy()         — recursive depth-limited tree flattening
        ├── FilterHierarchyBySearch()  — ancestor-inclusive search for tree items
        ├── BuildGroupedRows()         — category-key grouping with collapse/expand headers
        ├── IsSearchMatch()            — prefix / contains / fuzzy scoring
        └── MeasureText(), FindItemByText()

  └── BeepListBoxLayoutHelper
        ├── CalculateLayout()          — DPI-scaled per-row rect cache with viewport culling
        └── GetCachedLayout()          — ListItemInfo (RowRect, CheckRect, IconRect, TextRect, ChevronRect, Depth)

  └── BeepListBoxHitTestHelper
        └── RegisterHitAreas()         — hit-test rects for checkbox, chevron, row body

  └── BeepListBox.Accessibility.cs   — screen-reader announcements, selection change notifications
  └── BeepListBox.HighContrast.cs   — HC colour overrides, focus-ring thickness
  └── BeepListBox.Drag.cs            — drag-to-reorder with insertion indicator
  └── BeepListBox.Keyboard.cs        — arrow nav, PageUp/PageDown, Home/End, Space toggle, typeahead
```

---

## Variant Catalog

| Variant | Key Visual Traits | Best For |
|---------|-------------------|----------|
| **Standard** | Clean rows, subtle hover tint, selection border | Default single-select lists |
| **Minimal** | No borders, ghost hover, ultra-compact | Embedded in tight forms |
| **Outlined** | 1px row borders, rounded corners | Settings panels, config lists |
| **Rounded** | Full rounded pill rows | Modern mobile-style UI |
| **MaterialOutlined** | Material 3 ripple-style focus ring | Material design apps |
| **Filled** | Solid background tint per row | Dense data tables |
| **Borderless** | Transparent chrome, separator lines only | Inline editable lists |
| **CategoryChips** | Chips as category filters above list | Filter + select workflows |
| **SearchableList** | Built-in search bar, highlight matches | Long searchable item lists |
| **WithIcons** | Leading icon + text, icon tinting | File pickers, app launchers |
| **CheckboxList** | Checkbox per row, multi-select | Permission editors, tag lists |
| **SimpleList** | No chrome, plain text | Log viewers, simple read-only |
| **LanguageSelector** | Flag icons + language names | Locale pickers |
| **CardList** | Card-style rows with shadow | Content browsing (articles, products) |
| **Compact** | 24px rows, minimal padding | Data-dense grids |
| **Grouped** | Collapsible category headers | Settings categories, folder trees |
| **TeamMembers** | Avatar + name + role badge | Contact lists, org charts |
| **FilledStyle** | Solid accent fill for selected row | High-visibility selection |
| **FilterStatus** | Colour-coded status badges | Ticket lists, task boards |
| **ErrorStates** | Red tint for error rows, green for success | Validation result lists |
| **MultiSelectionTeal** | Teal accent multi-select chips | Bulk action item lists |
| **OutlinedCheckboxes** | Bordered checkbox squares | Accessibility-first check lists |
| **RaisedCheckboxes** | 3D raised checkbox style | Legacy-compatible forms |
| **ColoredSelection** | Full-row colour change on select | Visual emphasis needed |
| **RadioSelection** | Radio button instead of checkbox | Single-choice surveys |
| **Custom** | Delegates to user-supplied `CustomItemRenderer` | Specialised bespoke UI |
| **Glassmorphism** | Frosted-glass blur background | Modern overlay panels |
| **Neumorphic** | Soft-shadow inset/outset depth | Tactile skeuomorphic UI |
| **GradientCard** | Horizontal gradient backgrounds | Hero / featured item lists |
| **ChipStyle** | Row styled as removable chips | Token / tag management |
| **AvatarList** | Circular avatar + 2-line text | Social contact lists |
| **Timeline** | Left vertical line, dot markers | Activity feeds, history |
| **InfiniteScroll** | Skeleton shimmer + load-more trigger | Streaming data (chat, logs) |
| **CommandList** | Keyboard shortcut badges per row | Command palettes, menus |
| **NavigationRail** | Icon + label, vertical compact | Side navigation mini-mode |
| **ChatList** | Message bubble styling, timestamp | Conversation lists |
| **ContactList** | Avatar + name + presence dot | Messenger-style contacts |
| **ThreeLineList** | Avatar + title + subtitle + metadata | Rich email / notification lists |
| **NotificationList** | Unread dot + timestamp + action buttons | Inbox / alert feeds |
| **ProfileCard** | Large avatar + name + bio | User directory |
| **RekaUI** | Accessible focus ring, accent bar, checkmark | ARIA-compliant minimal design |

---

## Key Behaviors

### Selection Modes
- **Single** — click or arrow keys to select one item.
- **MultiSimple** — Ctrl+click toggles; Shift+click range-selects using `_anchorItem`.
- **MultiExtended** — same as MultiSimple with keyboard shortcuts.
- **Checkbox mode** (`ShowCheckBox = true`) — each row has a checkbox; `CheckedItemsChanged` event fires per toggle.

### Search
- Prefix, contains, or fuzzy scoring (`ListSearchMode`).
- Highlight matching substring in row text (`HighlightSearchMatches`).
- Hierarchy search includes ancestors so tree paths remain visible.

### Grouping
- Items grouped by `Category` / `GroupName`.
- Collapsible headers with chevron (▶ / ▼) and child count badge.
- `IsGroupCollapsed(key)` / `ToggleGroupCollapse(key)` API.

### Hierarchy
- `ShowHierarchy = true` enables tree view inside the list box.
- `Children` collection on `SimpleItem` with `IsExpanded` toggle.
- Chevron hit-target for expand/collapse; indent per depth level.
- Search filter preserves ancestor chain.

### Keyboard Navigation
| Key | Action |
|-----|--------|
| ↑ / ↓ | Previous / next item |
| PageUp / PageDown | Jump by viewport height |
| Home / End | First / last visible item |
| Space | Toggle checkbox (if `ShowCheckBox`) |
| Ctrl+A | Select all (multi-select) |
| Typeahead | Jump to first matching item (700ms buffer) |

### Drag-to-Reorder
- `AllowDragReorder = true` enables drag handles.
- Visual insertion indicator line drawn between rows.
- `ItemReordered` event provides old and new indices.

### Loading / Empty States
- `IsLoading = true` shows skeleton shimmer rows with avatar + title + subtext placeholders.
- `ShowEmptyState = true` shows centred icon + headline + subtext when count is zero.

### Accessibility
- `AccessibleRole = List` with `AccessibleName` and `AccessibleDescription`.
- Selection change raises `AccessibleEvents.StateChange` + `Focus`.
- High-contrast mode overrides (`HCItemBackground`, `HCBorderColor`, `HCFocusColor`).

### Theme & DPI
- All colours/fonts resolved via `IBeepTheme` + `ListBoxTokens`.
- `DpiScalingHelper.ScaleValue(token, control)` used for every metric.
- Row hover animation with `HoverAnimationDuration` and `EnableHoverAnimation`.

---

## Usage Examples

### Basic single-select
```csharp
var list = new BeepListBox
{
    ListBoxType = ListBoxType.Standard,
    ListItems = new BindingList<SimpleItem>
    {
        new SimpleItem { Text = "Apple",  Item = 1 },
        new SimpleItem { Text = "Banana", Item = 2 }
    }
};
list.SelectedItemChanged += (s, e) => Console.WriteLine(e.SelectedItem?.Text);
```

### Searchable with icons
```csharp
var list = new BeepListBox
{
    ListBoxType = ListBoxType.SearchableList,
    ShowSearch = true,
    ShowImage = true,
    SearchMode = ListSearchMode.Contains
};
```

### Multi-select checkbox with groups
```csharp
var list = new BeepListBox
{
    ListBoxType = ListBoxType.CheckboxList,
    ShowCheckBox = true,
    ShowGroups = true,
    SelectionMode = SelectionModeEnum.MultiSimple
};
list.CheckedItemsChanged += (s, e) => Console.WriteLine($"Checked: {e.CheckedItems.Count}");
```

### Hierarchical tree list
```csharp
var parent = new SimpleItem { Text = "Documents", IsExpanded = true };
parent.Children.Add(new SimpleItem { Text = "Report.pdf" });
parent.Children.Add(new SimpleItem { Text = "Notes.txt" });

var list = new BeepListBox
{
    ListBoxType = ListBoxType.Standard,
    ShowHierarchy = true,
    ListItems = new BindingList<SimpleItem> { parent }
};
```

### Drag-to-reorder
```csharp
var list = new BeepListBox
{
    ListBoxType = ListBoxType.Standard,
    AllowDragReorder = true
};
list.ItemReordered += (s, e) => Console.WriteLine($"Moved {e.Item.Text} from {e.OldIndex} to {e.NewIndex}");
```

---

## File Organization

```
ListBoxs/
  BeepListBox.cs                  — public API shell (partial class)
  BeepListBox.Core.cs             — fields, init, dispose, helper references, hover animation
  BeepListBox.Properties.cs       — 40+ designer-visible properties (ListBoxType, ListItems, ShowSearch, etc.)
  BeepListBox.Drawing.cs          — DrawContent override, painter factory, skeleton/empty-state helpers
  BeepListBox.Events.cs           — mouse, keyboard, scroll, focus, drag events
  BeepListBox.Methods.cs          — public methods (SelectItem, ScrollToItem, GetItemAtPoint, etc.)
  BeepListBox.Keyboard.cs         — key-nav, typeahead, shortcut handling
  BeepListBox.Drag.cs             — drag start/move/end, insertion indicator painting
  BeepListBox.Accessibility.cs    — AccessibleObject, screen-reader notifications
  BeepListBox.HighContrast.cs     — HC colour resolution, focus-ring overrides
  ListBoxType.cs                  — enum of 30+ variant types
  ListBoxVariantMetadata.cs       — per-variant density/defaults catalog
  ListBoxEventArgs.cs             — SelectedItemChanged, SelectionChanged, CheckedChanged, ItemReordered
  Helpers/
    BeepListBoxHelper.cs          — visible-items, search, grouping, hierarchy, colour helpers
    BeepListBoxLayoutHelper.cs    — per-row rect cache with viewport culling and DPI scaling
    BeepListBoxHitTestHelper.cs   — interactive hit-area registration
  Painters/
    BaseListBoxPainter.cs         — shared paint pipeline (background, text, image, checkbox, focus ring, badge, separator)
    IListBoxPainter.cs            — interface contract
    StandardListBoxPainter.cs     — and 30+ variant overrides
  Models/
    BeepListItem.cs               — rich item with Category, SubText, IsGroupHeader, IsSeparator, IsDisabled, Children
  Tokens/
    ListBoxTokens.cs                — DPI-aware constants (item heights, padding, alpha values, radii)
```

---

## Recent Changes (2026-04-23)

- Fixed `RekaUIListBoxPainter` icon placeholder — now draws actual image via `StyledImagePainter.Paint` clipped to ellipse.
- Fixed `BaseListBoxPainter.DrawItemBackgroundEx` call order — `DrawItemBackground` (painter override) is now called BEFORE selection/hover/focus overlays, preventing painters from overwriting focus rings and selection borders.
- Fixed `BeepListBox.GetItemAtPoint()` — migrated from uniform-height helper to layout-cache iteration, correctly handling variable row heights, grouped headers, and hierarchy.
- Removed stale `GetItemAtPoint` from `BeepListBoxHelper` (superseded by layout cache).
- Removed duplicate `using System.Collections.Generic;` in `BeepListBox.Core.cs`.
