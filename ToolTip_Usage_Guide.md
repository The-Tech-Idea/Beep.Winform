# ToolTip System - Usage Guide

## Quick Start

### 1. Simple Text Tooltip
```csharp
// Show a simple tooltip at cursor position
await ToolTipManager.ShowTooltipAsync("Click to save", Cursor.Position);
```

### 2. Tooltip with Icon/Image
```csharp
var config = new ToolTipConfig
{
    Text = "File saved successfully!",
    Title = "Success",
    ImagePath = "icons/check-circle.svg",  // SVG icon path
    ApplyThemeOnImage = true,              // Apply theme colors to SVG
    Theme = ToolTipTheme.Success,          // Green success colors from theme
    Style = ToolTipStyle.Alert,            // Alert style with left bar
    Animation = ToolTipAnimation.Fade
};

await ToolTipManager.ShowTooltipAsync(config);
```

### 3. Attach Tooltip to Control
```csharp
// Attach tooltip to a button
ToolTipManager.SetTooltip(myButton, "Save your work", new ToolTipConfig
{
    ImagePath = "icons/save.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Standard
});

// Tooltip will show on hover
```

## Image/Icon Options

### Option 1: ImagePath (Recommended - Theme Support)
```csharp
config.ImagePath = "icons/alert.svg";        // SVG with theme colors
config.ApplyThemeOnImage = true;             // Theme colors applied
```

### Option 2: IconPath (Backward Compatible)
```csharp
config.IconPath = "icons/alert.png";         // PNG/JPG file
// No theme colors applied to raster images
```

### Option 3: Icon Property
```csharp
config.Icon = myImage;                        // Direct Image object
```

### Priority Order
1. `ImagePath` (checked first)
2. `IconPath` (fallback)
3. `Icon` (final fallback)

## Tooltip Styles

### Standard Style
Clean, modern tooltip with rounded corners
```csharp
Style = ToolTipStyle.Standard
```

### Premium Style
Gradient background with "PREMIUM" badge
```csharp
Style = ToolTipStyle.Premium
Theme = ToolTipTheme.Primary  // Blue gradient
```

### Alert Style
Colored left accent bar with status icon
```csharp
Style = ToolTipStyle.Alert
Theme = ToolTipTheme.Error    // Red bar + error icon
```

## Theme Options

All themes use colors from `IBeepTheme`:

| Theme | Colors Used | Best For |
|-------|-------------|----------|
| `ToolTipTheme.Light` | `theme.BackColor`, `theme.ForeColor` | Light backgrounds |
| `ToolTipTheme.Dark` | `theme.SecondaryBackColor`, `theme.ForeColor` | Dark backgrounds |
| `ToolTipTheme.Primary` | `theme.AccentColor` | Primary actions |
| `ToolTipTheme.Success` | `theme.SuccessColor` | Success messages |
| `ToolTipTheme.Warning` | `theme.WarningColor` | Warnings |
| `ToolTipTheme.Error` | `theme.ErrorColor` | Errors |
| `ToolTipTheme.Info` | `theme.InfoColor` | Information |

## Animations

```csharp
// Fade in/out (default)
Animation = ToolTipAnimation.Fade

// Scale from center
Animation = ToolTipAnimation.Scale

// Slide from edge
Animation = ToolTipAnimation.Slide

// Bounce effect
Animation = ToolTipAnimation.Bounce

// No animation
Animation = ToolTipAnimation.None
```

## Positioning

```csharp
// Auto positioning (smart placement)
Placement = ToolTipPlacement.Auto

// Specific positions
Placement = ToolTipPlacement.Top
Placement = ToolTipPlacement.Bottom
Placement = ToolTipPlacement.Left
Placement = ToolTipPlacement.Right

// Aligned positions
Placement = ToolTipPlacement.TopStart    // Top-left aligned
Placement = ToolTipPlacement.BottomEnd   // Bottom-right aligned
```

## Full Configuration Example

