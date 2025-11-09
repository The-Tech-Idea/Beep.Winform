# Dialog Presets System

## Overview

The Dialog Presets system provides predefined, professionally-designed dialog styles matching modern UI design patterns. Similar to how we enhanced the context menu system, dialogs now support both flexible `BeepControlStyle`-based rendering and preset templates.

## Architecture

```
DialogManager
    ├── DialogConfig (with Preset property)
    ├── DialogPreset enum (ConfirmAction, SmoothPositive, etc.)
    ├── DialogPainterFactory
    │   ├── Creates BeepStyledDialogPainter (for Style-based)
    │   └── Creates PresetDialogPainter (for Preset-based)
    └── Painters
        ├── IDialogPainter (interface)
        ├── DialogPainterBase (abstract base)
        ├── BeepStyledDialogPainter (20+ BeepControlStyle designs)
        └── PresetDialogPainter (preset templates)
```

## Available Presets

### 1. ConfirmAction
**Visual Style:** White card with rounded corners, subtle shadow, icon at top  
**Colors:** White background, blue confirm button, red cancel button  
**Use Case:** Standard confirmation dialogs  
**Example:**
```csharp
var result = DialogManager.ShowConfirmAction(
    "Confirm Action", 
    "Are you sure you want to proceed?");
```

