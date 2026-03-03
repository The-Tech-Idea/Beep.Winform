# BeepListBox Control

## Overview
BeepListBox is a commercial-grade list-box control extending `BeepPanel`.  
It implements the Painter strategy pattern, a three-helper architecture, and all 7 enhancement sprints described in the [listbox-enhancement-plan.md](../plans/listbox-enhancement-plan.md).

---

## Architecture

### Partial Class Structure
| File | Responsibility |
|---|---|
| `BeepListBox.cs` | Component registration |
| `BeepListBox.Core.cs` | Core fields, helpers, constructor, scrollbars |
| `BeepListBox.Properties.cs` | All public/designable properties + Sprint 2–6 additions |
| `BeepListBox.Events.cs` | Mouse events, SelectionChanged, InfiniteScroll sentinel click |
| `BeepListBox.Methods.cs` | Public API: item management, selection, BeginUpdate/EndUpdate, ApplyDataSource |
| `BeepListBox.Drawing.cs` | Paint pipeline, CreatePainter factory, skeleton/empty-state, drag indicator |
| `BeepListBox.Keyboard.cs` | *(Sprint 3)* Full ARIA keyboard nav, type-ahead, inline-edit, context menu |
| `BeepListBox.Drag.cs` | *(Sprint 3)* Drag-to-reorder — ghost window, insertion indicator, `OnItemReordered` |
| `BeepListBox.Accessibility.cs` | *(Sprint 5)* WCAG 2.2 AA AccessibleObjects + HC colour helpers |
| `BeepListBox.HighContrast.cs` | *(Sprint 5)* `IsHighContrast`, HC colour API, `UserPreferenceChanged` listener |

### Helper System
| Helper | Role |
|---|---|
| `BeepListBoxHelper` | Item management, search, selection coordination |
| `BeepListBoxLayoutHelper` | Calculates item rectangles (row, icon, checkbox, text) |
| `BeepListBoxHitTestHelper` | Maps mouse/touch points → item, hit sub-region |

### Painter System
All painters live in `Painters/` and implement `IListBoxPainter`.
The abstract `BaseListBoxPainter` provides default implementations of all interface members.

```csharp
internal interface IListBoxPainter
{
    BeepControlStyle Style { get; set; }
    void Initialize(BeepListBox owner, IBeepTheme theme);
    void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect);
    int  GetPreferredItemHeight();
    Padding GetPreferredPadding();
    bool SupportsSearch();
    bool SupportsCheckboxes();
    // Sprint 7 additions:
    int  GetItemHeight(BeepListBox owner, object item);           // variable-height support
    void DrawGroupHeader(...);                                     // data-driven group headers
}
```

---

## ListBoxType Enum (34 values)

| Value | Type | Description |
|---|---|---|
| 0–25 | Existing | Standard → Custom |
| 26 | Glassmorphism | Frosted-glass effect |
| 27 | Neumorphic | Soft-UI embossed shadows |
| 28 | GradientCard | Colorful gradient card items |
| 29 | ChipStyle | Selectable chip/tag items |
| 30 | AvatarList | Circular-avatar user list |
| 31 | Timeline | Activity-log timeline |
| **32** | **InfiniteScroll** | "Load more…" sentinel at bottom |
| **33** | **CommandList** | VS Code-style icon + name + kbd shortcut |
| **34** | **NavigationRail** | Large icon above label (mobile nav) |

---

## Design Tokens (`Tokens/ListBoxTokens.cs`)

All painters MUST reference these constants instead of magic numbers.

| Constant | Value | Purpose |
|---|---|---|
| `ItemHeightComfortable` | 52 px | Default density |
| `ItemHeightCompact` | 40 px | Compact density |
| `ItemHeightDense` | 28 px | Dense density |
| `HoverOverlayAlpha` | 18 | 7% hover fill alpha |
| `ActiveOverlayAlpha` | 38 | 15% pressed fill |
| `SubTextAlpha` | 140 | Secondary text opacity |
| `DisabledAlpha` | 100 | Greyed-out opacity |
| `MinTouchTargetPx` | 44 | WCAG 2.5.5 minimum |

