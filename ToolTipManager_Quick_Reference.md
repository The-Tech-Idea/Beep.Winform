# ToolTipManager - Quick Reference Guide

## File Organization

### ✅ One Class Per File - Complete Structure

```
TheTechIdea.Beep.Winform.Controls\ToolTips\
│
├── ToolTipManager.cs              (Main: Fields, Settings, State)
├── ToolTipManager.Api.cs          (Public API: Show, Hide, Update)
├── ToolTipManager.Controls.cs     (Control Integration)
├── ToolTipManager.Events.cs       (Event Handlers & Cleanup)
│
├── ToolTipConfig.cs               (Configuration Class)
├── ToolTipEnums.cs                (All Enumerations)
├── ToolTipInstance.cs             (Internal Lifecycle)
│
├── Helpers\
│   └── ToolTipHelpers.cs          (Positioning & Utilities)
│
└── Painters\
    ├── IToolTipPainter.cs         (Painter Interface)
    ├── StandardToolTipPainter.cs
    ├── PremiumToolTipPainter.cs
    ├── AlertToolTipPainter.cs
    ├── NotificationToolTipPainter.cs
    └── StepToolTipPainter.cs
```

## Quick API Reference

### Show Tooltips

```csharp
// Simple text tooltip
var key = await ToolTipManager.ShowTooltipAsync("Text", location);

// With title
var key = await ToolTipManager.ShowTooltipAsync("Title", "Text", location, ToolTipTheme.Primary);

// Full configuration
var config = new ToolTipConfig { /* ... */ };
var key = await ToolTipManager.ShowTooltipAsync(config);
```

### Hide Tooltips

```csharp
// Hide specific
await ToolTipManager.HideTooltipAsync(key);

// Hide all
await ToolTipManager.HideAllTooltipsAsync();
```

### Control Integration

```csharp
// Attach to control
ToolTipManager.SetTooltip(button, "Click me!");

// With theme
ToolTipManager.SetTooltip(button, "Title", "Text", ToolTipTheme.Success);

// Remove
ToolTipManager.RemoveTooltip(button);

// Batch operations
ToolTipManager.SetTooltipForControls("Same text", button1, button2, button3);
```

### Global Settings

```csharp
ToolTipManager.DefaultTheme = ToolTipTheme.Dark;
ToolTipManager.DefaultShowDelay = 300;
ToolTipManager.DefaultHideDelay = 3000;
ToolTipManager.EnableAnimations = true;
ToolTipManager.EnableAccessibility = true;
```

## Configuration Options

### ToolTipConfig Properties

| Category | Properties |
|----------|-----------|
| **Content** | Text, Title, Html |
| **Position** | Position, Placement, Offset, FollowCursor |
| **Timing** | Duration, ShowDelay, HideDelay |
| **Theme** | Theme, Style |
| **Colors** | BackColor, ForeColor, BorderColor |
| **Visual** | ShowArrow, ShowShadow, Closable |
| **Images** | Icon, IconPath, ImagePath, MaxImageSize |
| **Animation** | Animation, AnimationDuration |
| **Events** | OnShow, OnClose |

### Enumerations

#### ToolTipTheme
- `Auto`, `Light`, `Dark`
- `Primary`, `Success`, `Warning`, `Error`, `Info`
- `Custom`

#### ToolTipStyle
- `Standard` - Clean modern look
- `Premium` - Gradient with badge
- `Alert` - Accent bar with icons
- `Notification` - Toast/popup style
- `Step` - Tutorial walkthrough

#### ToolTipPlacement
- `Auto`
- `Top`, `TopStart`, `TopEnd`
- `Bottom`, `BottomStart`, `BottomEnd`
- `Left`, `LeftStart`, `LeftEnd`
- `Right`, `RightStart`, `RightEnd`

#### ToolTipAnimation
- `None`, `Fade`, `Scale`, `Slide`, `Bounce`

## Common Patterns

### Pattern 1: Simple Info Tooltip
```csharp
ToolTipManager.SetTooltip(infoButton, "Additional information about this feature");
```

### Pattern 2: Warning Tooltip
```csharp
ToolTipManager.SetTooltip(deleteButton, "Warning", 
    "This action cannot be undone", ToolTipTheme.Warning);
```

### Pattern 3: Success Confirmation
```csharp
await ToolTipManager.ShowTooltipAsync(new ToolTipConfig
{
    Title = "Success!",
    Text = "Your changes have been saved",
    Theme = ToolTipTheme.Success,
    Style = ToolTipStyle.Notification,
    Duration = 2000,
    Position = button.PointToScreen(new Point(button.Width / 2, 0))
});
```

