using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;  // SvgsUI, Svgs

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    // ─────────────────────────────────────────────────────────────────────────
    // Enums (active)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Type of notification.</summary>
    public enum NotificationType
    {
        Info, Success, Warning, Error, System, Custom
    }

    /// <summary>Priority level – affects display order and persistence.</summary>
    public enum NotificationPriority
    {
        Low, Normal, High, Critical
    }

    /// <summary>Screen position for notification stack.</summary>
    public enum NotificationPosition
    {
        TopLeft, TopCenter, TopRight,
        BottomLeft, BottomCenter, BottomRight,
        Center
    }

    /// <summary>Animation style.</summary>
    public enum NotificationAnimation
    {
        None, Slide, Fade, SlideAndFade, Bounce, Scale
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Enums (obsolete — kept for source compatibility; painters deleted 2026-07)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Legacy layout enum — retained for source-compat only. The painter system
    /// that consumed these values was deleted; rendering is now driven by
    /// child controls via standard docking. NotificationData.Layout is a
    /// no-op setter/getter kept so existing callers compile.
    /// </summary>
    [Obsolete("Painter system removed 2026-07. Notifications now use child controls; this enum is no-op.")]
    public enum NotificationLayout
    {
        Standard, Compact, Prominent, Banner, Toast,
        Elevated, Snackbar, InlineAlert, Callout, Chip,
        Timeline, MediaRich, ActionSheet, StatusBar, Overlay, FullWidth,
    }

    /// <summary>
    /// Legacy visual-style enum — retained for source-compat only. Selection
    /// used to choose a <c>NotificationPainter</c>; painters were removed. All
    /// styling now comes from the host theme via UseThemeColors on the
    /// notification's child Beep controls.
    /// </summary>
    [Obsolete("Painter system removed 2026-07. NotificationVisualStyle is no-op; use host theme tokens instead.")]
    public enum NotificationVisualStyle
    {
        Material3, iOS15, AntDesign, Fluent2, MaterialYou,
        Windows11Mica, MacOSBigSur, ChakraUI, TailwindCard,
        NotionMinimal, Minimal, VercelClean, StripeDashboard,
        DarkGlow, DiscordStyle, GradientModern,
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Data model
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Full data model for a single notification.</summary>
    public class NotificationData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; } = NotificationType.Info;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        /// <summary>
        /// Legacy layout selector — painters were removed 2026-07. Settable
        /// for source compat but no rendering effect.
        /// </summary>
        [Obsolete("Painter system removed; this property is no-op.")]
        public NotificationLayout Layout { get; set; } = NotificationLayout.Standard;

        /// <summary>
        /// Legacy visual style — painters were removed 2026-07. Settable for
        /// source compat but no rendering effect; theme is sourced from
        /// <c>BeepThemesManager.CurrentTheme</c> via UseThemeColors on the
        /// child controls.
        /// </summary>
        [Obsolete("Painter system removed; this property is no-op. Use host theme tokens.")]
        public NotificationVisualStyle VisualStyle { get; set; } = NotificationVisualStyle.Material3;

        /// <summary>Group key for stacking similar notifications.</summary>
        public string GroupKey { get; set; }

        /// <summary>Source identifier (e.g. "EmailClient", "FileSystem").</summary>
        public string Source { get; set; }

        /// <summary>
        /// Icon image path. If null, uses the default SVG for the type
        /// (see <see cref="GetDefaultIconForType"/>). Rendered via
        /// <see cref="Styling.ImagePainters.StyledImagePainter"/>.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>Custom icon colour tint (null = auto from type colour).</summary>
        public Color? IconTint { get; set; }

        /// <summary>
        /// Optional anchor Control for showing the notification near a UI
        /// element. Manager uses this for placement; not consumed by the
        /// notification form itself.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public System.Windows.Forms.Control AnchorControl { get; set; }

        /// <summary>Progress value 0–100 shown on the embedded BeepProgressBar.</summary>
        public int? ProgressValue { get; set; }

        /// <summary>Label for the progress indicator.</summary>
        public string ProgressText { get; set; }

        /// <summary>Custom background colour (overrides theme-derived type color).</summary>
        public Color? CustomBackColor { get; set; }

        /// <summary>Custom foreground colour.</summary>
        public Color? CustomForeColor { get; set; }

        /// <summary>Auto-dismiss duration in ms; 0 = no auto-dismiss.</summary>
        public int Duration { get; set; } = 5000;

        public bool ShowCloseButton { get; set; } = true;
        public bool ShowProgressBar { get; set; } = true;
        public bool PauseOnHover { get; set; } = true;
        public bool PlaySound { get; set; } = false;
        public string CustomSoundPath { get; set; }

        /// <summary>Action buttons shown inside the notification.</summary>
        public NotificationAction[] Actions { get; set; }

        /// <summary>Arbitrary tag for custom handling.</summary>
        public object Tag { get; set; }

        /// <summary>UTC timestamp when the notification was created.</summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Whether the notification has been read by the user.</summary>
        public bool IsRead { get; set; } = false;

        /// <summary>UTC timestamp when the notification was marked as read.</summary>
        public DateTime? ReadTimestamp { get; set; }

        /// <summary>Whether the notification is pinned (persists until explicitly dismissed).</summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>Whether the notification should persist after auto-dismiss duration expires.</summary>
        public bool Persistent { get; set; } = false;

        /// <summary>Number of times this notification has been shown (for grouped/repeated notifications).</summary>
        public int ShowCount { get; set; } = 1;

        /// <summary>Path to an embedded image resource shown in the notification.</summary>
        public string EmbeddedImagePath { get; set; }

        /// <summary>Whether to show an accent stripe on the notification edge.</summary>
        public bool ShowAccentStripe { get; set; }

        /// <summary>Color of the accent stripe when <see cref="ShowAccentStripe"/> is true.</summary>
        public Color? AccentStripeColor { get; set; }

        /// <summary>Corner radius override in pixels; 0 = use theme default.</summary>
        public int CornerRadiusOverride { get; set; }

        /// <summary>Whether Rich Text formatting is enabled in the message body.</summary>
        public bool EnableRichText { get; set; }

        // ── Helpers ───────────────────────────────────────────────────────────

        public static int GetDefaultDuration(NotificationPriority priority) => priority switch
        {
            NotificationPriority.Low      => 3000,
            NotificationPriority.Normal   => 5000,
            NotificationPriority.High     => 8000,
            NotificationPriority.Critical => 0,
            _                             => 5000,
        };

        /// <summary>
        /// Returns the SVG embedded-resource path for the default icon for a notification type.
        /// Uses <see cref="SvgsUI"/> (TheTechIdea.Beep.Icons) verified fields.
        /// </summary>
        public static string GetDefaultIconForType(NotificationType type) => type switch
        {
            NotificationType.Success => SvgsUI.CircleCheck,
            NotificationType.Warning => SvgsUI.AlertTriangle,
            NotificationType.Error   => SvgsUI.CircleX,
            NotificationType.Info    => SvgsUI.InfoCircle,
            NotificationType.System  => SvgsUI.Settings,
            _                        => SvgsUI.Bell,
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Supporting types
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>An action button displayed inside the notification.</summary>
    public class NotificationAction
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsPrimary { get; set; }
        public Action<NotificationData> OnClick { get; set; }
        public Color? CustomColor { get; set; }
    }

    /// <summary>Event arguments for all notification events.</summary>
    public class NotificationEventArgs : EventArgs
    {
        public NotificationData Notification { get; set; }
        public NotificationAction Action { get; set; }
        public bool Cancel { get; set; }
    }
}