---

## Rich Item Model (`Models/BeepListItem.cs`)

Extends `SimpleItem` with:

| Property | Type | Purpose |
|---|---|---|
| `SubText` | `string?` | Second line / metadata |
| `BadgeText` | `string?` | Short chip label ("3", "New") |
| `BadgeColor` | `Color` | Pill background (empty = theme) |
| `Category` | `string?` | Group key for `ShowGroups` |
| `IsPinned` | `bool` | Floats to top, never virtualised |
| `IsDisabled` | `bool` | 39% opacity, ignores input |
| `IsSeparator` | `bool` | Renders as 1 px horizontal rule |
| `ItemAccentColor` | `Color` | 3 px left-edge accent bar |
| `Tag` | `object?` | Arbitrary consumer payload |

---

## Key New Properties (Sprints 2–6)

### Grouping
```csharp
public bool ShowGroups       { get; set; }   // group by BeepListItem.Category
public bool CollapsibleGroups { get; set; }  // click header to collapse
```

### Density
```csharp
public ListDensityMode Density { get; set; }  // Comfortable / Compact / Dense
public bool AutoItemHeight     { get; set; }  // variable row heights via GetItemHeight()
```

### Loading state
```csharp
public bool IsLoading       { get; set; }   // show animated skeleton rows
public int  SkeletonRowCount { get; set; }  // number of skeleton placeholder rows
```

### Data binding
```csharp
public object? DataSource    { get; set; }   // IList / IBindingList
public string  DisplayMember { get; set; }
public string  ValueMember   { get; set; }
public object? SelectedValue { get; }
```

### Interaction
```csharp
public bool AllowItemReorder { get; set; }   // drag-to-reorder
public bool AllowInlineEdit  { get; set; }   // F2 / double-click inline edit
public bool ShowContextMenu  { get; set; }   // right-click menu
public ContextMenuStrip? ItemContextMenu { get; set; }  // custom menu
```

---

## New Events (Sprint 6)

| Event | Args | When |
|---|---|---|
| `ItemActivated` | `ListBoxItemEventArgs` | Enter or double-click |
| `ItemDeleteRequested` | `ListBoxItemEventArgs` | Delete key |
| `ItemTextChanged` | `ListBoxItemTextChangedEventArgs` | Inline-edit committed |
| `ItemReordered` | `ListBoxReorderEventArgs` | Drag-reorder complete |
| `ContextMenuOpening` | `ListBoxContextMenuEventArgs` | Before right-click menu |
| `LoadMoreRequested` | `EventArgs` | Infinite-scroll sentinel hit |
| `GroupCollapsed` | `ListBoxGroupEventArgs` | Group header collapsed |
| `GroupExpanded` | `ListBoxGroupEventArgs` | Group header expanded |
| `SearchChanged` | `ListBoxSearchEventArgs` | SearchText changed (+ match count) |

---

## Batch Update API (Sprint 1)

```csharp
listBox.BeginUpdate();   // suspend layout & repaint
for (int i = 0; i < 10_000; i++)
    listBox.AddItem(new BeepListItem($"Item {i}"));
listBox.EndUpdate();     // one layout pass + one repaint
```

---

## Keyboard Navigation (Sprint 3)

| Key | Action |
|---|---|
| ↑ / ↓ | Move focus |
| Home / End | First / last item |
| Page Up / Page Down | One visible page |
| Space | Toggle checkbox or select |
| Enter | Activate item (`ItemActivated`) |
| Ctrl+A | Select all (multi-select mode) |
| Ctrl+C | Copy selected text to clipboard |
| Escape | Clear selection |
| F2 | Begin inline edit |
| Delete | Raise `ItemDeleteRequested` |
| Printable chars | Type-ahead search (800 ms buffer) |

