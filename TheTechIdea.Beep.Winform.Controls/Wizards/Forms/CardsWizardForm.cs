using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
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

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();

            // Apply Config.Theme if configured, otherwise use default
            if (_instance.Config.Theme != null)
            {
                CurrentTheme = _instance.Config.Theme;
            }
            InitializeColors(CurrentTheme);
            ApplyTheme();
            UpdateUI();
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
                _cardBgColor = ControlPaint.Dark(theme.BackColor, 0.03f);
            }
            else
            {
                _completedColor = Color.FromArgb(46, 125, 50);
                _currentColor = Color.FromArgb(25, 118, 210);
                _pendingColor = Color.FromArgb(150, 150, 150);
                _textColor = Color.FromArgb(50, 50, 50);
                _subtextColor = Color.FromArgb(120, 120, 120);
                _cardBgColor = Color.FromArgb(245, 245, 250);
            }

            try { _cardTitleFont?.Dispose(); } catch { }
            try { _cardDescFont?.Dispose(); } catch { }
            try { _cardNumberFont?.Dispose(); } catch { }

            _cardTitleFont = new Font("Segoe UI Semibold", 10f);
            _cardDescFont = new Font("Segoe UI", 8.5f);
            _cardNumberFont = new Font("Segoe UI Semibold", 12f);
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
                Width = 220,
                AutoScroll = true,
                Padding = new Padding(10, 15, 10, 15),
                BackColor = _cardBgColor
            };

            // Inline error panel (top of content area, hidden by default)
            _errorPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Visible = false,
                BackColor = Color.FromArgb(255, 235, 235),
                Padding = new Padding(16, 0, 16, 0)
            };
            _lblError = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(180, 30, 30),
                Font = new Font("Segoe UI", 9.5f),
                AutoEllipsis = true
            };
            _errorPanel.Controls.Add(_lblError);

            // Button panel (bottom)
            _buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Content panel (fill)
            _contentPanel = new Panel
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
                Height = 40,
                Font = new Font("Segoe UI Semibold", 13f),
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
            var card = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Margin = new Padding(0, 0, 0, 6),
                Padding = new Padding(10),
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

            // Left accent bar for current step
            if (isCurrent)
            {
                using var accentBrush = new SolidBrush(_currentColor);
                g.FillRectangle(accentBrush, bounds.Left, bounds.Top + 8, 4, bounds.Height - 16);
            }

            // Step number circle
            int circleSize = 30;
            int circleX = bounds.Left + 14;
            int circleY = bounds.Top + (bounds.Height - circleSize) / 2;
            var circleRect = new Rectangle(circleX, circleY, circleSize, circleSize);

            Color circleColor, innerColor;
            if (isCompleted)
            {
                circleColor = _completedColor;
                innerColor = Color.White;
            }
            else if (isCurrent)
            {
                circleColor = _currentColor;
                innerColor = Color.White;
            }
            else
            {
                circleColor = _pendingColor;
                innerColor = _pendingColor;
            }

            // Draw circle
            if (isCompleted || isCurrent)
            {
                using var circleBrush = new SolidBrush(circleColor);
                g.FillEllipse(circleBrush, circleRect);
            }
            else
            {
                using var circlePen = new Pen(circleColor, 2f);
                g.DrawEllipse(circlePen, circleRect);
            }

            // Draw content in circle
            if (isCompleted)
            {
                // Checkmark
                using var pen = new Pen(innerColor, 2f);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                int cx = circleRect.X + circleRect.Width / 2;
                int cy = circleRect.Y + circleRect.Height / 2;
                g.DrawLines(pen, new Point[]
                {
                    new Point(cx - 5, cy),
                    new Point(cx - 2, cy + 4),
                    new Point(cx + 6, cy - 4)
                });
            }
            else
            {
                // Step number
                using var brush = new SolidBrush(innerColor);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString((stepIndex + 1).ToString(), _cardNumberFont, brush, circleRect, sf);
            }

            // Step title and description
            int textX = circleX + circleSize + 12;
            int textWidth = bounds.Width - textX - 10;

            var titleColor = isCurrent ? _textColor : _subtextColor;
            using (var titleBrush = new SolidBrush(titleColor))
            {
                var titleFont = isCurrent ? _cardTitleFont : _cardDescFont;
                g.DrawString(step.Title ?? $"Step {stepIndex + 1}", titleFont, titleBrush,
                    new Rectangle(textX, circleY, textWidth, 18));
            }

            if (!string.IsNullOrEmpty(step.Description))
            {
                using var descBrush = new SolidBrush(_subtextColor);
                g.DrawString(step.Description, _cardDescFont, descBrush,
                    new Rectangle(textX, circleY + 20, textWidth, 16));
            }

            // Optional badge
            if (step.IsOptional && !isCompleted)
            {
                using var optBrush = new SolidBrush(Color.FromArgb(60, _pendingColor));
                using var optFont = new Font("Segoe UI", 7f, FontStyle.Italic);
                g.DrawString("Optional", optFont, optBrush,
                    new Rectangle(textX, circleY + 36, textWidth, 14));
            }
        }

        private void CardClicked(int stepIndex)
        {
            if (_instance.Config.AllowSkip || stepIndex <= _instance.CurrentStepIndex)
            {
                _instance.NavigateToAsync(stepIndex);
            }
        }

        #endregion

        #region IWizardFormHost Implementation

        public void UpdateUI()
        {
            var currentStep = _instance.CurrentStep;

            _btnBack.Enabled = _instance.Config.AllowBack && !_instance.IsFirstStep;

            var nextText = currentStep?.NextButtonText
                ?? (_instance.IsLastStep ? _instance.Config.FinishButtonText : _instance.Config.NextButtonText);
            _btnNext.Text = nextText;

            if (_btnSkip != null)
            {
                _btnSkip.Visible = currentStep?.IsOptional ?? false;
            }

            // Get previous and new content controls
            Control previousControl = _contentPanel.Controls.Count > 0 ? _contentPanel.Controls[0] : null;
            Control newControl = currentStep?.Content;
            int currentIndex = _instance.CurrentStepIndex;

            if (newControl != null)
            {
                newControl.Dock = DockStyle.Fill;

                if (newControl is IWizardStepContent stepContent)
                {
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                    stepContent.ValidationStateChanged += StepContent_ValidationStateChanged;
                }
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
                newControl.Dock = DockStyle.None;
                newControl.Size = _contentPanel.ClientSize;

                _contentPanel.Controls.Clear();
                _contentPanel.Controls.Add(previousControl);
                _contentPanel.Controls.Add(newControl);

                WizardHelpers.AnimateStepTransition(previousControl, newControl, forward, () =>
                {
                    _contentPanel.Controls.Clear();
                    if (newControl != null)
                    {
                        newControl.Dock = DockStyle.Fill;
                        _contentPanel.Controls.Add(newControl);
                    }
                }, _activeAnimationTimers);
            }
            else
            {
                _contentPanel.Controls.Clear();
                if (newControl != null)
                {
                    _contentPanel.Controls.Add(newControl);
                }
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
            ShowValidationError(result.ErrorMessage ?? "Validation failed");
        }

        public void HideValidationError()
        {
            _errorPanel.Visible = false;
        }

        public Panel GetContentPanel() => _contentPanel;

        #endregion

        #region Event Handlers

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_instance.IsLastStep)
                _instance.Complete();
            else
                _instance.NavigateNext();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            _instance.NavigateBack();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this,
                "Are you sure you want to cancel?",
                "Cancel Wizard",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                _instance.Cancel();
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            if (_instance.CurrentStep?.IsOptional == true)
            {
                _instance.CurrentStep.State = StepState.Skipped;
                _instance.NavigateNext();
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
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _instance.Config.HelpUrl,
                    UseShellExecute = true
                });
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
            }
        }

        #endregion

        #region Theme

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (CurrentTheme != null)
            {
                InitializeColors(CurrentTheme);

                BackColor = CurrentTheme.BackColor;
                _contentPanel.BackColor = CurrentTheme.BackColor;
                _buttonPanel.BackColor = CurrentTheme.BackColor;
                _cardPanel.BackColor = _cardBgColor;

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

            // Dispose fonts
            _cardTitleFont?.Dispose();
            _cardDescFont?.Dispose();
            _cardNumberFont?.Dispose();

            WizardManager.UnregisterWizard(_instance.Config.Key);
            base.OnFormClosing(e);
        }

        #endregion
    }
}
