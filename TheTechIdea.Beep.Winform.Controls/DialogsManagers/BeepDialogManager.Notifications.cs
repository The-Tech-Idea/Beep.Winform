using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Notifications;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Toast and notification methods for BeepDialogManager.
    /// Delegates everything to <see cref="BeepNotificationManager"/> —
    /// no raw Form / Label / GDI painting.
    /// </summary>
    public partial class BeepDialogManager
    {
        // BeepNotificationManager is a singleton; we configure it with our
        // host form so it positions relative to the correct screen.
        private BeepNotificationManager NotificationMgr => ConfigureNotificationMgr();

        private BeepNotificationManager ConfigureNotificationMgr()
        {
            var mgr = BeepNotificationManager.Instance;
            if (_hostForm != null)
                mgr.AnchorForm = _hostForm;
            return mgr;
        }

        #region Toast Types and Positions

        /// <summary>Toast notification type.</summary>
        public enum ToastType
        {
            /// <summary>Success toast (green)</summary>
            Success,
            /// <summary>Error toast (red)</summary>
            Error,
            /// <summary>Warning toast (yellow/orange)</summary>
            Warning,
            /// <summary>Information toast (blue)</summary>
            Info,
            /// <summary>Loading toast (gray with spinner)</summary>
            Loading
        }

        /// <summary>Toast position on screen.</summary>
        public enum ToastPosition
        {
            /// <summary>Top left corner</summary>
            TopLeft,
            /// <summary>Top center</summary>
            TopCenter,
            /// <summary>Top right corner</summary>
            TopRight,
            /// <summary>Bottom left corner</summary>
            BottomLeft,
            /// <summary>Bottom center</summary>
            BottomCenter,
            /// <summary>Bottom right corner</summary>
            BottomRight
        }

        #endregion

        #region Toast Methods

        /// <summary>Shows a toast notification.</summary>
        public void Toast(string message, ToastType type = ToastType.Info, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
        {
            var mgr = NotificationMgr;
            mgr.DefaultPosition = MapPosition(position);
            mgr.Show(new NotificationData
            {
                Title = MapToastTitle(type),
                Message = message,
                Type = MapType(type),
                Duration = durationMs,
                Layout = NotificationLayout.Toast,
                ShowProgressBar = durationMs > 0,
                IconPath = GetToastIconPath(type),
            });
        }

        /// <summary>Shows a success toast.</summary>
        public void ToastSuccess(string message, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
            => Toast(message, ToastType.Success, durationMs, position);

        /// <summary>Shows an error toast.</summary>
        public void ToastError(string message, int durationMs = 5000, ToastPosition position = ToastPosition.TopRight)
            => Toast(message, ToastType.Error, durationMs, position);

        /// <summary>Shows a warning toast.</summary>
        public void ToastWarning(string message, int durationMs = 4000, ToastPosition position = ToastPosition.TopRight)
            => Toast(message, ToastType.Warning, durationMs, position);

        /// <summary>Shows an info toast.</summary>
        public void ToastInfo(string message, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
            => Toast(message, ToastType.Info, durationMs, position);

        /// <summary>
        /// Shows a loading toast that must be manually dismissed.
        /// Dispose the returned handle to dismiss.
        /// </summary>
        public IDisposable ToastLoading(string message, ToastPosition position = ToastPosition.TopCenter)
        {
            var mgr = NotificationMgr;
            mgr.DefaultPosition = MapPosition(position);
            mgr.Show(new NotificationData
            {
                Title = "Loading",
                Message = message,
                Type = Notifications.NotificationType.Info,
                Duration = 0,
                Layout = NotificationLayout.Toast,
                ShowProgressBar = false,
                IconPath = Svgs.Loading,
            });
            return new LoadingToastHandle(mgr);
        }

        #endregion

        #region Snackbar Methods

        /// <summary>Shows a snackbar notification with optional action.</summary>
        public void Snackbar(string message, string? actionText = null, Action? action = null, int durationMs = 5000)
        {
            var mgr = NotificationMgr;
            mgr.DefaultPosition = NotificationPosition.BottomCenter;

            NotificationAction[]? actions = null;
            if (!string.IsNullOrEmpty(actionText) && action != null)
            {
                actions = new[]
                {
                    new NotificationAction
                    {
                        Id = "action",
                        Text = actionText,
                        IsPrimary = true,
                        OnClick = _ => action.Invoke()
                    }
                };
            }

            mgr.Show(new NotificationData
            {
                Title = "",
                Message = message,
                Type = Notifications.NotificationType.Info,
                Duration = durationMs,
                Layout = NotificationLayout.Snackbar,
                ShowProgressBar = false,
                ShowCloseButton = false,
                Actions = actions,
            });
        }

        /// <summary>Shows a snackbar with undo action.</summary>
        public void SnackbarUndo(string message, Action undoAction, int durationMs = 5000)
            => Snackbar(message, "UNDO", undoAction, durationMs);

        #endregion

        #region Banner Methods

        /// <summary>Shows a banner notification at the top of the host form.</summary>
        public IDisposable Banner(string message, ToastType type = ToastType.Info, bool dismissible = true)
        {
            var mgr = NotificationMgr;
            mgr.DefaultPosition = NotificationPosition.TopCenter;
            mgr.Show(new NotificationData
            {
                Title = MapToastTitle(type),
                Message = message,
                Type = MapType(type),
                Duration = 0,
                Layout = NotificationLayout.Banner,
                ShowCloseButton = dismissible,
                ShowProgressBar = false,
                IconPath = GetToastIconPath(type),
            });
            return new EmptyDisposable();
        }

        #endregion

        #region Mapping Helpers

        private static Notifications.NotificationType MapType(ToastType t) => t switch
        {
            ToastType.Success => Notifications.NotificationType.Success,
            ToastType.Error   => Notifications.NotificationType.Error,
            ToastType.Warning => Notifications.NotificationType.Warning,
            _                 => Notifications.NotificationType.Info
        };

        private static NotificationPosition MapPosition(ToastPosition p) => p switch
        {
            ToastPosition.TopLeft      => NotificationPosition.TopLeft,
            ToastPosition.TopCenter    => NotificationPosition.TopCenter,
            ToastPosition.BottomLeft   => NotificationPosition.BottomLeft,
            ToastPosition.BottomCenter => NotificationPosition.BottomCenter,
            ToastPosition.BottomRight  => NotificationPosition.BottomRight,
            _                          => NotificationPosition.TopRight
        };

        private static string MapToastTitle(ToastType t) => t switch
        {
            ToastType.Success => "Success",
            ToastType.Error   => "Error",
            ToastType.Warning => "Warning",
            ToastType.Loading => "Loading",
            _                 => "Info"
        };

        private static string GetToastIconPath(ToastType type) => type switch
        {
            ToastType.Success => Svgs.CheckCircle,
            ToastType.Error   => Svgs.Error,
            ToastType.Warning => Svgs.InfoWarning,
            ToastType.Loading => Svgs.Loading,
            _                 => Svgs.Information
        };

        #endregion

        #region IDialogManager Implementation (Notifications)

        async Task IDialogManager.ShowToastAsync(string message, int durationMs, string? icon, System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var type = icon?.ToLower() switch
            {
                "success" => ToastType.Success,
                "error"   => ToastType.Error,
                "warning" => ToastType.Warning,
                _         => ToastType.Info
            };
            Toast(message, type, durationMs);
            await Task.CompletedTask;
        }

        #endregion

        #region Helper Classes

        private class LoadingToastHandle : IDisposable
        {
            private readonly BeepNotificationManager _mgr;
            public LoadingToastHandle(BeepNotificationManager mgr) => _mgr = mgr;
            public void Dispose() => _mgr.DismissAllByType(Notifications.NotificationType.Info);
        }

        private class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        #endregion
    }
}
