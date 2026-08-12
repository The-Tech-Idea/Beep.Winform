using TheTechIdea.Beep.Winform.Controls.Steppers.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards.Layout;
using TheTechIdea.Beep.Winform.Controls.Wizards.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    /// <summary>
    /// Horizontal stepper wizard form with progress bar at top
    /// </summary>
    public partial class HorizontalStepperWizardForm : BeepiFormPro, IWizardFormHost
    {
        #region Fields

        private WizardInstance _instance;
        private HorizontalStepperPainter _painter;
        private HorizontalStepperLayout _layout;
        
        

        private readonly List<Timer> _activeAnimationTimers = new List<Timer>();
        private int _previousStepIndex = -1;
        private Panel _loadingOverlay;

        private readonly Dictionary<int, Control> _cachedPages = new Dictionary<int, Control>();

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

            InitializeComponent();
            InitializeForm();
            ApplyConfigToControls();
            SetupEventHandlers();
            // Theme might be null at design time, handled safely in ApplyTheme
            UpdateUI();
        }

        public HorizontalStepperWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instance.BindFormHost(this);

            _painter = new HorizontalStepperPainter();
            _layout = new HorizontalStepperLayout();
            _layout.Initialize(this, _instance);

            InitializeComponent();
            InitializeForm();
            ApplyConfigToControls();
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

            // RTL support
            if (_instance.Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }

            // Set double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Applies the wizard's configuration to controls the designer already built.
        /// </summary>
        /// <remarks>
        /// The layout lives in HorizontalStepperWizardForm.Designer.cs so it shows on the design surface.
        /// Only config-dependent state belongs here - captions, and whether the optional Skip and
        /// Help buttons appear - because a conditional inside InitializeComponent breaks the
        /// designer.
        /// </remarks>
        private void ApplyConfigToControls()
        {
            _btnNext.Text   = _instance.Config.NextButtonText;
            _btnBack.Text   = _instance.Config.BackButtonText;
            _btnCancel.Text = _instance.Config.CancelButtonText;
            _btnSkip.Text   = _instance.Config.SkipButtonText;

            _btnSkip.Visible = _instance.Config.AllowSkip;
            _btnHelp.Visible = _instance.Config.ShowHelp;

            _errorPanel.BackColor = WizardHelpers.GetWarningBackColor(CurrentTheme);
            _lblError.ForeColor   = WizardHelpers.GetErrorColor(CurrentTheme);

            // Depends on the instance, so it cannot live in the designer - and dropping it is what
            // left the step rail blank after the layout moved into InitializeComponent.
            _painter.Initialize(this, CurrentTheme, _instance);

            // Feed the stepper from the wizard's own step list. It draws the circles, connectors,
            // labels and states that the form used to paint by hand.
            // ASSIGN the property rather than mutating the existing list: the setter is what
            // syncs the control's stepCount, and stepCount is what the painter takes its models
            // through (`_stepModels.Take(stepCount)`). Mutating in place left the count stale.
            var models = new System.ComponentModel.BindingList<StepModel>();
            foreach (var wizardStep in _instance.Config.Steps)
                models.Add(new StepModel { Text = wizardStep.Title, Subtitle = wizardStep.Description });
            _stepper.StepModels = models;
            _stepper.CurrentStep = _instance.CurrentStepIndex;
        }

        private void SetupEventHandlers()
        {
            // Button clicks are wired in the designer file, beside the controls.

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

            // Update step count label
            // The stepper shows the step position itself; the separate count label is gone.

            // Screen reader accessibility
            _stepper.AccessibleName = $"Step {currentIndex + 1} of {totalSteps}: {currentStep?.Title ?? ""}";
            _stepper.AccessibleDescription = currentStep?.Description ?? "";
            _stepper.AccessibleRole = AccessibleRole.ProgressBar;
            AccessibilityNotifyClients(AccessibleEvents.Focus, 0);

            // Update progress bar
            if (_progressBar != null && _instance.Config.ShowProgressBar)
            {
                _progressBar.Visible = true;
                _progressBar.Value = (int)_instance.Context.CompletionPercentage;
            }

            // Update button states
            _btnBack.Enabled = _instance.Config.AllowBack && !_instance.IsFirstStep;
            
            // Update Next button text
            var nextText = (currentStep?.Content as IWizardStepContent)?.NextButtonText
                ?? currentStep?.NextButtonText 
                ?? (_instance.IsLastStep ? _instance.Config.FinishButtonText : _instance.Config.NextButtonText);
            _btnNext.Text = nextText;

            // Show/hide skip button for optional steps
            if (_btnSkip != null)
            {
                _btnSkip.Visible = currentStep?.IsOptional ?? false;
            }

            // Get new content control — reuse cached if available
            Control newControl = null;
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

            // INDICATOR-ONLY ANIMATION: Instant page swap + animated step indicator bar
            // Pattern matches BeepDisplayContainer2 — no bitmap capture, no DrawToBitmap overhead
            _contentPanel.SuspendLayout();
            try
            {
                if (newControl is IWizardStepContent stepContent)
                {
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                    stepContent.ValidationStateChanged += StepContent_ValidationStateChanged;
                    _btnNext.Enabled = _instance.IsLastStep || stepContent.IsComplete;
                }

                if (newControl.Parent != _contentPanel)
                {
                    newControl.Dock = DockStyle.Fill;
                    _contentPanel.Controls.Add(newControl);
                }

                // Instant visibility swap — zero bitmap overhead
                for (int i = 0; i < _contentPanel.Controls.Count; i++)
                    _contentPanel.Controls[i].Visible = false;
                newControl.Visible = true;
            }
            finally { _contentPanel.ResumeLayout(false); }

            // Animate the step indicator connector bar for visual continuity
            if (_previousStepIndex >= 0)
            {
                _painter.StartConnectorAnimation(currentIndex, _instance.Config.TransitionDurationMs);
            }

            _previousStepIndex = currentIndex;

            // Auto-hide errors on step change
            if (_instance.Config.AutoHideErrors)
            {
                HideValidationError();
            }

            // Repaint step indicators
            _stepper.Invalidate();
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
                _loadingOverlay = new Panel
                {
                    BackColor = Color.FromArgb(160, BackColor),
                    Dock = DockStyle.Fill,
                    Visible = false
                };
                var label = new Label
                {
                    Text = message ?? "Please wait...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = ForeColor,
                    Font = WizardHelpers.GetFont(CurrentTheme, CurrentTheme?.BodyStyle, 12f, FontStyle.Regular),
                    Dock = DockStyle.Fill
                };
                _loadingOverlay.Controls.Add(label);
                Controls.Add(_loadingOverlay);
                _loadingOverlay.BringToFront();
            }
            _loadingOverlay.Visible = true;
        }

        public void HideLoading()
        {
            if (_loadingOverlay != null)
                _loadingOverlay.Visible = false;
        }

        public Panel GetContentPanel() => _contentPanel;

        #endregion

        #region Event Handlers

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            if (_instance.IsLastStep)
            {
                await _instance.CompleteAsync();
            }
            else
            {
                await _instance.NavigateNextAsync();
            }
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

        private void Form_Resize(object sender, EventArgs e)
        {
            _stepper.Invalidate();
        }
        // StepIndicatorPanel_Paint is gone: BeepStepperBar draws the steps now, so the
        // form no longer hand-paints circles, connectors and labels.

        private async void StepIndicatorPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (_layout == null || _instance.IsNavigating) return;
            int stepIndex = _layout.HitTestStep(e.Location);
            if (stepIndex >= 0 && stepIndex < _instance.CurrentStepIndex)
                await _instance.NavigateToAsync(stepIndex);
        }

        #endregion

        #region Theme

        public override void ApplyTheme()
        {
            // Guard: base ctor triggers ApplyTheme before any wizard fields are initialized
            if (_instance == null || _contentPanel == null || _buttonPanel == null || _stepper == null)
                return;
            base.ApplyTheme();

            if (CurrentTheme != null)
            {
                BackColor = CurrentTheme.BackColor;
                _contentPanel.BackColor = CurrentTheme.BackColor;
                _buttonPanel.BackColor = CurrentTheme.BackColor;
                _stepper.BackColor = CurrentTheme.BackColor;
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

                _progressBar?.ApplyTheme();
                _lblError?.ApplyTheme();

                _painter.Initialize(this, CurrentTheme, _instance);
            }
        }

        #endregion

        // Phase D: WS_EX_COMPOSITED for system-level double-buffering — eliminates flicker
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (_instance?.Config?.EnableCompositedRendering != false)
                    cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
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
            for (int i = 0; i < _contentPanel.Controls.Count; i++)
            {
                if (_contentPanel.Controls[i] is IWizardStepContent stepContent)
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
            }

            _cachedPages.Clear();

            WizardManager.UnregisterWizard(_instance.Config.Key);
            base.OnFormClosing(e);
        }

        #endregion
    }
}