---

## Accessibility (Sprint 5, WCAG 2.2 AA)

- `BeepListBoxAccessible` — control-level AccessibleObject (role = List)
- `BeepListItemAccessible` — per-item objects with name, state, value, bounds
- Live-region notifications on selection change, reorder, search filter
- High-contrast colour helpers: `GetHoverFillColor()`, `GetSelectionFillColor()`, `GetFocusRingColor()`
- Minimum row height clamped to 44 px touch target (WCAG 2.5.5)

### High-Contrast Mode (`BeepListBox.HighContrast.cs`)

```csharp
// Check HC state
bool hcActive = owner.IsHighContrast;

// Painter-facing helpers (invoked inside DrawItem)
Color bg  = owner.HCItemBackground(isHovered, isSelected); // Empty = not HC
Color fg  = owner.HCItemForeground(isSelected);
Color bdr = owner.HCBorderColor;
Color foc = owner.HCFocusRingColor;

// Draw 3 px focus rect in HC mode
owner.PaintFocusRectIfHC(g, rowRect);
```

`UserPreferenceChanged` is automatically subscribed/unsubscribed with the handle lifetime so the control re-paints immediately when the user toggles HC mode.

---

## Sprint 7 — Painter Quality Pass

`BaseListBoxPainter` now provides five shared drawing helpers available to all concrete painters:

| Helper | Purpose |
|---|---|
| `DrawAccentBar(g, rowRect, color)` | 3 px left-edge accent bar (`BeepListItem.ItemAccentColor`) |
| `DrawBadgePill(g, rowRect, text, color)` | Pill badge top-right of row |
| `DrawSubText(g, subRect, text, color, font)` | Muted secondary line (55 % opacity) |
| `DrawSeparatorRow(g, rowRect, label?)` | 1 px horizontal rule with optional label |
| `DrawFocusRing(g, rowRect)` | Theme / HC focus ring; uses `ListBoxTokens.FocusRingThickness` |

`DrawItemBackgroundEx` (base) also now:
- Applies **`DisabledAlpha` overlay** for `BeepListItem.IsDisabled` items
- Overrides colours with **`SystemColors.*`** when `owner.IsHighContrast` is `true`

`GetItemHeight` returns the correct taller height for items with `SubText` automatically.

**Upgraded painters (Sprint 7 priority):**
- `StandardListBoxPainter` — 2-line layout, badge, disabled alpha, focus ring, HC-aware  
- `CheckboxListPainter` — same, plus scaled-DPI checkbox rect; removed hard `BeepStyling` references

---

## Search Highlight (Sprint 4)

Painters can call `owner.DrawHighlightedText(...)` to render query matches with a coloured pill background (like VS Code search), instead of a plain `DrawString`.

---

## Example Usage

```csharp
var list = new BeepListBox
{
    Density = ListDensityMode.Comfortable,
    ShowGroups = true,
    ListBoxType = ListBoxType.AvatarList,
    ShowSearch = true,
    AllowInlineEdit = true,
    AllowItemReorder = true,
};

list.BeginUpdate();
list.AddItem(new BeepListItem("Alice", "Engineering", "avatars/alice.png", "Team A") { IsPinned = true });
list.AddItem(new BeepListItem("Bob",   "Design",      "avatars/bob.png",   "Team B"));
list.AddItem(BeepListItem.Separator());
list.AddItem(new BeepListItem("Carol", "Marketing",   null, "Team A") { BadgeText = "New" });
list.EndUpdate();

list.ItemActivated    += (s, e) => Console.WriteLine($"Activated: {e.Item.Text}");
list.ItemDeleteRequested += (s, e) => list.RemoveItem(e.Item);
list.SearchChanged    += (s, e) => Console.WriteLine($"{e.MatchCount} matches for '{e.Query}'");
```

- Icon/text region identification
- Integrates with `ControlInputHelper` from BaseControl

