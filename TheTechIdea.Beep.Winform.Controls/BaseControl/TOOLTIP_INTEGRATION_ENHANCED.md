# BaseControl ToolTip Integration - Enhanced

## Overview

The ToolTip system has been fully integrated into `BaseControl`, making it available to all controls that inherit from `BaseControl`. The integration includes:

- ✅ Full theme support via `ApplyTheme()`
- ✅ ControlStyle synchronization
- ✅ Rich tooltip properties
- ✅ Notification methods
- ✅ Automatic cleanup

---

## Features

### 1. **Theme Integration**

Tooltips automatically use colors from `ApplyTheme()`:

```csharp
// Tooltips use _currentTheme from BaseControl
myControl.ApplyTheme(theme);
// Tooltip colors update automatically
```

- Uses `ToolTipThemeHelpers` for color management
- Respects `TooltipUseThemeColors` property
- Updates when theme changes

### 2. **ControlStyle Integration**

Tooltips can match the control's visual style:

```csharp
myControl.ControlStyle = BeepControlStyle.Material3;
// Tooltip style updates automatically if TooltipUseControlStyle = true
```

- Synchronized with `ControlStyle` property
- Updates when `ControlStyle` changes
- Can be disabled with `TooltipUseControlStyle = false`

### 3. **Rich Properties**

All tooltip features are exposed as properties:

```csharp
myControl.TooltipText = "Helpful tooltip";
myControl.TooltipTitle = "Title";
myControl.TooltipType = ToolTipType.Info;
myControl.TooltipAnimation = ToolTipAnimation.Slide;
myControl.TooltipShowArrow = true;
myControl.TooltipShowShadow = true;
myControl.TooltipFollowCursor = false;
myControl.TooltipShowDelay = 500;
myControl.TooltipClosable = false;
myControl.TooltipMaxSize = new Size(300, 200);
myControl.TooltipFont = new Font("Arial", 10);
```

### 4. **Notification Methods**

Quick notification methods for common scenarios:

```csharp
myControl.ShowSuccess("Operation completed!");
myControl.ShowError("Something went wrong!");
myControl.ShowWarning("Please check this");
myControl.ShowInfo("Information message");
```

### 5. **Automatic Cleanup**

Tooltips are automatically cleaned up when:
- Control is disposed
- `EnableTooltip` is set to `false`
- `TooltipText` is cleared

---

## Properties

### Basic Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TooltipText` | `string` | `""` | Tooltip text content |
| `TooltipTitle` | `string` | `""` | Tooltip title/header |
| `TooltipType` | `ToolTipType` | `Default` | Semantic type (Success, Error, Warning, Info, etc.) |
| `TooltipIconPath` | `string` | `""` | Path to icon image |
| `EnableTooltip` | `bool` | `true` | Enable/disable tooltips |

### Behavior Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TooltipDuration` | `int` | `3000` | Display duration in milliseconds (0 = indefinite) |
| `TooltipPlacement` | `ToolTipPlacement` | `Auto` | Preferred placement relative to control |
| `TooltipShowDelay` | `int` | `500` | Delay before showing (milliseconds) |
| `TooltipFollowCursor` | `bool` | `false` | Follow mouse cursor |
| `TooltipClosable` | `bool` | `false` | Allow user to close manually |

### Visual Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TooltipAnimation` | `ToolTipAnimation` | `Fade` | Animation style |
| `TooltipShowArrow` | `bool` | `true` | Show arrow pointing to control |
| `TooltipShowShadow` | `bool` | `true` | Enable shadow effect |
| `TooltipMaxSize` | `Size?` | `null` | Maximum size (null = no limit) |
| `TooltipFont` | `Font` | `null` | Custom font (null = default) |

### Integration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TooltipUseControlStyle` | `bool` | `true` | Use same ControlStyle as control |
| `TooltipUseThemeColors` | `bool` | `true` | Use theme colors from ApplyTheme() |

---

## Usage Examples

### Basic Tooltip

```csharp
var button = new BeepButton();
button.TooltipText = "Click to save";
button.TooltipType = ToolTipType.Info;
```

### Rich Tooltip

```csharp
var button = new BeepButton();
button.TooltipText = "Save your changes";
button.TooltipTitle = "Save Button";
button.TooltipType = ToolTipType.Success;
button.TooltipIconPath = "save-icon.svg";
button.TooltipShowArrow = true;
button.TooltipShowShadow = true;
```

### Notification

```csharp
var button = new BeepButton();
button.Click += (s, e) => {
    if (SaveData())
        button.ShowSuccess("Data saved successfully!");
    else
        button.ShowError("Failed to save data!");
};
```

### Custom Styling

```csharp
var control = new BeepControl();
control.ControlStyle = BeepControlStyle.Material3;
control.TooltipUseControlStyle = true; // Tooltip matches control style
control.TooltipText = "Styled tooltip";
```

### Theme Integration

```csharp
var control = new BeepControl();
control.ApplyTheme(darkTheme);
control.TooltipUseThemeColors = true; // Uses theme colors
control.TooltipText = "Themed tooltip";
```

---

## Integration Points

### 1. ApplyTheme() Integration

When `ApplyTheme()` is called, tooltips are automatically updated:

```csharp
public virtual void ApplyTheme()
{
    // ... theme application ...
    
    // Update tooltip with new theme colors
    UpdateTooltipTheme();
}
```

### 2. ControlStyle Integration

When `ControlStyle` changes, tooltips are updated:

```csharp
public BeepControlStyle ControlStyle
{
    set
    {
        // ... style change ...
        
        // Update tooltip if it uses ControlStyle
        UpdateTooltipTheme();
    }
}
```

### 3. Cleanup Integration

Tooltips are cleaned up in `Dispose()`:

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Cleanup ToolTipManager registration
        CleanupTooltip();
    }
}
```

---

## Internal Methods

### UpdateTooltip()

Updates the tooltip configuration and registers with `ToolTipManager`:
- Uses `_currentTheme` from `BaseControl`
- Applies theme colors via `ToolTipThemeHelpers`
- Prevents re-entrancy with `_isUpdatingTooltip` flag

### UpdateTooltipTheme()

Called when theme or style changes:
- Updates tooltip if enabled and text is set
- Used by `ApplyTheme()` and `ControlStyle` setter

### GetTooltipConfig()

Protected virtual method that can be overridden by derived controls:
- Returns `ToolTipConfig` with all settings
- Applies theme colors
- Can be customized in derived classes

---

## Benefits

1. **Automatic Theme Support**: Tooltips automatically use theme colors
2. **Style Synchronization**: Tooltips match control style automatically
3. **Rich Features**: All ToolTip features available via properties
4. **Easy to Use**: Simple properties, no complex setup
5. **Automatic Cleanup**: No manual disposal needed
6. **Re-entrancy Safe**: Prevents infinite loops during updates

---

## Notes

- Tooltips are managed by `ToolTipManager` singleton
- Theme colors are applied via `ToolTipThemeHelpers`
- All properties trigger automatic updates
- Cleanup is automatic on disposal
- Re-entrancy is prevented with flags

---

## See Also

- `ToolTipManager` - Central tooltip management
- `ToolTipThemeHelpers` - Theme color management
- `CustomToolTip` - Visual tooltip form
- `ToolTipConfig` - Configuration model

