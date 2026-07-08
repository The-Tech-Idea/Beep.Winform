using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    /// <summary>
    /// Card-based wizard form with clickable step cards on the left side.
    /// Each step is represented as a card showing step number, title, and completion status.
    /// </summary>
    public class CardsWizardForm : BeepiFormPro, IWizardFormHost
    {
        #region Fields

        private WizardInstance _instance;

        private Panel _cardPanel;
        private Panel _contentPanel;
        private Panel _buttonPanel;
        private Panel _errorPanel;
        private Label _lblError;

        private BeepButton _btnNext;
        private BeepButton _btnBack;
        private BeepButton _btnCancel;
        private BeepButton _btnSkip;
        private BeepButton _btnHelp;

        private readonly List<Panel> _stepCards = new List<Panel>();
        private readonly List<Timer> _activeAnimationTimers = new List<Timer>();
        private int _previousStepIndex = -1;
        private Panel _loadingOverlay;
        private readonly Dictionary<int, Control> _cachedPages = new Dictionary<int, Control>();

        // Card painting
        private Color _completedColor;
        private Color _currentColor;
        private Color _pendingColor;
        private Color _textColor;
        private Color _subtextColor;
        private Color _cardBgColor;
        private Font _cardTitleFont;
        private Font _cardDescFont;
        private Font _cardNumberFont;

        #endregion

        #region Properties

        public WizardInstance Instance => _instance;

        #endregion

        #region Constructor

        public CardsWizardForm()
        {
            // Designer Mode Setup
            var config = new WizardConfig { Title = "Wizard Design Mode" };
            config.Steps.Add(new WizardStep { Title = "Step 1", Description = "Design Mode Step 1" });
            config.Steps.Add(new WizardStep { Title = "Step 2", Description = "Design Mode Step 2" });
            config.Steps.Add(new WizardStep { Title = "Step 3", Description = "Design Mode Step 3" });

            _instance = new WizardInstance(config);
            _instance.BindFormHost(this);

            InitializeColors(null);
            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();
            UpdateUI();
        }

        public CardsWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instance.BindFormHost(this);

            // Apply Config.Theme and initialize colors BEFORE building controls (colors needed during init)
            if (_instance.Config.Theme != null)
            {
                CurrentTheme = _instance.Config.Theme;
            }
            InitializeColors(CurrentTheme);

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();

            UpdateUI();
            ApplyTheme();
        }

        #endregion

        #region Initialization

        private void InitializeColors(IBeepTheme theme)
        {
            if (theme != null)
            {
                _completedColor = theme.SuccessColor;
                _currentColor = theme.PrimaryColor;
                _pendingColor = Color.FromArgb(80, theme.ForeColor);
                _textColor = theme.ForeColor;
                _subtextColor = Color.FromArgb(128, theme.ForeColor);
                _cardBgColor = ColorUtils.ShiftLuminance(theme.BackColor, -0.03f);
            }
            else
            {
                _completedColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                _currentColor = ColorUtils.MapSystemColor(SystemColors.HotTrack);
                _pendingColor = Color.FromArgb(80, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _textColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                _subtextColor = Color.FromArgb(128, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _cardBgColor = ColorUtils.ShiftLuminance(ColorUtils.MapSystemColor(SystemColors.Window), -0.03f);
            }

            _cardTitleFont?.Dispose();
            _cardTitleFont = WizardHelpers.GetFont(theme, theme?.TitleStyle, 10f, FontStyle.Bold);
            _cardDescFont?.Dispose();
            _cardDescFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 8.5f, FontStyle.Regular);
            _cardNumberFont?.Dispose();
            _cardNumberFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 12f, FontStyle.Bold);
        }

        private void InitializeForm()
        {
            Text = _instance.Config.Title;
            Size = _instance.Config.Size;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        private void InitializeControls()
        {
            // Card panel (left side)
            _cardPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = DpiScalingHelper.ScaleValue(220, this),
                AutoScroll = true,
                Padding = new Padding(DpiScalingHelper.ScaleValue(10, this), DpiScalingHelper.ScaleValue(15, this),
                    DpiScalingHelper.ScaleValue(10, this), DpiScalingHelper.ScaleValue(15, this)),
                BackColor = _cardBgColor
            };

            // Inline error panel (top of content area, hidden by default)
            _errorPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(40, this),
                Visible = false,
                BackColor = WizardHelpers.GetWarningBackColor(CurrentTheme),
                Padding = new Padding(16, 0, 16, 0)
            };
            _lblError = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = WizardHelpers.GetErrorColor(CurrentTheme),
                Font = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.BodyStyle, 9.5f, FontStyle.Regular),
                AutoEllipsis = true
            };
            _errorPanel.Controls.Add(_lblError);

            // Button panel (bottom)
            _buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = DpiScalingHelper.ScaleValue(70, this),
                Padding = new Padding(DpiScalingHelper.ScaleValue(20, this), DpiScalingHelper.ScaleValue(15, this),
                    DpiScalingHelper.ScaleValue(20, this), DpiScalingHelper.ScaleValue(15, this))
            };

            // Content panel (fill) — uses BufferedPanel to eliminate flicker
            _contentPanel = new BufferedPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 20)
            };

            // Navigation buttons
            _btnNext = new BeepButton
            {
                Text = _instance.Config.NextButtonText,
                Size = new Size(120, 40),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            _btnBack = new BeepButton
            {
                Text = _instance.Config.BackButtonText,
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false
            };

            _btnCancel = new BeepButton
            {
                Text = _instance.Config.CancelButtonText,
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            if (_instance.Config.AllowSkip)
            {
                _btnSkip = new BeepButton
                {
                    Text = _instance.Config.SkipButtonText,
                    Size = new Size(100, 40),
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                    Visible = false
                };
            }

            if (_instance.Config.ShowHelp)
            {
                _btnHelp = new BeepButton
                {
                    Text = "Help",
                    Size = new Size(80, 40),
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };
            }

            // Build step cards
            BuildStepCards();
        }

        private void BuildStepCards()
        {
            _cardPanel.Controls.Clear();
            _stepCards.Clear();

            // Add wizard title label at top of card panel
            var titleLabel = new Label
            {
                Text = _instance.Config.Title,
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(40, this),
                Font = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.TitleStyle, 13f, FontStyle.Bold),
                ForeColor = _textColor,
                Padding = new Padding(5, 0, 0, 10),
                TextAlign = ContentAlignment.BottomLeft
            };
            _cardPanel.Controls.Add(titleLabel);

            for (int i = _instance.Config.Steps.Count - 1; i >= 0; i--)
            {
                var step = _instance.Config.Steps[i];
                var card = CreateStepCard(step, i);
                _stepCards.Insert(0, card);
                _cardPanel.Controls.Add(card);
            }
        }

        private Panel CreateStepCard(WizardStep step, int stepIndex)
        {
            int p10 = DpiScalingHelper.ScaleValue(10, this);
            var card = new Panel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(70, this),
                Margin = new Padding(0, 0, 0, DpiScalingHelper.ScaleValue(6, this)),
                Padding = new Padding(p10),
                Tag = stepIndex,
                Cursor = Cursors.Hand
            };

            card.Paint += StepCard_Paint;
            card.Click += (s, e) => CardClicked(stepIndex);
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(20, _currentColor); };
            card.MouseLeave += (s, e) => { card.Invalidate(); };

            return card;
        }

        private void LayoutControls()
        {
            // Layout buttons
            int rightEdge = _buttonPanel.ClientSize.Width - _buttonPanel.Padding.Right;
            int buttonY = (_buttonPanel.Height - _btnNext.Height) / 2;

            _btnNext.Location = new Point(rightEdge - _btnNext.Width, buttonY);
            _btnBack.Location = new Point(_btnNext.Left - _btnBack.Width - 10, buttonY);
            _btnCancel.Location = new Point(_cardPanel.Width + 20, buttonY);

            _buttonPanel.Controls.Add(_btnNext);
            _buttonPanel.Controls.Add(_btnBack);
            _buttonPanel.Controls.Add(_btnCancel);

            if (_btnSkip != null)
            {
                _btnSkip.Location = new Point(_btnCancel.Right + 10, buttonY);
                _buttonPanel.Controls.Add(_btnSkip);
            }

            if (_btnHelp != null)
            {
                var helpX = _btnSkip != null ? _btnSkip.Right + 10 : _btnCancel.Right + 10;
                _btnHelp.Location = new Point(helpX, buttonY);
                _buttonPanel.Controls.Add(_btnHelp);
            }

            Controls.Add(_contentPanel);
            Controls.Add(_errorPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_cardPanel);
        }

        private void SetupEventHandlers()
        {
            _btnNext.Click += BtnNext_Click;
            _btnBack.Click += BtnBack_Click;
            _btnCancel.Click += BtnCancel_Click;
            if (_btnSkip != null)
                _btnSkip.Click += BtnSkip_Click;
            if (_btnHelp != null)
                _btnHelp.Click += BtnHelp_Click;

            KeyDown += Form_KeyDown;
        }

        #endregion

        #region Card Painting

        private void StepCard_Paint(object sender, PaintEventArgs e)
        {
            if (sender is not Panel card || card.Tag is not int stepIndex) return;
            if (stepIndex < 0 || stepIndex >= _instance.Config.Steps.Count) return;

            var step = _instance.Config.Steps[stepIndex];
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = card.ClientRectangle;
            bool isCurrent = stepIndex == _instance.CurrentStepIndex;
            bool isCompleted = step.State == StepState.Completed || stepIndex < _instance.CurrentStepIndex;
            bool isSkipped = step.State == StepState.Skipped;

            // Card background
            Color bgColor = isCurrent ? Color.FromArgb(15, _currentColor) : Color.Transparent;
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // DPI-scaled layout values
            int circleSize = DpiScalingHelper.ScaleValue(30, this);
            int circleX = bounds.Left + DpiScalingHelper.ScaleValue(14, this);
            int circleY = bounds.Top + (bounds.Height - circleSize) / 2;
            var circleRect = new Rectangle(circleX, circleY, circleSize, circleSize);
            int textX = circleX + circleSize + DpiScalingHelper.ScaleValue(12, this);
            int textWidth = bounds.Width - textX - DpiScalingHelper.ScaleValue(10, this);
            int titleHeight = DpiScalingHelper.ScaleValue(18, this);
            int descTop = circleY + DpiScalingHelper.ScaleValue(20, this);
            int descHeight = DpiScalingHelper.ScaleValue(16, this);
            int badgeTop = circleY + DpiScalingHelper.ScaleValue(36, this);
            int badgeHeight = DpiScalingHelper.ScaleValue(14, this);
            float penW = DpiScalingHelper.ScaleValue(2f, this);
            int checkSize = DpiScalingHelper.ScaleValue(5, this);
            int accentTop = bounds.Top + DpiScalingHelper.ScaleValue(8, this);
            int accentH = bounds.Height - DpiScalingHelper.ScaleValue(16, this);
            int accentW = DpiScalingHelper.ScaleValue(4, this);

            // Left accent bar for current step
            if (isCurrent)
            {
                using var accentBrush = new SolidBrush(_currentColor);
                g.FillRectangle(accentBrush, bounds.Left, accentTop, accentW, accentH);
            }

            Color circleColor, innerColor;
            if (isCompleted)
            { circleColor = _completedColor; innerColor = ColorUtils.GetContrastColor(_completedColor); }
            else if (isCurrent)
            { circleColor = _currentColor; innerColor = ColorUtils.GetContrastColor(_currentColor); }
            else
            { circleColor = _pendingColor; innerColor = _pendingColor; }

            if (isCompleted || isCurrent)
            {
                using var circleBrush = new SolidBrush(circleColor);
                g.FillEllipse(circleBrush, circleRect);
            }
            else
            {
                using var circlePen = new Pen(circleColor, penW);
                g.DrawEllipse(circlePen, circleRect);
            }

            if (isCompleted)
            {
                using var pen = new Pen(innerColor, penW);
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round; pen.LineJoin = LineJoin.Round;
                int cx = circleRect.X + circleRect.Width / 2, cy = circleRect.Y + circleRect.Height / 2;
                g.DrawLines(pen, new Point[] { new(cx - checkSize, cy), new(cx - checkSize * 2 / 5, cy + checkSize * 4 / 5), new(cx + checkSize * 6 / 5, cy - checkSize * 4 / 5) });
            }
            else
            {
                TextUtils.DrawText(g, (stepIndex + 1).ToString(), _cardNumberFont, circleRect, innerColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            var titleColor = isCurrent ? _textColor : _subtextColor;
            var titleFont = isCurrent ? _cardTitleFont : _cardDescFont;
            TextUtils.DrawText(g, step.Title ?? $"Step {stepIndex + 1}", titleFont,
                new Rectangle(textX, circleY, textWidth, titleHeight), titleColor);

            if (!string.IsNullOrEmpty(step.Description))
            {
                TextUtils.DrawText(g, step.Description, _cardDescFont,
                    new Rectangle(textX, descTop, textWidth, descHeight), _subtextColor);
            }

            if (step.IsOptional && !isCompleted)
            {
                using var optFont = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.CaptionStyle, 7f, FontStyle.Italic);
                TextUtils.DrawText(g, "Optional", optFont,
                    new Rectangle(textX, badgeTop, textWidth, badgeHeight), Color.FromArgb(60, _pendingColor));
            }
        }

        private async void CardClicked(int stepIndex)
        {
            if (_instance.Config.AllowSkip || stepIndex <= _instance.CurrentStepIndex)
            {
                await _instance.NavigateToAsync(stepIndex);
            }
        }

        #endregion

        #region IWizardFormHost Implementation

        public void UpdateUI()
        {
            var currentStep = _instance.CurrentStep;
            // Accessibility
            _cardPanel.AccessibleName = $"Step {_instance.CurrentStepIndex + 1} of {_instance.Config.Steps.Count}: {currentStep?.Title ?? ""}";
            AccessibilityNotifyClients(AccessibleEvents.Focus, 0);

            _btnBack.Enabled = _instance.Config.AllowBack && !_instance.IsFirstStep;

            var nextText = currentStep?.NextButtonText
                ?? (_instance.IsLastStep ? _instance.Config.FinishButtonText : _instance.Config.NextButtonText);
            _btnNext.Text = nextText;

            if (_btnSkip != null)
            {
                _btnSkip.Visible = currentStep?.IsOptional ?? false;
            }

            // Get new content control — reuse cached if available
            Control newControl = null;
            int currentIndex = _instance.CurrentStepIndex;
            if (currentIndex >= 0 && currentIndex < _instance.Config.Steps.Count)
            {
                if (!_cachedPages.TryGetValue(currentIndex, out newControl))
                {
                    newControl = currentStep?.Content;
                    if (newControl != null)
                        _cachedPages[currentIndex] = newControl;
                }
            }

            if (newControl == null) return;

            // Find currently visible page
            Control previousControl = null;
            for (int i = 0; i < _contentPanel.Controls.Count; i++)
            {
                var c = _contentPanel.Controls[i];
                if (c.Visible)
                {
                    previousControl = c;
                    break;
                }
            }

            _contentPanel.SuspendLayout();

            try
            {
                // Subscribe to validation state changes
                if (newControl is IWizardStepContent stepContent)
                {
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                    stepContent.ValidationStateChanged += StepContent_ValidationStateChanged;
                }

                // Ensure the new control is parented
                if (newControl.Parent != _contentPanel)
                {
                    newControl.Dock = DockStyle.Fill;
                    _contentPanel.Controls.Add(newControl);
                }

                // Animate transition if enabled
                bool shouldAnimate = WizardManager.EnableAnimations
                    && previousControl != null
                    && newControl != null
                    && previousControl != newControl
                    && _previousStepIndex >= 0;

                if (shouldAnimate)
                {
                    bool forward = currentIndex > _previousStepIndex;
                    previousControl.Dock = DockStyle.None;
                    previousControl.Size = _contentPanel.ClientSize;
                    previousControl.Visible = true;

                    WizardHelpers.AnimateStepTransition(previousControl, newControl, forward, () =>
                    {
                        _contentPanel.SuspendLayout();
                        try
                        {
                            for (int i = 0; i < _contentPanel.Controls.Count; i++)
                                _contentPanel.Controls[i].Visible = false;
                            newControl.Dock = DockStyle.Fill;
                            newControl.Visible = true;
                        }
                        finally
                        {
                            _contentPanel.ResumeLayout(false);
                        }
                    }, _activeAnimationTimers);
                }
                else
                {
                    for (int i = 0; i < _contentPanel.Controls.Count; i++)
                        _contentPanel.Controls[i].Visible = false;
                    newControl.Dock = DockStyle.Fill;
                    newControl.Visible = true;
                }
            }
            finally
            {
                _contentPanel.ResumeLayout(false);
            }

            _previousStepIndex = currentIndex;

            // Auto-hide errors on step change
            if (_instance.Config.AutoHideErrors)
            {
                HideValidationError();
            }

            // Repaint all step cards
            foreach (var card in _stepCards)
            {
                card.Invalidate();
            }
        }

        public void ShowValidationError(string message)
        {
            if (_instance.Config.ShowInlineErrors)
            {
                _lblError.Text = "\u26A0 " + message;
                _errorPanel.Visible = true;
            }
            else
            {
                MessageBox.Show(this, message, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ShowValidationError(WizardValidationResult result)
        {
            if (result == null || result.IsValid) return;

            var msg = result.ErrorMessage ?? "Validation failed";
            if (!string.IsNullOrEmpty(result.FieldName))
                msg = result.FieldName + ": " + msg;
            if (result.ErrorMessages?.Count > 1)
                msg += " (+" + (result.ErrorMessages.Count - 1) + " more)";

            ShowValidationError(msg);
        }

        public void HideValidationError()
        {
            _errorPanel.Visible = false;
        }

        public void ShowLoading(string message = null)
        {
            if (_loadingOverlay == null)
            {
                _loadingOverlay = new Panel { BackColor = Color.FromArgb(160, BackColor), Dock = DockStyle.Fill, Visible = false };
                var label = new Label { Text = message ?? "Please wait...", AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, ForeColor = ForeColor, Font = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.BodyStyle, 12f, FontStyle.Regular), Dock = DockStyle.Fill };
                _loadingOverlay.Controls.Add(label);
                Controls.Add(_loadingOverlay);
                _loadingOverlay.BringToFront();
            }
            _loadingOverlay.Visible = true;
        }

        public void HideLoading()
        {
            if (_loadingOverlay != null) _loadingOverlay.Visible = false;
        }

        public Panel GetContentPanel() => _contentPanel;

        #endregion

        #region Event Handlers

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            if (_instance.IsLastStep)
                await _instance.CompleteAsync();
            else
                await _instance.NavigateNextAsync();
        }

        private async void BtnBack_Click(object sender, EventArgs e)
        {
            await _instance.NavigateBackAsync();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_instance.Config.ConfirmOnCancel)
            {
                var result = MessageBox.Show(this,
                    _instance.Config.CancelConfirmationMessage,
                    "Cancel Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
            }
            _instance.Cancel();
        }

        private async void BtnSkip_Click(object sender, EventArgs e)
        {
            if (_instance.CurrentStep?.IsOptional == true)
            {
                _instance.CurrentStep.State = StepState.Skipped;
                await _instance.NavigateNextAsync();
            }
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            // Step-specific help first
            var currentStep = _instance.CurrentStep;
            if (currentStep?.Tag is WizardStepHelp helpContent)
            {
                MessageBox.Show(this, helpContent.Content, helpContent.Title ?? "Help",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Global help URL fallback
            if (!string.IsNullOrEmpty(_instance.Config.HelpUrl))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = _instance.Config.HelpUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Unable to open help URL: {ex.Message}", "Help Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void StepContent_ValidationStateChanged(object sender, StepValidationEventArgs e)
        {
            _btnNext.Enabled = e.IsValid;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    // Don't handle Enter if focus is in a multi-line textbox
                    if (ActiveControl is TextBox textBox && textBox.Multiline)
                        break;
                    // Don't handle Enter if focus is in a BeepTextBox
                    if (ActiveControl is BeepTextBox)
                        break;

                    if (e.Control && _instance.IsLastStep)
                    {
                        // Ctrl+Enter to finish on last step
                        BtnNext_Click(sender, e);
                        e.Handled = true;
                    }
                    else if (!e.Control && _btnNext.Enabled)
                    {
                        // Enter to go to next
                        BtnNext_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Keys.Escape:
                    BtnCancel_Click(sender, e);
                    e.Handled = true;
                    break;
                case Keys.Left:
                    if (ActiveControl is not TextBox && ActiveControl is not BeepTextBox)
                    { BtnBack_Click(sender, e); e.Handled = true; }
                    break;
                case Keys.Right:
                    if (ActiveControl is not TextBox && ActiveControl is not BeepTextBox)
                    { BtnNext_Click(sender, e); e.Handled = true; }
                    break;
                case Keys.N when e.Control:
                    BtnNext_Click(sender, e); e.Handled = true; break;
                case Keys.B when e.Control:
                    BtnBack_Click(sender, e); e.Handled = true; break;
                case Keys.Home when e.Control:
                    if (_instance.Config.Steps.Count > 0)
                    { _ = _instance.NavigateToAsync(0); e.Handled = true; }
                    break;
                case Keys.End when e.Control:
                    if (_instance.Config.Steps.Count > 0)
                    { _ = _instance.NavigateToAsync(_instance.Config.Steps.Count - 1); e.Handled = true; }
                    break;
                case Keys.F1:
                    if (_btnHelp != null && _btnHelp.Visible) { BtnHelp_Click(sender, e); e.Handled = true; }
                    break;
            }
        }

        #endregion

        #region Theme

        public override void ApplyTheme()
        {
            // Guard: base ctor triggers ApplyTheme before any wizard fields are initialized
            if (_instance == null || _contentPanel == null || _buttonPanel == null || _cardPanel == null)
                return;
            base.ApplyTheme();

            if (CurrentTheme != null)
            {
                InitializeColors(CurrentTheme);

                BackColor = CurrentTheme.BackColor;
                _contentPanel.BackColor = CurrentTheme.BackColor;
                _buttonPanel.BackColor = CurrentTheme.BackColor;
                _cardPanel.BackColor = _cardBgColor;
                _errorPanel.BackColor = WizardHelpers.GetWarningBackColor(CurrentTheme);
                _lblError.ForeColor = WizardHelpers.GetErrorColor(CurrentTheme);
                _lblError.Font = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.BodyStyle, 9.5f, FontStyle.Regular);

                _btnNext.Theme = CurrentTheme.ThemeName;
                _btnBack.Theme = CurrentTheme.ThemeName;
                _btnCancel.Theme = CurrentTheme.ThemeName;
                if (_btnSkip != null) _btnSkip.Theme = CurrentTheme.ThemeName;
                if (_btnHelp != null) _btnHelp.Theme = CurrentTheme.ThemeName;

                _btnNext.ApplyTheme();
                _btnBack.ApplyTheme();
                _btnCancel.ApplyTheme();
                _btnSkip?.ApplyTheme();
                _btnHelp?.ApplyTheme();

                // Repaint cards with new theme
                foreach (var card in _stepCards)
                {
                    card.Invalidate();
                }
            }
        }

        #endregion

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (_instance?.Config?.EnableCompositedRendering != false)
                    cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        #region Cleanup

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_instance.Result == WizardResult.None && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                BtnCancel_Click(this, EventArgs.Empty);
                return;
            }

            // Dispose animation timers
            foreach (var timer in _activeAnimationTimers)
            {
                timer?.Stop();
                timer?.Dispose();
            }
            _activeAnimationTimers.Clear();

            // Clean up any animation overlay PictureBox left behind if form closed mid-animation
            for (int i = _contentPanel.Controls.Count - 1; i >= 0; i--)
            {
                if (_contentPanel.Controls[i] is PictureBox pb)
                {
                    _contentPanel.Controls.RemoveAt(i);
                    pb.Dispose();
                }
            }

            // Unsubscribe from current step validation
            if (_contentPanel.Controls.Count > 0)
            {
                for (int i = 0; i < _contentPanel.Controls.Count; i++)
                {
                    if (_contentPanel.Controls[i] is IWizardStepContent stepContent)
                        stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                }
            }

            // Dispose card fonts
            _cardTitleFont?.Dispose();
            _cardDescFont?.Dispose();
            _cardNumberFont?.Dispose();

            _cachedPages.Clear();

            WizardManager.UnregisterWizard(_instance.Config.Key);
            base.OnFormClosing(e);
        }

        #endregion
    }
}