### Pattern 4: Tutorial Step
```csharp
var config = new ToolTipConfig
{
    Style = ToolTipStyle.Step,
    CurrentStep = 1,
    TotalSteps = 5,
    StepTitle = "Getting Started",
    Text = "Click here to begin",
    ShowNavigationButtons = true,
    Theme = ToolTipTheme.Primary
};
await ToolTipManager.ShowTooltipAsync(config);
```

### Pattern 5: Follow Cursor
```csharp
var config = new ToolTipConfig
{
    Text = "Drag to resize",
    FollowCursor = true,
    ShowArrow = false,
    Animation = ToolTipAnimation.None
};
ToolTipManager.SetTooltip(resizeHandle, config.Text, config);
```

### Pattern 6: Rich Content with Icon
```csharp
var config = new ToolTipConfig
{
    Title = "Pro Feature",
    Text = "Upgrade to access this premium feature",
    IconPath = "icons/star.svg",
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Premium,
    MaxImageSize = new Size(32, 32)
};
ToolTipManager.SetTooltip(proFeatureButton, config.Text, config);
```

## Architecture Principles

### ✅ Separation of Concerns
- Each partial class handles ONE aspect
- Each file contains ONE class
- Clear boundaries between responsibilities

### ✅ Modern UI Framework Patterns
- Declarative configuration
- Smart defaults with easy overrides
- Event-driven interactions
- Theme integration

### ✅ Helper, Partial, Painter Pattern
- **Helper**: ToolTipHelpers for utilities
- **Partial**: ToolTipManager split into logical files
- **Painter**: IToolTipPainter for flexible rendering

## Best Practices

### DO ✅
- Use `SetTooltip` for controls with automatic hover behavior
- Use `ShowTooltipAsync` for programmatic, timed tooltips
- Set global defaults at application startup
- Use themes for consistency
- Enable accessibility features
- Clean up tooltips when disposing forms

### DON'T ❌
- Don't create tooltip instances directly
- Don't forget to await async methods
- Don't disable accessibility without good reason
- Don't use overly long text (keep it concise)
- Don't show multiple tooltips simultaneously (confusing)

## Performance Tips

1. **Reuse Configurations**: Create config templates for common patterns
2. **Batch Operations**: Use `SetTooltipForControls` for multiple controls
3. **Cleanup**: Call `RemoveAllControlTooltips()` when disposing forms
4. **Animations**: Disable globally if performance is critical
5. **Delays**: Adjust show/hide delays based on UX needs

## Accessibility Considerations

```csharp
// Enable accessibility features
ToolTipManager.EnableAccessibility = true;

// This automatically sets:
// - control.AccessibleDescription
// - control.AccessibleName
// - ARIA attributes (when applicable)

// For users who need reduced motion
ToolTipManager.EnableAnimations = false;
```

## Thread Safety

All ToolTipManager methods are thread-safe:
- Uses `ConcurrentDictionary` for shared state
- Proper async/await patterns
- Cancellation token support
- Safe for use from any thread

## Memory Management

Automatic cleanup ensures no memory leaks:
- Cleanup timer runs every 5 seconds
- Expired tooltips are automatically removed
- Proper disposal of resources
- Weak references for controls (implicit via GC)

## Debugging Tips

```csharp
// Check active tooltips
var count = ToolTipManager.ActiveTooltipCount;
var keys = ToolTipManager.GetActiveTooltipKeys();

// Check if tooltip is active
if (ToolTipManager.IsTooltipActive(key))
{
    await ToolTipManager.HideTooltipAsync(key);
}

// Check if control has tooltip
if (ToolTipManager.HasTooltip(button))
{
    var key = ToolTipManager.GetControlTooltipKey(button);
}

// Debug output is logged to Debug console
// Look for "[ToolTipManager]" prefix in output
```

## Summary

The ToolTipManager provides:
- ✅ Modern, framework-inspired API
- ✅ Clean separation of concerns (one class per file)
- ✅ Partial classes for organized functionality
- ✅ Painter pattern for flexible rendering
- ✅ Thread-safe operations
- ✅ Automatic memory management
- ✅ Rich theming and styling
- ✅ Accessibility support
- ✅ Animation capabilities
- ✅ Enterprise-grade quality

**Perfect for production applications!**
