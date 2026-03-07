using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
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
        private static readonly Dictionary<string, DateTime> _notificationDedupe = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object _notificationDedupeLock = new();
        private NotificationPolicy _notificationPolicy = NotificationPolicy.CreateDefault();

        /// <summary>
        /// Gets current notification policy defaults used by toast/snackbar helpers.
        /// </summary>
        public NotificationPolicy NotificationPolicy => _notificationPolicy;

        /// <summary>
        /// Replaces the active notification policy.
        /// </summary>
        public BeepDialogManager SetNotificationPolicy(NotificationPolicy policy)
        {
            _notificationPolicy = policy ?? NotificationPolicy.CreateDefault();
            return this;
        }

        /// <summary>
        /// Mutates the active notification policy in place.
        /// </summary>
        public BeepDialogManager ConfigureNotifications(Action<NotificationPolicy> configure)
        {
            configure?.Invoke(_notificationPolicy);
            return this;
        }

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
            if (_notificationPolicy.SuppressDuplicateToasts)
            {
                var dedupeKey = $"toast::{type}::{message.Trim()}";
                if (ShouldSuppressDuplicate(dedupeKey, TimeSpan.FromMilliseconds(Math.Max(250, _notificationPolicy.ToastDedupeWindowMs))))
                    return;
            }

            var mgr = NotificationMgr;
            var targetPosition = position == ToastPosition.TopRight
                ? _notificationPolicy.DefaultToastPosition
                : MapPosition(position);
            mgr.DefaultPosition = targetPosition;
            var priority = MapPriority(type);
            var resolvedDuration = ResolveDuration(type, durationMs, priority, _notificationPolicy);
            mgr.Show(new NotificationData
            {
                Title = MapToastTitle(type),
                Message = message,
                Type = MapType(type),
                Priority = priority,
                Duration = resolvedDuration,
                Layout = NotificationLayout.Toast,
                ShowProgressBar = resolvedDuration > 0,
                IconPath = GetToastIconPath(type),
            });
        }

        /// <summary>Shows a deduped toast notification, suppressing repeats for a short window.</summary>
        public void ToastDeduped(
            string message,
            ToastType type = ToastType.Info,
            int durationMs = 3000,
            ToastPosition position = ToastPosition.TopRight,
            string? dedupeKey = null,
            int dedupeWindowMs = 2000)
        {
            var key = string.IsNullOrWhiteSpace(dedupeKey)
                ? $"toast::{type}::{message.Trim()}"
                : dedupeKey.Trim();

            if (dedupeWindowMs <= 0)
                dedupeWindowMs = _notificationPolicy.ToastDedupeWindowMs;

            if (ShouldSuppressDuplicate(key, TimeSpan.FromMilliseconds(Math.Max(250, dedupeWindowMs))))
                return;

            Toast(message, type, durationMs, position);
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
        public void Snackbar(
            string message,
            string? actionText = null,
            Action? action = null,
            int durationMs = 5000,
            NotificationPriority priority = NotificationPriority.Normal,
            string? dedupeKey = null,
            int dedupeWindowMs = 1500)
        {
            var effectivePriority = priority == NotificationPriority.Normal
                ? _notificationPolicy.DefaultSnackbarPriority
                : priority;

            if (dedupeWindowMs <= 0)
                dedupeWindowMs = _notificationPolicy.SnackbarDedupeWindowMs;

            var effectiveDedupeKey = dedupeKey;
            if (string.IsNullOrWhiteSpace(effectiveDedupeKey) && _notificationPolicy.SuppressDuplicateSnackbars)
            {
                effectiveDedupeKey = $"snackbar::{message.Trim()}::{actionText?.Trim()}";
            }

            if (!string.IsNullOrWhiteSpace(effectiveDedupeKey) &&
                ShouldSuppressDuplicate(effectiveDedupeKey.Trim(), TimeSpan.FromMilliseconds(Math.Max(250, dedupeWindowMs))))
            {
                return;
            }

            var mgr = NotificationMgr;
            mgr.DefaultPosition = _notificationPolicy.DefaultSnackbarPosition;

            var resolvedDuration = durationMs > 0
                ? durationMs
                : _notificationPolicy.DefaultSnackbarDurationMs > 0
                    ? _notificationPolicy.DefaultSnackbarDurationMs
                    : NotificationData.GetDefaultDuration(effectivePriority);

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
                Priority = effectivePriority,
                Duration = resolvedDuration,
                Layout = NotificationLayout.Snackbar,
                ShowProgressBar = false,
                ShowCloseButton = false,
                GroupKey = effectiveDedupeKey,
                Actions = actions,
            });
        }

        /// <summary>Shows a snackbar with undo action.</summary>
        public void SnackbarUndo(string message, Action undoAction, int durationMs = 5000, string? dedupeKey = null)
            => Snackbar(
                message,
                "UNDO",
                undoAction,
                durationMs,
                NotificationPriority.High,
                dedupeKey,
                _notificationPolicy.SnackbarDedupeWindowMs);

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

        /// <summary>
        /// Shows a consistent deduped "feature pending" informational notification.
        /// </summary>
        public void NotifyFeaturePending(string featureName, string? shortcutHint = null, string? dedupeKey = null)
        {
            var safeFeature = string.IsNullOrWhiteSpace(featureName) ? "This feature" : featureName.Trim();
            var message = string.IsNullOrWhiteSpace(shortcutHint)
                ? $"{safeFeature} is coming in a future update."
                : $"{safeFeature} ({shortcutHint.Trim()}) is coming in a future update.";

            var key = string.IsNullOrWhiteSpace(dedupeKey)
                ? $"feature-pending::{safeFeature.ToLowerInvariant()}::{shortcutHint?.Trim().ToLowerInvariant()}"
                : dedupeKey.Trim();

            ToastDeduped(message, ToastType.Info, dedupeKey: key);
        }

        /// <summary>Shows import success notification with duplicate suppression.</summary>
        public void NotifyImportSuccess(string sourceName, int importedCount)
        {
            var safeSource = string.IsNullOrWhiteSpace(sourceName) ? "items" : sourceName.Trim();
            var message = importedCount > 0
                ? $"Imported {importedCount} item(s) from {safeSource}."
                : $"Import completed for {safeSource}.";

            ToastDeduped(
                message,
                ToastType.Success,
                dedupeKey: $"import-success::{safeSource}::{importedCount}");
        }

        /// <summary>Shows import failure notification with duplicate suppression.</summary>
        public void NotifyImportFailure(string sourceName, string error)
        {
            var safeSource = string.IsNullOrWhiteSpace(sourceName) ? "source" : sourceName.Trim();
            var safeError = string.IsNullOrWhiteSpace(error) ? "Unknown error." : error.Trim();
            ToastDeduped(
                $"Import failed for {safeSource}: {safeError}",
                ToastType.Error,
                durationMs: 0,
                dedupeKey: $"import-failed::{safeSource}::{safeError}");
        }

        /// <summary>Shows export success notification with optional undo action.</summary>
        public void NotifyExportSuccess(string destinationName, int exportedCount, Action? undoAction = null)
        {
            var safeDestination = string.IsNullOrWhiteSpace(destinationName) ? "destination" : destinationName.Trim();
            var message = exportedCount > 0
                ? $"Exported {exportedCount} item(s) to {safeDestination}."
                : $"Export completed to {safeDestination}.";

            if (undoAction != null)
            {
                SnackbarUndo(message, undoAction, 5000, $"export-success-undo::{safeDestination}::{exportedCount}");
                return;
            }

            ToastDeduped(
                message,
                ToastType.Success,
                dedupeKey: $"export-success::{safeDestination}::{exportedCount}");
        }

        /// <summary>Shows export failure notification with duplicate suppression.</summary>
        public void NotifyExportFailure(string destinationName, string error)
        {
            var safeDestination = string.IsNullOrWhiteSpace(destinationName) ? "destination" : destinationName.Trim();
            var safeError = string.IsNullOrWhiteSpace(error) ? "Unknown error." : error.Trim();
            ToastDeduped(
                $"Export failed to {safeDestination}: {safeError}",
                ToastType.Error,
                durationMs: 0,
                dedupeKey: $"export-failed::{safeDestination}::{safeError}");
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

        private static NotificationPriority MapPriority(ToastType type) => type switch
        {
            ToastType.Error => NotificationPriority.High,
            ToastType.Warning => NotificationPriority.High,
            ToastType.Loading => NotificationPriority.Normal,
            ToastType.Success => NotificationPriority.Normal,
            _ => NotificationPriority.Normal
        };

        private static int ResolveDuration(ToastType type, int requestedDurationMs, NotificationPriority priority, NotificationPolicy policy)
        {
            if (type == ToastType.Loading)
                return policy.LoadingToastDurationMs;

            if (requestedDurationMs > 0)
                return requestedDurationMs;

            var policyDuration = type switch
            {
                ToastType.Success => policy.SuccessToastDurationMs,
                ToastType.Warning => policy.WarningToastDurationMs,
                ToastType.Error => policy.ErrorToastDurationMs,
                _ => policy.InfoToastDurationMs
            };

            if (policyDuration > 0)
                return policyDuration;

            return NotificationData.GetDefaultDuration(priority);
        }

        private static bool ShouldSuppressDuplicate(string key, TimeSpan dedupeWindow)
        {
            lock (_notificationDedupeLock)
            {
                var now = DateTime.UtcNow;

                if (_notificationDedupe.TryGetValue(key, out var lastShown) && (now - lastShown) < dedupeWindow)
                {
                    return true;
                }

                _notificationDedupe[key] = now;

                if (_notificationDedupe.Count > 256)
                {
                    var cutoff = now.AddMinutes(-3);
                    var keysToRemove = new List<string>();
                    foreach (var entry in _notificationDedupe)
                    {
                        if (entry.Value < cutoff)
                            keysToRemove.Add(entry.Key);
                    }

                    foreach (var item in keysToRemove)
                        _notificationDedupe.Remove(item);
                }

                return false;
            }
        }

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
