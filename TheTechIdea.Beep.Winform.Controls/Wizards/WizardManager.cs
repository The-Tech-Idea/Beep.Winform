using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

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
    public class WizardConfig
    {
        public string Key { get; set; }
        public string Title { get; set; } = "Wizard";
        public string Description { get; set; }
        public Size Size { get; set; } = new Size(800, 600);
        public WizardStyle Style { get; set; } = WizardStyle.Modern;
        public bool ShowProgressBar { get; set; } = true;
        public bool ShowStepList { get; set; } = true;
        public bool AllowCancel { get; set; } = true;
        public bool AllowBack { get; set; } = true;
        public bool AllowSkip { get; set; } = false;
        public bool ShowHelp { get; set; } = false;
        public string HelpUrl { get; set; }
        public IBeepTheme Theme { get; set; }
        
        // Button text customization
        public string NextButtonText { get; set; } = "Next";
        public string BackButtonText { get; set; } = "Back";
        public string FinishButtonText { get; set; } = "Finish";
        public string CancelButtonText { get; set; } = "Cancel";
        public string SkipButtonText { get; set; } = "Skip";
        
        // Callbacks
        public Action<WizardContext> OnComplete { get; set; }
        public Action<WizardContext> OnCancel { get; set; }
        public Func<int, WizardContext, bool> OnStepChanging { get; set; }
        public Action<int, WizardContext> OnStepChanged { get; set; }
        
        // Steps
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
    }

    /// <summary>
    /// Shared data context for wizard steps
    /// Passes data between steps and validates wizard state
    /// </summary>
    public class WizardContext
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

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
            StepValidation.Clear();
            NavigationHistory.Clear();
        }

        /// <summary>
        /// Get all data as dictionary
        /// </summary>
        public Dictionary<string, object> GetAllData()
        {
            return new Dictionary<string, object>(_data);
        }

        // Validation state per step
        public Dictionary<int, bool> StepValidation { get; set; } = new Dictionary<int, bool>();

        // Navigation history for back button
        public Stack<int> NavigationHistory { get; set; } = new Stack<int>();

        // Current step index
        public int CurrentStepIndex { get; set; }

        // Total steps
        public int TotalSteps { get; set; }

        // User data (custom object)
        public object UserData { get; set; }
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
