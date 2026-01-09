using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards.Validation;
using TheTechIdea.Beep.Winform.Controls.Wizards.Validation;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Modern Wizard Manager for multi-step processes
    /// Inspired by modern installation wizards and stepper components
    /// Features: Step validation, data context, navigation, theming
    /// </summary>
    public static class WizardManager
    {
        private static readonly Dictionary<string, WizardInstance> _activeWizards = new();

        // Global settings
        public static WizardStyle DefaultStyle { get; set; } = WizardStyle.Modern;
        public static bool ShowProgressBar { get; set; } = true;
        public static bool AllowStepSkipping { get; set; } = false;
        public static bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Create and show a new wizard
        /// </summary>
        public static WizardInstance CreateWizard(WizardConfig config)
        {
            if (string.IsNullOrEmpty(config.Key))
                config.Key = Guid.NewGuid().ToString();

            // Remove existing wizard with same key
            if (_activeWizards.TryGetValue(config.Key, out var existing))
            {
                existing.Close();
            }

            var instance = new WizardInstance(config);
            _activeWizards[config.Key] = instance;

            return instance;
        }

        /// <summary>
        /// Show wizard as modal dialog
        /// </summary>
        public static DialogResult ShowWizard(WizardConfig config)
        {
            var instance = CreateWizard(config);
            return instance.ShowDialog();
        }

        /// <summary>
        /// Get active wizard by key
        /// </summary>
        public static WizardInstance GetWizard(string key)
        {
            _activeWizards.TryGetValue(key, out var wizard);
            return wizard;
        }

        /// <summary>
        /// Close wizard
        /// </summary>
        public static void CloseWizard(string key)
        {
            if (_activeWizards.TryGetValue(key, out var wizard))
            {
                wizard.Close();
                _activeWizards.Remove(key);
            }
        }

        /// <summary>
        /// Close all active wizards
        /// </summary>
        public static void CloseAllWizards()
        {
            foreach (var wizard in _activeWizards.Values.ToList())
            {
                wizard.Close();
            }
            _activeWizards.Clear();
        }

        internal static void UnregisterWizard(string key)
        {
            _activeWizards.Remove(key);
        }
    }

    #region Configuration Classes

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
        public Size Size { get; set; } = new Size(800, 600);
        
        [Category("Wizard")]
        [Description("Visual style of the wizard")]
        public WizardStyle Style { get; set; } = WizardStyle.Modern;
        
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
        [Description("Allow skipping steps")]
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
        
        // Callbacks (not shown in property grid)
        [Browsable(false)]
        public Action<WizardContext> OnComplete { get; set; }
        
        [Browsable(false)]
        public Action<WizardContext> OnCancel { get; set; }
        
        [Browsable(false)]
        public Func<int, WizardContext, bool> OnStepChanging { get; set; }
        
        [Browsable(false)]
        public Action<int, WizardContext> OnStepChanged { get; set; }
        
        [Category("Steps")]
        [Description("List of wizard steps")]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<WizardStep> Steps { get; set; } = new List<WizardStep>();
    }

    /// <summary>
    /// Represents a single step in the wizard
    /// </summary>
    public class WizardStep
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; } // SVG path
        public Control Content { get; set; } // UserControl for this step
        public Func<WizardContext, bool> CanNavigateNext { get; set; }
        public Func<WizardContext, bool> CanNavigateBack { get; set; }
        public Action<WizardContext> OnEnter { get; set; }
        public Action<WizardContext> OnLeave { get; set; }
        public bool IsOptional { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public object Tag { get; set; }
        
        [Category("Validation")]
        [Description("List of validators for this step")]
        [Browsable(false)]
        public List<IWizardStepValidator> Validators { get; set; } = new List<IWizardStepValidator>();

        /// <summary>
        /// Validate this step
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

        /// <summary>
        /// Check if step is valid
        /// </summary>
        [Category("Validation")]
        [Description("Whether this step is valid")]
        [ReadOnly(true)]
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Update validation state
        /// </summary>
        internal void UpdateValidationState(WizardContext context)
        {
            var result = Validate(context);
            IsValid = result.IsValid;
        }
    }

    /// <summary>
    /// Shared data context for wizard steps
    /// Passes data between steps and validates wizard state
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
        /// Set step-specific data
        /// </summary>
        public void SetStepData(string stepKey, WizardStepData stepData)
        {
            _stepData[stepKey] = stepData;
        }

        /// <summary>
        /// Get strongly-typed step data value
        /// </summary>
        public T GetStepDataValue<T>(string stepKey, string key, T defaultValue = default)
        {
            var stepData = GetStepData(stepKey);
            return stepData.GetValue(key, defaultValue);
        }

        /// <summary>
        /// Set step-specific data value
        /// </summary>
        public void SetStepDataValue(string stepKey, string key, object value)
        {
            var stepData = GetStepData(stepKey);
            stepData.SetValue(key, value);
        }

        /// <summary>
        /// Check if key exists in context
        /// </summary>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Remove value from context
        /// </summary>
        public void Remove(string key)
        {
            _data.Remove(key);
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
        /// Get all data as dictionary (backward compatibility)
        /// </summary>
        public Dictionary<string, object> GetAllData()
        {
            return new Dictionary<string, object>(_data);
        }

        // Validation state per step
        [Category("Validation")]
        [Description("Validation state per step index")]
        public Dictionary<int, bool> StepValidation { get; set; } = new Dictionary<int, bool>();

        // Navigation history for back button
        [Category("Navigation")]
        [Description("Navigation history stack")]
        [Browsable(false)]
        public Stack<int> NavigationHistory { get; set; } = new Stack<int>();

        // Current step index
        [Category("Navigation")]
        [Description("Current step index")]
        public int CurrentStepIndex { get; set; }

        // Total steps
        [Category("Navigation")]
        [Description("Total number of steps")]
        public int TotalSteps { get; set; }

        // User data (custom object)
        [Category("Data")]
        [Description("Custom user data object")]
        [Browsable(false)]
        public object UserData { get; set; }

        // Navigation state (strongly-typed)
        [Category("Navigation")]
        [Description("Navigation state information")]
        [Browsable(false)]
        public WizardNavigationState NavigationState { get; set; } = new WizardNavigationState();

        // Completion percentage
        [Category("Progress")]
        [Description("Completion percentage (0-100)")]
        [ReadOnly(true)]
        public double CompletionPercentage
        {
            get
            {
                if (TotalSteps == 0) return 0;
                return (CurrentStepIndex + 1) / (double)TotalSteps * 100.0;
            }
        }

        // Steps completed
        [Category("Progress")]
        [Description("Number of steps completed")]
        [ReadOnly(true)]
        public int StepsCompleted
        {
            get
            {
                return StepValidation.Values.Count(v => v);
            }
        }
    }

    #endregion

    #region Enums

    /// <summary>
    /// Visual Style for wizard
    /// </summary>
    public enum WizardStyle
    {
        Classic,        // Traditional wizard with side panel (Image 1)
        Modern,         // Clean modern with top progress bar (Image 5)
        Stepper,        // Horizontal stepper with numbered steps (Image 3)
        VerticalStepper,// Vertical timeline stepper (Image 2)
        Cards           // Card-based step selection (Image 6)
    }

    /// <summary>
    /// Navigation result from wizard
    /// </summary>
    public enum WizardResult
    {
        None,
        Completed,
        Cancelled,
        Failed
    }

    #endregion
}
