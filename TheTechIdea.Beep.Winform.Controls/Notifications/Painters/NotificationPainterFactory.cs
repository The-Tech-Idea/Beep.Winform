using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Factory for creating notification painters based on layout style
    /// </summary>
    public static class NotificationPainterFactory
    {
        /// <summary>
        /// Creates a painter for the specified notification layout
        /// </summary>
        public static INotificationPainter CreatePainter(NotificationLayout layout)
        {
            return layout switch
            {
                NotificationLayout.Standard => new StandardNotificationPainter(),
                NotificationLayout.Compact => new CompactNotificationPainter(),
                NotificationLayout.Prominent => new ProminentNotificationPainter(),
                NotificationLayout.Banner => new BannerNotificationPainter(),
                NotificationLayout.Toast => new ToastNotificationPainter(),
                _ => new StandardNotificationPainter()
            };
        }
    }
}