### Painter Strategy Pattern

The control uses the **IListBoxPainter** interface for pluggable visual styles. Each painter is responsible for:
- Drawing all visual elements (background, items, checkboxes, icons, text)
- Computing preferred item heights
- Defining interaction areas

#### IListBoxPainter Interface
```csharp
public interface IListBoxPainter
{
    void DrawContent(
        Graphics g, 
        BeepListBox owner, 
        List<ListItemInfo> itemLayouts, 
        SimpleItem selectedItem, 
        SimpleItem hoveredItem);
    
    int GetPreferredItemHeight();
}
```

#### Available Painters
Located in `TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters`:

1. **StandardListBoxPainter**: Classic list appearance with subtle hover
2. **RadioSelectionPainter**: Radio-button style selection indicators
3. **CategoryChipsPainter**: Pill-shaped category tags
4. **GroupedListPainter**: Section headers with grouped items
5. **BorderlessListBoxPainter**: Clean, minimal style
6. **MaterialOutlinedListBoxPainter**: Material Design outlined style
7. **MinimalListBoxPainter**: Ultra-minimal with accent line
8. **OutlinedListBoxPainter**: Traditional outlined list
9. **SearchableListPainter**: Integrated search with filtering
10. **TeamMembersPainter**: Avatar-style team member list

Each painter:
- Uses `IBeepTheme` for consistent styling
- Respects `ShowCheckBox`, `ShowImage`, `ShowHilightBox` properties
- Handles hover/selection states
- Supports disabled items

### Theme Integration

BeepListBox uses the centralized theme system:

```csharp
// Get theme instance
var theme = BeepThemesManager.GetTheme(ThemeEnum);

// Access colors
var primary = theme.PrimaryColor;
var background = theme.BackgroundColor;
var foreground = theme.ForegroundColor;
var hover = theme.HoverBackColor;
var selected = theme.SelectedBackColor;
```

All painters use `IBeepTheme` functions for consistent styling across the application.

## Key Features

### 1. Multiple Visual Styles
Switch between 10+ predefined styles via `ListBoxType` property:

```csharp
listBox.ListBoxType = ListBoxType.Material;
listBox.ListBoxType = ListBoxType.Minimal;
listBox.ListBoxType = ListBoxType.Grouped;
```

### 2. Search Functionality
Built-in search with live filtering:

```csharp
listBox.ShowSearch = true;
// User types in search box, list filters automatically
```

### 3. Checkbox Support
Optional checkboxes with state tracking:

```csharp
listBox.ShowCheckBox = true;
listBox.AllowMultipleSelection = true;
```

### 4. Image/Icon Display
Show icons for each item:

```csharp
listBox.ShowImage = true;
listBox.ImageSize = 24; // DPI-scaled
```

### 5. Hover and Selection States
Visual feedback for interaction:

```csharp
listBox.ShowHilightBox = true; // Highlight on hover
```

### 6. DPI Awareness
Automatic scaling for high-DPI displays:
- Item heights
- Icon sizes
- Spacing and margins

### 7. Disabled Item Support
Items can be individually disabled:

```csharp
item.Disabled = true;
// Item rendered with muted colors, non-interactive
```

## Usage Examples

### Basic Setup
```csharp
var listBox = new BeepListBox();
listBox.ListBoxType = ListBoxType.Standard;
listBox.ShowCheckBox = false;
listBox.ShowImage = true;
listBox.AllowMultipleSelection = false;

// Add items
listBox.ListItems.Add(new SimpleItem { DisplayField = "Item 1", IDField = "1" });
listBox.ListItems.Add(new SimpleItem { DisplayField = "Item 2", IDField = "2" });

// Handle selection
listBox.SelectedItemChanged += (s, e) => {
    var selected = listBox.SelectedItem;
  //  Console.WriteLine($"Selected: {selected?.DisplayField}");
};
```

