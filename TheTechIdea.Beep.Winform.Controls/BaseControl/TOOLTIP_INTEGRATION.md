# BaseControl ToolTipManager Integration

## Overview

BaseControl now fully integrates with the ToolTipManager system, providing rich, styled tooltips to all Beep controls. This integration replaces the legacy WinForms ToolTip with a modern, theme-aware tooltip system.

## Architecture

### File Structure
- **BaseControl.Tooltip.cs** - New partial class containing all tooltip functionality
- **BaseControl.cs** - Updated Dispose method to call `CleanupTooltip()`

### Integration Pattern
```
BaseControl (partial)
    ├── BaseControl.Tooltip.cs
    │   ├── Properties (8)
    │   │   ├── TooltipText
    │   │   ├── TooltipTitle
    │   │   ├── TooltipType
    │   │   ├── TooltipIconPath
    │   │   ├── EnableTooltip
    │   │   ├── TooltipDuration
    │   │   ├── TooltipPlacement
    │   │   └── TooltipUseControlStyle
    │   └── Methods (8)
    │       ├── UpdateTooltip() - Register/update with ToolTipManager
    │       ├── RemoveTooltip() - Unregister from ToolTipManager
    │       ├── ShowNotification() - Show temporary notification
    │       ├── ShowSuccess() - Success notification
    │       ├── ShowError() - Error notification
    │       ├── ShowWarning() - Warning notification
    │       ├── ShowInfo() - Info notification
    │       └── CleanupTooltip() - Cleanup on disposal
    └── BaseControl.cs
        └── Dispose() - Calls CleanupTooltip()
```

## Properties

### TooltipText
**Type:** `string`  
**Default:** `""` (empty string)  
**Category:** Tooltip  
**Description:** The main text displayed in the tooltip when hovering over the control.

```csharp
myControl.TooltipText = "Click to open file dialog";
```

### TooltipTitle
**Type:** `string`  
**Default:** `""` (empty string)  
**Category:** Tooltip  
**Description:** Optional header text displayed above the main tooltip text.

```csharp
myControl.TooltipTitle = "File Selection";
myControl.TooltipText = "Click to open file dialog";
```

### TooltipType
**Type:** `ToolTipType`  
**Default:** `ToolTipType.Default`  
**Category:** Tooltip  
**Description:** Semantic type of the tooltip (Default, Success, Warning, Error, Info, Custom, Notification, Help).

```csharp
myControl.TooltipType = ToolTipType.Warning;
myControl.TooltipText = "This action cannot be undone";
```

### TooltipIconPath
**Type:** `string`  
**Default:** `""` (empty string)  
**Category:** Tooltip  
**Description:** Path to icon/image displayed in the tooltip.

```csharp
myControl.TooltipIconPath = "icons/help.svg";
myControl.TooltipText = "Need assistance? Click here";
```

### EnableTooltip
**Type:** `bool`  
**Default:** `true`  
**Category:** Tooltip  
**Description:** Whether to show the tooltip when hovering over the control.

```csharp
myControl.EnableTooltip = false; // Disable tooltip
```

### TooltipDuration
**Type:** `int`  
**Default:** `3000` (3 seconds)  
**Category:** Tooltip  
**Description:** How long the tooltip remains visible (milliseconds).

```csharp
myControl.TooltipDuration = 5000; // Show for 5 seconds
```

### TooltipPlacement
**Type:** `ToolTipPlacement`  
**Default:** `ToolTipPlacement.Auto`  
**Category:** Tooltip  
**Description:** Preferred placement of the tooltip (Auto, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight).

```csharp
myControl.TooltipPlacement = ToolTipPlacement.Top;
```

### TooltipUseControlStyle
**Type:** `bool`  
**Default:** `true`  
**Category:** Tooltip  
**Description:** Whether the tooltip should use the control's BeepControlStyle for consistent theming.

```csharp
myControl.ControlStyle = BeepControlStyle.Material3;
myControl.TooltipUseControlStyle = true; // Tooltip will use Material3 style
```

## Methods

### UpdateTooltip()
**Access:** Private  
**Purpose:** Internal method that registers/updates the tooltip with ToolTipManager. Called automatically when tooltip properties change.

### RemoveTooltip()
**Access:** Public  
**Purpose:** Removes the tooltip from the control.

