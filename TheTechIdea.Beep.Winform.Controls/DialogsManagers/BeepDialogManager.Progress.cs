using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Progress and loading dialog methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Progress Handle Interface

        /// <summary>
        /// Semantic progress states used by progress dialogs.
        /// </summary>
        public enum ProgressState
        {
            Pending,
            InProgress,
            Completed,
            Failed,
            Retrying
        }

        /// <summary>
        /// Handle for managing progress dialogs
        /// </summary>
        public interface IProgressDialogHandle : IDisposable
        {
            /// <summary>
            /// Updates the progress percentage and optional status message
            /// </summary>
            void UpdateProgress(int percent, string? status = null);

            /// <summary>
            /// Updates only the status message
            /// </summary>
            void UpdateStatus(string status);

            /// <summary>
            /// Sets the progress to indeterminate mode
            /// </summary>
            void SetIndeterminate();

            /// <summary>
            /// Completes the progress with optional success message
            /// </summary>
            void Complete(string? message = null);

            /// <summary>
            /// Shows an error and closes the progress
            /// </summary>
            void Error(string message);

            /// <summary>
            /// Updates the semantic state and optional message/percent.
            /// </summary>
            void UpdateState(ProgressState state, string? status = null, int? percent = null);

            /// <summary>
            /// Gets whether cancellation has been requested
            /// </summary>
            bool IsCancellationRequested { get; }

            /// <summary>
            /// Gets the cancellation token
            /// </summary>
            CancellationToken CancellationToken { get; }
        }

        #endregion

        #region Progress Methods

        /// <summary>
        /// Shows a progress dialog with determinate progress
        /// </summary>
        public IProgressDialogHandle ShowProgress(string title, string? message = null, bool cancellable = false)
        {
            var handle = new ProgressHandle(this, title, message, cancellable, false);
            handle.Show();
            return handle;
        }

        /// <summary>
        /// Shows an indeterminate progress dialog
        /// </summary>
        public IProgressDialogHandle ShowIndeterminate(string title, string? message = null, bool cancellable = false)
        {
            var handle = new ProgressHandle(this, title, message, cancellable, true);
            handle.Show();
            return handle;
        }

        /// <summary>
        /// Shows a busy overlay on the host form
        /// </summary>
        public IDisposable ShowBusy(string? message = null)
        {
            if (_hostForm == null)
            {
                return ShowIndeterminate("Please wait...", message);
            }

            var overlay = new BusyOverlay(_hostForm, message);
            overlay.Show();
            return overlay;
        }

        /// <summary>
        /// Runs an async operation with progress dialog
        /// </summary>
        public async Task<T> RunWithProgressAsync<T>(string title, string message, Func<IProgressDialogHandle, Task<T>> operation, bool cancellable = false)
        {
            using var progress = ShowProgress(title, message, cancellable);
            try
            {
                return await operation(progress);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                progress.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Runs an async operation with progress dialog (no return value)
        /// </summary>
        public async Task RunWithProgressAsync(string title, string message, Func<IProgressDialogHandle, Task> operation, bool cancellable = false)
        {
            using var progress = ShowProgress(title, message, cancellable);
            try
            {
                await operation(progress);
                progress.Complete();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                progress.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Runs an async operation with busy overlay
        /// </summary>
        public async Task<T> RunWithBusyAsync<T>(string message, Func<Task<T>> operation)
        {
            using var busy = ShowBusy(message);
            return await operation();
        }

        /// <summary>
        /// Runs an async operation with busy overlay (no return value)
        /// </summary>
        public async Task RunWithBusyAsync(string message, Func<Task> operation)
        {
            using var busy = ShowBusy(message);
            await operation();
        }

        #endregion

        #region IDialogManager Implementation (Progress)

        Task<TheTechIdea.Beep.Vis.Modules.IProgressHandle> IDialogManager.ShowProgressAsync(string title, string? message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var handle = new ProgressHandle(this, title, message, false, false);
            handle.Show();
            return Task.FromResult<TheTechIdea.Beep.Vis.Modules.IProgressHandle>(handle);
        }

        #endregion

        #region Progress Handle Implementation

        private class ProgressHandle : IProgressDialogHandle, TheTechIdea.Beep.Vis.Modules.IProgressHandle, IAsyncDisposable
        {
            private readonly BeepDialogManager _manager;
            private readonly string _title;
            private readonly string? _initialMessage;
            private readonly bool _cancellable;
            private readonly bool _indeterminate;
            private readonly CancellationTokenSource _cts;

            private BeepProgressDialog? _dialog;
            private bool _disposed;
            private ProgressState _state = ProgressState.Pending;

            public ProgressHandle(BeepDialogManager manager, string title, string? message, bool cancellable, bool indeterminate)
            {
                _manager = manager;
                _title = title;
                _initialMessage = message;
                _cancellable = cancellable;
                _indeterminate = indeterminate;
                _cts = new CancellationTokenSource();
            }

            public bool IsCancellationRequested => _cts.IsCancellationRequested;
            public CancellationToken CancellationToken => _cts.Token;

            public void Show()
            {
                _dialog = new BeepProgressDialog(_title, _initialMessage, _cancellable, _indeterminate);

                // Wire cancel button
                if (_cancellable && _dialog.CancelButton != null)
                {
                    _dialog.CancelButton.Click += (s, e) =>
                    {
                        _cts.Cancel();
                        if (_dialog.MessageLabel != null)
                            _dialog.MessageLabel.Text = "Cancelling...";
                        if (_dialog.CancelButton != null)
                            _dialog.CancelButton.Enabled = false;
                    };
                }

                // Apply theme from manager's default theme
                var theme = _manager._defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
                if (theme != null)
                {
                    _dialog.Theme = theme.ThemeName;
                    _dialog.ApplyTheme();
                }

                var owner = _manager._hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);

                if (owner != null)
                {
                    _dialog.StartPosition = FormStartPosition.CenterParent;
                    _dialog.Show(owner);
                }
                else
                {
                    _dialog.StartPosition = FormStartPosition.CenterScreen;
                    _dialog.Show();
                }

                if (string.IsNullOrWhiteSpace(_initialMessage))
                {
                    UpdateState(ProgressState.Pending);
                }
            }

            public void UpdateProgress(int percent, string? status = null)
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(() => UpdateProgress(percent, status)));
                    return;
                }

                if (!_indeterminate)
                {
                    _dialog.ProgressBarControl.Value = Math.Max(0, Math.Min(100, percent));
                    if (_dialog.PercentLabel != null)
                        _dialog.PercentLabel.Text = $"{percent}%";
                }

                _state = ProgressState.InProgress;
                if (status != null)
                    _dialog.MessageLabel.Text = status;

                _dialog.Refresh();
            }

            public void UpdateStatus(string status)
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(() => UpdateStatus(status)));
                    return;
                }

                _state = ProgressState.InProgress;
                _dialog.MessageLabel.Text = status;
                _dialog.Refresh();
            }

            public void SetIndeterminate()
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(SetIndeterminate));
                    return;
                }

                _dialog.ProgressBarControl.ProgressBarStyle = ProgressBars.ProgressBarStyle.Animated;
                if (_dialog.PercentLabel != null)
                    _dialog.PercentLabel.Visible = false;

                _dialog.Refresh();
            }

            public void Complete(string? message = null)
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(() => Complete(message)));
                    return;
                }

                _dialog.ProgressBarControl.Value = 100;
                if (_dialog.PercentLabel != null)
                {
                    _dialog.PercentLabel.Text = "100%";
                    _dialog.PercentLabel.Visible = true;
                }

                _state = ProgressState.Completed;
                _dialog.MessageLabel.Text = string.IsNullOrWhiteSpace(message)
                    ? BuildStatePrefix(ProgressState.Completed)
                    : message;

                _dialog.Refresh();
            }

            public void Error(string message)
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(() => Error(message)));
                    return;
                }

                _state = ProgressState.Failed;
                _dialog.MessageLabel.Text = message;
                _dialog.Refresh();
            }

            public void UpdateState(ProgressState state, string? status = null, int? percent = null)
            {
                if (_disposed || _dialog == null || _dialog.IsDisposed)
                    return;

                if (_dialog.InvokeRequired)
                {
                    _dialog.BeginInvoke(new Action(() => UpdateState(state, status, percent)));
                    return;
                }

                _state = state;

                if (percent.HasValue && !_indeterminate)
                {
                    var clamped = Math.Max(0, Math.Min(100, percent.Value));
                    _dialog.ProgressBarControl.Value = clamped;
                    if (_dialog.PercentLabel != null)
                    {
                        _dialog.PercentLabel.Text = $"{clamped}%";
                        _dialog.PercentLabel.Visible = true;
                    }
                }

                _dialog.MessageLabel.Text = string.IsNullOrWhiteSpace(status)
                    ? BuildStatePrefix(state)
                    : status;

                _dialog.Refresh();
            }

            private static string BuildStatePrefix(ProgressState state)
            {
                return state switch
                {
                    ProgressState.Pending => "Pending...",
                    ProgressState.InProgress => "In progress...",
                    ProgressState.Completed => "Completed",
                    ProgressState.Failed => "Failed",
                    ProgressState.Retrying => "Retrying...",
                    _ => "Working..."
                };
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _cts.Cancel();
                _cts.Dispose();

                if (_dialog != null && !_dialog.IsDisposed)
                {
                    if (_dialog.InvokeRequired)
                    {
                        _dialog.BeginInvoke(new Action(() =>
                        {
                            _dialog.Close();
                            _dialog.Dispose();
                        }));
                    }
                    else
                    {
                        _dialog.Close();
                        _dialog.Dispose();
                    }
                }

                _dialog = null;
            }

            void TheTechIdea.Beep.Vis.Modules.IProgressHandle.Update(int percent, string? status)
            {
                UpdateProgress(percent, status);
            }

            void TheTechIdea.Beep.Vis.Modules.IProgressHandle.Complete(string? finalMessage)
            {
                Complete(finalMessage);
            }

            public ValueTask DisposeAsync()
            {
                Dispose();
                return ValueTask.CompletedTask;
            }
        }

        #endregion

        #region Busy Overlay

        private class BusyOverlay : IDisposable
        {
            private readonly Form _hostForm;
            private readonly BeepPanel _overlay;
            private bool _disposed;

            public BusyOverlay(Form hostForm, string? message)
            {
                _hostForm = hostForm;

                var overlayAlpha = TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.IsDarkTheme == true ? 200 : 180;
                var overlayColor = Color.FromArgb(overlayAlpha, TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.IsDarkTheme == true ? 30 : 0,
                    TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.IsDarkTheme == true ? 30 : 0,
                    TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.IsDarkTheme == true ? 35 : 0);

                _overlay = new BeepPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = overlayColor,
                    ShowTitle = false,
                    ShowTitleLine = false,
                };

                // Spinner via BeepProgressBar DotsLoader — no raw GDI painting
                var spinner = new ProgressBars.BeepProgressBar
                {
                    Size = new Size(120, 24),
                    PainterKind = ProgressBars.ProgressPainterKind.DotsLoader,
                    ProgressBarStyle = ProgressBars.ProgressBarStyle.Animated,
                    UseThemeColors = true,
                    VisualMode = ProgressBars.ProgressBarDisplayMode.NoText,
                };
                spinner.Location = new Point(
                    (_hostForm.ClientSize.Width - spinner.Width) / 2,
                    (_hostForm.ClientSize.Height - spinner.Height) / 2 - 16
                );
                _overlay.Controls.Add(spinner);

                if (!string.IsNullOrEmpty(message))
                {
                    var messageLabel = new BeepLabel
                    {
                        Text = message,
                        ForeColor = ColorUtils.GetContrastColor(overlayColor),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Size = new Size(320, 24),
                        BackColor = Color.Transparent,
                        UseThemeColors = false,
                    };
                    messageLabel.Location = new Point(
                        (_hostForm.ClientSize.Width - messageLabel.Width) / 2,
                        spinner.Bottom + 10
                    );
                    _overlay.Controls.Add(messageLabel);
                }
            }

            public void Show()
            {
                _hostForm.Controls.Add(_overlay);
                _overlay.BringToFront();
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;

                if (!_hostForm.IsDisposed)
                    _hostForm.Controls.Remove(_overlay);

                _overlay.Dispose();
            }
        }

        #endregion
    }
}

