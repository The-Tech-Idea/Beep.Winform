using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Base class for all wizard form implementations
    /// </summary>
    public abstract class BaseWizardForm : Form, IWizardForm
    {
        protected readonly WizardInstance _instance;
        protected Panel _contentPanel;
        protected Panel _buttonPanel;
        protected Panel _headerPanel;
        protected Button _btnNext;
        protected Button _btnBack;
        protected Button _btnCancel;
        protected Button _btnHelp;

        public BaseWizardForm(WizardInstance instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            
            InitializeComponents();
            LayoutControls();
            UpdateUI();
        }

        /// <summary>
        /// Initialize common controls
        /// </summary>
        protected virtual void InitializeComponents()
        {
            // Header panel (override in derived classes)
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White
            };

            // Content panel (where step controls are displayed)
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Button panel
            _buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Buttons
            _btnNext = new Button
            {
                Text = _instance.Config.NextButtonText,
                Size = new Size(100, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = _instance.Config.Theme?.AccentColor ?? Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            _btnNext.FlatAppearance.BorderSize = 0;
            _btnNext.Click += BtnNext_Click;

            _btnBack = new Button
            {
                Text = _instance.Config.BackButtonText,
                Size = new Size(100, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnBack.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            _btnBack.Click += BtnBack_Click;

            _btnCancel = new Button
            {
                Text = _instance.Config.CancelButtonText,
                Size = new Size(100, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            _btnCancel.Click += BtnCancel_Click;

            if (_instance.Config.ShowHelp)
            {
                _btnHelp = new Button
                {
                    Text = "?",
                    Size = new Size(36, 36),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(100, 100, 100),
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                _btnHelp.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
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
            _btnNext.Text = isLastStep ? _instance.Config.FinishButtonText : _instance.Config.NextButtonText;

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
                "Are you sure you want to cancel?",
                "Cancel Wizard",
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