### With Checkboxes
```csharp
var listBox = new BeepListBox();
listBox.ShowCheckBox = true;
listBox.AllowMultipleSelection = true;

// Handle checkbox changes
listBox.CheckBoxChanged += (s, e) => {
    var checkedItems = listBox.SelectedItems; // Items with checked boxes
  //  Console.WriteLine($"{checkedItems.Count} items checked");
};
```

### With Search
```csharp
var listBox = new BeepListBox();
listBox.ShowSearch = true;
listBox.ListBoxType = ListBoxType.Searchable;

// Automatically filters as user types
```

### Custom Painter
```csharp
// Implement IListBoxPainter
public class MyCustomPainter : IListBoxPainter
{
    public void DrawContent(Graphics g, BeepListBox owner, 
        List<ListItemInfo> itemLayouts, 
        SimpleItem selectedItem, 
        SimpleItem hoveredItem)
    {
        var theme = BeepThemesManager.GetTheme(owner.Theme);
        
        foreach (var layout in itemLayouts)
        {
            // Draw custom item appearance
            var bg = layout.Item == selectedItem ? theme.SelectedBackColor : theme.BackgroundColor;
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, layout.RowRect);
            
            // Draw text
            TextRenderer.DrawText(g, layout.Item.DisplayField, owner.Font, 
                layout.TextRect, theme.ForegroundColor);
        }
    }
    
    public int GetPreferredItemHeight() => 32;
}

// Use custom painter
listBox.SetCustomPainter(new MyCustomPainter());
```

## Data Model

### SimpleItem
The standard item model:

```csharp
public class SimpleItem
{
    public string IDField { get; set; }
    public string DisplayField { get; set; }
    public string ImagePath { get; set; }
    public object Tag { get; set; }
    public bool Disabled { get; set; }
    public List<SimpleItem> Children { get; set; } // For hierarchical items
}
```

### ListItemInfo
Layout information for each item:

```csharp
public class ListItemInfo
{
    public SimpleItem Item { get; set; }
    public Rectangle RowRect { get; set; }      // Full row bounds
    public Rectangle CheckRect { get; set; }    // Checkbox area
    public Rectangle IconRect { get; set; }     // Icon area
    public Rectangle TextRect { get; set; }     // Text area
    public int Index { get; set; }              // Position in list
}
```

## Properties Reference

| Property | Type | Description |
|----------|------|-------------|
| `ListBoxType` | `ListBoxType` | Visual style (Standard, Material, Minimal, etc.) |
| `ListItems` | `BindingList<SimpleItem>` | Collection of items |
| `SelectedItem` | `SimpleItem` | Currently selected item |
| `SelectedItems` | `List<SimpleItem>` | Multiple selected items (checkbox mode) |
| `SelectedIndex` | `int` | Index of selected item |
| `ShowSearch` | `bool` | Show/hide search box |
| `ShowCheckBox` | `bool` | Show/hide checkboxes |
| `ShowImage` | `bool` | Show/hide item icons |
| `ShowHilightBox` | `bool` | Show/hide hover highlights |
| `AllowMultipleSelection` | `bool` | Enable multiple selection |
| `MenuItemHeight` | `int` | Height of each item (DPI-scaled) |
| `ImageSize` | `int` | Size of item icons (DPI-scaled) |
| `TextFont` | `Font` | Font for item text |
| `Theme` | `ThemeEnum` | Theme selection for styling |

## Events Reference

| Event | EventArgs | Description |
|-------|-----------|-------------|
| `SelectedItemChanged` | `EventArgs` | Fired when selection changes |
| `CheckBoxChanged` | `EventArgs` | Fired when checkbox state changes |
| `ItemClicked` | `EventArgs` | Fired when item is clicked |
| `SearchTextChanged` | `EventArgs` | Fired when search text changes |

## Integration with BaseControl

BeepListBox inherits from `BeepPanel`, which extends `BaseControl`. This provides:

