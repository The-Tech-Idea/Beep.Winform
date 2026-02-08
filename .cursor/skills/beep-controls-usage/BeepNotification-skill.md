# BeepNotification Skill

## Overview
`BeepNotification` and `BeepNotificationManager` provide a comprehensive notification system with 6 types, 4 priorities, 5 layouts, animations, sounds, grouping, and history.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Notifications;
```

## NotificationType
```csharp
public enum NotificationType
{
    Info,      // Blue - Informational
    Success,   // Green - Confirmation
    Warning,   // Yellow/Orange - Warning
    Error,     // Red - Error
    System,    // Gray - System/debug
    Custom     // Custom colors
}
```

## NotificationPriority
```csharp
public enum NotificationPriority
{
    Low,       // 3 seconds auto-dismiss
    Normal,    // 5 seconds (default)
    High,      // 8 seconds
    Critical   // No auto-dismiss
}
```

## NotificationPosition
```csharp
public enum NotificationPosition
{
    TopLeft, TopCenter, TopRight,
    BottomLeft, BottomCenter, BottomRight,
    Center
}
```

## NotificationLayout
```csharp
public enum NotificationLayout
{
    Standard,   // Icon left, text right, buttons below
    Compact,    // Inline icon and text
    Prominent,  // Large icon, prominent text
    Banner,     // Full-width banner style
    Toast       // Minimal, auto-dismiss
}
```

## NotificationAnimation
```csharp
public enum NotificationAnimation
{
    None, Slide, Fade, SlideAndFade, Bounce, Scale
}
```

## NotificationData Model
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier |
| `Title` | `string` | Notification title |
| `Message` | `string` | Notification message |
| `Type` | `NotificationType` | Type (Info, Success, etc.) |
| `Priority` | `NotificationPriority` | Priority level |
| `Layout` | `NotificationLayout` | Visual layout |
| `GroupKey` | `string` | Stacking group key |
| `Source` | `string` | Source identifier |
| `IconPath` | `string` | Custom icon path |
| `ImagePath` | `string` | Embedded image |
| `ProgressValue` | `int?` | Progress 0-100 |
| `Duration` | `int` | Auto-dismiss (ms, 0=never) |
| `ShowCloseButton` | `bool` | Show close button |
| `ShowProgressBar` | `bool` | Countdown progress |
| `PauseOnHover` | `bool` | Pause on mouse hover |
| `PlaySound` | `bool` | Play sound |
| `CustomSoundPath` | `string` | Custom WAV path |
| `Actions` | `NotificationAction[]` | Action buttons |

## NotificationAction
```csharp
public class NotificationAction
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsPrimary { get; set; }
    public Action<NotificationData> OnClick { get; set; }
}
```

## Usage Examples

### Using Manager (Recommended)
```csharp
// Get manager instance
var manager = BeepNotificationManager.Instance;

// Show simple notification
manager.ShowInfo("Title", "Information message");
manager.ShowSuccess("Saved", "File saved successfully");
manager.ShowWarning("Warning", "Disk space low");
manager.ShowError("Error", "Connection failed");

// With custom options
manager.Show(new NotificationData
{
    Title = "Custom",
    Message = "Custom notification",
    Type = NotificationType.Info,
    Priority = NotificationPriority.High,
    Duration = 10000,
    Layout = NotificationLayout.Prominent,
    Actions = new[]
    {
        new NotificationAction { Text = "Retry", IsPrimary = true, OnClick = d => Retry() },
        new NotificationAction { Text = "Cancel", OnClick = d => Cancel() }
    }
});
```

### With Progress
```csharp
var notification = manager.Show(new NotificationData
{
    Title = "Uploading",
    Message = "Uploading file...",
    Type = NotificationType.Info,
    ProgressValue = 0,
    Duration = 0  // No auto-dismiss
});

// Update progress
notification.ProgressValue = 50;
notification.Message = "Uploading 50%...";
```

### Configure Position
```csharp
manager.Position = NotificationPosition.BottomRight;
manager.Animation = NotificationAnimation.SlideAndFade;
```

## Related Components
- `BeepNotificationGroup` - Grouped notifications
- `BeepNotificationHistory` - History management
- `BeepNotificationAnimator` - Animations
- `BeepNotificationSound` - Sound playback

## Related Controls
- `BeepCard` (AlertCard) - For inline alerts
- `BeepDialogBox` - For modal dialogs
