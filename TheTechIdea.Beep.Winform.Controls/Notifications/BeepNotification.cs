using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Visual notification form based on BeepiFormPro
    /// Displays toast/notification with icon, title, message, action buttons, and progress bar
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern notification/toast form with auto-dismiss and action buttons")]
    public class BeepNotification : BeepiFormPro
    {
        #region Private Fields
        private NotificationData _notificationData;
        private Timer _autoDismissTimer;
        private Timer _progressTimer;
        private float _progressPercentage = 100f;
        private bool _isPaused = false;
        private DateTime _startTime;
        private int _remainingDuration;
        private string _iconPath;
        private Color _iconTint = Color.Gray;
        private Rectangle _iconRect;
        private Rectangle _titleRect;
        private Rectangle _messageRect;
        private Rectangle _closeButtonRect;
        private Rectangle _progressBarRect;
        private Rectangle _actionsRect;
        private const int ICON_SIZE = 24;
        private const int CLOSE_BUTTON_SIZE = 20;
        private const int PROGRESS_BAR_HEIGHT = 4;
        private const int ACTION_BUTTON_HEIGHT = 32;
        private const int PADDING = 12;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepNotification
        /// </summary>
        public BeepNotification()
        {
            _notificationData = new NotificationData();
            
            // Initialize timers
            _autoDismissTimer = new Timer { Interval = 100 };
            _autoDismissTimer.Tick += AutoDismissTimer_Tick;
            
            _progressTimer = new Timer { Interval = 50 };
            _progressTimer.Tick += ProgressTimer_Tick;

            // BeepiFormPro configuration
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false;
            FormStyle = BeepThemesManager.CurrentStyle;
            
            // Size configuration
            MinimumSize = new Size(280, 60);
            MaximumSize = new Size(420, 300);
            Size = new Size(350, 80);
            
            // Enable opacity for fade animations
            Opacity = 1.0;

            // Mouse events for pause on hover
            MouseEnter += BeepNotification_MouseEnter;
            MouseLeave += BeepNotification_MouseLeave;
           
            // Keyboard support
            this.KeyPreview = true;
            this.TabStop = true;
            this.KeyDown += BeepNotification_KeyDown;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Notification data model
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NotificationData NotificationData
        {
            get => _notificationData;
            set
            {
                _notificationData = value ?? new NotificationData();
                UpdateNotificationVisuals();
                Invalidate();
            }
        }

        /// <summary>
        /// Notification title
        /// </summary>
        [Category("Appearance")]
        [Description("The title text of the notification")]
        public string Title
        {
            get => _notificationData?.Title;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Title = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification message
        /// </summary>
        [Category("Appearance")]
        [Description("The message text of the notification")]
        public string Message
        {
            get => _notificationData?.Message;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Message = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification type
        /// </summary>
        [Category("Appearance")]
        [Description("The type of notification (Info, Success, Warning, Error, etc.)")]
        [DefaultValue(NotificationType.Info)]
        public NotificationType NotificationType
        {
            get => _notificationData?.Type ?? NotificationType.Info;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Type = value;
                    UpdateNotificationVisuals();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification layout style
        /// </summary>
        [Category("Appearance")]
        [Description("The layout style of the notification")]
        [DefaultValue(NotificationLayout.Standard)]
        public NotificationLayout LayoutStyle
        {
            get => _notificationData?.Layout ?? NotificationLayout.Standard;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Layout = value;
                    RecalculateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Auto-dismiss duration in milliseconds (0 = no auto-dismiss)
        /// </summary>
        [Category("Behavior")]
        [Description("Duration before auto-dismiss in milliseconds (0 = no auto-dismiss)")]
        [DefaultValue(5000)]
        public int Duration
        {
            get => _notificationData?.Duration ?? 5000;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Duration = value;
                }
            }
        }

        /// <summary>
        /// Show close button
        /// </summary>
        [Category("Appearance")]
        [Description("Show close button")]
        [DefaultValue(true)]
        public new bool ShowCloseButton
        {
            get => _notificationData?.ShowCloseButton ?? true;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.ShowCloseButton = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Show progress bar
        /// </summary>
        [Category("Appearance")]
        [Description("Show progress bar for auto-dismiss countdown")]
        [DefaultValue(true)]
        public bool ShowProgressBar
        {
            get => _notificationData?.ShowProgressBar ?? true;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.ShowProgressBar = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Raised when the notification is dismissed
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationDismissed;

        /// <summary>
        /// Raised when an action button is clicked
        /// </summary>
        public event EventHandler<NotificationEventArgs> ActionClicked;

        /// <summary>
        /// Raised when the notification is clicked (not action button)
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationClicked;
        #endregion

        #region Public Methods

        /// <summary>
        /// Show the notification and start auto-dismiss timer if duration > 0
        /// </summary>
        public new void Show()
        {
            base.Show();
            _progressPercentage = 100f;

            if (_notificationData != null && _notificationData.Duration > 0)
            {
                _startTime = DateTime.Now;
                _remainingDuration = _notificationData.Duration;
                _autoDismissTimer.Start();
                
                if (_notificationData.ShowProgressBar)
                {
                    _progressTimer.Start();
                }
            }
        }

        /// <summary>
        /// Dismiss the notification
        /// </summary>
        public void Dismiss()
        {
            StopTimers();
            
            var args = new NotificationEventArgs
            {
                Notification = _notificationData
            };
            
            NotificationDismissed?.Invoke(this, args);
            
            if (!args.Cancel)
            {
                Visible = false;
            }
        }

        /// <summary>
        /// Pause auto-dismiss timer
        /// </summary>
        public void Pause()
        {
            if (!_isPaused && _autoDismissTimer.Enabled)
            {
                _isPaused = true;
                var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
                _remainingDuration = Math.Max(0, _notificationData.Duration - (int)elapsed);
                _autoDismissTimer.Stop();
                _progressTimer.Stop();
            }
        }

        /// <summary>
        /// Resume auto-dismiss timer
        /// </summary>
        public void Resume()
        {
            if (_isPaused && _remainingDuration > 0)
            {
                _isPaused = false;
                _startTime = DateTime.Now;
                _autoDismissTimer.Start();
                _progressTimer.Start();
            }
        }
        #endregion

        #region Private Methods - Timers

        private void AutoDismissTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused)
                return;

            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            
            if (elapsed >= _remainingDuration)
            {
                Dismiss();
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused || _notificationData == null || _notificationData.Duration <= 0)
                return;

            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            _progressPercentage = Math.Max(0, 100f - (float)(elapsed / _notificationData.Duration * 100));
            Invalidate();
        }

        private void StopTimers()
        {
            _autoDismissTimer?.Stop();
            _progressTimer?.Stop();
            _isPaused = false;
        }

        private void BeepNotification_MouseEnter(object sender, EventArgs e)
        {
            if (_notificationData != null && _notificationData.PauseOnHover)
            {
                Pause();
            }
        }

        private void BeepNotification_MouseLeave(object sender, EventArgs e)
        {
            if (_notificationData != null && _notificationData.PauseOnHover)
            {
                Resume();
            }
        }
        #endregion

        #region Private Methods - Visuals

        private void UpdateNotificationVisuals()
        {
            if (_notificationData == null)
                return;

            // Update colors based on type
            var colors = GetColorsForType(_notificationData.Type);
            
            if (_notificationData.CustomBackColor.HasValue)
            {
                BackColor = _notificationData.CustomBackColor.Value;
            }
            else
            {
                BackColor = colors.BackColor;
            }

            if (_notificationData.CustomForeColor.HasValue)
            {
                ForeColor = _notificationData.CustomForeColor.Value;
            }
            else
            {
                ForeColor = colors.ForeColor;
            }

            BorderColor = colors.BorderColor;

            // Update icon path and tint
            _iconPath = !string.IsNullOrEmpty(_notificationData.IconPath)
                ? _notificationData.IconPath
                : NotificationData.GetDefaultIconForType(_notificationData.Type);

            _iconTint = _notificationData.IconTint ?? colors.IconColor;
        }

        private (Color BackColor, Color ForeColor, Color BorderColor, Color IconColor) GetColorsForType(NotificationType type)
        {
            // Try to use BeepStyling if available, otherwise use defaults
            var style = BeepStyling.CurrentControlStyle;
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = theme.BackColor;
            ForeColor = theme.ForeColor;
            BorderColor = theme.BorderColor;
         ;
            return (BackColor, ForeColor, BorderColor, theme.AccentColor);
        }

        private void RecalculateLayout()
        {
            if (_notificationData == null)
                return;

            // Use ClientRectangle for the content area
            Rectangle content = ClientRectangle;
            if (content.IsEmpty || content.Width <= 0 || content.Height <= 0)
                return;

            int x = content.X + PADDING;
            int y = content.Y + PADDING;
            int availableWidth = content.Width - (PADDING * 2);

            // Close button (top-right)
            if (_notificationData.ShowCloseButton)
            {
                _closeButtonRect = new Rectangle(
                    content.Right - PADDING - CLOSE_BUTTON_SIZE,
                    y,
                    CLOSE_BUTTON_SIZE,
                    CLOSE_BUTTON_SIZE
                );
                availableWidth -= (CLOSE_BUTTON_SIZE + PADDING);
            }
            else
            {
                _closeButtonRect = Rectangle.Empty;
            }

            // Icon (left side)
            bool hasIcon = !string.IsNullOrEmpty(_notificationData.IconPath) ||
                          _notificationData.Type != NotificationType.Custom;

            if (hasIcon && _notificationData.Layout != NotificationLayout.Compact)
            {
                _iconRect = new Rectangle(x, y, ICON_SIZE, ICON_SIZE);
                x += ICON_SIZE + PADDING;
                availableWidth -= (ICON_SIZE + PADDING);
            }
            else
            {
                _iconRect = Rectangle.Empty;
            }

            // Title and message
            int textX = x;
            int textWidth = availableWidth;

            if (!string.IsNullOrEmpty(_notificationData.Title))
            {
                using (var g = CreateGraphics())
                {
                    var titleFont = new Font(Font.FontFamily, Font.Size + 2, FontStyle.Bold);
                    var titleSize = TextRenderer.MeasureText(g, _notificationData.Title, titleFont,
                        new Size(textWidth, int.MaxValue), TextFormatFlags.WordBreak);

                    _titleRect = new Rectangle(textX, y, textWidth, titleSize.Height);
                    y += titleSize.Height + 4;
                }
            }
            else
            {
                _titleRect = Rectangle.Empty;
            }

            // Message
            if (!string.IsNullOrEmpty(_notificationData.Message))
            {
                using (var g = CreateGraphics())
                {
                    var messageSize = TextRenderer.MeasureText(g, _notificationData.Message, Font,
                        new Size(textWidth, int.MaxValue), TextFormatFlags.WordBreak);

                    _messageRect = new Rectangle(textX, y, textWidth, messageSize.Height);
                    y += messageSize.Height + PADDING;
                }
            }
            else
            {
                _messageRect = Rectangle.Empty;
            }

            // Action buttons
            if (_notificationData.Actions != null && _notificationData.Actions.Length > 0)
            {
                _actionsRect = new Rectangle(textX, y, textWidth, ACTION_BUTTON_HEIGHT);
                y += ACTION_BUTTON_HEIGHT + PADDING;
            }
            else
            {
                _actionsRect = Rectangle.Empty;
            }

            // Progress bar (bottom) - positioned at bottom of client area
            if (_notificationData.ShowProgressBar && _notificationData.Duration > 0)
            {
                _progressBarRect = new Rectangle(
                    content.X,
                    content.Bottom - PROGRESS_BAR_HEIGHT,
                    content.Width,
                    PROGRESS_BAR_HEIGHT
                );
            }
            else
            {
                _progressBarRect = Rectangle.Empty;
            }

            // Adjust height if needed
            int requiredHeight = y - content.Y + PADDING;
            if (_notificationData.ShowProgressBar)
                requiredHeight += PROGRESS_BAR_HEIGHT;

            if (Height < requiredHeight)
            {
                Height = Math.Min(requiredHeight, MaximumSize.Height);
            }
        }
        #endregion

        #region Override - Drawing

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalculateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_notificationData == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            RecalculateLayout();

            // Draw icon using static StyledImagePainter
            if (!_iconRect.IsEmpty && !string.IsNullOrEmpty(_iconPath))
            {
                StyledImagePainter.PaintWithTint(g, _iconRect, _iconPath, _iconTint, 1f, 0);
            }

            // Draw close button
            if (!_closeButtonRect.IsEmpty)
            {
                DrawCloseButton(g, _closeButtonRect);
            }

            // Draw title
            if (!_titleRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Title))
            {
                using (var titleFont = new Font(Font.FontFamily, Font.Size + 2, FontStyle.Bold))
                using (var brush = new SolidBrush(ForeColor))
                {
                    TextRenderer.DrawText(g, _notificationData.Title, titleFont, _titleRect,
                        ForeColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
                }
            }

            // Draw message
            if (!_messageRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Message))
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, ForeColor)))
                {
                    TextRenderer.DrawText(g, _notificationData.Message, Font, _messageRect,
                        Color.FromArgb(180, ForeColor), TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
                }
            }

            // Draw action buttons
            if (!_actionsRect.IsEmpty && _notificationData.Actions != null)
            {
                DrawActionButtons(g, _actionsRect);
            }

            // Draw progress bar
            if (!_progressBarRect.IsEmpty)
            {
                DrawProgressBar(g, _progressBarRect);
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle rect)
        {
            var hoverColor = Color.FromArgb(200, ForeColor);
            var closeColor = _closeButtonRect.Contains(PointToClient(Cursor.Position))
                ? hoverColor
                : ForeColor;

            using (var pen = new Pen(closeColor, 2))
            {
                int padding = 6;
                g.DrawLine(pen,
                    rect.X + padding, rect.Y + padding,
                    rect.Right - padding, rect.Bottom - padding);
                g.DrawLine(pen,
                    rect.Right - padding, rect.Y + padding,
                    rect.X + padding, rect.Bottom - padding);
            }
        }

        private void DrawActionButtons(Graphics g, Rectangle rect)
        {
            if (_notificationData.Actions == null || _notificationData.Actions.Length == 0)
                return;

            int buttonWidth = (rect.Width - (PADDING * (_notificationData.Actions.Length - 1))) / _notificationData.Actions.Length;
            int x = rect.X;

            foreach (var action in _notificationData.Actions)
            {
                var buttonRect = new Rectangle(x, rect.Y, buttonWidth, rect.Height);
                DrawActionButton(g, buttonRect, action);
                x += buttonWidth + PADDING;
            }
        }

        private void DrawActionButton(Graphics g, Rectangle rect, NotificationAction action)
        {
            var buttonColor = action.CustomColor ?? (action.IsPrimary ? ForeColor : Color.FromArgb(150, ForeColor));
            var isHovered = rect.Contains(PointToClient(Cursor.Position));

            if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, ForeColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            using (var borderPen = new Pen(buttonColor, 1))
            {
                g.DrawRectangle(borderPen, rect);
            }

            TextRenderer.DrawText(g, action.Text, Font, rect, buttonColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void DrawProgressBar(Graphics g, Rectangle rect)
        {
            // Background
            using (var brush = new SolidBrush(Color.FromArgb(50, ForeColor)))
            {
                g.FillRectangle(brush, rect);
            }

            // Progress
            int progressWidth = (int)(rect.Width * (_progressPercentage / 100f));
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, progressWidth, rect.Height);
                var colors = GetColorsForType(_notificationData.Type);
                
                using (var brush = new SolidBrush(colors.IconColor))
                {
                    g.FillRectangle(brush, progressRect);
                }
            }
        }
        #endregion

        #region Override - Mouse Handling

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Check close button
            if (_closeButtonRect.Contains(e.Location))
            {
                Dismiss();
                return;
            }

            // Check action buttons
            if (!_actionsRect.IsEmpty && _notificationData.Actions != null)
            {
                int buttonWidth = (_actionsRect.Width - (PADDING * (_notificationData.Actions.Length - 1))) / _notificationData.Actions.Length;
                int x = _actionsRect.X;

                for (int i = 0; i < _notificationData.Actions.Length; i++)
                {
                    var buttonRect = new Rectangle(x, _actionsRect.Y, buttonWidth, _actionsRect.Height);
                    if (buttonRect.Contains(e.Location))
                    {
                        var action = _notificationData.Actions[i];
                        var args = new NotificationEventArgs
                        {
                            Notification = _notificationData,
                            Action = action
                        };

                        ActionClicked?.Invoke(this, args);
                        action.OnClick?.Invoke(_notificationData);
                        return;
                    }
                    x += buttonWidth + PADDING;
                }
            }

            // General notification click
            NotificationClicked?.Invoke(this, new NotificationEventArgs { Notification = _notificationData });
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Redraw if hovering over interactive elements
            bool needsRedraw = _closeButtonRect.Contains(e.Location) ||
                             (!_actionsRect.IsEmpty && _actionsRect.Contains(e.Location));
            
            if (needsRedraw)
            {
                Invalidate();
            }
        }

        private void BeepNotification_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    // Escape key dismisses notification
                    Dismiss();
                    e.Handled = true;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    // Enter/Space triggers primary action or dismisses
                    if (_notificationData?.Actions != null && _notificationData.Actions.Length > 0)
                    {
                        var primaryAction = Array.Find(_notificationData.Actions, a => a.IsPrimary) 
                                          ?? _notificationData.Actions[0];
                        
                        var args = new NotificationEventArgs
                        {
                            Notification = _notificationData,
                            Action = primaryAction
                        };

                        ActionClicked?.Invoke(this, args);
                        primaryAction.OnClick?.Invoke(_notificationData);
                    }
                    else
                    {
                        Dismiss();
                    }
                    e.Handled = true;
                    break;

                case Keys.D1:
                case Keys.NumPad1:
                    TriggerActionByIndex(0);
                    e.Handled = true;
                    break;

                case Keys.D2:
                case Keys.NumPad2:
                    TriggerActionByIndex(1);
                    e.Handled = true;
                    break;

                case Keys.D3:
                case Keys.NumPad3:
                    TriggerActionByIndex(2);
                    e.Handled = true;
                    break;
            }
        }

        private void TriggerActionByIndex(int index)
        {
            if (_notificationData?.Actions == null || index >= _notificationData.Actions.Length)
                return;

            var action = _notificationData.Actions[index];
            var args = new NotificationEventArgs
            {
                Notification = _notificationData,
                Action = action
            };

            ActionClicked?.Invoke(this, args);
            action.OnClick?.Invoke(_notificationData);
        }
        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopTimers();
                _autoDismissTimer?.Dispose();
                _progressTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