```csharp
var config = new ToolTipConfig
{
    // Content
    Title = "Connection Status",
    Text = "Unable to connect to server. Check your internet connection.",
    
    // Appearance
    Theme = ToolTipTheme.Error,
    Style = ToolTipStyle.Alert,
    
    // Image (uses ImagePainter for rendering)
    ImagePath = "icons/wifi-off.svg",
    ApplyThemeOnImage = true,          // SVG will use theme colors
    MaxImageSize = new Size(24, 24),   // Max icon size
    
    // Behavior
    ShowArrow = true,
    ShowShadow = true,
    Closable = false,
    Duration = 3000,                   // 3 seconds
    
    // Animation
    Animation = ToolTipAnimation.Fade,
    AnimationDuration = 200,           // 200ms
    
    // Position
    Placement = ToolTipPlacement.Top,
    Offset = 8,                        // 8px from target
    Position = Cursor.Position,
    
    // Constraints
    MaxSize = new Size(300, 200),
    
    // Callbacks
    OnShow = (key) => Console.WriteLine($"Tooltip {key} shown"),
    OnClose = (key) => Console.WriteLine($"Tooltip {key} closed")
};

// Show the tooltip
string tooltipKey = await ToolTipManager.ShowTooltipAsync(config);

// Hide it programmatically later
await ToolTipManager.HideTooltipAsync(tooltipKey);
```

## Advanced Usage

### Manual Control
```csharp
// Show and get the key
string key = await ToolTipManager.ShowTooltipAsync(config);

// Update content
ToolTipManager.UpdateTooltip(key, "New text", "New title");

// Hide specific tooltip
await ToolTipManager.HideTooltipAsync(key);

// Hide all tooltips
ToolTipManager.HideAll();
```

### Control Tooltips with Follow Cursor
```csharp
var config = new ToolTipConfig
{
    Text = "Drag to move",
    FollowCursor = true,  // Tooltip follows mouse
    Theme = ToolTipTheme.Info
};

ToolTipManager.SetTooltip(myControl, "Tooltip text", config);
```

## ImagePainter Integration

All tooltips use `ImagePainter` for rendering images:

- ✅ **SVG Support** - Full SVG rendering with theme colors
- ✅ **Theme Colors** - When `ApplyThemeOnImage = true`, SVG colors match theme
- ✅ **Scaling** - Automatic aspect ratio preservation
- ✅ **Quality** - High-quality rendering with anti-aliasing
- ✅ **Caching** - Efficient caching for repeated rendering
- ✅ **Disposal** - Proper resource cleanup

### How ImagePainter Works in Tooltips

```csharp
// Inside each painter's DrawImageFromPath method:
using (var painter = new ImagePainter(config.ImagePath, theme))
{
    painter.ApplyThemeOnImage = config.ApplyThemeOnImage;
    painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
    painter.Alignment = ContentAlignment.MiddleCenter;
    
    if (config.ApplyThemeOnImage && theme != null)
    {
        painter.ApplyThemeToSvg();  // Apply theme colors
    }
    
    painter.DrawImage(g, iconRect);
}
```

## Best Practices

1. **Use ImagePath for SVG icons** - Theme colors work best with SVG
2. **Enable ApplyThemeOnImage** - For consistent UI matching your theme
3. **Choose appropriate Style** - Alert for status, Premium for highlights
4. **Set reasonable Duration** - 2-5 seconds for informational tooltips
5. **Use Auto placement** - Let the system find the best position
6. **Keep text concise** - Tooltips should be brief and helpful
7. **Use appropriate Theme** - Match the message type (Error, Success, etc.)

## Common Patterns

### Error Notification
```csharp
var error = new ToolTipConfig
{
    Title = "Error",
    Text = "Operation failed",
    ImagePath = "icons/x-circle.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Error,
    Style = ToolTipStyle.Alert,
    Duration = 5000
};
```

### Success Confirmation
```csharp
var success = new ToolTipConfig
{
    Title = "Success",
    Text = "Changes saved",
    ImagePath = "icons/check-circle.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Success,
    Style = ToolTipStyle.Alert,
    Animation = ToolTipAnimation.Scale
};
```

### Help Tooltip
```csharp
var help = new ToolTipConfig
{
    Text = "Click to view more details",
    ImagePath = "icons/help-circle.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Info,
    Style = ToolTipStyle.Standard,
    Duration = 0  // Stay until manually closed
};
```

### Premium Feature Badge
```csharp
var premium = new ToolTipConfig
{
    Title = "Premium Feature",
    Text = "Upgrade to access advanced analytics",
    ImagePath = "icons/star.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Premium,
    Closable = true
};
```
