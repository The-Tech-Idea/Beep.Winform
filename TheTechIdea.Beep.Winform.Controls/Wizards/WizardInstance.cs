using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Wizard instance managing a single wizard dialog
    /// </summary>
    public class WizardInstance
    {
        private readonly WizardConfig _config;
        private readonly WizardContext _context;
        private Form _wizardForm;
        private int _currentStepIndex = 0;
        private WizardResult _result = WizardResult.None;

        public WizardConfig Config => _config;
        public WizardContext Context => _context;
        public int CurrentStepIndex => _currentStepIndex;
        public WizardResult Result => _result;

        public WizardInstance(WizardConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _context = new WizardContext
            {
                TotalSteps = config.Steps?.Count ?? 0
            };

            CreateWizardForm();
        }

        /// <summary>
        /// Show wizard as modal dialog
        /// </summary>
        public DialogResult ShowDialog()
        {
            return _wizardForm.ShowDialog();
        }

        /// <summary>
        /// Show wizard as non-modal
        /// </summary>
        public void Show()
        {
            _wizardForm.Show();
        }

        /// <summary>
        /// Close wizard
        /// </summary>
        public void Close()
        {
            _wizardForm?.Close();
        }

        /// <summary>
        /// Navigate to next step
        /// </summary>
        public bool NavigateNext()
        {
            if (_currentStepIndex >= _config.Steps.Count - 1)
                return false;

            var currentStep = _config.Steps[_currentStepIndex];

            // Check if can navigate
            if (currentStep.CanNavigateNext != null && !currentStep.CanNavigateNext(_context))
                return false;

            // Fire OnStepChanging event
            if (_config.OnStepChanging != null && !_config.OnStepChanging(_currentStepIndex + 1, _context))
                return false;

            // Leave current step
            currentStep.OnLeave?.Invoke(_context);
            currentStep.IsCompleted = true;

            // Add to history
            _context.NavigationHistory.Push(_currentStepIndex);

            // Move to next
            _currentStepIndex++;
            _context.CurrentStepIndex = _currentStepIndex;

            // Enter new step
            var nextStep = _config.Steps[_currentStepIndex];
            nextStep.OnEnter?.Invoke(_context);

            // Fire OnStepChanged event
            _config.OnStepChanged?.Invoke(_currentStepIndex, _context);

            UpdateWizardUI();
            return true;
        }

        /// <summary>
        /// Navigate to previous step
        /// </summary>
        public bool NavigateBack()
        {
            if (!_config.AllowBack || _context.NavigationHistory.Count == 0)
                return false;

            var currentStep = _config.Steps[_currentStepIndex];

            // Check if can navigate back
            if (currentStep.CanNavigateBack != null && !currentStep.CanNavigateBack(_context))
                return false;

            // Leave current step
            currentStep.OnLeave?.Invoke(_context);

            // Get previous step from history
            _currentStepIndex = _context.NavigationHistory.Pop();
            _context.CurrentStepIndex = _currentStepIndex;

            // Enter previous step
            var prevStep = _config.Steps[_currentStepIndex];
            prevStep.OnEnter?.Invoke(_context);

            // Fire OnStepChanged event
            _config.OnStepChanged?.Invoke(_currentStepIndex, _context);

            UpdateWizardUI();
            return true;
        }

        /// <summary>
        /// Navigate to specific step
        /// </summary>
        public bool NavigateToStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _config.Steps.Count)
                return false;

            if (stepIndex == _currentStepIndex)
                return true;

            // Check if skipping is allowed
            if (!_config.AllowSkip && stepIndex > _currentStepIndex + 1)
                return false;

            var currentStep = _config.Steps[_currentStepIndex];

            // Fire OnStepChanging event
            if (_config.OnStepChanging != null && !_config.OnStepChanging(stepIndex, _context))
                return false;

            // Leave current step
            currentStep.OnLeave?.Invoke(_context);

            // Add to history if moving forward
            if (stepIndex > _currentStepIndex)
            {
                _context.NavigationHistory.Push(_currentStepIndex);
            }

            // Move to new step
            _currentStepIndex = stepIndex;
            _context.CurrentStepIndex = _currentStepIndex;

            // Enter new step
            var newStep = _config.Steps[_currentStepIndex];
            newStep.OnEnter?.Invoke(_context);

            // Fire OnStepChanged event
            _config.OnStepChanged?.Invoke(_currentStepIndex, _context);

            UpdateWizardUI();
            return true;
        }

        /// <summary>
        /// Complete wizard
        /// </summary>
        public void Complete()
        {
            var currentStep = _config.Steps[_currentStepIndex];
            currentStep.OnLeave?.Invoke(_context);
            currentStep.IsCompleted = true;

            _result = WizardResult.Completed;
            _config.OnComplete?.Invoke(_context);

            _wizardForm.DialogResult = DialogResult.OK;
            Close();
            WizardManager.UnregisterWizard(_config.Key);
        }

        /// <summary>
        /// Cancel wizard
        /// </summary>
        public void Cancel()
        {
            _result = WizardResult.Cancelled;
            _config.OnCancel?.Invoke(_context);

            _wizardForm.DialogResult = DialogResult.Cancel;
            Close();
            WizardManager.UnregisterWizard(_config.Key);
        }

        private void CreateWizardForm()
        {
            // Create wizard form based on style
            switch (_config.Style)
            {
                case WizardStyle.Classic:
                    _wizardForm = new ClassicWizardForm(this);
                    break;
                case WizardStyle.Modern:
                    _wizardForm = new ModernWizardForm(this);
                    break;
                case WizardStyle.Stepper:
                    _wizardForm = new StepperWizardForm(this);
                    break;
                case WizardStyle.VerticalStepper:
                    _wizardForm = new VerticalStepperWizardForm(this);
                    break;
                case WizardStyle.Cards:
                    _wizardForm = new CardsWizardForm(this);
                    break;
                default:
                    _wizardForm = new ModernWizardForm(this);
                    break;
            }

            _wizardForm.Text = _config.Title;
            _wizardForm.Size = _config.Size;
            _wizardForm.StartPosition = FormStartPosition.CenterScreen;
            _wizardForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            _wizardForm.MaximizeBox = false;
            _wizardForm.MinimizeBox = false;

            // Enter first step
            if (_config.Steps.Count > 0)
            {
                var firstStep = _config.Steps[0];
                firstStep.OnEnter?.Invoke(_context);
            }
        }

        private void UpdateWizardUI()
        {
            // Update UI through form's interface
            if (_wizardForm is IWizardForm wizardFormInterface)
            {
                wizardFormInterface.UpdateUI();
            }
        }
    }

    /// <summary>
    /// Interface for wizard forms to implement
    /// </summary>
    internal interface IWizardForm
    {
        void UpdateUI();
    }
}
