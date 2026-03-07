using TheTechIdea.Beep.Winform.Controls.Notifications;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Centralized defaults for toast/snackbar behavior used by BeepDialogManager.
    /// </summary>
    public class NotificationPolicy
    {
        public bool SuppressDuplicateToasts { get; set; } = false;
        public bool SuppressDuplicateSnackbars { get; set; } = true;

        public int ToastDedupeWindowMs { get; set; } = 2000;
        public int SnackbarDedupeWindowMs { get; set; } = 1500;

        public int InfoToastDurationMs { get; set; } = 3000;
        public int SuccessToastDurationMs { get; set; } = 3000;
        public int WarningToastDurationMs { get; set; } = 4000;
        public int ErrorToastDurationMs { get; set; } = 5000;
        public int LoadingToastDurationMs { get; set; } = 0;

        public NotificationPosition DefaultToastPosition { get; set; } = NotificationPosition.TopRight;
        public NotificationPosition DefaultSnackbarPosition { get; set; } = NotificationPosition.BottomCenter;

        public NotificationPriority DefaultSnackbarPriority { get; set; } = NotificationPriority.Normal;
        public int DefaultSnackbarDurationMs { get; set; } = 5000;

        public static NotificationPolicy CreateDefault() => new NotificationPolicy();
    }
}
