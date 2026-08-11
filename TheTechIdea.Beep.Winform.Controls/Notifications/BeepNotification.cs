using TheTechIdea.Beep.Winform.Controls.Images;
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
        // One control per cell in a TableLayoutPanel - not dock stacks. Docking three
        // auto-sizing controls inside each other is what stretched the labels and let them
        // overlap; a grid gives each element a cell whose size is negotiated once.
        private TableLayoutPanel _grid;
        private BeepImage _iconImage;         // BeepImage is the control that renders/themes SVGs
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private BeepButton _closeButton;
        private BeepProgressBar _progressBar;
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
            // AutoSize is deliberately OFF: with Fill-docked content it reported the
            // MinimumSize forever, so long messages were clipped. RecomputeSize measures
            // the real text instead.
            AutoSize = false;
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

            // ── Composition: one TableLayoutPanel, one control per cell ──
            //   col 0 = icon (fixed)   col 1 = text (fills)   col 2 = close (fixed)
            //   row 0 = title          row 1 = message        row 2 = actions
            _grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _iconImage = new BeepImage
            {
                IsFrameless = true,
                IsChild = true,
                IsTransparentBackground = true,
                ShowAllBorders = false,
                ShowShadow = false,
                ControlStyle = BeepControlStyle.None,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Anchor = AnchorStyles.Top,
                Margin = new Padding(0, 0, 0, 0)
            };

            _titleLabel = new BeepLabel
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                TabIndex = 1,
                TabStop = false,
                IsFrameless = true, IsChild = true, IsTransparentBackground = true,
                ShowAllBorders = false, ShowShadow = false, ControlStyle = BeepControlStyle.None
            };

            // BeepLabel needs BOTH WordWrap and Multiline: one decides where lines break,
            // the other renders more than one.
            _messageLabel = new BeepLabel
            {
                AutoSize = false,
                Multiline = true,
                WordWrap = true,
                TextAlign = ContentAlignment.TopLeft,
                TabIndex = 2,
                TabStop = false,
                IsFrameless = true, IsChild = true, IsTransparentBackground = true,
                ShowAllBorders = false, ShowShadow = false, ControlStyle = BeepControlStyle.None
            };

            _closeButton = new BeepButton
            {
                Text = "\u2715",
                TabIndex = 0,
                TabStop = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                IsFrameless = true, IsChild = true, IsTransparentBackground = true,
                ShowAllBorders = false, ShowShadow = false, ControlStyle = BeepControlStyle.None
            };
            _closeButton.Click += (s, e) => Dismiss();

            _actionsLayout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            _progressBar = new BeepProgressBar
            {
                Dock = DockStyle.Bottom,
                Visible = false,
                IsFrameless = true,
                ShowAllBorders = false,
                ControlStyle = BeepControlStyle.None
            };

            _grid.Controls.Add(_iconImage,     0, 0);
            _grid.SetRowSpan(_iconImage, 2);
            _grid.Controls.Add(_titleLabel,    1, 0);
            _grid.Controls.Add(_closeButton,   2, 0);
            _grid.Controls.Add(_messageLabel,  1, 1);
            _grid.Controls.Add(_actionsLayout, 1, 2);
            _grid.SetColumnSpan(_actionsLayout, 2);

            this.Controls.Add(_grid);
            this.Controls.Add(_progressBar);


            if (IsHandleCreated) RescaleLayout();

            ApplyTypography();
            RefreshAccessibility();
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
            int pad      = DpiScalingHelper.ScaleValue(12, this);
            int iconSize = DpiScalingHelper.ScaleValue(24, this);
            int closeSz  = DpiScalingHelper.ScaleValue(18, this);
            int gap      = DpiScalingHelper.ScaleValue(10, this);
            int rowGap   = DpiScalingHelper.ScaleValue(3, this);

            if (_grid != null) _grid.Padding = new Padding(pad);

            if (_iconImage != null)
            {
                _iconImage.Size   = new Size(iconSize, iconSize);
                _iconImage.Margin = new Padding(0, 0, gap, 0);
            }
            if (_titleLabel   != null) _titleLabel.Margin   = new Padding(0, 0, 0, rowGap);
            if (_messageLabel != null) _messageLabel.Margin = new Padding(0, 0, 0, 0);
            if (_closeButton  != null)
            {
                _closeButton.Size   = new Size(closeSz, closeSz);
                _closeButton.Margin = new Padding(gap, 0, 0, 0);
            }
            if (_actionsLayout != null)
                _actionsLayout.Margin = new Padding(0, DpiScalingHelper.ScaleValue(6, this), 0, 0);
            if (_progressBar != null)
                _progressBar.Height = DpiScalingHelper.ScaleValue(4, this);

            MinimumSize = DpiScalingHelper.ScaleSize(new Size(300, 60), this);
            MaximumSize = DpiScalingHelper.ScaleSize(new Size(460, 320), this);

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
                ThemeManagement.BeepThemesManager.CurrentTheme,
                _notificationData.CustomBackColor,
                _notificationData.CustomForeColor,
                null,
                _notificationData.IconTint);

            BackColor   = _notificationData.CustomBackColor ?? colors.BackColor;
            ForeColor   = _notificationData.CustomForeColor ?? colors.ForeColor;
            BorderColor = colors.BorderColor;

            // Inner children are frameless AND transparent, so the card is one surface. Only the
            // INK has to be handed down: each child's own ApplyTheme resolves a label/panel colour
            // that has nothing to do with this toast's semantic type.
            foreach (var child in new Control[] { _titleLabel, _messageLabel, _closeButton })
            {
                if (child != null) child.ForeColor = ForeColor;
            }

            // Pre-compute the resolved icon tint so IconPicture_Paint
            // (called every paint tick) reads it without re-entering
            // NotificationThemeHelpers.GetColorsForType.
            _iconTintResolved = _notificationData.IconTint ?? colors.IconColor;

            // Icon path: default per type unless overridden. BeepImage is the control that
            // renders and themes SVGs - the old PictureBox + manual SVG paint drew nothing.
            string iconPath = !string.IsNullOrEmpty(_notificationData.IconPath)
                ? _notificationData.IconPath
                : NotificationData.GetDefaultIconForType(_notificationData.Type);

            if (_iconImage != null)
            {
                _iconImage.ImagePath = iconPath;
                _iconImage.Visible = !string.IsNullOrEmpty(iconPath);
            }

            // Hide labels entirely if their text is empty; the Fill dock of the
            // message label collapses the empty region so the form shrinks to
            // fit the other content (no awkward blank rows).
            var title = _notificationData.Title ?? string.Empty;
            _titleLabel.Text = title;
            _titleLabel.Visible = title.Length > 0;
            _titleLabel.ForeColor = ForeColor;

            var message = _notificationData.Message ?? string.Empty;
            _messageLabel.Text = message;
            _messageLabel.Visible = message.Length > 0;
            _messageLabel.ForeColor = ForeColor;

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

            int tabStart = 3;

            if (_notificationData?.Actions != null && _notificationData.Actions.Length > 0)
            {
                int i = 0;
                foreach (var action in _notificationData.Actions)
                {
                    var btn = new BeepButton
                    {
                        Text = action.Text,
                        AutoSize = true,
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
                    _actionsLayout.Controls.Add(btn);
                    i++;
                }

                _actionsLayout.Visible = true;
            }
            else
            {
                _actionsLayout.Visible = false;
            }

            _actionsLayout.ResumeLayout();
        }

        /// <summary>
        /// Sizes the card from its CONTENT: measured title + wrapped message inside the
        /// available text width, plus icon/close/actions/progress. WinForms AutoSize was
        /// useless here (Fill-docked labels report no preferred height), which is why the
        /// toast sat at MinimumSize with the message clipped away entirely.
        /// </summary>
        /// <summary>
        /// Sizes the card from the grid's own preferred size. The message label is capped to the
        /// text column first, so it WRAPS instead of widening the card indefinitely - an
        /// AutoSize label with no MaximumSize reports one very long line, which is what made the
        /// old layout look stretched.
        /// </summary>
        private void RecomputeSize()
        {
            if (_grid == null) return;

            int maxW = MaximumSize.Width > 0 ? MaximumSize.Width : DpiScalingHelper.ScaleValue(460, this);
            int minW = MinimumSize.Width > 0 ? MinimumSize.Width : DpiScalingHelper.ScaleValue(300, this);

            int sideChrome = _grid.Padding.Horizontal
                           + (_iconImage != null && !string.IsNullOrEmpty(_iconImage.ImagePath) ? _iconImage.Width + _iconImage.Margin.Horizontal : 0)
                           + (_closeButton != null && (_notificationData?.ShowCloseButton ?? true) ? _closeButton.Width + _closeButton.Margin.Horizontal : 0);

            int textW = Math.Max(DpiScalingHelper.ScaleValue(120, this), maxW - sideChrome);

            // Measure each label against the text column and give it that exact size. A
            // BaseControl-derived label does not negotiate AutoSize with TableLayoutPanel the way
            // a WinForms Label does - left to itself the message collapsed to 2px wide and was
            // laid out on top of the title.
            // Two passes. Pass 1 finds the natural (unwrapped) width of each line so a short
            // toast does not stretch to the maximum. Pass 2 re-measures HEIGHT at the width the
            // label will actually be given - measuring height at one width and rendering at a
            // narrower one is what truncated the message.
            string titleText = _titleLabel?.Text ?? string.Empty;
            string messageText = _messageLabel?.Text ?? string.Empty;
            int contentW, titleH = 0, messageH = 0;

            using (var g = CreateGraphics())
            {
                var titleFont = _titleLabel?.TextFont ?? _titleLabel?.Font;
                var msgFont = _messageLabel?.TextFont ?? _messageLabel?.Font;

                int natural = 0;
                if (titleText.Length > 0 && titleFont != null)
                    natural = Math.Max(natural, TextRenderer.MeasureText(g, titleText, titleFont,
                        new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width);
                if (messageText.Length > 0 && msgFont != null)
                    natural = Math.Max(natural, TextRenderer.MeasureText(g, messageText, msgFont,
                        new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width);

                // A couple of pixels of slack: MeasureText without padding is a tight fit and the
                // label's own renderer can need one more pixel before it decides to wrap.
                natural += DpiScalingHelper.ScaleValue(4, this);
                contentW = Math.Max(DpiScalingHelper.ScaleValue(120, this), Math.Min(textW, natural));

                if (titleText.Length > 0 && titleFont != null)
                    titleH = TextRenderer.MeasureText(g, titleText, titleFont,
                        new Size(contentW, int.MaxValue), TextFormatFlags.WordBreak).Height
                        + DpiScalingHelper.ScaleValue(2, this);
                if (messageText.Length > 0 && msgFont != null)
                    messageH = TextRenderer.MeasureText(g, messageText, msgFont,
                        new Size(contentW, int.MaxValue), TextFormatFlags.WordBreak).Height
                        + DpiScalingHelper.ScaleValue(2, this);
            }

            if (_titleLabel != null) _titleLabel.Size = new Size(contentW, titleH);
            if (_messageLabel != null) _messageLabel.Size = new Size(contentW, messageH);

            _grid.PerformLayout();
            Size preferred = _grid.GetPreferredSize(new Size(maxW, 0));

            // Width comes from the MEASURED content, not the grid's preferred size: the grid is
            // Dock=Fill, so it reports the form's current width and the card could never shrink.
            int newW = Math.Min(Math.Max(contentW + sideChrome, minW), maxW);
            int newH = preferred.Height + ((_progressBar != null && (_notificationData?.ShowProgressBar ?? false)) ? _progressBar.Height : 0);
            newH = Math.Min(Math.Max(newH, MinimumSize.Height), MaximumSize.Height);

            if (Width != newW || Height != newH) Size = new Size(newW, newH);
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

                // Focus the first ACTION button when the toast has actions - that is a real
                // affordance the user may want to trigger with Enter.
                //
                // Never focus the close button just to make Esc work: the form sets
                // KeyPreview and handles Escape itself, and focusing a child was actively
                // harmful. A tooltip registered on that child is keyboard-triggerable, so
                // focusing it satisfied ToolTipManager.TriggerStillValid and popped a second
                // little tooltip window next to every notification, with the pointer nowhere
                // near it. This form also shows without activation, so taking focus at all is
                // a contradiction.
                if (_actionsLayout?.Controls.Count > 0)
                {
                    _actionsLayout.Controls[0].Focus();
                }
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
