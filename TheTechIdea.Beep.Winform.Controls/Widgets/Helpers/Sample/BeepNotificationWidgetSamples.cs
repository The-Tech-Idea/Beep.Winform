using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepNotificationWidget with all notification styles
    /// </summary>
    public static class BeepNotificationWidgetSamples
    {
        /// <summary>
        /// Creates a toast notification widget
        /// Uses ToastNotificationPainter.cs
        /// </summary>
        public static BeepNotificationWidget CreateToastNotificationWidget()
        {
            return new BeepNotificationWidget
            {
                Style = NotificationWidgetStyle.ToastNotification,
                NotificationType = NotificationType.Success,
                Title = "Success!",
                Message = "Your changes have been saved successfully.",
                IsDismissible = true,
                ShowIcon = true,
                Size = new Size(350, 80),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates an alert banner notification widget
        /// Uses AlertBannerPainter.cs
        /// </summary>
        public static BeepNotificationWidget CreateAlertBannerWidget()
        {
            return new BeepNotificationWidget
            {
                Style = NotificationWidgetStyle.AlertBanner,
                NotificationType = NotificationType.Warning,
                Title = "System Maintenance",
                Message = "Scheduled maintenance will begin at 2:00 AM EST.",
                ShowIcon = true,
                Size = new Size(500, 60),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a progress alert notification widget
        /// Uses ProgressAlertPainter.cs
        /// </summary>
        public static BeepNotificationWidget CreateProgressAlertWidget()
        {
            return new BeepNotificationWidget
            {
                Style = NotificationWidgetStyle.ProgressAlert,
                NotificationType = NotificationType.Progress,
                Title = "Uploading files...",
                Message = "Please wait while we process your files.",
                Progress = 65,
                Size = new Size(300, 80),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a status card notification widget
        /// Uses StatusCardPainter.cs  
        /// </summary>
        public static BeepNotificationWidget CreateStatusCardWidget()
        {
            return new BeepNotificationWidget
            {
                Style = NotificationWidgetStyle.StatusCard,
                NotificationType = NotificationType.Error,
                Title = "Connection Error",
                Message = "Unable to connect to the database. Please check your connection and try again.",
                ShowIcon = true,
                Size = new Size(400, 100),
                AccentColor = Color.FromArgb(244, 67, 54)
            };
        }

        /// <summary>
        /// Gets all notification widget samples
        /// </summary>
        public static BeepNotificationWidget[] GetAllSamples()
        {
            return new BeepNotificationWidget[]
            {
                CreateToastNotificationWidget(),
                CreateAlertBannerWidget(),
                CreateProgressAlertWidget(),
                CreateStatusCardWidget()
            };
        }
    }
}