using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Wizards.Painters;
using TheTechIdea.Beep.Winform.Controls.Wizards.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    /// <summary>
    /// Minimal wizard form with simple progress indicator
    /// </summary>
    public class MinimalWizardForm : BeepiFormPro, IWizardFormHost
    {
        #region Fields

        private WizardInstance _instance;
        private MinimalPainter _painter;
        private MinimalLayout _layoutManager;
        
        private Panel _headerPanel;
        private Panel _contentPanel;
        private Panel _buttonPanel;
        
        private BeepButton _btnNext;
        private BeepButton _btnBack;
        private BeepButton _btnCancel;

        #endregion

        #region Properties

        public WizardInstance Instance => _instance;

        #endregion

        #region Constructor

        public MinimalWizardForm()
        {
            // Designer Mode Setup
            var config = new WizardConfig { Title = "Wizard Design Mode" };
            config.Steps.Add(new WizardStep { Title = "Step 1", Description = "Design Mode Step 1" });
            config.Steps.Add(new WizardStep { Title = "Step 2", Description = "Design Mode Step 2" });
            config.Steps.Add(new WizardStep { Title = "Step 3", Description = "Design Mode Step 3" });

            _instance = new WizardInstance(config);
            _instance.BindFormHost(this);
            
            _painter = new MinimalPainter();
            _layoutManager = new MinimalLayout();

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();
            UpdateUI();
        }

        public MinimalWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instance.BindFormHost(this);
            
            _painter = new MinimalPainter();
            _layoutManager = new MinimalLayout();

            InitializeForm();
            InitializeControls();
            LayoutControls();
            SetupEventHandlers();
            ApplyTheme();
            UpdateUI();
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

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        private void InitializeControls()
        {
            // Header panel with minimal progress (top)
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.Transparent
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            // Button panel (bottom)
            _buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Content panel (fill)
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 20, 40, 20)
            };

            // Navigation buttons
            _btnNext = new BeepButton
            {
                Text = _instance.Config.NextButtonText,
                Size = new Size(120, 36),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            _btnBack = new BeepButton
            {
                Text = _instance.Config.BackButtonText,
                Size = new Size(100, 36),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false
            };

            _btnCancel = new BeepButton
            {
                Text = _instance.Config.CancelButtonText,
                Size = new Size(100, 36),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            _painter.Initialize(this, CurrentTheme, _instance);
        }

        private void LayoutControls()
        {
            int rightEdge = _buttonPanel.ClientSize.Width - _buttonPanel.Padding.Right;
            int buttonY = (_buttonPanel.Height - _btnNext.Height) / 2;

            _btnNext.Location = new Point(rightEdge - _btnNext.Width, buttonY);
            _btnBack.Location = new Point(_btnNext.Left - _btnBack.Width - 10, buttonY);
            _btnCancel.Location = new Point(_buttonPanel.Padding.Left, buttonY);

            _buttonPanel.Controls.Add(_btnNext);
            _buttonPanel.Controls.Add(_btnBack);
            _buttonPanel.Controls.Add(_btnCancel);

            Controls.Add(_contentPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);
        }

        private void SetupEventHandlers()
        {
            _btnNext.Click += BtnNext_Click;
            _btnBack.Click += BtnBack_Click;
            _btnCancel.Click += BtnCancel_Click;
            KeyDown += Form_KeyDown;
            Resize += Form_Resize;
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

            _contentPanel.Controls.Clear();
            if (currentStep?.Content != null)
            {
                currentStep.Content.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(currentStep.Content);

                if (currentStep.Content is IWizardStepContent stepContent)
                {
                    stepContent.ValidationStateChanged -= StepContent_ValidationStateChanged;
                    stepContent.ValidationStateChanged += StepContent_ValidationStateChanged;
                }
            }

            _headerPanel.Invalidate();
        }

        public void ShowValidationError(string message)
        {
            MessageBox.Show(this, message, "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowValidationError(WizardValidationResult result)
        {
            if (result == null || result.IsValid) return;
            ShowValidationError(result.ErrorMessage ?? "Validation failed");
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

        private void StepContent_ValidationStateChanged(object sender, StepValidationEventArgs e)
        {
            _btnNext.Enabled = e.IsValid;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (_btnNext.Enabled)
                    {
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

        private void Form_Resize(object sender, EventArgs e)
        {
            _headerPanel.Invalidate();
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            _painter.PaintMinimalProgress(e.Graphics, _headerPanel.ClientRectangle,
                _instance.CurrentStepIndex, _instance.Config.Steps.Count, _instance.Config.Steps);
        }

        #endregion

        #region Theme

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (CurrentTheme != null)
            {
                BackColor = CurrentTheme.BackColor;
                _contentPanel.BackColor = CurrentTheme.BackColor;
                _buttonPanel.BackColor = CurrentTheme.BackColor;
                _headerPanel.BackColor = CurrentTheme.BackColor;

                _btnNext.Theme = CurrentTheme.ThemeName;
                _btnBack.Theme = CurrentTheme.ThemeName;
                _btnCancel.Theme = CurrentTheme.ThemeName;

                _btnNext.ApplyTheme();
                _btnBack.ApplyTheme();
                _btnCancel.ApplyTheme();

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

            WizardManager.UnregisterWizard(_instance.Config.Key);
            base.OnFormClosing(e);
        }

        #endregion
    }
}
