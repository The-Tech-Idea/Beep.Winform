using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Progress and loading dialog methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Progress Handle Interface

        /// <summary>
        /// Handle for managing progress dialogs
        /// </summary>
        public interface IProgressHandle : IDisposable
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
        public IProgressHandle ShowProgress(string title, string? message = null, bool cancellable = false)
        {
            var handle = new ProgressHandle(this, title, message, cancellable, false);
            handle.Show();
            return handle;
        }

        /// <summary>
        /// Shows an indeterminate progress dialog
        /// </summary>
        public IProgressHandle ShowIndeterminate(string title, string? message = null, bool cancellable = false)
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
        public async Task<T> RunWithProgressAsync<T>(string title, string message, Func<IProgressHandle, Task<T>> operation, bool cancellable = false)
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
        public async Task RunWithProgressAsync(string title, string message, Func<IProgressHandle, Task> operation, bool cancellable = false)
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

        int IDialogManager.ShowProgress(string title, string? message)
        {
            int token = ++_progressTokenCounter;
            var handle = new ProgressHandle(this, title, message, false, false);
            handle.Show();
            _progressDialogs[token] = handle;
            return token;
        }

        void IDialogManager.UpdateProgress(int token, int percent, string? status)
        {
            if (_progressDialogs.TryGetValue(token, out var handle))
            {
                handle.UpdateProgress(percent, status);
            }
        }

        void IDialogManager.CloseProgress(int token)
        {
            if (_progressDialogs.TryGetValue(token, out var handle))
            {
                handle.Dispose();
                _progressDialogs.Remove(token);
            }
        }

        #endregion

        #region Progress Handle Implementation

        private class ProgressHandle : IProgressHandle
        {
            private readonly BeepDialogManager _manager;
            private readonly string _title;
            private readonly string? _initialMessage;
            private readonly bool _cancellable;
            private readonly bool _indeterminate;
            private readonly CancellationTokenSource _cts;

            private Form? _dialog;
            private Label? _messageLabel;
            private ProgressBar? _progressBar;
            private Label? _percentLabel;
            private Button? _cancelButton;
            private bool _disposed;

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
                _dialog = CreateProgressDialog();

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

                Application.DoEvents();
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

                if (_progressBar != null && !_indeterminate)
                {
                    _progressBar.Value = Math.Max(0, Math.Min(100, percent));
                }

                if (_percentLabel != null && !_indeterminate)
                {
                    _percentLabel.Text = $"{percent}%";
                }

                if (status != null && _messageLabel != null)
                {
                    _messageLabel.Text = status;
                }

                Application.DoEvents();
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

                if (_messageLabel != null)
                {
                    _messageLabel.Text = status;
                }

                Application.DoEvents();
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

                if (_progressBar != null)
                {
                    _progressBar.Style = ProgressBarStyle.Marquee;
                }

                if (_percentLabel != null)
                {
                    _percentLabel.Visible = false;
                }
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

                if (_progressBar != null)
                {
                    _progressBar.Style = ProgressBarStyle.Continuous;
                    _progressBar.Value = 100;
                }

                if (_percentLabel != null)
                {
                    _percentLabel.Text = "100%";
                    _percentLabel.Visible = true;
                }

                if (message != null && _messageLabel != null)
                {
                    _messageLabel.Text = message;
                }

                Application.DoEvents();
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

                if (_messageLabel != null)
                {
                    _messageLabel.Text = message;
                    _messageLabel.ForeColor = Color.FromArgb(220, 38, 38);
                }

                if (_progressBar != null)
                {
                    _progressBar.Style = ProgressBarStyle.Continuous;
                    _progressBar.ForeColor = Color.FromArgb(220, 38, 38);
                }

                Application.DoEvents();
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

            private Form CreateProgressDialog()
            {
                var theme = _manager._defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;

                var dialog = new Form
                {
                    Text = _title,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowInTaskbar = false,
                    Size = new Size(400, _cancellable ? 160 : 130),
                    BackColor = theme?.BackColor ?? Color.White
                };

                // Title label
                var titleLabel = new Label
                {
                    Text = _title,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = theme?.ForeColor ?? Color.Black,
                    Location = new Point(20, 16),
                    Size = new Size(360, 24),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                dialog.Controls.Add(titleLabel);

                // Message label
                _messageLabel = new Label
                {
                    Text = _initialMessage ?? "Please wait...",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, theme?.ForeColor ?? Color.Black),
                    Location = new Point(20, 44),
                    Size = new Size(320, 20),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                dialog.Controls.Add(_messageLabel);

                // Percent label
                _percentLabel = new Label
                {
                    Text = _indeterminate ? "" : "0%",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = theme?.ForeColor ?? Color.Black,
                    Location = new Point(340, 44),
                    Size = new Size(40, 20),
                    TextAlign = ContentAlignment.MiddleRight,
                    Visible = !_indeterminate
                };
                dialog.Controls.Add(_percentLabel);

                // Progress bar
                _progressBar = new ProgressBar
                {
                    Location = new Point(20, 70),
                    Size = new Size(360, 20),
                    Style = _indeterminate ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous,
                    MarqueeAnimationSpeed = 30
                };
                dialog.Controls.Add(_progressBar);

                // Cancel button
                if (_cancellable)
                {
                    _cancelButton = new Button
                    {
                        Text = "Cancel",
                        Font = new Font("Segoe UI", 9, FontStyle.Regular),
                        Location = new Point(290, 100),
                        Size = new Size(90, 32),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(243, 244, 246),
                        ForeColor = Color.FromArgb(55, 65, 81)
                    };
                    _cancelButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                    _cancelButton.Click += (s, e) =>
                    {
                        _cts.Cancel();
                        if (_messageLabel != null)
                            _messageLabel.Text = "Cancelling...";
                        if (_cancelButton != null)
                            _cancelButton.Enabled = false;
                    };
                    dialog.Controls.Add(_cancelButton);
                }

                return dialog;
            }
        }

        #endregion

        #region Busy Overlay

        private class BusyOverlay : IDisposable
        {
            private readonly Form _hostForm;
            private readonly Panel _overlay;
            private readonly Timer _spinnerTimer;
            private int _spinnerAngle = 0;
            private bool _disposed;

            public BusyOverlay(Form hostForm, string? message)
            {
                _hostForm = hostForm;

                _overlay = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(180, 0, 0, 0)
                };

                // Spinner panel
                var spinnerPanel = new Panel
                {
                    Size = new Size(100, 100),
                    BackColor = Color.Transparent
                };
                spinnerPanel.Location = new Point(
                    (_hostForm.ClientSize.Width - spinnerPanel.Width) / 2,
                    (_hostForm.ClientSize.Height - spinnerPanel.Height) / 2 - 20
                );
                spinnerPanel.Paint += SpinnerPanel_Paint;

                _overlay.Controls.Add(spinnerPanel);

                // Message label
                if (!string.IsNullOrEmpty(message))
                {
                    var messageLabel = new Label
                    {
                        Text = message,
                        Font = new Font("Segoe UI", 11, FontStyle.Regular),
                        ForeColor = Color.White,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Size = new Size(300, 30),
                        BackColor = Color.Transparent
                    };
                    messageLabel.Location = new Point(
                        (_hostForm.ClientSize.Width - messageLabel.Width) / 2,
                        spinnerPanel.Bottom + 16
                    );
                    _overlay.Controls.Add(messageLabel);
                }

                // Animation timer
                _spinnerTimer = new Timer { Interval = 50 };
                _spinnerTimer.Tick += (s, e) =>
                {
                    _spinnerAngle = (_spinnerAngle + 30) % 360;
                    spinnerPanel.Invalidate();
                };
            }

            private void SpinnerPanel_Paint(object? sender, PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var center = new Point(50, 50);
                int radius = 30;
                int dotCount = 8;
                int dotSize = 8;

                for (int i = 0; i < dotCount; i++)
                {
                    double angle = (i * 360.0 / dotCount + _spinnerAngle) * Math.PI / 180;
                    int x = center.X + (int)(radius * Math.Cos(angle)) - dotSize / 2;
                    int y = center.Y + (int)(radius * Math.Sin(angle)) - dotSize / 2;

                    int alpha = (int)(255 * (1.0 - (double)i / dotCount));
                    using var brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
                    g.FillEllipse(brush, x, y, dotSize, dotSize);
                }
            }

            public void Show()
            {
                _hostForm.Controls.Add(_overlay);
                _overlay.BringToFront();
                _spinnerTimer.Start();
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _spinnerTimer.Stop();
                _spinnerTimer.Dispose();

                if (!_hostForm.IsDisposed)
                {
                    _hostForm.Controls.Remove(_overlay);
                }
                _overlay.Dispose();
            }
        }

        #endregion
    }
}

