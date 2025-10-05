# ToolTip Quick Reference

## 🎯 Most Common Usage

```csharp
// Simple tooltip with icon
await ToolTipManager.ShowTooltipAsync(new ToolTipConfig
{
    Text = "Connection error",
    Title = "Error",
    ImagePath = "icons/alert.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Error,
    Style = ToolTipStyle.Alert
});
```

## 📋 Property Cheat Sheet

### Image Properties (Priority Order)
```csharp
ImagePath = "path/to/image.svg"  // ← Use this (best for SVG with theme)
IconPath = "path/to/image.png"   // ← Fallback
Icon = myImageObject              // ← Last resort
```

### Style + Theme Combinations

| Style | Theme | Result |
|-------|-------|--------|
| `Standard` | `Primary` | Blue clean tooltip |
| `Premium` | `Success` | Green gradient with badge |
| `Alert` | `Error` | Red left bar with X icon |
| `Alert` | `Warning` | Orange left bar with ! icon |
| `Alert` | `Success` | Green left bar with ✓ icon |

### Quick Styles Reference

```csharp
// Clean modern
Style = ToolTipStyle.Standard

// Fancy with badge
Style = ToolTipStyle.Premium

// Status with colored bar
Style = ToolTipStyle.Alert
```

### Theme Colors (from IBeepTheme)

```csharp
ToolTipTheme.Primary  → theme.AccentColor
ToolTipTheme.Success  → theme.SuccessColor
ToolTipTheme.Error    → theme.ErrorColor
ToolTipTheme.Warning  → theme.WarningColor
ToolTipTheme.Info     → theme.InfoColor
ToolTipTheme.Light    → theme.BackColor
ToolTipTheme.Dark     → theme.SecondaryBackColor
```

## 🚀 Common Patterns

### Error Message
```csharp
new ToolTipConfig
{
    Title = "Error",
    Text = "Operation failed",
    ImagePath = "icons/x-circle.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Error,
    Style = ToolTipStyle.Alert
}
```

### Success Message
```csharp
new ToolTipConfig
{
    Title = "Success",
    Text = "Saved successfully",
    ImagePath = "icons/check-circle.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Success,
    Style = ToolTipStyle.Alert
}
```

### Info Tooltip
```csharp
new ToolTipConfig
{
    Text = "Click for details",
    ImagePath = "icons/info.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Info,
    Style = ToolTipStyle.Standard
}
```

### Premium Feature
```csharp
new ToolTipConfig
{
    Title = "Premium",
    Text = "Upgrade to unlock",
    ImagePath = "icons/star.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Premium
}
```

## 💡 Quick Tips

1. **Always use ImagePath for SVG** - Gets theme colors automatically
2. **Set ApplyThemeOnImage = true** - Makes icons match your theme
3. **Use Alert style for status** - Colored bar makes it obvious
4. **Use Premium for highlights** - Gradient catches attention
5. **Keep Duration 2-5 seconds** - Not too fast, not too slow
6. **Use Auto placement** - Smart positioning handles edge cases

## 🎨 Animation Shortcuts

```csharp
Animation = ToolTipAnimation.Fade   // Smooth fade (default)
Animation = ToolTipAnimation.Scale  // Grow from center
Animation = ToolTipAnimation.Slide  // Slide from edge
Animation = ToolTipAnimation.Bounce // Bounce effect
Animation = ToolTipAnimation.None   // Instant show
```

## 📍 Position Shortcuts

```csharp
Placement = ToolTipPlacement.Auto   // Smart (default)
Placement = ToolTipPlacement.Top    // Above
Placement = ToolTipPlacement.Bottom // Below
Placement = ToolTipPlacement.Left   // Left side
Placement = ToolTipPlacement.Right  // Right side
```

## 🔧 Control Attachment

```csharp
// Attach to button
ToolTipManager.SetTooltip(myButton, "Click me", new ToolTipConfig
{
    ImagePath = "icons/pointer.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Info
});

// Remove tooltip
ToolTipManager.RemoveTooltip(myButton);
```

## ⚙️ Advanced Options

```csharp
var config = new ToolTipConfig
{
    // Required
    Text = "Tooltip text",
    
    // Recommended
    ImagePath = "icons/icon.svg",
    ApplyThemeOnImage = true,
    Theme = ToolTipTheme.Primary,
    Style = ToolTipStyle.Standard,
    
    // Optional
    Title = "Title",
    ShowArrow = true,
    ShowShadow = true,
    Closable = false,
    Duration = 3000,
    Animation = ToolTipAnimation.Fade,
    AnimationDuration = 200,
    Placement = ToolTipPlacement.Auto,
    Offset = 8,
    MaxSize = new Size(300, 200),
    MaxImageSize = new Size(24, 24),
    FollowCursor = false,
    
    // Callbacks
    OnShow = (key) => Debug.WriteLine($"Shown: {key}"),
    OnClose = (key) => Debug.WriteLine($"Closed: {key}")
};
```

## 🐛 Troubleshooting

**Image not showing?**
- Check ImagePath is correct
- Verify file exists
- Try Icon property as test

**Wrong colors?**
- Set ApplyThemeOnImage = true
- Check theme is set
- Use ImagePath (not IconPath) for SVG

**Positioning issues?**
- Use Placement = Auto
- Check screen bounds
- Set specific Placement if needed

**Animation not working?**
- Check Animation != None
- Set AnimationDuration (default 200ms)
- Verify Duration > 0

## 📚 Documentation

- `ToolTip_Refactoring_Summary.md` - Complete overview
- `ToolTip_Usage_Guide.md` - Detailed examples
- `ToolTip_Architecture_Diagram.md` - Architecture diagrams
