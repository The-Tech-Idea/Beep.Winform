# BeepNotification - Toast & Notification System

Modern, commercial-grade notification/toast control system for Beep WinForms applications, following industry best practices from [SetProduct.com's Notification UI Design Guide](https://www.setproduct.com/blog/notifications-ui-design).

## ✨ Features

### Core Features
- ✅ **Professional UI** - Modern design with Material3, iOS15, Fluent2 styles
- ✅ **Auto-dismiss** - Configurable timeouts with pause on hover
- ✅ **Action Buttons** - Interactive buttons with callbacks
- ✅ **Progress Indicators** - Visual countdown and custom progress bars
- ✅ **Type-based Colors** - Success (green), Error (red), Warning (orange), Info (blue)
- ✅ **Animations** - Smooth slide, fade, bounce, scale effects (300ms @ 60 FPS)

### Phase 1 - Essential Features
- ✅ **Sound System** - SystemSounds + custom WAV files with global enable/disable
- ✅ **Notification History** - Searchable history panel with filtering
- ✅ **Keyboard Shortcuts** - Esc to dismiss, Enter for primary action, 1/2/3 for action buttons
- ✅ **Batch Dismiss** - DismissAll, DismissAllByType, DismissAllByPriority

### Phase 2 - Professional Features
- ✅ **Grouping/Stacking** - Auto-group similar notifications with expand/collapse
- ✅ **Rich Content** - Embedded images, formatted text, progress indicators
- ✅ **Do Not Disturb Mode** - Suppress non-critical notifications
- ✅ **Smart Positioning** - Multi-monitor support, active screen detection

## Architecture

The notification system consists of four main components:

1. **BeepNotification** - Visual control that displays a single notification (BaseControl-based)
2. **BeepNotificationManager** - Singleton manager for showing, queuing, and managing notifications
3. **BeepNotificationAnimator** - Animation engine for smooth show/hide transitions
4. **NotificationModels** - Data structures and enums for notification configuration

## BeepNotificationManager - Toast Manager

### Overview

`BeepNotificationManager` is a singleton toast/notification manager similar to commercial solutions like DevExpress, Telerik, and Syncfusion. It handles:
- **Automatic positioning** - Smart positioning based on screen and preferred location
- **Queue management** - Queues notifications when max visible count is reached
- **Priority handling** - Critical notifications shown first
- **Auto-dismiss** - Configurable timeouts with pause on hover
- **Multi-screen support** - Target specific screens
- **Event system** - Track notification lifecycle

### Quick Start

```csharp
// Simple notification
BeepNotificationManager.Instance.Show("Success", "File saved successfully", NotificationType.Success);

// Or use type-specific helpers
BeepNotificationManager.Instance.ShowSuccess("Done", "Operation completed");
BeepNotificationManager.Instance.ShowError("Failed", "Could not connect to server");
BeepNotificationManager.Instance.ShowWarning("Warning", "Unsaved changes");
BeepNotificationManager.Instance.ShowInfo("Info", "New update available");
```

### Configuration

```csharp
var manager = BeepNotificationManager.Instance;

// Position notifications
manager.DefaultPosition = NotificationPosition.BottomRight;

// Limit visible notifications
manager.MaxVisibleNotifications = 5;

// Spacing and margins
manager.NotificationSpacing = 10;    // Between notifications
manager.ScreenMargin = 20;           // From screen edge

// Target specific screen
manager.TargetScreen = Screen.AllScreens[1];

// Animation style
manager.DefaultAnimation = NotificationAnimation.SlideAndFade;
```

### Advanced Usage

#### With Action Buttons

```csharp
BeepNotificationManager.Instance.ShowWithActions(
    "Update Available",
    "A new version is ready to install.",
    NotificationType.Info,
    new NotificationAction
    {
        Id = "install",
        Text = "Install Now",
        IsPrimary = true,
        OnClick = (data) => InstallUpdate()
    },
    new NotificationAction
    {
        Id = "later",
        Text = "Later",
        IsPrimary = false,
        OnClick = (data) => ScheduleLater()
    }
);
```

#### Custom Notification Data

```csharp
var notification = new NotificationData
{
    Title = "Payment Received",
    Message = "$125.00 from John Doe",
    Type = NotificationType.Success,
    Priority = NotificationPriority.High,
    Duration = 8000,
    IconPath = "payment-icon.svg",
    CustomBackColor = Color.FromArgb(240, 255, 240),
    ShowProgressBar = true,
    PauseOnHover = true,
    Actions = new[]
    {
        new NotificationAction
        {
            Text = "View Details",
            OnClick = (data) => ShowPaymentDetails()
        }
    }
};

BeepNotificationManager.Instance.Show(notification);
```

#### Event Handling

```csharp
var manager = BeepNotificationManager.Instance;

manager.NotificationShown += (s, e) =>
{
    Console.WriteLine($"Shown: {e.Notification.Title}");
};

manager.NotificationDismissed += (s, e) =>
{
    Console.WriteLine($"Dismissed: {e.Notification.Title}");
};

manager.NotificationActionClicked += (s, e) =>
{
    Console.WriteLine($"Action '{e.Action.Text}' clicked");
};
```

### Management Methods

```csharp
var manager = BeepNotificationManager.Instance;

// Check status
int active = manager.ActiveCount;    // Currently visible
int queued = manager.QueuedCount;    // Waiting in queue

// Dismiss specific notification
manager.Dismiss(notification);

// Dismiss all active
manager.DismissAll();

// Clear queue
manager.ClearQueue();

// Clear everything
manager.Clear();
```

### Positioning Options

```csharp
public enum NotificationPosition
{
    TopLeft,        // Stack downward from top-left
    TopCenter,      // Stack downward from top-center
    TopRight,       // Stack downward from top-right
    BottomLeft,     // Stack upward from bottom-left
    BottomCenter,   // Stack upward from bottom-center
    BottomRight,    // Stack upward from bottom-right (default)
    Center          // Center of screen
}
```

### Real-World Examples

#### Save Confirmation

```csharp
BeepNotificationManager.Instance.ShowSuccess(
    "Saved",
    "Document saved successfully",
    3000
);
```

#### Undo Delete

```csharp
BeepNotificationManager.Instance.ShowWithActions(
    "Item Deleted",
    "The item has been moved to trash.",
    NotificationType.Info,
    new NotificationAction
    {
        Text = "Undo",
        IsPrimary = true,
        OnClick = (data) => RestoreItem()
    }
);
```

#### Connection Error

```csharp
BeepNotificationManager.Instance.ShowError(
    "Connection Failed",
    "Unable to connect to server. Please check your internet connection.",
    0  // No auto-dismiss for errors
);
```

#### Progress with Auto-Update

```csharp
var uploadData = new NotificationData
{
    Title = "Uploading File",
    Message = "Please wait...",
    Type = NotificationType.Info,
    Duration = 0,
    ShowProgressBar = true,
    ShowCloseButton = false
};

BeepNotificationManager.Instance.Show(uploadData);

// Update progress externally
// (Progress bar updates automatically based on duration)
```

#### Critical System Alert

```csharp
var criticalAlert = new NotificationData
{
    Title = "System Error",
    Message = "Critical system failure detected. Please restart the application.",
    Type = NotificationType.Error,
    Priority = NotificationPriority.Critical,
    Duration = 0,  // Must be manually dismissed
    ShowCloseButton = true,
    Actions = new[]
    {
        new NotificationAction
        {
            Text = "Restart Now",
            IsPrimary = true,
            OnClick = (data) => Application.Restart()
        },
        new NotificationAction
        {
            Text = "Save & Exit",
            OnClick = (data) => SaveAndExit()
        }
    }
};

BeepNotificationManager.Instance.Show(criticalAlert);
```

### Best Practices

#### ✅ Do

- Use appropriate notification types (Success, Error, Warning, Info)
- Set priority based on importance (Critical for errors, High for warnings)
- Keep messages concise and actionable
- Use auto-dismiss for low-importance notifications
- Disable auto-dismiss for errors and actions
- Limit visible notifications (3-5 recommended)
- Use action buttons for user decisions

#### ❌ Don't

- Don't spam with too many notifications
- Don't use long messages (keep under 2 lines)
- Don't auto-dismiss critical errors
- Don't use more than 2 action buttons
- Don't show notifications for every minor event
- Don't use inappropriate types (e.g., Success for warnings)

### Integration with Forms

```csharp
public partial class MainForm : Form
{
    private BeepNotificationManager _notificationManager;

    public MainForm()
    {
        InitializeComponent();
        
        _notificationManager = BeepNotificationManager.Instance;
        _notificationManager.DefaultPosition = NotificationPosition.BottomRight;
        _notificationManager.MaxVisibleNotifications = 3;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            // Save logic
            SaveDocument();
            
            _notificationManager.ShowSuccess("Saved", "Document saved successfully");
        }
        catch (Exception ex)
        {
            _notificationManager.ShowError("Save Failed", ex.Message);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _notificationManager.DismissAll();
        base.OnFormClosing(e);
    }
}
```

### Multi-Screen Support

```csharp
// Show on secondary monitor
var manager = BeepNotificationManager.Instance;
manager.TargetScreen = Screen.AllScreens[1];
manager.Show("Info", "Displayed on secondary screen");

// Show on screen containing mouse cursor
manager.TargetScreen = Screen.FromPoint(Cursor.Position);
manager.Show("Info", "Displayed on current screen");

// Show on primary screen
manager.TargetScreen = Screen.PrimaryScreen;
```

### Performance Considerations

- **Queue management**: Notifications are queued when max count reached
- **Auto-cleanup**: Dismissed notifications are properly disposed
- **Memory efficient**: Only active notifications consume resources
- **Thread-safe**: Manager handles concurrent calls safely
- **Smart positioning**: Efficient repositioning algorithm

## BeepNotification Control

### Overview

`BeepNotification` is a BaseControl-derived control that displays toast/notification messages with:
- Icon support (default icons per type or custom icons)
- Title and message text with word-wrapping
- Action buttons (primary and secondary)
- Auto-dismiss with countdown progress bar
- Pause on hover support
- Close button
- Multiple layout styles
- Full theme integration via BeepStyling

### Features

✅ **Inherits from BaseControl** - Full BaseControl painter system support  
✅ **Type-based styling** - Auto colors/icons for Info, Success, Warning, Error, System  
✅ **Auto-dismiss** - Configurable duration with pause on hover  
✅ **Progress indicator** - Visual countdown bar  
✅ **Action buttons** - Primary and secondary action support  
✅ **Multiple layouts** - Standard, Compact, Prominent, Banner, Toast  
✅ **Custom colors** - Override type-based colors  
✅ **Icon support** - SVG icons via StyledImagePainter  
✅ **Responsive** - Auto-adjusts height based on content  

### Notification Types

```csharp
public enum NotificationType
{
    Info,      // Blue - informational messages
    Success,   // Green - success confirmations
    Warning,   // Yellow/Orange - warnings
    Error,     // Red - error messages
    System,    // Gray - system/debug messages
    Custom     // Custom colors
}
```

### Layout Styles

```csharp
public enum NotificationLayout
{
    Standard,   // Icon left, text right, actions below
    Compact,    // Inline icon and text, minimal
    Prominent,  // Large icon, prominent text
    Banner,     // Banner style across top/bottom
    Toast       // Minimal toast style
}
```

### Priority Levels

```csharp
public enum NotificationPriority
{
    Low,       // 3 seconds auto-dismiss
    Normal,    // 5 seconds auto-dismiss
    High,      // 8 seconds auto-dismiss
    Critical   // No auto-dismiss, requires user action
}
```

### Basic Usage

#### Simple Notification

```csharp
var notification = new BeepNotification
{
    Title = "Success!",
    Message = "Your changes have been saved.",
    NotificationType = NotificationType.Success
};

notification.Show();
```

#### With Data Model

```csharp
var data = new NotificationData
{
    Title = "New Message",
    Message = "You have a new message from John Doe.",
    Type = NotificationType.Info,
    Priority = NotificationPriority.High,
    Duration = 8000,
    ShowProgressBar = true,
    PauseOnHover = true
};

var notification = new BeepNotification
{
    NotificationData = data
};

notification.Show();
```

#### With Action Buttons

```csharp
var data = new NotificationData
{
    Title = "Update Available",
    Message = "A new version is available. Would you like to install it now?",
    Type = NotificationType.Info,
    Priority = NotificationPriority.Normal,
    Duration = 0, // No auto-dismiss
    Actions = new[]
    {
        new NotificationAction
        {
            Id = "install",
            Text = "Install Now",
            IsPrimary = true,
            OnClick = (notif) => InstallUpdate()
        },
        new NotificationAction
        {
            Id = "later",
            Text = "Later",
            IsPrimary = false,
            OnClick = (notif) => DismissUpdate()
        }
    }
};

var notification = new BeepNotification { NotificationData = data };
notification.Show();
```

#### Custom Colors

```csharp
var data = new NotificationData
{
    Title = "Custom Notification",
    Message = "This uses custom colors.",
    Type = NotificationType.Custom,
    CustomBackColor = Color.FromArgb(255, 240, 245),
    CustomForeColor = Color.FromArgb(136, 14, 79),
    IconPath = "custom-icon.svg",
    IconTint = Color.FromArgb(233, 30, 99)
};

var notification = new BeepNotification { NotificationData = data };
notification.Show();
```

### Properties

#### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | `null` | Notification title text |
| `Message` | `string` | `null` | Notification message text |
| `NotificationType` | `NotificationType` | `Info` | Type of notification |
| `LayoutStyle` | `NotificationLayout` | `Standard` | Layout style |
| `ShowCloseButton` | `bool` | `true` | Show close button |
| `ShowProgressBar` | `bool` | `true` | Show progress bar |

#### Behavior

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Duration` | `int` | `5000` | Auto-dismiss duration (ms), 0 = no auto-dismiss |
| `NotificationData` | `NotificationData` | `null` | Complete notification data model |

### Events

```csharp
// Raised when notification is dismissed (auto or manual)
public event EventHandler<NotificationEventArgs> NotificationDismissed;

// Raised when an action button is clicked
public event EventHandler<NotificationEventArgs> ActionClicked;

// Raised when notification body is clicked (not action button)
public event EventHandler<NotificationEventArgs> NotificationClicked;
```

#### Event Usage

```csharp
notification.NotificationDismissed += (s, e) => 
{
    Console.WriteLine($"Notification '{e.Notification.Title}' was dismissed");
};

notification.ActionClicked += (s, e) => 
{
    Console.WriteLine($"Action '{e.Action.Text}' clicked");
};

notification.NotificationClicked += (s, e) => 
{
    Console.WriteLine($"Notification clicked");
};
```

### Methods

```csharp
// Show notification and start auto-dismiss timer
public void Show();

// Manually dismiss notification
public void Dismiss();

// Pause auto-dismiss timer
public void Pause();

// Resume auto-dismiss timer
public void Resume();
```

### Default Colors by Type

#### Success (Green)
- Background: `#F0FFF4` (Light green)
- Text: `#166534` (Dark green)
- Border: `#86EFAC` (Green)
- Icon: `#22C55E` (Green)

#### Warning (Yellow/Orange)
- Background: `#FEFCE8` (Light yellow)
- Text: `#713F12` (Dark brown)
- Border: `#FBBF24` (Yellow)
- Icon: `#F59E0B` (Orange)

#### Error (Red)
- Background: `#FEF2F2` (Light red)
- Text: `#7F1D1D` (Dark red)
- Border: `#FCA5A5` (Red)
- Icon: `#EF4444` (Red)

#### Info (Blue)
- Background: `#EFF6FF` (Light blue)
- Text: `#1E3A8A` (Dark blue)
- Border: `#93C5FD` (Blue)
- Icon: `#3B82F6` (Blue)

#### System (Gray)
- Background: `#F9FAFB` (Light gray)
- Text: `#374151` (Dark gray)
- Border: `#D1D5DB` (Gray)
- Icon: `#6B7280` (Gray)

### Default Icons by Type

- **Success**: `checkmark-circle`
- **Warning**: `warning-triangle`
- **Error**: `error-circle`
- **Info**: `info-circle`
- **System**: `settings-gear`
- **Custom**: `bell`

### Layout Breakdown

```
┌─────────────────────────────────────────┐
│  [Icon]  Title Text            [Close]  │
│          Message text that wraps        │
│          to multiple lines...           │
│                                         │
│          [Primary]  [Secondary]         │
│  ═════════════════════ (Progress)       │
└─────────────────────────────────────────┘
```

**Spacing:**
- Padding: 12px
- Icon size: 24px
- Close button: 20px
- Progress bar: 4px height
- Action button: 32px height

### Integration with BaseControl

BeepNotification inherits all BaseControl features:

```csharp
var notification = new BeepNotification
{
    // BaseControl properties
    PainterKind = BaseControlPainterKind.Card,
    IsRounded = true,
    BorderRadius = 8,
    ShowShadow = true,
    ShadowOffset = 4,
    
    // Notification properties
    Title = "Hello",
    Message = "World",
    NotificationType = NotificationType.Info
};
```

### Responsive Sizing

- **Minimum size**: 280 x 60
- **Maximum size**: 420 x 300
- **Default size**: 350 x 80
- **Auto-height**: Adjusts based on content

### Best Practices

#### 1. Use Appropriate Types
```csharp
// ✅ Good - Clear type based on action
NotificationType.Success // After save operation
NotificationType.Error   // After failed validation
NotificationType.Warning // Before destructive action
NotificationType.Info    // General information
```

#### 2. Set Appropriate Priorities
```csharp
// ✅ Good - Priority matches importance
Priority.Low      // "Settings saved" (3s)
Priority.Normal   // "Email sent" (5s)
Priority.High     // "Connection lost" (8s)
Priority.Critical // "Payment failed" (no auto-dismiss)
```

#### 3. Keep Messages Concise
```csharp
// ✅ Good
Title = "File Saved"
Message = "Document.docx saved successfully."

// ❌ Bad - Too verbose
Title = "The file has been saved to your computer"
Message = "Your document named Document.docx has been successfully saved..."
```

#### 4. Use Actions Wisely
```csharp
// ✅ Good - Clear, actionable
Actions = new[] {
    new NotificationAction { Text = "Undo", IsPrimary = true },
    new NotificationAction { Text = "Dismiss", IsPrimary = false }
}

// ❌ Bad - Too many actions
Actions = new[] {
    new NotificationAction { Text = "Undo" },
    new NotificationAction { Text = "Redo" },
    new NotificationAction { Text = "Save" },
    new NotificationAction { Text = "Cancel" }
}
```

#### 5. Respect Auto-Dismiss Guidelines
```csharp
// ✅ Good
Duration = 0 // Critical errors, require acknowledgment
Duration = 3000 // Low importance
Duration = 5000 // Normal
Duration = 8000 // High importance

// ❌ Bad
Duration = 30000 // Too long, annoying
Duration = 500   // Too short, can't read
```

### Theming

BeepNotification automatically integrates with BeepStyling and FormStyle:

```csharp
// Set global style
BeepStyling.SetControlStyle(BeepControlStyle.Modern);

// Notification will use Modern style colors
var notification = new BeepNotification
{
    Title = "Styled Notification",
    NotificationType = NotificationType.Success
};
```

### Performance Considerations

- **Timer optimization**: Timers stop when notification is dismissed
- **Image caching**: StyledImagePainter caches icons
- **Efficient redraws**: Only redraws when necessary
- **Proper disposal**: All resources cleaned up in Dispose

### Common Patterns

#### Undo Notification
```csharp
var data = new NotificationData
{
    Title = "Item Deleted",
    Message = "The item has been moved to trash.",
    Type = NotificationType.Info,
    Duration = 8000,
    Actions = new[]
    {
        new NotificationAction
        {
            Text = "Undo",
            IsPrimary = true,
            OnClick = (n) => RestoreItem()
        }
    }
};
```

#### Progress Notification
```csharp
var data = new NotificationData
{
    Title = "Uploading...",
    Message = "Please wait while we upload your file.",
    Type = NotificationType.Info,
    Duration = 0, // No auto-dismiss
    ShowProgressBar = true,
    ShowCloseButton = false
};

// Update progress externally
// notification.ProgressPercentage = 50;
```

#### Error with Retry
```csharp
var data = new NotificationData
{
    Title = "Connection Failed",
    Message = "Unable to connect to server.",
    Type = NotificationType.Error,
    Priority = NotificationPriority.High,
    Duration = 0,
    Actions = new[]
    {
        new NotificationAction
        {
            Text = "Retry",
            IsPrimary = true,
            OnClick = (n) => RetryConnection()
        },
        new NotificationAction
        {
            Text = "Settings",
            IsPrimary = false,
            OnClick = (n) => OpenSettings()
        }
    }
};
```

## Next Steps

1. **BeepNotificationManager** - Queue and manage multiple notifications
2. **BeepNotificationContainer** - Position and animate notifications on screen
3. **Sound support** - Play notification sounds
4. **Animation system** - Slide, fade, bounce animations

## References

- [SetProduct Notification Design Guide](https://www.setproduct.com/blog/notifications-ui-design)
- BaseControl Architecture: `BaseControl\Readme.md`
- BeepStyling System: `Styling\Readme.md`
