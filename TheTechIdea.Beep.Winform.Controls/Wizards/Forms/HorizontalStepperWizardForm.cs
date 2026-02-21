using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards.Painters;
using TheTechIdea.Beep.Winform.Controls.Wizards.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    /// <summary>
    /// Horizontal stepper wizard form with progress bar at top
    /// </summary>
    public class HorizontalStepperWizardForm : BeepiFormPro, IWizardFormHost
    {
        #region Fields

        private WizardInstance _instance;
        private HorizontalStepperPainter _painter;
        private HorizontalStepperLayout _layoutManager;
        
        private Panel _contentPanel;
        private Panel _stepIndicatorPanel;
        private Panel _buttonPanel;
        private Panel _errorPanel;
        private Label _lblError;
        
        private BeepButton _btnNext;
        private BeepButton _btnBack;
        private BeepButton _btnCancel;
        private BeepButton _btnSkip;
        private BeepButton _btnHelp;

        private readonly List<Timer> _activeAnimationTimers = new List<Timer>();
        private int _previousStepIndex = -1;

        #endregion

        #region Properties

        public WizardInstance Instance => _instance;

        #endregion

        #region Constructor

        public HorizontalStepperWizardForm()
        {
            // Designer Mode Setup
            var config = new WizardConfig { Title = "Wizard Design Mode" };
            config.Steps.Add(new WizardStep { Title = "Step 1", Description = "Design Mode Step 1" });
            config.Steps.Add(new WizardStep { Title = "Step 2", Description = "Design Mode Step 2" });
            config.Steps.Add(new WizardStep { Title = "Step 3", Description = "Design Mode Step 3" });

            _instance = new WizardInstance(config);
            _instance.BindFormHost(this);
            
            _painter = new HorizontalStepperPainter();
            _layoutManager = new HorizontalStepperLayout();

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();
            // Theme might be null at design time, handled safely in ApplyTheme
            UpdateUI();
        }

        public HorizontalStepperWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instance.BindFormHost(this);
            
            _painter = new HorizontalStepperPainter();
            _layoutManager = new HorizontalStepperLayout();

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();

            // Apply Config.Theme if configured, otherwise use default
            if (_instance.Config.Theme != null)
            {
                CurrentTheme = _instance.Config.Theme;
            }
            UpdateUI();
            ApplyTheme();
        }

        #endregion

        #region Initialization

        private void InitializeForm()
        {
            Text = _instance.Config.Title;
            Size = _instance.Config.Size;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            // Set double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        private void InitializeControls()
        {
            // Step indicator panel (top)
            _stepIndicatorPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };
            _stepIndicatorPanel.Paint += StepIndicatorPanel_Paint;

            // Inline error panel (below step indicator, hidden by default)
            _errorPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
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
                Size = new Size(110, 40),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            _btnBack = new BeepButton
            {
                Text = _instance.Config.BackButtonText,
                Size = new Size(110, 40),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false
            };

            _btnCancel = new BeepButton
            {
                Text = _instance.Config.CancelButtonText,
                Size = new Size(110, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            if (_instance.Config.AllowSkip)
            {
                _btnSkip = new BeepButton
                {
                    Text = _instance.Config.SkipButtonText,
                    Size = new Size(110, 40),
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

            // Initialize painter
            _painter.Initialize(this, CurrentTheme, _instance);
        }

        private void LayoutControls()
        {
            // Layout buttons
            int rightEdge = _buttonPanel.ClientSize.Width - _buttonPanel.Padding.Right;
            int buttonY = (_buttonPanel.Height - _btnNext.Height) / 2;

            _btnNext.Location = new Point(rightEdge - _btnNext.Width, buttonY);
            _btnBack.Location = new Point(_btnNext.Left - _btnBack.Width - 10, buttonY);
            _btnCancel.Location = new Point(_buttonPanel.Padding.Left, buttonY);

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

            // Add panels to form (order matters for Dock layout)
            Controls.Add(_contentPanel);
            Controls.Add(_errorPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_stepIndicatorPanel);
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
            Resize += Form_Resize;
        }

        #endregion

        #region IWizardFormHost Implementation

        public void UpdateUI()
        {
            var currentStep = _instance.CurrentStep;
            int currentIndex = _instance.CurrentStepIndex;
            int totalSteps = _instance.Config.Steps.Count;

            // Update button states
            _btnBack.Enabled = _instance.Config.AllowBack && !_instance.IsFirstStep;
            
            // Update Next button text
            var nextText = currentStep?.NextButtonText 
                ?? (_instance.IsLastStep ? _instance.Config.FinishButtonText : _instance.Config.NextButtonText);
            _btnNext.Text = nextText;

            // Show/hide skip button for optional steps
            if (_btnSkip != null)
            {
                _btnSkip.Visible = currentStep?.IsOptional ?? false;
            }

            // Get previous and new content controls
            Control previousControl = _contentPanel.Controls.Count > 0 ? _contentPanel.Controls[0] : null;
            Control newControl = currentStep?.Content;

            if (newControl != null)
            {
                newControl.Dock = DockStyle.Fill;

                // Subscribe to validation state changes
                if (newControl is IWizardStepContent stepContent)
                {
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                    stepContent.ValidationStateChanged += StepContent_ValidationStateChanged;
                }
            }

            // Animate transition if enabled and we have both controls
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
                // No animation - direct swap
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

            // Repaint step indicators
            _stepIndicatorPanel.Invalidate();
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
            {
                _instance.Complete();
            }
            else
            {
                _instance.NavigateNext();
            }
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
            {
                _instance.Cancel();
            }
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            if (_instance.CurrentStep?.IsOptional == true)
            {
                _instance.CurrentStep.State = StepState.Skipped;
                _instance.NavigateNext();
            }
        }

        private void StepContent_ValidationStateChanged(object sender, StepValidationEventArgs e)
        {
            // Enable/disable Next button based on validation state
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

        private void Form_Resize(object sender, EventArgs e)
        {
            _stepIndicatorPanel.Invalidate();
        }

        private void StepIndicatorPanel_Paint(object sender, PaintEventArgs e)
        {
            _painter.PaintStepIndicators(e.Graphics, _stepIndicatorPanel.ClientRectangle,
                _instance.CurrentStepIndex, _instance.Config.Steps.Count, _instance.Config.Steps);
        }

        #endregion

        #region Theme

        public override void ApplyTheme()
        {
            // Guard: base ctor triggers ApplyTheme before any wizard fields are initialized
            if (_instance == null || _contentPanel == null || _buttonPanel == null || _stepIndicatorPanel == null)
                return;
            base.ApplyTheme();

            if (CurrentTheme != null)
            {
                BackColor = CurrentTheme.BackColor;
                _contentPanel.BackColor = CurrentTheme.BackColor;
                _buttonPanel.BackColor = CurrentTheme.BackColor;
                _stepIndicatorPanel.BackColor = CurrentTheme.BackColor;
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

                _painter.Initialize(this, CurrentTheme, _instance);
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

            WizardManager.UnregisterWizard(_instance.Config.Key);
            base.OnFormClosing(e);
        }

        #endregion
    }
}
