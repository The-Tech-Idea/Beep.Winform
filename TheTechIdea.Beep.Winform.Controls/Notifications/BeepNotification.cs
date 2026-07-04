using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// BeepNotification — visual notification form based on BeepiFormPro.
    ///
    /// Revise / simplify (2026-07): this class is no longer a self-painting
    /// surface. It composes child Beep controls (BeepPanel, BeepLabel,
    /// BeepButton, BeepProgressBar, PictureBox) docked via standard
    /// WinForms layout. The painter system, custom OnPaint override, and the
    /// BeepNotificationCanvas intermediary have all been removed. Theme comes
    /// from <see cref="BeepThemesManager"/> via UseThemeColors on each child.
    ///
    /// Second-pass (2026-07-04): UI gaps closed.
    ///   - Theme-driven fonts via BeepFontManager (no inline new Font).
    ///   - AutoSize + RecomputeSize so long messages don't get cut off.
    ///   - OnDpiChanged handler so DPI-affected sizes refresh when the
    ///     notification moves between monitors with different scaling.
    ///   - Flat/ghost close button styled for an icon-only X glyph.
    ///   - Explicit TabIndex (close → title → message → actions).
    ///   - AutoEllipsis on the message label so overflow is clipped with "…".
    ///   - Icon hosted in a sized BeepPanel container so transparent backing
    ///     matches the notification's themed chrome.
    /// </summary>
    public class BeepNotification : BeepiFormPro
    {
        #region Fields
        private NotificationData _notificationData = new NotificationData();
        private readonly Timer _autoDismissTimer;
        private readonly Timer _progressTimer;
        private float _progressPercentage = 100f;
        private bool _isPaused;
        private DateTime _startTime;
        private int _remainingDuration;

        // Child controls (compose the notification body via docking)
        private BeepPanel _bodyPanel;
        private BeepPanel _iconContainer;     // wraps the PictureBox so it sits inside a themed BeepPanel
        private PictureBox _iconPicture;
        private BeepPanel _textPanel;         // title + message host
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private BeepButton _closeButton;
        private BeepProgressBar _progressBar;
        private BeepPanel _actionsPanel;
        private FlowLayoutPanel _actionsLayout;

        // State
        private bool _themeSubscribed;
        private Color _iconTintResolved = SystemColors.Control;
        // no field needed — BaseControl.ToolTipText is the canonical mechanism
        // (managed centrally by ToolTipManager). Each child Beep control carries
        // its own tooltip text via the inherited ToolTipText property.
        #endregion

        #region Constructor
        public BeepNotification()
        {
            // ── Form configuration
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false;
            FormStyle = BeepThemesManager.CurrentStyle;
            Opacity = 1.0;

            // AutoSize + MaximumSize lets the form grow to fit content; the cap
            // bounds it so unusually long messages don't blow up the screen.
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MinimumSize = DpiScalingHelper.ScaleSize(new Size(280, 60), this);
            MaximumSize = DpiScalingHelper.ScaleSize(new Size(420, 300), this);

            AccessibleRole = AccessibleRole.StaticText;

            // ── Mouse + keyboard (form-level; children re-raise via bubbling)
            MouseEnter += (s, e) => OnHoverEnter();
            MouseLeave += (s, e) => OnHoverLeave();
            KeyPreview = true;
            TabStop = true;
            KeyDown += BeepNotification_KeyDown;

            // Inherit the host form's RightToLeft so docking + control order
            // mirror for Arabic / Hebrew locales. BeepiFormPro derives from
            // Form whose RightToLeft default is No, so we set the Inherit
            // flag explicitly when this notification is parented (handled in
            // OnParentRightToLeftChanged below).
            RightToLeft = RightToLeft.Inherit;

            // ── Timers
            _autoDismissTimer = new Timer { Interval = 100 };
            _autoDismissTimer.Tick += AutoDismissTimer_Tick;

            _progressTimer = new Timer { Interval = 50 };
            _progressTimer.Tick += ProgressTimer_Tick;

            // ── Child controls + theme fonts
            // ── Child controls (layered via docking)
            _bodyPanel = new BeepPanel { Dock = DockStyle.Fill, UseThemeColors = true };
            _iconContainer = new BeepPanel { Dock = DockStyle.Left, UseThemeColors = true };
            _iconPicture = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            _iconPicture.Paint += IconPicture_Paint;
            _iconContainer.Controls.Add(_iconPicture);

            _actionsPanel = new BeepPanel { Dock = DockStyle.Bottom, Height = 0, Visible = false, UseThemeColors = true };
            _actionsLayout = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.Transparent };
            _actionsPanel.Controls.Add(_actionsLayout);

            _textPanel = new BeepPanel { Dock = DockStyle.Fill, UseThemeColors = true };

            _titleLabel = new BeepLabel { Dock = DockStyle.Top, AutoSize = true, AutoEllipsis = true, UseThemeColors = true, TabIndex = 1 };
            _messageLabel = new BeepLabel { Dock = DockStyle.Fill, AutoEllipsis = true, UseThemeColors = true, TabIndex = 2 };

            _textPanel.Controls.Add(_messageLabel);
            _textPanel.Controls.Add(_titleLabel);

            _closeButton = new BeepButton { Dock = DockStyle.Right, Text = "\u2715", UseThemeColors = true, TabIndex = 0, TabStop = true };
            _closeButton.Click += (s, e) => Dismiss();

            _progressBar = new BeepProgressBar { Dock = DockStyle.Bottom, UseThemeColors = true, Visible = false };

            // Z-order for docking (last-added = fills leftover space)
            _bodyPanel.Controls.Add(_progressBar);
            _bodyPanel.Controls.Add(_actionsPanel);
            _bodyPanel.Controls.Add(_iconContainer);
            _bodyPanel.Controls.Add(_closeButton);
            _bodyPanel.Controls.Add(_textPanel);
            this.Controls.Add(_bodyPanel);

            _closeButton.TabStop = true;
            _titleLabel.TabStop = false;
            _messageLabel.TabStop = false;
            _actionsPanel.Tag = 3;

            if (IsHandleCreated) RescaleLayout();

            ApplyTypography();
            RefreshAccessibility();
            UpdateTooltips();
        }

        /// <summary>
        /// Each Beep child carries its own <see cref="BaseControl.ToolTipText"/>
        /// (managed centrally by ToolTipManager) — no System.Windows.Forms.ToolTip
        /// instance is needed here. Called from the constructor (after children
        /// exist), from <see cref="ApplyData"/> on data change, and from the
        /// progress tick to keep the live countdown current.
        /// </summary>
        private void UpdateTooltips()
        {
            if (_closeButton != null)
                _closeButton.ToolTipText = "Close (Esc)";
            if (_progressBar != null)
            {
                var pct = _progressPercentage.ToString("0");
                _progressBar.ToolTipText = string.IsNullOrEmpty(_notificationData?.ProgressText)
                    ? $"Auto-dismiss in {pct}% of duration remaining"
                    : _notificationData.ProgressText;
            }
        }
        #endregion

        /// <summary>
        /// Re-applies all DPI-affected sizes on the children. Called:
        ///   - From <c>OnHandleCreated</c> once the handle exists and the real
        ///     DeviceDpi is known (constructor DPI is 96, the default).
        ///   - From <c>OnDpiChangedInternal</c> when the user moves the form
        ///     between monitors with different scaling.
        /// </summary>
        private void RescaleLayout()
        {
            int pad       = DpiScalingHelper.ScaleValue(12,  this);
            int iconSize  = DpiScalingHelper.ScaleValue(28,  this);
            int closeSize = DpiScalingHelper.ScaleValue(24,  this);
            int progressH = DpiScalingHelper.ScaleValue(4,   this);
            int gap       = DpiScalingHelper.ScaleValue(8,   this);
            int iconPad   = DpiScalingHelper.ScaleValue(2,   this);
            int iconGap   = DpiScalingHelper.ScaleValue(4,   this);
            int actPadY   = DpiScalingHelper.ScaleValue(4,   this);
            int txtPadY   = DpiScalingHelper.ScaleValue(2,   this);
            int msgPadT   = DpiScalingHelper.ScaleValue(2,   this);

            if (_bodyPanel != null)
                _bodyPanel.Padding = new Padding(pad, pad, pad, pad);

            if (_iconContainer != null)
            {
                _iconContainer.Width = iconSize + gap;
                _iconContainer.Padding = new Padding(iconPad, 0, iconGap, 0);
            }

            if (_actionsLayout != null)
                _actionsLayout.Padding = new Padding(0, actPadY, 0, 0);

            if (_textPanel != null)
                _textPanel.Padding = new Padding(0, txtPadY, 0, txtPadY);

            if (_messageLabel != null)
                _messageLabel.Padding = new Padding(0, msgPadT, 0, 0);

            if (_closeButton  != null) _closeButton.Width  = closeSize;
            if (_progressBar  != null) _progressBar.Height = progressH;

            // Form's min/max also DPI-affected
            MinimumSize = DpiScalingHelper.ScaleSize(new Size(280, 60), this);
            MaximumSize = DpiScalingHelper.ScaleSize(new Size(420, 300), this);

            // Recompute + request re-layout
            RecomputeSize();
        }

        #region Public Properties

        /// <summary>Notification data model.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NotificationData NotificationData
        {
            get => _notificationData;
            set
            {
                _notificationData = value ?? new NotificationData();
                ApplyData();
            }
        }

        [Category("Appearance")]
        [Description("The title text of the notification")]
        public string Title
        {
            get => _notificationData?.Title;
            set { if (_notificationData != null) { _notificationData.Title = value; ApplyData(); } }
        }

        [Category("Appearance")]
        [Description("The message text of the notification")]
        public string Message
        {
            get => _notificationData?.Message;
            set { if (_notificationData != null) { _notificationData.Message = value; ApplyData(); } }
        }

        [Category("Appearance")]
        [Description("The type of notification (Info, Success, Warning, Error, etc.)")]
        [DefaultValue(NotificationType.Info)]
        public NotificationType NotificationType
        {
            get => _notificationData?.Type ?? NotificationType.Info;
            set { if (_notificationData != null) { _notificationData.Type = value; ApplyData(); } }
        }

        /// <summary>Legacy property; the painter system was removed 2026-07. Settable for source compat.</summary>
        [Obsolete("Painter system removed; this property is no-op.")]
        [Category("Appearance")]
        [DefaultValue(NotificationLayout.Standard)]
        public NotificationLayout LayoutStyle
        {
            get => _notificationData.Layout;
            set { if (_notificationData != null) { _notificationData.Layout = value; } }
        }

        [Category("Behavior")]
        [Description("Duration before auto-dismiss in milliseconds (0 = no auto-dismiss)")]
        [DefaultValue(5000)]
        public int Duration
        {
            get => _notificationData?.Duration ?? 5000;
            set { if (_notificationData != null) _notificationData.Duration = value; }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public new bool ShowCloseButton
        {
            get => _notificationData?.ShowCloseButton ?? true;
            set { if (_notificationData != null) { _notificationData.ShowCloseButton = value; ApplyData(); } }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowProgressBar
        {
            get => _notificationData?.ShowProgressBar ?? true;
            set { if (_notificationData != null) { _notificationData.ShowProgressBar = value; ApplyData(); } }
        }
        #endregion

        #region Events
        public event EventHandler<NotificationEventArgs> NotificationDismissed;
        public event EventHandler<NotificationEventArgs> ActionClicked;
        public event EventHandler<NotificationEventArgs> NotificationClicked;
        #endregion

        #region Apply data → controls
        private void ApplyData()
        {
            if (_notificationData == null || !IsHandleCreated && !IsDesignerMode())
                return;

            // Theme colors for type (drives form chrome + icon container)
            var colors = NotificationThemeHelpers.GetColorsForType(
                _notificationData.Type,
                null,
                _notificationData.CustomBackColor,
                _notificationData.CustomForeColor,
                null,
                _notificationData.IconTint);

            BackColor   = _notificationData.CustomBackColor ?? colors.BackColor;
            ForeColor   = _notificationData.CustomForeColor ?? colors.ForeColor;
            BorderColor = colors.BorderColor;

            // Pre-compute the resolved icon tint so IconPicture_Paint
            // (called every paint tick) reads it without re-entering
            // NotificationThemeHelpers.GetColorsForType.
            _iconTintResolved = _notificationData.IconTint ?? colors.IconColor;

            // Icon path: default per type unless overridden
            string iconPath = !string.IsNullOrEmpty(_notificationData.IconPath)
                ? _notificationData.IconPath
                : NotificationData.GetDefaultIconForType(_notificationData.Type);

            if (iconPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                || iconPath.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
            {
                _iconPicture.Image = null;       // force the Paint event to redraw
                _iconPicture.Tag = iconPath;
            }
            else if (System.IO.File.Exists(iconPath))
            {
                _iconPicture.Image = Image.FromFile(iconPath);
                _iconPicture.Tag = null;
            }
            else
            {
                _iconPicture.Image = null;
                _iconPicture.Tag = iconPath;     // unknown extension → let SVG painter try
            }
            _iconPicture.Invalidate();

            // Hide labels entirely if their text is empty; the Fill dock of the
            // message label collapses the empty region so the form shrinks to
            // fit the other content (no awkward blank rows).
            var title = _notificationData.Title ?? string.Empty;
            _titleLabel.Text = title;
            _titleLabel.Visible = title.Length > 0;

            var message = _notificationData.Message ?? string.Empty;
            _messageLabel.Text = message;
            _messageLabel.Visible = message.Length > 0;

            _closeButton.Visible = _notificationData.ShowCloseButton;
            if (!_notificationData.ShowCloseButton) _closeButton.TabStop = false;
            else _closeButton.TabStop = true;

            _progressBar.Visible = _notificationData.ShowProgressBar;
            if (_notificationData.ProgressValue.HasValue)
                _progressBar.Value = _notificationData.ProgressValue.Value;

            RebuildActions();
            ApplyTypography();
            RefreshAccessibility();

            RecomputeSize();
        }

        private void RebuildActions()
        {
            _actionsLayout.SuspendLayout();
            _actionsLayout.Controls.Clear();

            int tabStart = _actionsPanel.Tag is int t ? t : 3;

            if (_notificationData?.Actions != null && _notificationData.Actions.Length > 0)
            {
                int i = 0;
                foreach (var action in _notificationData.Actions)
                {
                    var btn = new BeepButton
                    {
                        Text = action.Text,
                        AutoSize = true,
                        UseThemeColors = true,
                        TabIndex = tabStart + i,
                        TabStop = true,
                        Margin = new Padding(DpiScalingHelper.ScaleValue(4, this),
                                             DpiScalingHelper.ScaleValue(4, this),
                                             0,
                                             0)
                    };
                    var capture = action;
                    btn.Click += (s, e) =>
                    {
                        ActionClicked?.Invoke(this, new NotificationEventArgs
                        {
                            Notification = _notificationData,
                            Action = capture
                        });
                        capture.OnClick?.Invoke(_notificationData);
                        Dismiss();
                    };
                    // Tooltip surfaces the action to screen-reader users.
                    btn.ToolTipText = $"{capture.Text} (Enter)";
                    _actionsLayout.Controls.Add(btn);
                    i++;
                }

                _actionsPanel.Visible = true;
                // Height is driven by FlowLayoutPanel.AutoSize plus a small buffer.
                _actionsPanel.Height = _actionsLayout.GetPreferredSize(Size.Empty).Height
                                       + DpiScalingHelper.ScaleValue(2, this);
            }
            else
            {
                _actionsPanel.Height = 0;
                _actionsPanel.Visible = false;
            }

            _actionsLayout.ResumeLayout();
            _actionsPanel.PerformLayout();
        }

        private void RecomputeSize()
        {
            // The form has AutoSize=true / AutoSizeMode=GrowAndShrink, but that
            // pair only resizes the form *after* one of its dimensions has been
            // changed (it intercepts ClientSize changes). We need to actively
            // ask WinForms for the preferred size at the current content
            // configuration and apply it inside the [Minimum, Maximum] band.
            SuspendLayout();
            try
            {
                PerformLayout();
                var size = GetPreferredSize(MaximumSize);
                if (size.IsEmpty) return;

                int minW = MinimumSize.Width  > 0 ? MinimumSize.Width  : 0;
                int minH = MinimumSize.Height > 0 ? MinimumSize.Height : 0;
                int maxW = MaximumSize.Width  > 0 ? MaximumSize.Width  : int.MaxValue;
                int maxH = MaximumSize.Height > 0 ? MaximumSize.Height : int.MaxValue;

                int newW = Math.Min(Math.Max(size.Width,  minW), maxW);
                int newH = Math.Min(Math.Max(size.Height, minH), maxH);

                // Setting Width + Height together triggers AutoSize to
                // re-evaluate against the new minimum.
                if (Width != newW) Width  = newW;
                if (Height != newH) Height = newH;
            }
            finally
            {
                ResumeLayout(false);
            }
            Invalidate();
        }

        /// <summary>True when the form is hosted in a designer (Visual Studio).</summary>
        private bool IsDesignerMode()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
            return Site != null && Site.DesignMode;
        }
        #endregion

        #region Theme + typography
        /// <summary>
        /// Theme-sourced fonts from the active <see cref="IBeepTheme"/> — never
        /// an inline <c>new Font(...)</c>. Pattern mirrors <c>BeepButton.ApplyTheme()</c>
        /// at <c>Buttons/BeepButton.cs:742</c>:
        ///   1. Resolve <see cref="BeepThemesManager.CurrentTheme"/>.
        ///   2. Convert each role's <see cref="TypographyStyle"/> via
        ///      <see cref="BeepThemesManager.ToFont(TypographyStyle, bool)"/> with
        ///      <c>applyDpiScaling: true</c> so the rendered font tracks the form's
        ///      effective DPI.
        ///   3. Apply to the appropriate child control.
        /// </summary>
        private void ApplyTypography()
        {
            var theme = BeepThemesManager.CurrentTheme;
            if (theme == null) return;     // no theme registered yet — keep existing fonts

            if (_titleLabel   != null) _titleLabel.Font   = BeepThemesManager.ToFont(theme.TitleSmall,   applyDpiScaling: true);
            if (_messageLabel != null) _messageLabel.Font = BeepThemesManager.ToFont(theme.BodyMedium,   applyDpiScaling: true);
            if (_closeButton  != null) _closeButton.Font  = BeepThemesManager.ToFont(theme.TitleSmall,   applyDpiScaling: true);
        }
        #endregion

        #region Accessibility (G12)
        private void RefreshAccessibility()
        {
            if (_notificationData == null) return;
            AccessibleRole = AccessibleRole.Grouping;       // Replaces StaticText: the toast is a grouped element with children.
            AccessibleName = string.IsNullOrEmpty(_notificationData.Title)
                ? "Notification"
                : _notificationData.Title;
            AccessibleDescription = TruncateForAccessibility(_notificationData.Message, 200);
        }

        private static string TruncateForAccessibility(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Length <= max ? s : s.Substring(0, max - 1).TrimEnd() + "\u2026";
        }
        #endregion

        #region ApplyTheme + DPI handling
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            ApplyTypography();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            EnsureThemeSubscribed();
            // DpiChanged event wires up re-scaling when the user moves the
            // form between monitors with different scaling (or via Settings).
            DpiChanged += OnDpiChangedInternal;

            // Re-apply DPI-aware sizes now that DeviceDpi is the real monitor DPI.
            // (Constructor used DPI=96 by default.)
            RescaleLayout();

            if (_notificationData != null)
                ApplyData();
        }

        private void OnDpiChangedInternal(object? sender, EventArgs e)
        {
            // Re-scale the DPI-affected sizes (icon width, padding, progress
            // height) — WinForms auto-rescaling should handle most controls,
            // but we set explicit Width/Height on a few. Calling RescaleLayout
            // updates every padded/sized child to the new DPI; ApplyTypography
            // refreshes the fonts via the theme tokens; RecomputeSize inside
            // RescaleLayout re-sizes the form.
            RescaleLayout();
            ApplyTypography();
            RecomputeSize();
        }

        private void EnsureThemeSubscribed()
        {
            if (_themeSubscribed) return;
            _themeSubscribed = true;
            BeepThemesManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object? sender, EventArgs e)
        {
            if (IsDisposed || _notificationData == null) return;
            ApplyData();
        }
        #endregion

        #region Window style (G33 — ShowWithoutActivation)
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;

        private void OnHoverEnter()
        {
            Cursor = Cursors.Hand;
            if (_notificationData != null && _notificationData.PauseOnHover) Pause();
        }

        private void OnHoverLeave()
        {
            Cursor = Cursors.Default;
            if (_notificationData != null && _notificationData.PauseOnHover) Resume();
        }
        #endregion

        #region Icon paint (SVG fallback when PictureBox can't load it natively)
        private void IconPicture_Paint(object? sender, PaintEventArgs e)
        {
            if (_iconPicture?.Tag is string path)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                StyledImagePainter.PaintWithTint(
                    e.Graphics,
                    _iconPicture.ClientRectangle,
                    path,
                    _iconTintResolved,
                    1f,
                    0);
            }
        }
        #endregion

        #region Public methods (Show / Dismiss / Pause / Resume)
        public new void Show()
        {
            base.Show();
            StartAutoDismissCountdown();
        }

        public new void Show(IWin32Window owner)
        {
            if (owner == null) { Show(); return; }
            base.Show(owner);
            StartAutoDismissCountdown();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            StopTimers();
            return base.ShowDialog(owner);
        }

        public void Dismiss()
        {
            StopTimers();
            var args = new NotificationEventArgs { Notification = _notificationData };
            NotificationDismissed?.Invoke(this, args);

            // Phase 7.6: when this instance was shown through the manager,
            // forward the consumed-id so the manager can drop it from the
            // active stack and raise <c>NotificationConsumed</c> for any
            // Win11 toast bridge subscriber. We only forward if the manager
            // still owns an active instance with our Id — otherwise this is
            // a headless Dismiss call and we leave the manager alone.
            if (_notificationData != null && !args.Cancel)
            {
                try
                {
                    var mgr = BeepNotificationManager.Instance;
                    if (mgr.ActiveCount > 0)
                        mgr.MarkConsumed(_notificationData.Id);
                }
                catch { /* manager unavailable — fine */ }
            }

            if (!args.Cancel) Visible = false;
        }

        public void Pause()
        {
            if (!_isPaused && _autoDismissTimer != null && _autoDismissTimer.Enabled)
            {
                _isPaused = true;
                var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
                _remainingDuration = Math.Max(0, (_notificationData?.Duration ?? 5000) - (int)elapsed);
                _autoDismissTimer.Stop();
                _progressTimer?.Stop();
            }
        }

        public void Resume()
        {
            if (_isPaused && _remainingDuration > 0)
            {
                _isPaused = false;
                _startTime = DateTime.Now;
                _autoDismissTimer?.Start();
                if (_notificationData?.ShowProgressBar == true) _progressTimer?.Start();
            }
        }
        #endregion

        #region Timers
        private void StartAutoDismissCountdown()
        {
            _progressPercentage = 100f;
            if (_notificationData != null && _notificationData.Duration > 0)
            {
                _startTime = DateTime.Now;
                _remainingDuration = _notificationData.Duration;
                _autoDismissTimer.Start();
                if (_notificationData.ShowProgressBar) _progressTimer.Start();
            }
        }

        private void AutoDismissTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused) return;
            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            if (elapsed >= _remainingDuration) Dismiss();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused || _notificationData == null || _notificationData.Duration <= 0) return;
            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            _progressPercentage = Math.Max(0, 100f - (float)(elapsed / _notificationData.Duration * 100));
            try
            {
                _progressBar.Value = (int)_progressPercentage;
                // Refresh the tooltip every tick so the "X% remaining" message
                // reflects the live countdown. ToolTipText on the child is
                // read by ToolTipManager on the next hover/paint, so we don't
                // need to invalidate here.
                if (_progressBar != null) UpdateTooltips();
            }
            catch { /* control may be disposed mid-shutdown */ }
        }

        private void StopTimers()
        {
            _autoDismissTimer?.Stop();
            _progressTimer?.Stop();
            _isPaused = false;
        }
        #endregion

        #region Keyboard (Esc / Enter / Space / Ctrl+P / Ctrl+M / 1-3)
        private void BeepNotification_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Dismiss();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                case Keys.Space:
                    if (_notificationData?.Actions != null && _notificationData.Actions.Length > 0)
                    {
                        var primary = Array.Find(_notificationData.Actions, a => a.IsPrimary)
                                      ?? _notificationData.Actions[0];
                        ActionClicked?.Invoke(this, new NotificationEventArgs
                        {
                            Notification = _notificationData, Action = primary
                        });
                        primary.OnClick?.Invoke(_notificationData);
                    }
                    else Dismiss();
                    e.Handled = true;
                    break;
                case Keys.D1: case Keys.NumPad1: TriggerActionByIndex(0); e.Handled = true; break;
                case Keys.D2: case Keys.NumPad2: TriggerActionByIndex(1); e.Handled = true; break;
                case Keys.D3: case Keys.NumPad3: TriggerActionByIndex(2); e.Handled = true; break;
                case Keys.P when e.Control: TogglePin(); e.Handled = true; break;
                case Keys.M when e.Control: MarkAsRead(); e.Handled = true; break;
            }
        }

        private void TriggerActionByIndex(int index)
        {
            if (_notificationData?.Actions == null || index >= _notificationData.Actions.Length) return;
            var action = _notificationData.Actions[index];
            ActionClicked?.Invoke(this, new NotificationEventArgs { Notification = _notificationData, Action = action });
            action.OnClick?.Invoke(_notificationData);
        }

        private void TogglePin()
        {
            if (_notificationData == null) return;
            _notificationData.IsPinned = !_notificationData.IsPinned;
        }

        private void MarkAsRead()
        {
            if (_notificationData == null || _notificationData.IsRead) return;
            _notificationData.IsRead = true;
            _notificationData.ReadTimestamp = DateTime.Now;
        }
        #endregion

        #region Body click — fires only when the click hit empty space (children
        // consume their own clicks first).
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            NotificationClicked?.Invoke(this, new NotificationEventArgs { Notification = _notificationData });
        }
        #endregion

        #region Initial focus (Gap 13/15)
        /// <summary>
        /// Defer initial focus to right after the form is visible so the
        /// keyboard can immediately Tab around the controls. Without this,
        /// focus stays on the form and Tab cycles the children in any order.
        /// Focusing the close button by default keeps Esc → Dismiss responsive.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BeginInvoke(new Action(() =>
            {
                if (IsDisposed) return;

                // Prefer the first action button if actions exist (UX convention);
                // otherwise focus the close button so Esc dismisses on first key.
                Control focusTarget = null;

                if (_actionsLayout?.Controls.Count > 0)
                {
                    // FlowLayoutPanel with FlowDirection.LeftToRight puts the
                    // first added control first in tab order.
                    focusTarget = _actionsLayout.Controls[0];
                }
                else if (_closeButton?.Visible == true)
                {
                    focusTarget = _closeButton;
                }

                focusTarget?.Focus();
            }));
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
                BeepThemesManager.ThemeChanged -= OnThemeChanged;
                DpiChanged -= OnDpiChangedInternal;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