```csharp
myControl.RemoveTooltip();
```

### ShowNotification(message, type, duration)
**Access:** Public  
**Purpose:** Shows a temporary notification tooltip with custom type and duration.

```csharp
myControl.ShowNotification("File saved successfully", ToolTipType.Success, 2000);
```

### ShowSuccess(message, duration = 2000)
**Access:** Public  
**Purpose:** Shows a success notification tooltip.

```csharp
myControl.ShowSuccess("Operation completed!");
```

### ShowError(message, duration = 3000)
**Access:** Public  
**Purpose:** Shows an error notification tooltip.

```csharp
myControl.ShowError("Failed to save file");
```

### ShowWarning(message, duration = 2500)
**Access:** Public  
**Purpose:** Shows a warning notification tooltip.

```csharp
myControl.ShowWarning("This action cannot be undone");
```

### ShowInfo(message, duration = 2000)
**Access:** Public  
**Purpose:** Shows an info notification tooltip.

```csharp
myControl.ShowInfo("Press F1 for help");
```

### CleanupTooltip()
**Access:** Private  
**Purpose:** Internal method that unregisters the tooltip from ToolTipManager. Called automatically in Dispose().

## Usage Examples

### Basic Tooltip
```csharp
var button = new BeepButton
{
    Text = "Save",
    TooltipText = "Save the current document"
};
```

### Tooltip with Title and Icon
```csharp
var button = new BeepButton
{
    Text = "Delete",
    TooltipTitle = "Warning",
    TooltipText = "This will permanently delete the file",
    TooltipType = ToolTipType.Warning,
    TooltipIconPath = "icons/warning.svg"
};
```

### Notification Tooltips
```csharp
// Success notification
saveButton.Click += (s, e) => 
{
    SaveFile();
    saveButton.ShowSuccess("File saved successfully!");
};

// Error notification
deleteButton.Click += (s, e) => 
{
    if (!CanDelete())
    {
        deleteButton.ShowError("Cannot delete file - insufficient permissions");
        return;
    }
    DeleteFile();
};

// Warning notification
clearButton.Click += (s, e) => 
{
    clearButton.ShowWarning("This will clear all data");
};
```

### Theme-Aware Tooltips
```csharp
var control = new BeepTextBox
{
    ControlStyle = BeepControlStyle.Material3,
    TooltipText = "Enter your email address",
    TooltipUseControlStyle = true // Tooltip will match Material3 style
};
```

### Custom Placement
```csharp
var control = new BeepButton
{
    Text = "Help",
    TooltipText = "Click for help",
    TooltipPlacement = ToolTipPlacement.Top // Always show above button
};
```

### Programmatic Control
```csharp
// Temporarily disable tooltip
myControl.EnableTooltip = false;
PerformSensitiveOperation();
myControl.EnableTooltip = true;

// Change tooltip text dynamically
myControl.TooltipText = $"Processing {itemCount} items...";

// Remove tooltip completely
myControl.RemoveTooltip();
```

## Integration with Existing Controls

All controls inheriting from BaseControl automatically gain tooltip functionality:

```csharp
// All these controls now support rich tooltips:
BeepButton
BeepTextBox
BeepLabel
BeepPanel
BeepGrid
BeepTree
BeepImage
// ... and all other BaseControl-derived controls
```

## BeepControlStyle Support

Tooltips respect the control's `ControlStyle` when `TooltipUseControlStyle` is enabled:

```csharp
var button = new BeepButton
{
    ControlStyle = BeepControlStyle.Material3, // Material Design 3
    TooltipText = "Save changes",
    TooltipUseControlStyle = true // Tooltip will use Material3 colors/styling
};
```

Supported styles include:
- Material3, Material2, Material1
- Fluent, FluentDark
- Corporate, CorporateBlue, CorporateDark
- Minimalist, MinimalistDark
- Elegant, ElegantDark
- Modern, ModernDark
- Vintage, VintageDark
- Pastel, PastelDark
- NeonGlow, NeonGlowDark
- HighContrast, HighContrastLight
- Professional
- Custom

## Performance Considerations

1. **Lazy Registration**: Tooltips are only registered with ToolTipManager when needed
2. **Automatic Cleanup**: Tooltips are automatically cleaned up when controls are disposed
3. **Efficient Updates**: Changing tooltip properties triggers minimal overhead
4. **Theme Integration**: Uses existing BeepStyling and StyledImagePainter infrastructure

