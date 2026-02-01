using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Runtime wizard instance - manages navigation, validation, and state
    /// This is the Core Engine - NO UI code here
    /// </summary>
    public class WizardInstance
    {
        #region Fields

        private IWizardFormHost _formHost;
        private int _currentStepIndex = 0;
        private bool _isNavigating = false;

        #endregion

        #region Properties

        /// <summary>
        /// Wizard configuration
        /// </summary>
        public WizardConfig Config { get; }

        /// <summary>
        /// Shared data context
        /// </summary>
        public WizardContext Context { get; }

        /// <summary>
        /// Current step index (0-based)
        /// </summary>
        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            private set
            {
                if (_currentStepIndex != value)
                {
                    _currentStepIndex = value;
                    Context.CurrentStepIndex = value;
                }
            }
        }

        /// <summary>
        /// Current step
        /// </summary>
        public WizardStep CurrentStep => 
            CurrentStepIndex >= 0 && CurrentStepIndex < Config.Steps.Count 
                ? Config.Steps[CurrentStepIndex] 
                : null;

        /// <summary>
        /// Wizard result
        /// </summary>
        public WizardResult Result { get; private set; } = WizardResult.None;

        /// <summary>
        /// Whether wizard is on last step
        /// </summary>
        public bool IsLastStep => CurrentStepIndex >= Config.Steps.Count - 1;

        /// <summary>
        /// Whether wizard is on first step
        /// </summary>
        public bool IsFirstStep => CurrentStepIndex <= 0;

        #endregion

        #region Events

        /// <summary>
        /// Raised before step changes (can be cancelled)
        /// </summary>
        public event EventHandler<StepChangingEventArgs> StepChanging;

        /// <summary>
        /// Raised after step changed
        /// </summary>
        public event EventHandler<StepChangedEventArgs> StepChanged;

        /// <summary>
        /// Raised when wizard completes
        /// </summary>
        public event EventHandler<WizardCompletedEventArgs> Completed;

        /// <summary>
        /// Raised when wizard is cancelled
        /// </summary>
        public event EventHandler<WizardCancelledEventArgs> Cancelled;

        #endregion

        #region Constructor

        public WizardInstance(WizardConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Context = new WizardContext
            {
                TotalSteps = config.Steps.Count
            };

            // Initialize step states
            if (config.Steps.Count > 0)
            {
                config.Steps[0].State = StepState.Current;
            }
        }

        #endregion

        #region Form Binding

        /// <summary>
        /// Bind to a form host (called by form during initialization)
        /// </summary>
        public void BindFormHost(IWizardFormHost formHost)
        {
            _formHost = formHost ?? throw new ArgumentNullException(nameof(formHost));
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Navigate to next step
        /// </summary>
        public async Task<bool> NavigateNextAsync()
        {
            if (_isNavigating || CurrentStepIndex >= Config.Steps.Count - 1)
                return false;

            _isNavigating = true;

            try
            {
                var currentStep = CurrentStep;
                var nextIndex = CurrentStepIndex + 1;

                // Validate current step
                var validationResult = await ValidateCurrentStepAsync();
                if (!validationResult.IsValid)
                {
                    _formHost?.ShowValidationError(validationResult);
                    return false;
                }

                // Raise StepChanging event
                var changingArgs = new StepChangingEventArgs(CurrentStepIndex, nextIndex, Context);
                StepChanging?.Invoke(this, changingArgs);
                if (changingArgs.Cancel)
                    return false;

                // Call OnStepChanging callback
                if (Config.OnStepChanging != null && !Config.OnStepChanging(nextIndex, Context))
                    return false;

                // Leave current step
                await LeaveStepAsync(currentStep);

                // Mark current step as completed
                currentStep.State = StepState.Completed;
                Context.StepValidation[CurrentStepIndex] = true;

                // Skip steps that should be skipped
                while (nextIndex < Config.Steps.Count)
                {
                    var nextStep = Config.Steps[nextIndex];
                    if (nextStep.ShouldSkip != null && nextStep.ShouldSkip(Context))
                    {
                        nextStep.State = StepState.Skipped;
                        nextIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Navigate to next step
                CurrentStepIndex = nextIndex;
                Context.NavigationHistory.Push(CurrentStepIndex);

                // Enter new step
                var newStep = CurrentStep;
                if (newStep != null)
                {
                    newStep.State = StepState.Current;
                    await EnterStepAsync(newStep);
                }

                // Update UI
                _formHost?.UpdateUI();

                // Raise events
                StepChanged?.Invoke(this, new StepChangedEventArgs(CurrentStepIndex, Context));
                Config.OnStepChanged?.Invoke(CurrentStepIndex, Context);

                return true;
            }
            finally
            {
                _isNavigating = false;
            }
        }

        /// <summary>
        /// Navigate to next step (sync wrapper)
        /// </summary>
        public bool NavigateNext()
        {
            return NavigateNextAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Navigate to previous step
        /// </summary>
        public bool NavigateBack()
        {
            if (_isNavigating || !Config.AllowBack || CurrentStepIndex <= 0)
                return false;

            _isNavigating = true;

            try
            {
                var currentStep = CurrentStep;
                var prevIndex = CurrentStepIndex - 1;

                // Raise StepChanging event
                var changingArgs = new StepChangingEventArgs(CurrentStepIndex, prevIndex, Context);
                StepChanging?.Invoke(this, changingArgs);
                if (changingArgs.Cancel)
                    return false;

                // Leave current step (don't require validation for back navigation)
                currentStep?.OnLeave?.Invoke(Context);
                if (currentStep?.Content is IWizardStepContent stepContent)
                {
                    stepContent.OnStepLeave(Context);
                }

                // Mark current step as pending again
                if (currentStep != null)
                    currentStep.State = StepState.Pending;

                // Skip back over skipped steps
                while (prevIndex >= 0)
                {
                    var prevStep = Config.Steps[prevIndex];
                    if (prevStep.State == StepState.Skipped)
                    {
                        prevIndex--;
                    }
                    else
                    {
                        break;
                    }
                }

                // Navigate to previous step
                CurrentStepIndex = Math.Max(0, prevIndex);

                // Enter new step
                var newStep = CurrentStep;
                if (newStep != null)
                {
                    newStep.State = StepState.Current;
                    EnterStepAsync(newStep).GetAwaiter().GetResult();
                }

                // Update UI
                _formHost?.UpdateUI();

                // Raise events
                StepChanged?.Invoke(this, new StepChangedEventArgs(CurrentStepIndex, Context));
                Config.OnStepChanged?.Invoke(CurrentStepIndex, Context);

                return true;
            }
            finally
            {
                _isNavigating = false;
            }
        }

        /// <summary>
        /// Navigate to specific step by index
        /// </summary>
        public async Task<bool> NavigateToAsync(int stepIndex)
        {
            if (_isNavigating || stepIndex < 0 || stepIndex >= Config.Steps.Count)
                return false;

            if (stepIndex == CurrentStepIndex)
                return true;

            // Can only navigate forward if all previous steps validated
            if (stepIndex > CurrentStepIndex)
            {
                for (int i = CurrentStepIndex; i < stepIndex; i++)
                {
                    if (!await NavigateNextAsync())
                        return false;
                }
                return true;
            }

            // Navigate back
            while (CurrentStepIndex > stepIndex)
            {
                if (!NavigateBack())
                    return false;
            }
            return true;
        }

        #endregion

        #region Step Lifecycle

        private async Task EnterStepAsync(WizardStep step)
        {
            if (step == null) return;

            // Call OnEnter callback
            step.OnEnter?.Invoke(Context);

            // Call async OnEnter
            if (step.OnEnterAsync != null)
            {
                await step.OnEnterAsync(Context);
            }

            // Inject context into step content
            if (step.Content is IWizardStepContent stepContent)
            {
                stepContent.OnStepEnter(Context);
            }
        }

        private async Task LeaveStepAsync(WizardStep step)
        {
            if (step == null) return;

            // Save data from step content
            if (step.Content is IWizardStepContent stepContent)
            {
                stepContent.OnStepLeave(Context);
            }

            // Call OnLeave callback
            step.OnLeave?.Invoke(Context);

            // Call async OnLeave
            if (step.OnLeaveAsync != null)
            {
                await step.OnLeaveAsync(Context);
            }
        }

        #endregion

        #region Validation

        private async Task<WizardValidationResult> ValidateCurrentStepAsync()
        {
            var step = CurrentStep;
            if (step == null)
                return WizardValidationResult.Success();

            // Check CanNavigateNext delegate
            if (step.CanNavigateNext != null && !step.CanNavigateNext(Context))
            {
                return WizardValidationResult.Error("Cannot proceed from this step");
            }

            // Validate step content
            if (step.Content is IWizardStepContent stepContent)
            {
                // Try async validation first
                try
                {
                    var asyncResult = await stepContent.ValidateAsync();
                    if (!asyncResult.IsValid)
                        return asyncResult;
                }
                catch (NotImplementedException)
                {
                    // Fall back to sync validation
                    var syncResult = stepContent.Validate();
                    if (!syncResult.IsValid)
                        return syncResult;
                }
            }

            // Run registered validators
            return step.Validate(Context);
        }

        #endregion

        #region Completion

        /// <summary>
        /// Complete the wizard
        /// </summary>
        public async Task<bool> CompleteAsync()
        {
            // Validate final step
            var validationResult = await ValidateCurrentStepAsync();
            if (!validationResult.IsValid)
            {
                _formHost?.ShowValidationError(validationResult);
                return false;
            }

            // Leave final step
            await LeaveStepAsync(CurrentStep);

            // Mark step as completed
            if (CurrentStep != null)
                CurrentStep.State = StepState.Completed;

            Result = WizardResult.Completed;

            // Raise events
            Completed?.Invoke(this, new WizardCompletedEventArgs(Context));
            Config.OnComplete?.Invoke(Context);

            // Close form
            if (_formHost != null)
            {
                _formHost.DialogResult = DialogResult.OK;
                _formHost.Close();
            }

            return true;
        }

        /// <summary>
        /// Complete the wizard (sync wrapper)
        /// </summary>
        public bool Complete()
        {
            return CompleteAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Cancel the wizard
        /// </summary>
        public void Cancel()
        {
            Result = WizardResult.Cancelled;

            // Raise events
            Cancelled?.Invoke(this, new WizardCancelledEventArgs(Context));
            Config.OnCancel?.Invoke(Context);

            // Close form
            if (_formHost != null)
            {
                _formHost.DialogResult = DialogResult.Cancel;
                _formHost.Close();
            }
        }

        /// <summary>
        /// Close wizard without setting result
        /// </summary>
        public void Close()
        {
            _formHost?.Close();
        }

        #endregion

        #region Show Dialog

        /// <summary>
        /// Show wizard as modal dialog
        /// </summary>
        public DialogResult ShowDialog()
        {
            InitializeFirstStep();
            var form = WizardFormFactory.CreateForm(Config.Style, this);
            _formHost = form;
            return form.ShowDialog();
        }

        /// <summary>
        /// Show wizard as modal dialog with owner
        /// </summary>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            InitializeFirstStep();
            var form = WizardFormFactory.CreateForm(Config.Style, this);
            _formHost = form;
            return form.ShowDialog(owner);
        }

        private void InitializeFirstStep()
        {
            try
            {
                if (CurrentStepIndex == 0 && Config.Steps.Count > 0)
                {
                    var step = Config.Steps[0];
                    
                    // Call OnEnter callback
                    step.OnEnter?.Invoke(Context);

                    // Call async OnEnter (sync)
                    if (step.OnEnterAsync != null)
                    {
                        step.OnEnterAsync(Context).GetAwaiter().GetResult();
                    }

                    // Inject context into step content
                    if (step.Content is IWizardStepContent stepContent)
                    {
                        stepContent.OnStepEnter(Context);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WizardInstance: Error initializing first step: {ex.Message}");
            }
        }

        #endregion
    }

    #region Event Args

    public class StepChangingEventArgs : EventArgs
    {
        public int OldStepIndex { get; }
        public int NewStepIndex { get; }
        public WizardContext Context { get; }
        public bool Cancel { get; set; }

        public StepChangingEventArgs(int oldIndex, int newIndex, WizardContext context)
        {
            OldStepIndex = oldIndex;
            NewStepIndex = newIndex;
            Context = context;
        }
    }

    public class StepChangedEventArgs : EventArgs
    {
        public int StepIndex { get; }
        public WizardContext Context { get; }

        public StepChangedEventArgs(int stepIndex, WizardContext context)
        {
            StepIndex = stepIndex;
            Context = context;
        }
    }

    public class WizardCompletedEventArgs : EventArgs
    {
        public WizardContext Context { get; }

        public WizardCompletedEventArgs(WizardContext context)
        {
            Context = context;
        }
    }

    public class WizardCancelledEventArgs : EventArgs
    {
        public WizardContext Context { get; }

        public WizardCancelledEventArgs(WizardContext context)
        {
            Context = context;
        }
    }

    #endregion
}
