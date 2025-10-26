# Beep Hierarchical Radio Group Documentation

## Overview

The `BeepHierarchicalRadioGroup` is a powerful control that extends the standard radio group functionality to support hierarchical data structures using the `Children` property of `SimpleItem`. This control provides tree-like visualization with expand/collapse functionality while maintaining the radio/checkbox selection behavior.

## Key Features

### ? Hierarchical Structure Support
- **Tree-like display**: Items are displayed in a tree structure with proper indentation
- **Expand/Collapse**: Parent items can be expanded or collapsed to show/hide children
- **Unlimited nesting**: Supports unlimited levels of hierarchy using `SimpleItem.Children`
- **Visual hierarchy lines**: Dotted lines connect parent and child items for clear visualization

### ? Image Rendering Support
- **ImagePath integration**: Fully supports `SimpleItem.ImagePath` property
- **BeepImage rendering**: Uses the same image rendering system as BeepListBox
- **Icon display**: Shows icons next to text for enhanced visual communication
- **SVG and standard image support**: Supports SVG, PNG, JPG, and other image formats

### ? Multiple Render Styles
- **Flat Design**: Modern flat design with rounded corners and subtle shadows
- **Material Design**: Google Material Design compliant styling
- **Card Style**: Card-based selection with elevated appearance
- **Circular**: Traditional circular radio buttons
- **Chip Style**: Pill/chip-like selection buttons

### ? Selection Modes
- **Single Selection**: Traditional radio button behavior (one item selected)
- **Multiple Selection**: Checkbox-like behavior (multiple items can be selected)
- **Hierarchical Selection**: Can select parent and child items independently

### ? Theme Integration
- **Full theme support**: Integrates with the Beep theming system
- **Automatic color adaptation**: Colors automatically adapt to the current theme
- **Font inheritance**: Uses theme fonts when `UseThemeFont` is enabled
- **Consistent styling**: Maintains visual consistency with other Beep controls

## Usage Examples

### Basic Setup
```csharp
var hierarchicalRadioGroup = new BeepHierarchicalRadioGroup
{
    RenderStyle = RadioGroupRenderStyle.Flat,
    AllowMultipleSelection = true,
    ShowExpanderButtons = true,
    IndentSize = 25,
    ItemSpacing = 6
};
```

### Adding Hierarchical Data
```csharp
// Create root item
var ktlo = new SimpleItem
{
    Text = "KTLO",
    SubText = "Keep The Lights On",
    ImagePath = "folder", // Image will be rendered
    IsExpanded = true
};

// Create child items
var maintenance = new SimpleItem
{
    Text = "Maint / Upgrade",
    SubText = "Maintenance and Upgrades",
    ImagePath = "settings",
    IsSelected = true
};

var security = new SimpleItem
{
    Text = "Security / Compliance",
    SubText = "Security and Compliance",
    ImagePath = "shield"
};

// Build hierarchy
ktlo.Children.Add(maintenance);
ktlo.Children.Add(security);

// Add to control
hierarchicalRadioGroup.AddRootItem(ktlo);
```

## Properties

### Data Properties
- `RootItems`: Collection of top-level `SimpleItem` objects
- `SelectedValue`: Currently selected value (single selection mode)
- `SelectedValues`: List of selected values (multiple selection mode)
- `SelectedItems`: List of selected `SimpleItem` objects

### Behavior Properties
- `AllowMultipleSelection`: Enable/disable multiple selection mode
- `ShowExpanderButtons`: Show/hide expand/collapse buttons
- `IndentSize`: Indentation size in pixels for each hierarchy level
- `AutoSizeItems`: Whether items auto-size to their content

### Appearance Properties
- `RenderStyle`: Visual style (Flat, Material, Card, Circular, Chip)
- `ItemSpacing`: Spacing between items in pixels

## Events

### Selection Events
```csharp
// Selection changed event
hierarchicalRadioGroup.SelectionChanged += (sender, e) => {
    Console.WriteLine($"Selected: {string.Join(", ", e.SelectedItems.Select(i => i.Text))}");
};

// Individual item selection changed
hierarchicalRadioGroup.ItemSelectionChanged += (sender, e) => {
    Console.WriteLine($"Item '{e.Item.Text}' selection: {e.IsSelected}");
};
```

### Hierarchy Events
```csharp
// Item expanded/collapsed
hierarchicalRadioGroup.ItemExpandedChanged += (sender, e) => {
    Console.WriteLine($"Item '{e.Item.Text}' {(e.IsExpanded ? "expanded" : "collapsed")}");
};
```

### Mouse Events
```csharp
// Item clicked
hierarchicalRadioGroup.ItemClicked += (sender, e) => {
    Console.WriteLine($"Clicked: {e.Item.Text}");
};

// Mouse hover events
hierarchicalRadioGroup.ItemHoverEnter += (sender, e) => {
    Console.WriteLine($"Hover enter: {e.Item.Text}");
};
```

## Methods

### Hierarchy Management
```csharp
// Add items
hierarchicalRadioGroup.AddRootItem(rootItem);
hierarchicalRadioGroup.AddChildItem(parentItem, childItem);

// Expand/collapse
hierarchicalRadioGroup.ExpandAll();
hierarchicalRadioGroup.CollapseAll();

// Find items
var item = hierarchicalRadioGroup.FindItem("guidId");
```

## Image Rendering

The control fully supports image rendering through the `ImagePath` property of `SimpleItem`:

### Supported Image Types
- SVG files (with theme color support)
- PNG, JPG, BMP, GIF files
- Embedded resources
- Icon fonts (when using appropriate paths)

### Image Rendering Behavior
- Images are rendered at 20x20 pixels by default
- Images appear to the left of the text content
- SVG images support theme color application
- Missing images are gracefully handled (no image shown)

### Example with Images
```csharp
var item = new SimpleItem
{
    Text = "Security Settings",
    SubText = "Configure security options",
    ImagePath = "shield.svg", // Will be rendered as an icon
    IsSelected = true
};
```

## Theme Integration

The control follows the same theming patterns as BeepListBox:

### Theme Properties Used
- `BorderColor`: Border colors for selections and focus
- `PrimaryColor`: Selected item colors
- `ButtonHoverBackColor`: Hover state background
- `ForeColor`: Text colors
- `DisabledForeColor`: Disabled text colors
- `LabelMedium`: Font for item text

### Custom Theme Support
```csharp
// Theme automatically applies when control theme changes
hierarchicalRadioGroup.Theme = "DarkTheme";
hierarchicalRadioGroup.ApplyTheme();
```

## Visual Examples

The control supports the exact hierarchy structures shown in your examples:

### KTLO Assessment Categories
```
? KTLO
  ? Maint / Upgrade
  ? Security / Compliance  
  ? Reliability / Performance
  ? Monitoring
  ? End User Services & Support
  ? Enhancements
  ? Retirements
  ? TLM
  ? Other
```

### Directional Selection
```
? North
   Side of the world
? South  
   Side of the world
? Strawberry
   Delicious berry
? East
   Side of the world
? West
   Side of the world
```

## Integration with BeepListBox Patterns

The hierarchical radio group follows the same patterns as BeepListBox:

### Common Features
- Theme integration (`_currentTheme` and `ApplyTheme()`)
- Image rendering using `BeepImage`
- Font handling with theme support
- Event patterns and naming conventions
- Helper class architecture

### Differences
- Hierarchical structure support
- Expand/collapse functionality
- Multiple rendering styles (radio vs list)
- Tree-like visual presentation

This provides a consistent experience across the Beep control suite while extending functionality for hierarchical data scenarios.