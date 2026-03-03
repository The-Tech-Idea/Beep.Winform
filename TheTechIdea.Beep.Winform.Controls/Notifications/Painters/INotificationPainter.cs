using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Interface for notification painters
    /// Defines methods for painting different parts of a notification
    /// </summary>
    public interface INotificationPainter
    {
        /// <summary>Owner control – used for DPI scaling inside the painter.</summary>
        Control OwnerControl { get; set; }

        /// <summary>
        /// Called by BeepNotification.ApplyTheme() to push theme-sourced fonts
        /// into the painter. Must use BeepFontManager.ToFont only.
        /// </summary>
        void RefreshFonts(IBeepTheme theme);

        void PaintBackground(Graphics g, Rectangle bounds, NotificationData data);
        void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data);
        void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data);
        void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data);
        void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data);
        void PaintCloseButton(Graphics g, Rectangle closeButtonRect, bool isHovered, NotificationData data);
        void PaintProgressBar(Graphics g, Rectangle progressBarRect, float progress, NotificationData data);
    }

    /// <summary>
    /// Notification item state for painting
    /// </summary>
    public struct NotificationItemState
    {
        public bool IsHovered { get; set; }
        public bool IsFocused { get; set; }
        public bool IsPressed { get; set; }
        public bool IsExpanded { get; set; }
    }

    /// <summary>
    /// Notification render options
    /// </summary>
    public class NotificationRenderOptions
    {
        public NotificationType Type { get; set; }
        public NotificationLayout Layout { get; set; }
        public NotificationVisualStyle VisualStyle { get; set; }
        public NotificationPriority Priority { get; set; }
        public Color? CustomBackColor { get; set; }
        public Color? CustomForeColor { get; set; }
        public Color? CustomBorderColor { get; set; }
        public Color? CustomIconColor { get; set; }
        public Color? AccentStripeColor { get; set; }
        public bool ShowAccentStripe { get; set; }
        public int BorderRadius { get; set; }
        public int Padding { get; set; }
        public int Spacing { get; set; }
        public int IconSize { get; set; }
        public bool ShowCloseButton { get; set; }
        public bool ShowProgressBar { get; set; }
        public float ProgressPercentage { get; set; }
    }
}
