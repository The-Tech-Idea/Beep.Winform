using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;  // SvgsUI, Svgs

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    // ─────────────────────────────────────────────────────────────────────────
    // Enums
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

    /// <summary>
    /// Layout variant – controls the geometric template used by the painter.
    /// </summary>
    public enum NotificationLayout
    {
        // ── Existing ──────────────────────────────────────────────────────────
        /// <summary>Icon left, title + message right, actions below.</summary>
        Standard,
        /// <summary>Icon and text on a single compact line.</summary>
        Compact,
        /// <summary>Large centred icon, prominent title.</summary>
        Prominent,
        /// <summary>Full-width banner strip.</summary>
        Banner,
        /// <summary>Minimal auto-dismiss toast.</summary>
        Toast,

        // ── New ───────────────────────────────────────────────────────────────
        /// <summary>Elevated card with drop-shadow, circle badge icon (56 dp).</summary>
        Elevated,
        /// <summary>Material Snackbar – single line, bottom-anchored, optional action link.</summary>
        Snackbar,
        /// <summary>Inline alert embedded inside a form body (non-floating).</summary>
        InlineAlert,
        /// <summary>Speech-bubble callout anchored to a specific control.</summary>
        Callout,
        /// <summary>Dismissible chip / pill (browser permission style).</summary>
        Chip,
        /// <summary>Timeline entry – icon left strip, multiline body, timestamp right.</summary>
        Timeline,
        /// <summary>Rich media – thumbnail left, text right, action below.</summary>
        MediaRich,
        /// <summary>Full-width slide-up action sheet (mobile-inspired).</summary>
        ActionSheet,
        /// <summary>Thin status-bar strip anchored to top/bottom of host form.</summary>
        StatusBar,
        /// <summary>Semi-transparent overlay covering the host form.</summary>
        Overlay,
        /// <summary>100 % host-form-width inline banner.</summary>
        FullWidth,
    }

    /// <summary>
    /// Visual painter style – one of 16 design-system tokens.
    /// Maps to a concrete <c>INotificationPainter</c> implementation.
    /// </summary>
    public enum NotificationVisualStyle
    {
        Material3,
        iOS15,
        AntDesign,
        Fluent2,
        MaterialYou,
        Windows11Mica,
        MacOSBigSur,
        ChakraUI,
        TailwindCard,
        NotionMinimal,
        Minimal,
        VercelClean,
        StripeDashboard,
        DarkGlow,
        DiscordStyle,
        GradientModern,
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
        public NotificationLayout Layout { get; set; } = NotificationLayout.Standard;

        /// <summary>Visual painter style (maps to 16-painter factory).</summary>
        public NotificationVisualStyle VisualStyle { get; set; } = NotificationVisualStyle.Material3;

        /// <summary>Group key for stacking similar notifications.</summary>
        public string GroupKey { get; set; }

        /// <summary>Source identifier (e.g. "EmailClient", "FileSystem").</summary>
        public string Source { get; set; }

        /// <summary>
        /// Icon image path (string). If null, uses the default SVG for the type.
        /// Rendered via <see cref="Styling.ImagePainters.StyledImagePainter"/>.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>Custom icon colour tint (null = auto from type colour).</summary>
        public Color? IconTint { get; set; }

        /// <summary>
        /// Body image path (string). Always a path – never a raw Image object.
        /// Rendered via <see cref="Styling.ImagePainters.StyledImagePainter"/>.
        /// </summary>
        public string EmbeddedImagePath { get; set; }

        /// <summary>Show a thin left-bar accent stripe (ChakraUI / Stripe style).</summary>
        public bool ShowAccentStripe { get; set; } = false;

        /// <summary>Accent stripe colour (null = auto from type colour).</summary>
        public Color? AccentStripeColor { get; set; }

        /// <summary>Corner radius override; 0 = use per-style default.</summary>
        public int CornerRadiusOverride { get; set; } = 0;

        /// <summary>
        /// Optional anchor control for <see cref="NotificationLayout.Callout"/> –
        /// the notification positions relative to this control's screen bounds.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public System.Windows.Forms.Control AnchorControl { get; set; }

        /// <summary>Enable simple HTML-like formatting in Message.</summary>
        public bool EnableRichText { get; set; } = false;

        /// <summary>Progress value 0–100 for an embedded progress indicator.</summary>
        public int? ProgressValue { get; set; }

        /// <summary>Label for the progress indicator.</summary>
        public string ProgressText { get; set; }

        /// <summary>Custom background colour (overrides type colour).</summary>
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
        public DateTime Timestamp { get; set; } = DateTime.Now;

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
            NotificationType.Success => SvgsUI.CircleCheck,      // check-circle.svg
            NotificationType.Warning => SvgsUI.AlertTriangle,    // alert-triangle.svg
            NotificationType.Error   => SvgsUI.CircleX,          // x-circle.svg
            NotificationType.Info    => SvgsUI.InfoCircle,              // info.svg
            NotificationType.System  => SvgsUI.Settings,         // settings.svg
            _                        => SvgsUI.Bell,              // bell.svg
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

