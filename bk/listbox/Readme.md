# BeepListBox Control

## Overview
BeepListBox is a modern, feature-rich list box control that extends `BeepPanel` with advanced styling capabilities, painter methodology, and comprehensive helper architecture. It provides a flexible foundation for displaying and interacting with lists of items with multiple visual styles.

## Architecture

### Partial Class Structure
BeepListBox uses partial classes for clean separation of concerns:

- **BeepListBox.cs**: Main class declaration and component registration
- **BeepListBox.Core.cs**: Core fields, helpers, painters, and initialization
- **BeepListBox.Properties.cs**: Public properties with change notification
- **BeepListBox.Events.cs**: Event declarations and handlers
- **BeepListBox.Methods.cs**: Public API methods
- **BeepListBox.Drawing.cs**: Paint pipeline and DrawContent override

### Helper System
The control uses three specialized helpers:

#### 1. BeepListBoxHelper (`_helper`)
Main business logic coordinator:
- Item management (add, remove, select)
- Search functionality
- Checkbox state management
- Event coordination
- Initialization and configuration

#### 2. BeepListBoxLayoutHelper (`_layoutHelper`)
Layout computation and measurements:
- Calculates item rectangles (row, checkbox, icon, text)
- Search area positioning
- Content area boundaries
- DPI-aware scaling
- Returns `ListItemInfo` with all computed rectangles

Key Method:
```csharp
public List<ListItemInfo> CalculateItemLayouts(
    Rectangle contentArea, 
    List<SimpleItem> items, 
    int itemHeight, 
    bool showCheckbox, 
    bool showImage)
```

#### 3. BeepListBoxHitTestHelper (`_hitHelper`)
Hit-testing and interaction:
- Mouse position to item resolution
- Checkbox hit detection
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
    Console.WriteLine($"Selected: {selected?.DisplayField}");
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
    Console.WriteLine($"{checkedItems.Count} items checked");
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