## Migration from Legacy Tooltips

### Old System (Legacy WinForms ToolTip)
```csharp
// Old way - not recommended
var tooltip = new ToolTip();
tooltip.SetToolTip(button, "Click me");
```

### New System (ToolTipManager)
```csharp
// New way - recommended
button.TooltipText = "Click me";
```

### Benefits of New System
- ✅ Theme-aware styling
- ✅ Rich content (title, icon, styled text)
- ✅ Consistent with Beep design system
- ✅ Automatic cleanup
- ✅ Designer support
- ✅ Notification support
- ✅ BeepControlStyle integration

## Designer Support

All tooltip properties are visible in the Visual Studio Properties window:

**Category: Tooltip**
- TooltipText
- TooltipTitle
- TooltipType
- TooltipIconPath
- EnableTooltip
- TooltipDuration
- TooltipPlacement
- TooltipUseControlStyle

Properties include proper `[Browsable]`, `[Category]`, `[Description]`, and `[DefaultValue]` attributes for optimal designer experience.

## Disposal Pattern

The tooltip system follows proper disposal pattern:

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Cleanup ToolTipManager registration
        CleanupTooltip(); // <-- Added to BaseControl.Dispose()
        
        // ... other cleanup
    }
    base.Dispose(disposing);
}
```

This ensures:
- No memory leaks
- Proper ToolTipManager cleanup
- Thread-safe disposal
- No orphaned tooltip instances

## Future Enhancements

Potential future improvements:
1. **Tooltip Templates** - Predefined layouts for common scenarios
2. **Rich Text Support** - Markdown or HTML formatting in tooltips
3. **Interactive Tooltips** - Clickable buttons/links in tooltips
4. **Tooltip Preview** - Designer-time preview of tooltip appearance
5. **Keyboard Navigation** - Show tooltips via keyboard (F1, Ctrl+?)
6. **Animation Effects** - Fade in/out, slide animations
7. **Multi-line Auto-wrap** - Intelligent text wrapping for long content
8. **Accessibility** - Enhanced screen reader support

## Best Practices

1. **Keep It Concise** - Tooltips should be brief and informative
2. **Use Titles Sparingly** - Only add titles when needed for context
3. **Match Control Style** - Enable `TooltipUseControlStyle` for visual consistency
4. **Appropriate Duration** - 2-3 seconds for normal tooltips, longer for notifications
5. **Semantic Types** - Use appropriate `ToolTipType` (Success, Error, Warning, Info)
6. **Icon Usage** - Add icons to enhance visual communication
7. **Placement** - Use `Auto` unless specific placement is required
8. **Notifications** - Use notification methods for transient feedback

## Testing Checklist

- [ ] Basic tooltip displays on hover
- [ ] Tooltip text updates dynamically
- [ ] Tooltip with title and icon renders correctly
- [ ] All ToolTipType variants display with correct styling
- [ ] TooltipUseControlStyle applies control's BeepControlStyle
- [ ] ShowSuccess/Error/Warning/Info notifications work
- [ ] Tooltip duration is respected
- [ ] Tooltip placement (Top, Bottom, Left, Right) works
- [ ] EnableTooltip toggle works correctly
- [ ] Tooltip cleanup on control disposal
- [ ] Designer properties window shows all tooltip properties
- [ ] Theme changes update tooltip styling
- [ ] Multiple tooltips on same form don't conflict

## Related Components

- **ToolTipManager** - Central tooltip management system
- **CustomToolTip** - Form-based tooltip renderer
- **BeepStyledToolTipPainter** - BeepStyling-integrated painter
- **BeepStyling** - Central styling system
- **StyledImagePainter** - Image rendering for icons
- **BeepThemesManager** - Theme management

## Documentation Files

- **TOOLTIP_INTEGRATION.md** (this file) - BaseControl integration guide
- **TOOLTIP_ENHANCEMENTS.md** - Overall tooltip system enhancements
- **BaseControl/Readme.md** - BaseControl documentation
- **ToolTips/Readme.md** - ToolTip system documentation

---

**Last Updated:** 2025-01-21  
**Version:** 1.0  
**Status:** ✅ Complete and Production Ready