### 2. SmoothPositive
**Visual Style:** Smooth rounded dialog with green background  
**Colors:** Green (#4CAF50), white text  
**Use Case:** Success messages, positive confirmations  
**Example:**
```csharp
DialogManager.ShowSmoothPositive(
    "Success", 
    "Operation completed successfully!");
```

### 3. SmoothDanger
**Visual Style:** Smooth rounded dialog with pink/red background  
**Colors:** Pink (#F48FB1), white text  
**Use Case:** Warnings, error confirmations  
**Example:**
```csharp
var result = DialogManager.ShowSmoothDanger(
    "Warning", 
    "This action cannot be undone.");
```

### 4. SmoothDense (with color variants)
**Visual Style:** Compact dialog with minimal padding  
**Variants:** Blue, Red, Gray, Green  
**Colors:** Depends on variant  
**Use Case:** Compact notifications, quick confirmations  
**Example:**
```csharp
// Blue variant (default)
DialogManager.ShowSmoothDense("Info", "Processing...");

// Red variant
DialogManager.ShowSmoothDense("Error", "Failed", 
    DialogManager.SmoothDenseVariant.Red);

// Green variant
DialogManager.ShowSmoothDense("Success", "Done", 
    DialogManager.SmoothDenseVariant.Green);
```

### 5. RaisedDense
**Visual Style:** Elevated white card with strong shadow, compact layout  
**Colors:** White background, blue primary button  
**Use Case:** Important confirmations that need elevation  
**Example:**
```csharp
var result = DialogManager.ShowRaisedDense(
    "Important", 
    "Please review before continuing.");
```

### 6. SmoothPrimary
**Visual Style:** Full blue background, prominent appearance  
**Colors:** Blue primary (#2196F3), white text  
**Use Case:** Primary actions, important notifications  
**Example:**
```csharp
DialogManager.ShowSmoothPrimary(
    "Welcome", 
    "Thank you for using our application!");
```

### 7. SetproductDesign
**Visual Style:** Light mint/green background, design system branding  
**Colors:** Mint (#C8E6C9), dark green text  
**Use Case:** Design system specific dialogs  
**Example:**
```csharp
DialogManager.ShowSetproductDesign(
    "Design System", 
    "Component library documentation.");
```

### 8. RaisedDanger
**Visual Style:** Light pink elevated card with red accents, strong shadow  
**Colors:** Light pink background (#FFEBEE), red text (#B71C1C)  
**Use Case:** Critical warnings, destructive actions  
**Example:**
```csharp
var result = DialogManager.ShowRaisedDanger(
    "Delete Confirmation", 
    "This will permanently delete all data.");
```

## Usage Patterns

### Quick Preset Dialog
```csharp
// Simple one-liner
DialogManager.ShowSmoothPositive("Success", "File saved!");

// With result checking
var result = DialogManager.ShowConfirmAction("Confirm", "Proceed?");
if (result.Result == BeepDialogResult.OK)
{
    // User confirmed
}
```

### Custom Preset Configuration
```csharp
var config = DialogConfig.CreateSmoothDanger("Warning", "Continue?");
config.ShowIcon = false; // Hide icon
config.Animation = DialogShowAnimation.SlideInFromTop;
config.AutoCloseTimeout = 3000; // Auto-close after 3 seconds

var result = DialogManager.ShowPresetDialog(config);
```

### Using DialogConfig Factory Methods
```csharp
// Create preset config
var config = DialogConfig.CreateConfirmAction("Title", "Message");
config.Buttons = new[] { 
    BeepDialogButtons.Yes, 
    BeepDialogButtons.No, 
    BeepDialogButtons.Cancel 
};

var result = DialogManager.ShowPresetDialog(config);
```

## Mixing Presets with BeepControlStyle

You can combine presets with `BeepControlStyle` customization:

```csharp
var config = DialogConfig.CreateSmoothPositive("Success", "Done!");

// Override colors while keeping preset layout
config.BackColor = Color.FromArgb(0, 150, 136); // Custom teal
config.ForeColor = Color.White;

// Or switch to Style-based rendering
config.Preset = DialogPreset.None;
config.Style = BeepControlStyle.Material3;

DialogManager.ShowPresetDialog(config);
```

## Color Schemes

Each preset has a predefined color scheme accessible via `DialogPresetColors`:

```csharp
var colorScheme = DialogPresetColors.GetColorScheme(DialogPreset.SmoothPositive);
// colorScheme.Background = Green
// colorScheme.Foreground = White
// colorScheme.PrimaryButton = Dark Green
// etc.
```

### Preset Properties

```csharp
// Get corner radius for preset
int radius = DialogPresetColors.GetCornerRadius(DialogPreset.ConfirmAction); // 16

// Check if uses raised buttons
bool raised = DialogPresetColors.UsesRaisedButtons(DialogPreset.RaisedDense); // true

// Get padding (Dense presets use 12px, others 20px)
int padding = DialogPresetColors.GetPadding(DialogPreset.SmoothDense); // 12
```

## Painter Architecture

### DialogPainterFactory

The factory pattern selects the appropriate painter:

```csharp
// Preset-based
var config = new DialogConfig { Preset = DialogPreset.ConfirmAction };
var painter = DialogPainterFactory.CreatePainter(config); // PresetDialogPainter

// Style-based
config.Preset = DialogPreset.None;
painter = DialogPainterFactory.CreatePainter(config); // BeepStyledDialogPainter

// Type-based with preset recommendation
painter = DialogPainterFactory.CreatePainterForType(
    BeepDialogIcon.Success, 
    usePreset: true); // Returns SmoothPositive preset
```

### PresetDialogPainter

Handles all preset rendering with:
- Custom shadow rendering (Raised vs Smooth styles)
- Gradient overlays for smooth presets
- Button styling (raised vs flat)
- Preset-specific corner radii and padding
- Color scheme application

### BeepStyledDialogPainter

Supports 20+ `BeepControlStyle` designs:
- Material3, MaterialDesign
- Fluent, FluentModern
- Corporate, CorporateBusiness
- Minimalist, Elegant
- Custom styles from BeepStyling system

## Integration with BeepStyling

Dialogs integrate with the global `BeepStyling` system:

```csharp
// Use current global style
var config = new DialogConfig 
{ 
    Preset = DialogPreset.None, // Disable preset
    Style = BeepStyling.CurrentControlStyle 
};

// Or specify explicit style
config.Style = BeepControlStyle.Material3;

// Apply theme colors
config.UseBeepThemeColors = true; // Uses IBeepTheme colors
```

## Button Customization

```csharp
var config = DialogConfig.CreateConfirmAction("Title", "Message");

// Change buttons
config.Buttons = new[] 
{ 
    BeepDialogButtons.Yes, 
    BeepDialogButtons.No 
};

// Set default button (receives focus)
config.DefaultButton = BeepDialogButtons.Yes;

// Button layout
config.ButtonLayout = DialogButtonLayout.Vertical; // Stack vertically
```

## Animation Effects

All presets support animations:

```csharp
var config = DialogConfig.CreateSmoothPrimary("Welcome", "Hello!");
config.Animation = DialogShowAnimation.FadeIn; // Default
config.AnimationDuration = 300; // milliseconds

// Available animations:
// - None
// - FadeIn
// - SlideInFromTop
// - SlideInFromBottom
// - SlideInFromLeft
// - SlideInFromRight
// - ZoomIn
```

## Shadow and Effects

```csharp
var config = DialogConfig.CreateRaisedDense("Title", "Message");

// Shadow customization
config.ShowShadow = true;
config.EnableShadow = true;
config.ShadowColor = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black

// Backdrop (dimming overlay)
config.ShowBackdrop = true;
config.BackdropOpacity = 0.5f; // 50% opacity
```

## Positioning

```csharp
var config = DialogConfig.CreateConfirmAction("Title", "Message");

// Preset positions
config.Position = DialogPosition.CenterParent; // Default
config.Position = DialogPosition.CenterScreen;
config.Position = DialogPosition.TopCenter;

// Custom position
config.Position = DialogPosition.Custom;
config.CustomLocation = new Point(100, 100);
```

## Size Control

```csharp
var config = DialogConfig.CreateSmoothPositive("Title", "Message");

// Auto-size with constraints
config.MinWidth = 300;
config.MaxWidth = 600;

// Fixed size
config.CustomSize = new Size(400, 250);
```

## Best Practices

### When to Use Presets

✅ **Use Presets When:**
- You want consistent, professionally-designed dialogs
- Matching specific design patterns (confirm action, success, error)
- Rapid prototyping with ready-made styles
- Color-coded user feedback (green=success, red=error)

❌ **Use BeepControlStyle When:**
- You need full customization aligned with your app theme
- Integrating with existing BeepStyling design system
- Maintaining consistency across all Beep controls
- Applying custom brand colors and styles

### Preset Selection Guide

| Scenario | Recommended Preset |
|----------|-------------------|
| Standard confirmation | `ConfirmAction` |
| Success notification | `SmoothPositive` |
| Error/warning | `SmoothDanger` or `RaisedDanger` |
| Quick info | `SmoothDense` (blue) |
| Critical warning | `RaisedDanger` |
| Primary action | `SmoothPrimary` |
| Compact layouts | `RaisedDense` or `SmoothDense` |

## Examples

### Example 1: File Delete Confirmation
```csharp
var result = DialogManager.ShowRaisedDanger(
    "Delete File",
    "This will permanently delete the selected file. Continue?"
);

if (result.Result == BeepDialogResult.OK)
{
    // Delete file
}
```

### Example 2: Save Success
```csharp
var config = DialogConfig.CreateSmoothPositive(
    "Saved",
    "Your changes have been saved successfully."
);
config.Buttons = new[] { BeepDialogButtons.Ok };
config.AutoCloseTimeout = 2000; // Auto-close after 2 seconds

DialogManager.ShowPresetDialog(config);
```

### Example 3: Multi-Option Dialog
```csharp
var config = DialogConfig.CreateConfirmAction(
    "Unsaved Changes",
    "Do you want to save your changes before closing?"
);
config.Buttons = new[] 
{ 
    BeepDialogButtons.Yes,  // Save
    BeepDialogButtons.No,   // Don't save
    BeepDialogButtons.Cancel // Cancel close
};

var result = DialogManager.ShowPresetDialog(config);

switch (result.Result)
{
    case BeepDialogResult.Yes:
        SaveDocument();
        break;
    case BeepDialogResult.No:
        // Close without saving
        break;
    case BeepDialogResult.Cancel:
        // Don't close
        break;
}
```

### Example 4: Dense Variant Selector
```csharp
// Show different colored dense dialogs based on severity
void ShowNotification(string message, NotificationLevel level)
{
    var variant = level switch
    {
        NotificationLevel.Info => DialogManager.SmoothDenseVariant.Blue,
        NotificationLevel.Success => DialogManager.SmoothDenseVariant.Green,
        NotificationLevel.Warning => DialogManager.SmoothDenseVariant.Gray,
        NotificationLevel.Error => DialogManager.SmoothDenseVariant.Red,
        _ => DialogManager.SmoothDenseVariant.Blue
    };

    DialogManager.ShowSmoothDense("Notification", message, variant);
}
```

## Extending the System

### Creating Custom Presets

1. Add new enum value to `DialogPreset`
2. Add color scheme in `DialogPresetColors.GetColorScheme()`
3. Update `PresetDialogPainter` if special rendering needed
4. Add factory method to `DialogConfig`
5. Add convenience method to `DialogManager.Presets.cs`

### Custom Painter Implementation

```csharp
public class MyCustomDialogPainter : DialogPainterBase
{
    public override void Paint(Graphics g, Rectangle bounds, 
        DialogConfig config, IBeepTheme theme)
    {
        // Custom rendering logic
    }

    // Implement other abstract methods...
}

// Use custom painter
var painter = new MyCustomDialogPainter();
// Apply to dialog rendering
```

## Migration Guide

### From Old DialogManager
```csharp
// Old way
DialogManager.MsgBox("Title", "Message");

// New way (keeping old method working)
DialogManager.MsgBox("Title", "Message"); // Still works

// Or use preset for better styling
DialogManager.ShowSmoothPositive("Title", "Message");
```

### From BeepDialogModal
```csharp
// Old way
using (var dialog = new BeepDialogModal())
{
    dialog.Title = "Confirm";
    dialog.Message = "Are you sure?";
    dialog.DialogType = DialogType.Question;
    dialog.ShowDialog();
}

// New way
DialogManager.ShowConfirmAction("Confirm", "Are you sure?");
```

## Performance Considerations

- Presets are lightweight (no theme lookups required)
- Shadow rendering cached where possible
- Gradient brushes disposed properly
- Painter instances reusable

## Accessibility

All presets support:
- High contrast mode detection
- Screen reader friendly
- Keyboard navigation (Tab, Enter, Esc)
- Focus indicators on buttons
- Semantic colors (green=success, red=error)

## Future Enhancements

Planned features:
- Custom icon sets per preset
- Animation timing curves
- Sound effects per preset type
- Per-monitor DPI scaling
- UIA automation events
- Enhanced keyboard navigation

---

## Summary

The Dialog Presets system provides:
- **11 professionally-designed preset templates**
- **Seamless integration** with BeepStyling
- **Flexible customization** via DialogConfig
- **Simple API** with convenience methods
- **Consistent UX** matching modern design patterns
- **Performance optimized** rendering

Use presets for rapid development and consistent UX, or fall back to `BeepControlStyle`-based rendering for full theme integration.
