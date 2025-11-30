using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Toast and notification methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Toast Types and Positions

        /// <summary>
        /// Toast notification type
        /// </summary>
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

        /// <summary>
        /// Toast position on screen
        /// </summary>
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

        /// <summary>
        /// Shows a toast notification
        /// </summary>
        public void Toast(string message, ToastType type = ToastType.Info, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
        {
            var toast = CreateToastForm(message, type, position);
            ShowToast(toast, durationMs);
        }

        /// <summary>
        /// Shows a success toast
        /// </summary>
        public void ToastSuccess(string message, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
        {
            Toast(message, ToastType.Success, durationMs, position);
        }

        /// <summary>
        /// Shows an error toast
        /// </summary>
        public void ToastError(string message, int durationMs = 5000, ToastPosition position = ToastPosition.TopRight)
        {
            Toast(message, ToastType.Error, durationMs, position);
        }

        /// <summary>
        /// Shows a warning toast
        /// </summary>
        public void ToastWarning(string message, int durationMs = 4000, ToastPosition position = ToastPosition.TopRight)
        {
            Toast(message, ToastType.Warning, durationMs, position);
        }

        /// <summary>
        /// Shows an info toast
        /// </summary>
        public void ToastInfo(string message, int durationMs = 3000, ToastPosition position = ToastPosition.TopRight)
        {
            Toast(message, ToastType.Info, durationMs, position);
        }

        /// <summary>
        /// Shows a loading toast that must be manually dismissed
        /// </summary>
        public IDisposable ToastLoading(string message, ToastPosition position = ToastPosition.TopCenter)
        {
            var toast = CreateToastForm(message, ToastType.Loading, position);
            ShowToast(toast, 0); // 0 = no auto-dismiss
            return new ToastHandle(toast, this);
        }

        #endregion

        #region Snackbar Methods

        /// <summary>
        /// Shows a snackbar notification with optional action
        /// </summary>
        public void Snackbar(string message, string? actionText = null, Action? action = null, int durationMs = 5000)
        {
            var snackbar = CreateSnackbarForm(message, actionText, action);
            ShowToast(snackbar, durationMs, ToastPosition.BottomCenter);
        }

        /// <summary>
        /// Shows a snackbar with undo action
        /// </summary>
        public void SnackbarUndo(string message, Action undoAction, int durationMs = 5000)
        {
            Snackbar(message, "UNDO", undoAction, durationMs);
        }

        #endregion

        #region Banner Methods

        /// <summary>
        /// Shows a banner notification at top of host form
        /// </summary>
        public IDisposable Banner(string message, ToastType type = ToastType.Info, bool dismissible = true)
        {
            if (_hostForm == null)
            {
                Toast(message, type);
                return new EmptyDisposable();
            }

            var banner = CreateBannerPanel(message, type, dismissible);
            _hostForm.Controls.Add(banner);
            banner.BringToFront();

            return new BannerHandle(banner, _hostForm);
        }

        #endregion

        #region Toast Form Creation

        private Form CreateToastForm(string message, ToastType type, ToastPosition position)
        {
            var colors = GetToastColors(type);
            var iconPath = GetToastIconPath(type);

            var toast = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = colors.Background,
                Size = new Size(320, 60),
                Opacity = 0
            };

            // Enable transparency (Form doesn't have SetStyle public, but we can skip this)

            // Create content panel with rounded corners
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Icon
            var iconLabel = new Label
            {
                Text = GetToastIconChar(type),
                Font = new Font("Segoe UI Symbol", 16, FontStyle.Regular),
                ForeColor = colors.Icon,
                Location = new Point(16, 18),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Message
            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = colors.Foreground,
                Location = new Point(48, 8),
                Size = new Size(256, 44),
                TextAlign = ContentAlignment.MiddleLeft
            };

            contentPanel.Controls.Add(iconLabel);
            contentPanel.Controls.Add(messageLabel);
            toast.Controls.Add(contentPanel);

            // Paint rounded background
            toast.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = GraphicsExtensions.GetRoundedRectPath(new Rectangle(0, 0, toast.Width, toast.Height), 8);
                using var brush = new SolidBrush(colors.Background);
                e.Graphics.FillPath(brush, path);

                // Border
                using var pen = new Pen(colors.Border, 1);
                e.Graphics.DrawPath(pen, path);
            };

            // Position
            PositionToast(toast, position);

            return toast;
        }

        private Form CreateSnackbarForm(string message, string? actionText, Action? action)
        {
            var snackbar = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = Color.FromArgb(50, 50, 50),
                Size = new Size(400, 48),
                Opacity = 0
            };

            // Message
            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(16, 0),
                Size = new Size(actionText != null ? 280 : 360, 48),
                TextAlign = ContentAlignment.MiddleLeft
            };
            snackbar.Controls.Add(messageLabel);

            // Action button
            if (!string.IsNullOrEmpty(actionText) && action != null)
            {
                var actionButton = new Label
                {
                    Text = actionText,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 180, 255),
                    Location = new Point(300, 0),
                    Size = new Size(80, 48),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                actionButton.Click += (s, e) =>
                {
                    action?.Invoke();
                    snackbar.Close();
                };
                actionButton.MouseEnter += (s, e) => actionButton.ForeColor = Color.FromArgb(150, 200, 255);
                actionButton.MouseLeave += (s, e) => actionButton.ForeColor = Color.FromArgb(100, 180, 255);
                snackbar.Controls.Add(actionButton);
            }

            // Paint rounded background
            snackbar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = GraphicsExtensions.GetRoundedRectPath(new Rectangle(0, 0, snackbar.Width, snackbar.Height), 4);
                using var brush = new SolidBrush(Color.FromArgb(50, 50, 50));
                e.Graphics.FillPath(brush, path);
            };

            return snackbar;
        }

        private Panel CreateBannerPanel(string message, ToastType type, bool dismissible)
        {
            var colors = GetToastColors(type);

            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = colors.Background
            };

            // Icon
            var iconLabel = new Label
            {
                Text = GetToastIconChar(type),
                Font = new Font("Segoe UI Symbol", 14, FontStyle.Regular),
                ForeColor = colors.Icon,
                Location = new Point(16, 10),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Message
            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = colors.Foreground,
                Location = new Point(48, 0),
                Size = new Size(banner.Width - 100, 44),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            banner.Controls.Add(iconLabel);
            banner.Controls.Add(messageLabel);

            // Dismiss button
            if (dismissible)
            {
                var dismissButton = new Label
                {
                    Text = "✕",
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    ForeColor = colors.Foreground,
                    Location = new Point(banner.Width - 40, 10),
                    Size = new Size(24, 24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                dismissButton.Click += (s, e) =>
                {
                    banner.Parent?.Controls.Remove(banner);
                    banner.Dispose();
                };
                banner.Controls.Add(dismissButton);
            }

            return banner;
        }

        #endregion

        #region Toast Display

        private void ShowToast(Form toast, int durationMs, ToastPosition position = ToastPosition.TopRight)
        {
            lock (_toastLock)
            {
                _activeToasts.Add(toast);
            }

            toast.Show();

            // Fade in
            var fadeInTimer = new Timer { Interval = 20 };
            int fadeInStep = 0;
            fadeInTimer.Tick += (s, e) =>
            {
                fadeInStep++;
                toast.Opacity = Math.Min(1.0, fadeInStep * 0.1);
                if (fadeInStep >= 10)
                {
                    fadeInTimer.Stop();
                    fadeInTimer.Dispose();
                }
            };
            fadeInTimer.Start();

            // Auto-dismiss
            if (durationMs > 0)
            {
                var dismissTimer = new Timer { Interval = durationMs };
                dismissTimer.Tick += (s, e) =>
                {
                    dismissTimer.Stop();
                    dismissTimer.Dispose();
                    DismissToast(toast);
                };
                dismissTimer.Start();
            }
        }

        private void DismissToast(Form toast)
        {
            if (toast == null || toast.IsDisposed)
                return;

            // Fade out
            var fadeOutTimer = new Timer { Interval = 20 };
            int fadeOutStep = 10;
            fadeOutTimer.Tick += (s, e) =>
            {
                fadeOutStep--;
                if (fadeOutStep <= 0)
                {
                    fadeOutTimer.Stop();
                    fadeOutTimer.Dispose();

                    lock (_toastLock)
                    {
                        _activeToasts.Remove(toast);
                    }

                    toast.Close();
                    toast.Dispose();
                }
                else
                {
                    toast.Opacity = fadeOutStep * 0.1;
                }
            };
            fadeOutTimer.Start();
        }

        private void PositionToast(Form toast, ToastPosition position)
        {
            var screen = _hostForm != null
                ? Screen.FromControl(_hostForm)
                : Screen.PrimaryScreen;

            var workingArea = screen.WorkingArea;
            int margin = 16;

            // Count existing toasts at this position for stacking
            int stackOffset = 0;
            lock (_toastLock)
            {
                stackOffset = _activeToasts.Count * (toast.Height + 8);
            }

            toast.Location = position switch
            {
                ToastPosition.TopLeft => new Point(workingArea.Left + margin, workingArea.Top + margin + stackOffset),
                ToastPosition.TopCenter => new Point(workingArea.Left + (workingArea.Width - toast.Width) / 2, workingArea.Top + margin + stackOffset),
                ToastPosition.TopRight => new Point(workingArea.Right - toast.Width - margin, workingArea.Top + margin + stackOffset),
                ToastPosition.BottomLeft => new Point(workingArea.Left + margin, workingArea.Bottom - toast.Height - margin - stackOffset),
                ToastPosition.BottomCenter => new Point(workingArea.Left + (workingArea.Width - toast.Width) / 2, workingArea.Bottom - toast.Height - margin - stackOffset),
                ToastPosition.BottomRight => new Point(workingArea.Right - toast.Width - margin, workingArea.Bottom - toast.Height - margin - stackOffset),
                _ => new Point(workingArea.Right - toast.Width - margin, workingArea.Top + margin + stackOffset)
            };
        }

        #endregion

        #region Toast Colors and Icons

        private (Color Background, Color Foreground, Color Icon, Color Border) GetToastColors(ToastType type)
        {
            var theme = _defaultTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;

            return type switch
            {
                ToastType.Success => (
                    Color.FromArgb(240, 253, 244),
                    Color.FromArgb(22, 101, 52),
                    Color.FromArgb(34, 197, 94),
                    Color.FromArgb(187, 247, 208)
                ),
                ToastType.Error => (
                    Color.FromArgb(254, 242, 242),
                    Color.FromArgb(153, 27, 27),
                    Color.FromArgb(239, 68, 68),
                    Color.FromArgb(254, 202, 202)
                ),
                ToastType.Warning => (
                    Color.FromArgb(255, 251, 235),
                    Color.FromArgb(146, 64, 14),
                    Color.FromArgb(245, 158, 11),
                    Color.FromArgb(253, 230, 138)
                ),
                ToastType.Info => (
                    Color.FromArgb(239, 246, 255),
                    Color.FromArgb(30, 64, 175),
                    Color.FromArgb(59, 130, 246),
                    Color.FromArgb(191, 219, 254)
                ),
                ToastType.Loading => (
                    Color.FromArgb(249, 250, 251),
                    Color.FromArgb(55, 65, 81),
                    Color.FromArgb(107, 114, 128),
                    Color.FromArgb(229, 231, 235)
                ),
                _ => (
                    Color.FromArgb(249, 250, 251),
                    Color.FromArgb(55, 65, 81),
                    Color.FromArgb(107, 114, 128),
                    Color.FromArgb(229, 231, 235)
                )
            };
        }

        private string GetToastIconChar(ToastType type)
        {
            return type switch
            {
                ToastType.Success => "✓",
                ToastType.Error => "✕",
                ToastType.Warning => "⚠",
                ToastType.Info => "ℹ",
                ToastType.Loading => "◌",
                _ => "ℹ"
            };
        }

        private string GetToastIconPath(ToastType type)
        {
            return type switch
            {
                ToastType.Success => "TheTechIdea.Beep.Winform.GFX.SVG.check.svg",
                ToastType.Error => "TheTechIdea.Beep.Winform.GFX.SVG.error.svg",
                ToastType.Warning => "TheTechIdea.Beep.Winform.GFX.SVG.warning.svg",
                ToastType.Info => "TheTechIdea.Beep.Winform.GFX.SVG.information.svg",
                ToastType.Loading => "TheTechIdea.Beep.Winform.GFX.SVG.loading.svg",
                _ => "TheTechIdea.Beep.Winform.GFX.SVG.information.svg"
            };
        }

        #endregion

        #region IDialogManager Implementation (Notifications)

        void IDialogManager.ShowToast(string message, int durationMs, string? icon)
        {
            var type = icon?.ToLower() switch
            {
                "success" => ToastType.Success,
                "error" => ToastType.Error,
                "warning" => ToastType.Warning,
                _ => ToastType.Info
            };
            Toast(message, type, durationMs);
        }

        #endregion

        #region Helper Classes

        private class ToastHandle : IDisposable
        {
            private readonly Form _toast;
            private readonly BeepDialogManager _manager;

            public ToastHandle(Form toast, BeepDialogManager manager)
            {
                _toast = toast;
                _manager = manager;
            }

            public void Dispose()
            {
                _manager.DismissToast(_toast);
            }
        }

        private class BannerHandle : IDisposable
        {
            private readonly Panel _banner;
            private readonly Form _hostForm;

            public BannerHandle(Panel banner, Form hostForm)
            {
                _banner = banner;
                _hostForm = hostForm;
            }

            public void Dispose()
            {
                if (_banner != null && !_banner.IsDisposed)
                {
                    _hostForm?.Controls.Remove(_banner);
                    _banner.Dispose();
                }
            }
        }

        private class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        #endregion
    }
}