- **ControlInputHelper**: Centralized input handling
- **ControlHitTestHelper**: Hit-testing infrastructure  
- **Theme Support**: Access to `BeepThemesManager`
- **DPI Awareness**: Automatic scaling
- **DrawContent Override**: Custom painting pipeline

## Best Practices

### 1. Use Helpers for Business Logic
Don't access private fields directly. Use helper methods:

```csharp
// Good
_helper.AddItem(newItem);
_helper.SelectItem(item);

// Avoid
_listItems.Add(newItem); // Bypasses event handling
```

### 2. Respect Painter Boundaries
Painters should only draw. Business logic belongs in helpers:

```csharp
// Painter responsibility: Draw items
public void DrawContent(Graphics g, BeepListBox owner, ...) {
    // Drawing code only
}

// Helper responsibility: Handle selection
_helper.SelectItem(item); // Triggers repaint via owner
```

### 3. Use Theme Functions
Always use theme for colors, never hardcode:

```csharp
// Good
var theme = BeepThemesManager.GetTheme(owner.Theme);
var bg = theme.BackgroundColor;

// Avoid
var bg = Color.White; // Breaks theme consistency
```

### 4. DPI Scaling
Use scaled values for measurements:

```csharp
int iconSize = (int)(24 * _scaleFactor);
int margin = (int)(8 * _scaleFactor);
```

### 5. Trigger Layout Updates
After property changes that affect layout:

```csharp
_needsLayoutUpdate = true;
Invalidate(); // Triggers repaint
```

## Performance Considerations

1. **Layout Caching**: Item layouts computed once per paint cycle
2. **Delayed Invalidation**: Timer prevents excessive repaints
3. **Incremental Updates**: Only affected items recalculated
4. **Efficient Hit-Testing**: Spatial indexing for large lists

## Extension Points

### Custom Painters
Implement `IListBoxPainter` for unique visual styles.

### Custom Helpers
Extend `BeepListBoxHelper` for specialized behavior.

### Event Handling
Subscribe to events for custom interaction logic.

### Item Rendering
Use `SimpleItem.Tag` to attach custom data for specialized rendering.

## Troubleshooting

### Items not displaying
- Ensure `ListItems` is populated
- Check `Visible` property
- Verify `Size` is sufficient

### Selection not working
- Check `AllowMultipleSelection` setting
- Ensure items are not `Disabled`
- Verify event handlers are attached

### Checkboxes not showing
- Set `ShowCheckBox = true`
- Verify painter supports checkboxes

### Theme not applying
- Ensure `Theme` property is set
- Verify `BeepThemesManager` is initialized
- Check painter uses theme functions

## File Structure

```
ListBoxs/
├── BeepListBox.cs                    # Main class
├── BeepListBox.Core.cs               # Fields and initialization
├── BeepListBox.Properties.cs         # Properties
├── BeepListBox.Events.cs             # Events
├── BeepListBox.Methods.cs            # Public methods
├── BeepListBox.Drawing.cs            # Paint pipeline
├── Helpers/
│   ├── BeepListBoxHelper.cs          # Business logic
│   ├── BeepListBoxLayoutHelper.cs    # Layout computation
│   └── BeepListBoxHitTestHelper.cs   # Hit-testing
├── Painters/
│   ├── IListBoxPainter.cs            # Painter interface
│   ├── StandardListBoxPainter.cs     # Standard style
│   ├── MaterialOutlinedListBoxPainter.cs
│   ├── MinimalListBoxPainter.cs
│   └── ... (10+ painters)
└── Models/
    └── ListItemInfo.cs               # Layout data
```

## Related Controls
- **BeepComboBox**: Dropdown with popup list
- **BeepTree**: Hierarchical tree view
- **BeepPopupListForm**: Popup list container

## Version History
- **v3.0**: Modernized with painter methodology, helpers, and theme integration
- **v2.0**: Added search and checkbox support
- **v1.0**: Initial release

## License
Part of TheTechIdea.Beep.Winform.Controls suite.
