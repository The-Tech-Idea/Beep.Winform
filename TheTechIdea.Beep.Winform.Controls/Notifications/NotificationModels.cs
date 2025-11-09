using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Type of notification following best practices from SetProduct guide
    /// </summary>
    public enum NotificationType
    {
        /// <summary>Informational message (blue/neutral)</summary>
        Info,
        /// <summary>Success confirmation (green)</summary>
        Success,
        /// <summary>Warning message (yellow/orange)</summary>
        Warning,
        /// <summary>Error message (red)</summary>
        Error,
        /// <summary>System/debug message (gray)</summary>
        System,
        /// <summary>Custom notification with custom colors</summary>
        Custom
    }

    /// <summary>
    /// Priority level for notifications - affects display order and persistence
    /// </summary>
    public enum NotificationPriority
    {
        /// <summary>Low priority - auto-dismiss quickly</summary>
        Low,
        /// <summary>Normal priority - standard display time</summary>
        Normal,
        /// <summary>High priority - longer display time, more prominent</summary>
        High,
        /// <summary>Critical - requires user interaction to dismiss</summary>
        Critical
    }

    /// <summary>
    /// Position where notifications appear on screen
    /// </summary>
    public enum NotificationPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        Center
    }

    /// <summary>
    /// Animation style for notification appearance/dismissal
    /// </summary>
    public enum NotificationAnimation
    {
        None,
        Slide,
        Fade,
        SlideAndFade,
        Bounce,
        Scale
    }

    /// <summary>
    /// Layout style for notification content
    /// </summary>
    public enum NotificationLayout
    {
        /// <summary>Icon on left, text on right, action buttons below</summary>
        Standard,
        /// <summary>Icon and text inline, compact</summary>
        Compact,
        /// <summary>Large icon, prominent text</summary>
        Prominent,
        /// <summary>Banner style across top/bottom</summary>
        Banner,
        /// <summary>Toast style - minimal, auto-dismiss</summary>
        Toast
    }

    /// <summary>
    /// Data model for a notification
    /// </summary>
    public class NotificationData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; } = NotificationType.Info;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        public NotificationLayout Layout { get; set; } = NotificationLayout.Standard;
        
        /// <summary>Group key for stacking similar notifications</summary>
        public string? GroupKey { get; set; }
        
        /// <summary>Source identifier for grouping (e.g., "EmailClient", "FileSystem")</summary>
        public string? Source { get; set; }
        
        /// <summary>Icon path (if null, uses default icon for type)</summary>
        public string IconPath { get; set; }
        
        /// <summary>Custom icon color tint</summary>
        public Color? IconTint { get; set; }
        
        /// <summary>Embedded image path (displayed in notification body)</summary>
        public string? ImagePath { get; set; }
        
        /// <summary>Embedded image (displayed in notification body)</summary>
        public Image? EmbeddedImage { get; set; }
        
        /// <summary>Enable HTML-like formatting in message (bold, italic, colors)</summary>
        public bool EnableRichText { get; set; } = false;
        
        /// <summary>Progress value (0-100) for progress indicator</summary>
        public int? ProgressValue { get; set; }
        
        /// <summary>Progress text label</summary>
        public string? ProgressText { get; set; }
        
        /// <summary>Custom background color (overrides type-based color)</summary>
        public Color? CustomBackColor { get; set; }
        
        /// <summary>Custom foreground color</summary>
        public Color? CustomForeColor { get; set; }
        
        /// <summary>Duration in milliseconds (0 = no auto-dismiss)</summary>
        public int Duration { get; set; } = 5000;
        
        /// <summary>Show close button</summary>
        public bool ShowCloseButton { get; set; } = true;
        
        /// <summary>Show progress bar for auto-dismiss countdown</summary>
        public bool ShowProgressBar { get; set; } = true;
        
        /// <summary>Allow user to interact/pause auto-dismiss on hover</summary>
        public bool PauseOnHover { get; set; } = true;
        
        /// <summary>Play sound when notification appears</summary>
        public bool PlaySound { get; set; } = false;
        
        /// <summary>Custom sound file path (WAV format). If null, uses default system sound.</summary>
        public string? CustomSoundPath { get; set; }
        
        /// <summary>Action buttons</summary>
        public NotificationAction[] Actions { get; set; }
        
        /// <summary>Additional data for custom handling</summary>
        public object Tag { get; set; }
        
        /// <summary>Timestamp when notification was created</summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Get default duration based on priority
        /// </summary>
        public static int GetDefaultDuration(NotificationPriority priority)
        {
            return priority switch
            {
                NotificationPriority.Low => 3000,      // 3 seconds
                NotificationPriority.Normal => 5000,   // 5 seconds
                NotificationPriority.High => 8000,     // 8 seconds
                NotificationPriority.Critical => 0,    // No auto-dismiss
                _ => 5000
            };
        }

        /// <summary>
        /// Get default icon for notification type
        /// </summary>
        public static string GetDefaultIconForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "checkmark-circle",
                NotificationType.Warning => "warning-triangle",
                NotificationType.Error => "error-circle",
                NotificationType.Info => "info-circle",
                NotificationType.System => "settings-gear",
                _ => "bell"
            };
        }
    }

    /// <summary>
    /// Action button for notification
    /// </summary>
    public class NotificationAction
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsPrimary { get; set; }
        public Action<NotificationData> OnClick { get; set; }
        public Color? CustomColor { get; set; }
    }

    /// <summary>
    /// Event arguments for notification events
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        public NotificationData Notification { get; set; }
        public NotificationAction Action { get; set; }
        public bool Cancel { get; set; }
    }
}
