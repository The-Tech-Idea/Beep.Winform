using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Form field attributes - strongly-typed replacement for Dictionary&lt;string, object&gt;
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FormFieldAttributes
    {
        [Category("Validation")]
        [Description("Regular expression pattern for validation")]
        public string Pattern { get; set; } = string.Empty;

        [Category("Validation")]
        [Description("Minimum length for text fields")]
        public int? MinLength { get; set; }

        [Category("Validation")]
        [Description("Maximum length for text fields")]
        public int? MaxLength { get; set; }

        [Category("Validation")]
        [Description("Custom validation error message")]
        public string ErrorMessage { get; set; } = string.Empty;

        [Category("Display")]
        [Description("Placeholder text")]
        public string Placeholder { get; set; } = string.Empty;

        [Category("Display")]
        [Description("Help text or tooltip")]
        public string HelpText { get; set; } = string.Empty;

        [Category("Behavior")]
        [Description("Whether the field is disabled")]
        public bool Disabled { get; set; } = false;

        [Category("Behavior")]
        [Description("Whether the field is readonly")]
        public bool ReadOnly { get; set; } = false;

        [Category("Behavior")]
        [Description("Whether the field is required")]
        public bool Required { get; set; } = false;

        /// <summary>
        /// Converts to dictionary for backward compatibility
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(Pattern)) dict["Pattern"] = Pattern;
            if (MinLength.HasValue) dict["MinLength"] = MinLength.Value;
            if (MaxLength.HasValue) dict["MaxLength"] = MaxLength.Value;
            if (!string.IsNullOrEmpty(ErrorMessage)) dict["ErrorMessage"] = ErrorMessage;
            if (!string.IsNullOrEmpty(Placeholder)) dict["Placeholder"] = Placeholder;
            if (!string.IsNullOrEmpty(HelpText)) dict["HelpText"] = HelpText;
            dict["Disabled"] = Disabled;
            dict["ReadOnly"] = ReadOnly;
            dict["Required"] = Required;
            return dict;
        }

        /// <summary>
        /// Creates from dictionary for migration
        /// </summary>
        public static FormFieldAttributes FromDictionary(Dictionary<string, object> dict)
        {
            var attrs = new FormFieldAttributes();
            if (dict.ContainsKey("Pattern")) attrs.Pattern = dict["Pattern"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("MinLength") && dict["MinLength"] is int minLen) attrs.MinLength = minLen;
            if (dict.ContainsKey("MaxLength") && dict["MaxLength"] is int maxLen) attrs.MaxLength = maxLen;
            if (dict.ContainsKey("ErrorMessage")) attrs.ErrorMessage = dict["ErrorMessage"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Placeholder")) attrs.Placeholder = dict["Placeholder"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("HelpText")) attrs.HelpText = dict["HelpText"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Disabled") && dict["Disabled"] is bool disabled) attrs.Disabled = disabled;
            if (dict.ContainsKey("ReadOnly") && dict["ReadOnly"] is bool readOnly) attrs.ReadOnly = readOnly;
            if (dict.ContainsKey("Required") && dict["Required"] is bool required) attrs.Required = required;
            return attrs;
        }
    }

    /// <summary>
    /// Text field data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TextFieldData
    {
        [Category("Data")]
        [Description("Text value")]
        public string Value { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Maximum length")]
        public int MaxLength { get; set; } = 0;
    }

    /// <summary>
    /// Number field data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumberFieldData
    {
        [Category("Data")]
        [Description("Numeric value")]
        public decimal Value { get; set; } = 0m;

        [Category("Data")]
        [Description("Minimum allowed value")]
        public decimal? Min { get; set; }

        [Category("Data")]
        [Description("Maximum allowed value")]
        public decimal? Max { get; set; }
    }

    /// <summary>
    /// Date field data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DateFieldData
    {
        [Category("Data")]
        [Description("Selected date")]
        public DateTime? Value { get; set; }

        [Category("Data")]
        [Description("Minimum allowed date")]
        public DateTime? Min { get; set; }

        [Category("Data")]
        [Description("Maximum allowed date")]
        public DateTime? Max { get; set; }
    }

    /// <summary>
    /// Select field data (dropdown/radio)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SelectFieldData
    {
        [Category("Data")]
        [Description("Selected value")]
        public string SelectedValue { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Available options")]
        public List<string> Options { get; set; } = new List<string>();
    }

    /// <summary>
    /// Checkbox field data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckboxFieldData
    {
        [Category("Data")]
        [Description("Checked state")]
        public bool Value { get; set; } = false;
    }

    /// <summary>
    /// File field data
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FileFieldData
    {
        [Category("Data")]
        [Description("File path")]
        public string FilePath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Maximum file size in bytes")]
        public long? MaxSize { get; set; }
    }
}
