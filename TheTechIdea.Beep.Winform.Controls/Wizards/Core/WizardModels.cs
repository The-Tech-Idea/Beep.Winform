using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        [Category("Animation")]
        [Description("Transition animation type between steps.")]
        [DefaultValue(TransitionType.Slide)]
        public TransitionType TransitionType { get; set; } = TransitionType.Slide;

        [Category("Animation")]
        [Description("Easing function for transition animations.")]
        [DefaultValue(TransitionEasing.EaseOutCubic)]
        public TransitionEasing TransitionEasing { get; set; } = TransitionEasing.EaseOutCubic;

        [Category("Animation")]
        [Description("Duration of step transition animation in milliseconds (100-800).")]
        [DefaultValue(300)]
        public int TransitionDurationMs { get; set; } = 300;

        [Category("Animation")]
        [Description("Enable animations for this wizard instance.")]
        [DefaultValue(true)]
        public bool EnableAnimations { get; set; } = true;

        [Category("Animation")]
        [Description("Pages with more than this many child controls auto-switch to Fade/None transition.")]
        [DefaultValue(30)]
        public int MaxControlsForAnimation { get; set; } = 30;

        [Category("Rendering")]
        [Description("Enable WS_EX_COMPOSITED for system-level double-buffering (reduces flicker).")]
        [DefaultValue(true)]
        public bool EnableCompositedRendering { get; set; } = true;

        [Category("Navigation")]
        [Description("Navigation mode: Sequential (back/next) or Breadcrumb (click any step title).")]
        [DefaultValue(NavigationMode.Sequential)]
        public NavigationMode NavigationMode { get; set; } = NavigationMode.Sequential;

        [Category("Navigation")]
        [Description("Enable branching/conditional navigation between steps.")]
        [DefaultValue(false)]
        public bool BranchingEnabled { get; set; } = false;

        [Category("Behavior")]
        [Description("Show a summary/review step before completion.")]
        [DefaultValue(false)]
        public bool ShowSummaryStep { get; set; } = false;

        [Category("Behavior")]
        [Description("Show confirmation dialog when user cancels the wizard.")]
        [DefaultValue(true)]
        public bool ConfirmOnCancel { get; set; } = true;

        [Category("Behavior")]
        [Description("Custom message for the cancel confirmation dialog.")]
        [DefaultValue("Are you sure you want to cancel?")]
        public string CancelConfirmationMessage { get; set; } = "Are you sure you want to cancel?";

        [Category("Behavior")]
        [Description("Enable undo of the last step completion.")]
        [DefaultValue(false)]
        public bool EnableUndo { get; set; } = false;

        [Category("State")]
        [Description("Automatically save wizard state on each step transition.")]
        [DefaultValue(false)]
        public bool EnableAutoSave { get; set; } = false;

        [Category("State")]
        [Description("File path for auto-save state. Defaults to TEMP folder if empty.")]
        [DefaultValue("")]
        public string AutoSavePath { get; set; } = "";

        [Category("Appearance")]
        [Description("Show radial/circular progress indicator (Minimal style only).")]
        [DefaultValue(false)]
        public bool ShowRadialProgress { get; set; } = false;

        [Category("Appearance")]
        [Description("Use gradient fills on completed step indicators.")]
        [DefaultValue(false)]
        public bool UseGradientSteps { get; set; } = false;

        [Category("Appearance")]
        [Description("Show a watermark of the current step title behind the content area.")]
        [DefaultValue(false)]
        public bool ShowWatermark { get; set; } = false;

        [Category("Accessibility")]
        [Description("Enable right-to-left layout for Arabic/Hebrew languages.")]
        [DefaultValue(false)]
        public bool RightToLeft { get; set; } = false;

        [Category("Accessibility")]
        [Description("Override high contrast mode (uses system colors when true).")]
        [DefaultValue(false)]
        public bool HighContrastMode { get; set; } = false;

        [Category("Demo")]
        [Description("Auto-advance through steps with a configurable delay (for demos/kiosks).")]
        [DefaultValue(false)]
        public bool AutoPlayMode { get; set; } = false;

        [Category("Demo")]
        [Description("Delay in milliseconds between auto-play step advances.")]
        [DefaultValue(3000)]
        public int AutoPlayDelayMs { get; set; } = 3000;

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

        // Branching: return target step key (or null for sequential next)
        [Browsable(false)]
        public Func<WizardContext, string> BranchCondition { get; set; }

        // Visibility: return false to hide step from indicators
        [Browsable(false)]
        public Func<WizardContext, bool> VisibleCondition { get; set; }

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

        /// <summary>
        /// Serialize the entire wizard state to JSON for save/resume.
        /// Only serializable values are preserved; non-serializable objects are skipped.
        /// </summary>
        public string SaveState()
        {
            var state = new WizardStateSnapshot
            {
                CurrentStepIndex = CurrentStepIndex,
                TotalSteps = TotalSteps,
                NavigationHistory = NavigationHistory.ToList(),
                StepValidation = new Dictionary<int, bool>(StepValidation),
                Data = new Dictionary<string, object>(),
                StepData = new Dictionary<string, Dictionary<string, object>>()
            };

            // Filter to serializable values only
            foreach (var kv in _data)
            {
                try { var json = JsonSerializer.Serialize(kv.Value); state.Data[kv.Key] = kv.Value; }
                catch { /* skip non-serializable */ }
            }
            foreach (var skv in _stepData)
            {
                state.StepData[skv.Key] = new Dictionary<string, object>();
                foreach (var vk in skv.Value._values)
                {
                    try { JsonSerializer.Serialize(vk.Value); state.StepData[skv.Key][vk.Key] = vk.Value; }
                    catch { /* skip */ }
                }
            }

            return JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Restore wizard state from a previously saved JSON snapshot.
        /// </summary>
        public void RestoreState(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            var state = JsonSerializer.Deserialize<WizardStateSnapshot>(json);
            if (state == null) return;

            Clear();
            CurrentStepIndex = state.CurrentStepIndex;
            TotalSteps = state.TotalSteps;
            NavigationHistory = new Stack<int>(state.NavigationHistory ?? new List<int>());
            StepValidation = new Dictionary<int, bool>(state.StepValidation ?? new Dictionary<int, bool>());

            if (state.Data != null)
                foreach (var kv in state.Data) _data[kv.Key] = kv.Value;
            if (state.StepData != null)
                foreach (var skv in state.StepData)
                {
                    var sd = new WizardStepData { StepKey = skv.Key };
                    foreach (var vk in skv.Value) sd.SetValue(vk.Key, vk.Value);
                    _stepData[skv.Key] = sd;
                }
        }

        /// <summary>
        /// Export all collected data as a flat dictionary.
        /// </summary>
        public Dictionary<string, object> ExportData()
        {
            var result = new Dictionary<string, object>(_data);
            foreach (var skv in _stepData)
                foreach (var vk in skv.Value._values)
                    result[$"{skv.Key}.{vk.Key}"] = vk.Value;
            return result;
        }

        /// <summary>
        /// Import data into context from a flat dictionary.
        /// </summary>
        public void ImportData(Dictionary<string, object> data)
        {
            if (data == null) return;
            foreach (var kv in data) _data[kv.Key] = kv.Value;
        }

        // Validation state per step index
        public Dictionary<int, bool> StepValidation { get; set; } = new Dictionary<int, bool>();

        // Navigation history for back button
        [Browsable(false)]
        public Stack<int> NavigationHistory { get; set; } = new Stack<int>();

        // Redo stack for undo reversal
        [Browsable(false)]
        public Stack<int> RedoStack { get; set; } = new Stack<int>();

        // Step completion history (audit trail)
        [Browsable(false)]
        public List<StepHistoryEntry> StepHistory { get; set; } = new();

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
        internal readonly Dictionary<string, object> _values = new Dictionary<string, object>();

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

    /// <summary>A branch choice in a branching wizard step.</summary>
    public class WizardBranch
    {
        public string Label { get; set; }
        public string Description { get; set; }
        public string TargetStepKey { get; set; }
        public string Icon { get; set; }
        public Func<WizardContext, bool> Condition { get; set; }
    }

    /// <summary>A wizard step that presents branch choices to the user.</summary>
    public class WizardBranchStep : WizardStep
    {
        public List<WizardBranch> Branches { get; set; } = new();
    }

    /// <summary>Timestamped record of a step state change.</summary>
    public class StepHistoryEntry
    {
        public string StepKey { get; set; }
        public StepState State { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public List<string> ValidationErrors { get; set; } = new();
    }

    /// <summary>Internal snapshot of wizard state for JSON serialization.</summary>
    internal class WizardStateSnapshot
    {
        public int CurrentStepIndex { get; set; }
        public int TotalSteps { get; set; }
        public List<int> NavigationHistory { get; set; } = new();
        public Dictionary<int, bool> StepValidation { get; set; } = new();
        public Dictionary<string, object> Data { get; set; } = new();
        public Dictionary<string, Dictionary<string, object>> StepData { get; set; } = new();
    }
}
