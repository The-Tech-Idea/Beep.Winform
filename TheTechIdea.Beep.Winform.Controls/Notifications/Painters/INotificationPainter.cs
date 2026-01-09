using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Interface for notification painters
    /// Defines methods for painting different parts of a notification
    /// </summary>
    public interface INotificationPainter
    {
        /// <summary>
        /// Paints the notification background
        /// </summary>
        void PaintBackground(Graphics g, Rectangle bounds, NotificationData data);

        /// <summary>
        /// Paints the notification icon
        /// </summary>
        void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data);

        /// <summary>
        /// Paints the notification title
        /// </summary>
        void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data);

        /// <summary>
        /// Paints the notification message
        /// </summary>
        void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data);

        /// <summary>
        /// Paints action buttons
        /// </summary>
        void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data);

        /// <summary>
        /// Paints the close button
        /// </summary>
        void PaintCloseButton(Graphics g, Rectangle closeButtonRect, bool isHovered, NotificationData data);

        /// <summary>
        /// Paints the progress bar
        /// </summary>
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
        public NotificationPriority Priority { get; set; }
        public Color? CustomBackColor { get; set; }
        public Color? CustomForeColor { get; set; }
        public Color? CustomBorderColor { get; set; }
        public Color? CustomIconColor { get; set; }
        public int BorderRadius { get; set; }
        public int Padding { get; set; }
        public int Spacing { get; set; }
        public int IconSize { get; set; }
        public Font TitleFont { get; set; }
        public Font MessageFont { get; set; }
        public bool ShowCloseButton { get; set; }
        public bool ShowProgressBar { get; set; }
        public float ProgressPercentage { get; set; }
    }
}
