using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    #region WizardConfig

    /// <summary>
    /// Configuration for wizard behavior and appearance
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardConfig
    {
        [Category("Wizard")]
        [Description("Unique key for the wizard instance")]
        public string Key { get; set; }

        [Category("Wizard")]
        [Description("Title of the wizard")]
        public string Title { get; set; } = "Wizard";

        [Category("Wizard")]
        [Description("Description of the wizard")]
        public string Description { get; set; }

        [Category("Wizard")]
        [Description("Size of the wizard form")]
        public Size Size { get; set; } = new Size(900, 650);

        [Category("Wizard")]
        [Description("Visual style of the wizard")]
        public WizardStyle Style { get; set; } = WizardStyle.HorizontalStepper;

        [Category("Wizard")]
        [Description("Show progress bar")]
        public bool ShowProgressBar { get; set; } = true;

        [Category("Wizard")]
        [Description("Show step list")]
        public bool ShowStepList { get; set; } = true;

        [Category("Wizard")]
        [Description("Allow cancel")]
        public bool AllowCancel { get; set; } = true;

        [Category("Wizard")]
        [Description("Allow back navigation")]
        public bool AllowBack { get; set; } = true;

        [Category("Wizard")]
        [Description("Allow skipping optional steps")]
        public bool AllowSkip { get; set; } = false;

        [Category("Wizard")]
        [Description("Show help button")]
        public bool ShowHelp { get; set; } = false;

        [Category("Wizard")]
        [Description("Help URL")]
        public string HelpUrl { get; set; }

        [Category("Wizard")]
        [Description("Theme for the wizard")]
        [Browsable(false)]
        public IBeepTheme Theme { get; set; }

        [Category("Buttons")]
        [Description("Text for Next button")]
        public string NextButtonText { get; set; } = "Next";

        [Category("Buttons")]
        [Description("Text for Back button")]
        public string BackButtonText { get; set; } = "Back";

        [Category("Buttons")]
        [Description("Text for Finish button")]
        public string FinishButtonText { get; set; } = "Finish";

        [Category("Buttons")]
        [Description("Text for Cancel button")]
        public string CancelButtonText { get; set; } = "Cancel";

        [Category("Buttons")]
        [Description("Text for Skip button")]
        public string SkipButtonText { get; set; } = "Skip";

        // Callbacks
        [Browsable(false)]
        public Action<WizardContext> OnComplete { get; set; }

        [Browsable(false)]
        public Action<WizardContext> OnCancel { get; set; }

        [Browsable(false)]
        public Func<int, WizardContext, bool> OnStepChanging { get; set; }

        [Browsable(false)]
        public Action<int, WizardContext> OnStepChanged { get; set; }

        [Category("Wizard")]
        [Description("Show inline validation errors instead of message box")]
        public bool ShowInlineErrors { get; set; } = true;

        [Category("Wizard")]
        [Description("Auto-hide validation errors on step change")]
        public bool AutoHideErrors { get; set; } = true;

        [Category("Events")]
        [Description("Progress callback for long-running operations")]
        [Browsable(false)]
        public Action<int, int, string> OnProgress { get; set; }

        [Category("Steps")]
        [Description("List of wizard steps")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<WizardStep> Steps { get; set; } = new List<WizardStep>();
    }

    #endregion

    #region WizardStep

    /// <summary>
    /// Represents a single step in the wizard
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardStep
    {
        [Category("Step")]
        [Description("Unique key for this step")]
        public string Key { get; set; }

        [Category("Step")]
        [Description("Title displayed in step indicator")]
        public string Title { get; set; }

        [Category("Step")]
        [Description("Description of this step")]
        public string Description { get; set; }

        [Category("Step")]
        [Description("SVG icon path for step indicator")]
        public string Icon { get; set; }

        [Category("Step")]
        [Description("UserControl content for this step")]
        [Browsable(false)]
        public Control Content { get; set; }

        [Category("Step")]
        [Description("Current state of this step")]
        public StepState State { get; set; } = StepState.Pending;

        [Category("Step")]
        [Description("Whether this step is optional")]
        public bool IsOptional { get; set; } = false;

        [Category("Step")]
        [Description("Custom Next button text for this step")]
        public string NextButtonText { get; set; }

        [Category("Step")]
        [Description("Custom Back button text for this step")]
        public string BackButtonText { get; set; }

        // Validators
        [Browsable(false)]
        public List<IWizardValidator> Validators { get; set; } = new List<IWizardValidator>();

        // Delegate-based validation
        [Browsable(false)]
        public Func<WizardContext, bool> CanNavigateNext { get; set; }

        [Browsable(false)]
        public Func<WizardContext, bool> CanNavigateBack { get; set; }

        // Conditional step skipping
        [Browsable(false)]
        public Func<WizardContext, bool> ShouldSkip { get; set; }

        // Callbacks
        [Browsable(false)]
        public Action<WizardContext> OnEnter { get; set; }

        [Browsable(false)]
        public Action<WizardContext> OnLeave { get; set; }

        [Browsable(false)]
        public Func<WizardContext, Task> OnEnterAsync { get; set; }

        [Browsable(false)]
        public Func<WizardContext, Task> OnLeaveAsync { get; set; }

        /// <summary>
        /// Additional data attached to this step
        /// </summary>
        [Browsable(false)]
        public object Tag { get; set; }

        /// <summary>
        /// Validate this step using all registered validators
        /// </summary>
        public WizardValidationResult Validate(WizardContext context)
        {
            if (Validators == null || Validators.Count == 0)
            {
                return WizardValidationResult.Success();
            }

            foreach (var validator in Validators)
            {
                var result = validator.Validate(context, this);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            return WizardValidationResult.Success();
        }
    }

    #endregion

    #region WizardContext

    /// <summary>
    /// Shared data context for wizard steps
    /// Passes data between steps and tracks validation state
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardContext
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        private readonly Dictionary<string, WizardStepData> _stepData = new Dictionary<string, WizardStepData>();

        /// <summary>
        /// Get strongly-typed value from context
        /// </summary>
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Set value in context
        /// </summary>
        public void SetValue(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// Check if key exists
        /// </summary>
        public bool ContainsKey(string key) => _data.ContainsKey(key);

        /// <summary>
        /// Remove value from context
        /// </summary>
        public void Remove(string key) => _data.Remove(key);

        /// <summary>
        /// Get step-specific data
        /// </summary>
        public WizardStepData GetStepData(string stepKey)
        {
            if (!_stepData.TryGetValue(stepKey, out var stepData))
            {
                stepData = new WizardStepData { StepKey = stepKey };
                _stepData[stepKey] = stepData;
            }
            return stepData;
        }

        /// <summary>
        /// Get step data value
        /// </summary>
        public T GetStepDataValue<T>(string stepKey, string key, T defaultValue = default)
        {
            var stepData = GetStepData(stepKey);
            return stepData.GetValue(key, defaultValue);
        }

        /// <summary>
        /// Set step data value
        /// </summary>
        public void SetStepDataValue(string stepKey, string key, object value)
        {
            var stepData = GetStepData(stepKey);
            stepData.SetValue(key, value);
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void Clear()
        {
            _data.Clear();
            _stepData.Clear();
            StepValidation.Clear();
            NavigationHistory.Clear();
        }

        /// <summary>
        /// Get all data as dictionary
        /// </summary>
        public Dictionary<string, object> GetAllData() => new Dictionary<string, object>(_data);

        // Validation state per step index
        public Dictionary<int, bool> StepValidation { get; set; } = new Dictionary<int, bool>();

        // Navigation history for back button
        [Browsable(false)]
        public Stack<int> NavigationHistory { get; set; } = new Stack<int>();

        // Current step index
        public int CurrentStepIndex { get; set; }

        // Total steps
        public int TotalSteps { get; set; }

        // User data object
        [Browsable(false)]
        public object UserData { get; set; }

        // Completion percentage (0-100)
        public double CompletionPercentage
        {
            get
            {
                if (TotalSteps == 0) return 0;
                return (CurrentStepIndex + 1) / (double)TotalSteps * 100.0;
            }
        }
    }

    #endregion

    #region WizardStepData

    /// <summary>
    /// Data specific to a single step
    /// </summary>
    public class WizardStepData
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public string StepKey { get; set; }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (_values.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            _values[key] = value;
        }

        public bool ContainsKey(string key) => _values.ContainsKey(key);
        public void Remove(string key) => _values.Remove(key);
    }

    #endregion

    #region Validation

    /// <summary>
    /// Interface for wizard step validators
    /// </summary>
    public interface IWizardValidator
    {
        WizardValidationResult Validate(WizardContext context, WizardStep step);
    }

    /// <summary>
    /// Result of validation
    /// </summary>
    public class WizardValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string FieldName { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public static WizardValidationResult Success() => new WizardValidationResult { IsValid = true };
        
        public static WizardValidationResult Error(string message) => 
            new WizardValidationResult { IsValid = false, ErrorMessage = message };

        public static WizardValidationResult Error(string message, string fieldName) =>
            new WizardValidationResult { IsValid = false, ErrorMessage = message, FieldName = fieldName };
    }

    #endregion

    #region IWizardStepContent

    /// <summary>
    /// Interface for wizard step UserControls to implement
    /// Provides two-way communication between wizard and step content
    /// </summary>
    public interface IWizardStepContent
    {
        /// <summary>
        /// Called when step becomes active - load data from context
        /// </summary>
        void OnStepEnter(WizardContext context);

        /// <summary>
        /// Called when leaving step - save data to context
        /// </summary>
        void OnStepLeave(WizardContext context);

        /// <summary>
        /// Real-time validation state change notification
        /// Wizard listens to this to enable/disable Next button
        /// </summary>
        event EventHandler<StepValidationEventArgs> ValidationStateChanged;

        /// <summary>
        /// Validate step content (called when user clicks Next)
        /// </summary>
        WizardValidationResult Validate();

        /// <summary>
        /// Async validation for server-side checks
        /// </summary>
        Task<WizardValidationResult> ValidateAsync();

        /// <summary>
        /// Current completion state
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Optional custom Next button text
        /// </summary>
        string NextButtonText { get; }
    }

    /// <summary>
    /// Event args for validation state changes
    /// </summary>
    public class StepValidationEventArgs : EventArgs
    {
        public bool IsValid { get; }
        public string Message { get; }

        public StepValidationEventArgs(bool isValid, string message = null)
        {
            IsValid = isValid;
            Message = message;
        }
    }

    #endregion

    #region WizardStepHelp

    /// <summary>
    /// Help content for a wizard step (attach to WizardStep.Tag for context-sensitive help)
    /// </summary>
    public class WizardStepHelp
    {
        /// <summary>
        /// Title for the help dialog
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Help content text
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Optional URL for more information
        /// </summary>
        public string Url { get; set; }

        public WizardStepHelp() { }

        public WizardStepHelp(string title, string content, string url = null)
        {
            Title = title;
            Content = content;
            Url = url;
        }
    }

    #endregion
}
