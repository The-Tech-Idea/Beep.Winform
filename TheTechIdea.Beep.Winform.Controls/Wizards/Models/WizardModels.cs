using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Models
{
    /// <summary>
    /// Strongly-typed data model for step-specific data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardStepData
    {
        [Category("Data")]
        [Description("Step identifier")]
        public string StepKey { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Step-specific data dictionary")]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Get strongly-typed value from step data
        /// </summary>
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (Data.TryGetValue(key, out var value))
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
        /// Set value in step data
        /// </summary>
        public void SetValue(string key, object value)
        {
            Data[key] = value;
        }

        public override string ToString() => $"StepData: {StepKey}";
    }

    /// <summary>
    /// Validation result model
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardValidationResult
    {
        [Category("Validation")]
        [Description("Whether validation passed")]
        public bool IsValid { get; set; }

        [Category("Validation")]
        [Description("Validation error message")]
        public string ErrorMessage { get; set; } = string.Empty;

        [Category("Validation")]
        [Description("Validation error code")]
        public string ErrorCode { get; set; } = string.Empty;

        [Category("Validation")]
        [Description("Field that failed validation")]
        public string FieldName { get; set; } = string.Empty;

        [Category("Validation")]
        [Description("Additional validation details")]
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();

        public static WizardValidationResult Success()
        {
            return new WizardValidationResult { IsValid = true };
        }

        public static WizardValidationResult Failure(string errorMessage, string fieldName = "", string errorCode = "")
        {
            return new WizardValidationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage,
                FieldName = fieldName,
                ErrorCode = errorCode
            };
        }

        public override string ToString() => IsValid ? "Valid" : $"Invalid: {ErrorMessage}";
    }

    /// <summary>
    /// Navigation state tracking model
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WizardNavigationState
    {
        [Category("Navigation")]
        [Description("Current step index")]
        public int CurrentStepIndex { get; set; }

        [Category("Navigation")]
        [Description("Previous step index")]
        public int? PreviousStepIndex { get; set; }

        [Category("Navigation")]
        [Description("Next step index")]
        public int? NextStepIndex { get; set; }

        [Category("Navigation")]
        [Description("Navigation history")]
        public List<int> History { get; set; } = new List<int>();

        [Category("Navigation")]
        [Description("Can navigate back")]
        public bool CanNavigateBack { get; set; }

        [Category("Navigation")]
        [Description("Can navigate next")]
        public bool CanNavigateNext { get; set; }

        [Category("Navigation")]
        [Description("Can navigate to specific step")]
        public Dictionary<int, bool> CanNavigateToStep { get; set; } = new Dictionary<int, bool>();

        public override string ToString() => $"Step {CurrentStepIndex}";
    }
}
