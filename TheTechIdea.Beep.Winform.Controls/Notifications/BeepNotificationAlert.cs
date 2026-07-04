using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Notifications;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Static factory for one-line notification usage. Inspired by
    /// <c>leandrovip/Vip.Notification</c> (★28) — the most popular WinForms
    /// toast library on GitHub — so that callers can drop in our notification
    /// surface with minimal friction.
    ///
    /// Phase 4.6 / G34 of plans/NOTIFICATION_ENHANCEMENT_PLAN.md.
    ///
    /// Usage:
    ///   <code>
    ///     BeepNotificationAlert.ShowSuccess("Saved");
    ///     var n = BeepNotificationAlert.ShowError("Connection lost", 0);
    ///     n.NotificationDismissed += (s, e) => { ... };
    ///   </code>
    /// </summary>
    public static class BeepNotificationAlert
    {
        /// <summary>Success toast, default 5000 ms.</summary>
        public static BeepNotification ShowSuccess(string message, int durationMs = 5000)
            => Build(message, NotificationType.Success, durationMs);

        /// <summary>Warning toast, default 8000 ms (extra dwell for high-impact).</summary>
        public static BeepNotification ShowWarning(string message, int durationMs = 8000)
            => Build(message, NotificationType.Warning, durationMs);

        /// <summary>Error toast, default 0 = no auto-dismiss (require explicit close).</summary>
        public static BeepNotification ShowError(string message, int durationMs = 0)
            => Build(message, NotificationType.Error, durationMs);

        /// <summary>Info toast, default 5000 ms.</summary>
        public static BeepNotification ShowInfo(string message, int durationMs = 5000)
            => Build(message, NotificationType.Info, durationMs);

        /// <summary>
        /// Custom toast: caller specifies a text message and a tint color
        /// (often a Type color override). Image is optional (raw <see cref="Image"/>
        /// is allowed here only because Vip parity requires it; not used internally —
        /// the painter will pick the type-color icon by default).
        /// </summary>
        public static BeepNotification ShowCustom(
            string message,
            int durationMs = 5000,
            Image image = null,
            Color? tint = null)
        {
            var n = Build(message, NotificationType.Custom, durationMs);
            if (tint.HasValue && n.NotificationData != null)
                n.NotificationData.IconTint = tint.Value;
            return n;
        }

        private static BeepNotification Build(string message, NotificationType type, int durationMs)
        {
            var notification = new BeepNotification
            {
                NotificationData = new NotificationData
                {
                    Title = type.ToString(),
                    Message = message,
                    Type = type,
                    Duration = durationMs
                }
            };

            // Reuse the manager's existing internal Show pipeline; this gives us
            // capacity-bypass-for-Critical, queueing, animation, history, sound,
            // persistence — all the manager's logic is in one place.
            BeepNotificationManager.Instance.Show(notification.NotificationData);

            // Return the freshly-shown notification so the caller can subscribe
            // to events or call Dismiss explicitly.
            return notification;
        }
    }
}
