using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards.Animations;
using TheTechIdea.Beep.Winform.Controls.Wizards.Localization;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Base class for all wizard form implementations
    /// </summary>
    public abstract class BaseWizardForm : BeepiFormPro, IWizardForm
    {
        protected readonly WizardInstance _instance;
        protected BeepPanel _contentPanel;
        protected BeepPanel _buttonPanel;
        protected BeepPanel _headerPanel;
        protected BeepButton _btnNext;
        protected BeepButton _btnBack;
        protected BeepButton _btnCancel;
        protected BeepButton _btnHelp;

        public BaseWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            
            InitializeComponents();
            LayoutControls();
            SetupAccessibility();
            SetupKeyboardNavigation();
            UpdateUI();
        }

        /// <summary>
        /// Initialize common controls
        /// </summary>
        protected virtual void InitializeComponents()
        {
            // Header panel (override in derived classes)
            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                ShowTitle = false,  // No title for header panel (title is handled by labels)
                ShowTitleLine = false
            };
            _headerPanel.ApplyTheme();

            // Content panel (where step controls are displayed)
            _contentPanel = new BeepPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ShowTitle = false,  // No title for content panel
                ShowTitleLine = false
            };
            _contentPanel.ApplyTheme();

            // Button panel
            _buttonPanel = new BeepPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20, 10, 20, 10),
                ShowTitle = false,  // No title for button panel
                ShowTitleLine = false
            };
            _buttonPanel.ApplyTheme();

            // Buttons
            _btnNext = new BeepButton
            {
                Text = _instance.Config.NextButtonText,
                Size = new Size(100, 36),
                ButtonType = ButtonType.Normal
            };
            _btnNext.ApplyTheme();
            _btnNext.Click += BtnNext_Click;

            _btnBack = new BeepButton
            {
                Text = _instance.Config.BackButtonText,
                Size = new Size(100, 36),
                ButtonType = ButtonType.Normal,
                Enabled = false
            };
            _btnBack.ApplyTheme();
            _btnBack.Click += BtnBack_Click;

            _btnCancel = new BeepButton
            {
                Text = _instance.Config.CancelButtonText,
                Size = new Size(100, 36),
                ButtonType = ButtonType.Normal
            };
            _btnCancel.ApplyTheme();
            _btnCancel.Click += BtnCancel_Click;

            if (_instance.Config.ShowHelp)
            {
                _btnHelp = new BeepButton
                {
                    Text = "?",
                    Size = new Size(36, 36),
                    ButtonType = ButtonType.Normal
                };
                _btnHelp.ApplyTheme();
                _btnHelp.Click += BtnHelp_Click;
            }
        }

        /// <summary>
        /// Layout controls in form
        /// </summary>
        protected virtual void LayoutControls()
        {
            // Add button panel controls
            var rightEdge = _buttonPanel.ClientSize.Width - _buttonPanel.Padding.Right;
            
            _btnNext.Location = new Point(rightEdge - _btnNext.Width, (_buttonPanel.Height - _btnNext.Height) / 2);
            _btnBack.Location = new Point(_btnNext.Left - _btnBack.Width - 10, (_buttonPanel.Height - _btnBack.Height) / 2);
            _btnCancel.Location = new Point(_buttonPanel.Padding.Left, (_buttonPanel.Height - _btnCancel.Height) / 2);

            _buttonPanel.Controls.Add(_btnNext);
            _buttonPanel.Controls.Add(_btnBack);
            _buttonPanel.Controls.Add(_btnCancel);

            if (_btnHelp != null)
            {
                _btnHelp.Location = new Point(_btnCancel.Right + 10, (_buttonPanel.Height - _btnHelp.Height) / 2);
                _buttonPanel.Controls.Add(_btnHelp);
            }

            // Add panels to form
            Controls.Add(_contentPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);
        }

        /// <summary>
        /// Update UI based on current wizard state
        /// </summary>
        public virtual void UpdateUI()
        {
            var currentIndex = _instance.CurrentStepIndex;
            var totalSteps = _instance.Config.Steps.Count;

            // Update button states
            _btnBack.Enabled = _instance.Config.AllowBack && currentIndex > 0;
            
            var isLastStep = currentIndex >= totalSteps - 1;
            _btnNext.Text = isLastStep 
                ? (_instance.Config.FinishButtonText ?? WizardLocalizer.FinishButton)
                : (_instance.Config.NextButtonText ?? WizardLocalizer.NextButton);

            // Update content panel
            _contentPanel.Controls.Clear();

            if (currentIndex >= 0 && currentIndex < totalSteps)
            {
                var currentStep = _instance.Config.Steps[currentIndex];
                if (currentStep.Content != null)
                {
                    currentStep.Content.Dock = DockStyle.Fill;
                    _contentPanel.Controls.Add(currentStep.Content);

                    // Inject wizard context if control implements IWizardStepControl
                    if (currentStep.Content is IWizardStepControl wizardControl)
                    {
                        wizardControl.SetWizardContext(_instance.Context);
                    }
                }
            }

            UpdateHeader();
        }

        /// <summary>
        /// Update header content (override in derived classes)
        /// </summary>
        protected abstract void UpdateHeader();

        /// <summary>
        /// Setup accessibility features
        /// </summary>
        protected virtual void SetupAccessibility()
        {
            // Set accessible names
            if (_btnNext != null)
            {
                _btnNext.AccessibleName = _instance.Config.NextButtonText;
                _btnNext.AccessibleDescription = "Navigate to the next step";
            }

            if (_btnBack != null)
            {
                _btnBack.AccessibleName = _instance.Config.BackButtonText;
                _btnBack.AccessibleDescription = "Navigate to the previous step";
            }

            if (_btnCancel != null)
            {
                _btnCancel.AccessibleName = _instance.Config.CancelButtonText;
                _btnCancel.AccessibleDescription = "Cancel the wizard";
            }

            // Set form accessible properties
            this.AccessibleName = _instance.Config.Title;
            this.AccessibleDescription = _instance.Config.Description ?? "Wizard dialog";
            this.AccessibleRole = AccessibleRole.Dialog;
        }

        /// <summary>
        /// Setup keyboard navigation
        /// </summary>
        protected virtual void SetupKeyboardNavigation()
        {
            this.KeyPreview = true;
            this.KeyDown += BaseWizardForm_KeyDown;
        }

        private void BaseWizardForm_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (e.Control)
                    {
                        // Ctrl+Enter to finish
                        if (_instance.CurrentStepIndex >= _instance.Config.Steps.Count - 1)
                        {
                            BtnNext_Click(this, EventArgs.Empty);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        // Enter to go to next
                        BtnNext_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;

                case Keys.Escape:
                    // Escape to cancel
                    BtnCancel_Click(this, EventArgs.Empty);
                    e.Handled = true;
                    break;

                case Keys.Left:
                case Keys.Back:
                    if (_btnBack != null && _btnBack.Enabled)
                    {
                        BtnBack_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;

                case Keys.Right:
                    if (_btnNext != null && _btnNext.Enabled)
                    {
                        BtnNext_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
            }
        }

        #region Event Handlers

        protected virtual void BtnNext_Click(object sender, EventArgs e)
        {
            var currentIndex = _instance.CurrentStepIndex;
            var isLastStep = currentIndex >= _instance.Config.Steps.Count - 1;

            if (isLastStep)
            {
                // Finish wizard
                _instance.Complete();
            }
            else
            {
                // Navigate to next step
                _instance.NavigateNext();
            }
        }

        protected virtual void BtnBack_Click(object sender, EventArgs e)
        {
            _instance.NavigateBack();
        }

        protected virtual void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                WizardLocalizer.CancelConfirmation,
                WizardLocalizer.CancelTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _instance.Cancel();
            }
        }

        protected virtual void BtnHelp_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_instance.Config.HelpUrl))
            {
                System.Diagnostics.Process.Start(_instance.Config.HelpUrl);
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_instance.Result == WizardResult.None && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                BtnCancel_Click(this, EventArgs.Empty);
            }

            base.OnFormClosing(e);
        }
    }

    /// <summary>
    /// Interface for wizard step UserControls to implement
    /// Allows dependency injection of WizardContext
    /// </summary>
    public interface IWizardStepControl
    {
        void SetWizardContext(WizardContext context);
    }
}
