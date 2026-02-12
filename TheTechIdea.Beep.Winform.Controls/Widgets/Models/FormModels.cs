using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Form field attributes with strongly typed properties.
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

    }

    /// <summary>
    /// Strongly typed form value wrapper used by FormField.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FormFieldValue
    {
        [Category("Data")]
        [Description("Text value")]
        public string Text { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Number value")]
        public decimal? Number { get; set; }

        [Category("Data")]
        [Description("Date value")]
        public DateTime? Date { get; set; }

        [Category("Data")]
        [Description("Boolean value")]
        public bool? Boolean { get; set; }

        [Category("Data")]
        [Description("Color value")]
        public Color? Color { get; set; }

        [Category("Data")]
        [Description("Selected option")]
        public string SelectedOption { get; set; } = string.Empty;

        [Category("Data")]
        [Description("File path")]
        public string FilePath { get; set; } = string.Empty;

        public bool HasValue =>
            !string.IsNullOrWhiteSpace(Text) ||
            Number.HasValue ||
            Date.HasValue ||
            Boolean.HasValue ||
            Color.HasValue ||
            !string.IsNullOrWhiteSpace(SelectedOption) ||
            !string.IsNullOrWhiteSpace(FilePath);

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Text)) return Text;
            if (Number.HasValue) return Number.Value.ToString();
            if (Date.HasValue) return Date.Value.ToString("g");
            if (Boolean.HasValue) return Boolean.Value.ToString();
            if (Color.HasValue) return $"#{Color.Value.R:X2}{Color.Value.G:X2}{Color.Value.B:X2}";
            if (!string.IsNullOrWhiteSpace(SelectedOption)) return SelectedOption;
            if (!string.IsNullOrWhiteSpace(FilePath)) return FilePath;
            return string.Empty;
        }

        public static FormFieldValue FromText(string value) => new FormFieldValue { Text = value ?? string.Empty };
        public static FormFieldValue FromNumber(decimal value) => new FormFieldValue { Number = value };
        public static FormFieldValue FromDate(DateTime value) => new FormFieldValue { Date = value };
        public static FormFieldValue FromBoolean(bool value) => new FormFieldValue { Boolean = value };
        public static FormFieldValue FromSelectedOption(string value) => new FormFieldValue { SelectedOption = value ?? string.Empty };
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

    /// <summary>
    /// Typed layout cache for inline/form painters.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class InlineFieldLayout
    {
        public int Index { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public Rectangle LabelRect { get; set; } = Rectangle.Empty;
        public Rectangle InputRect { get; set; } = Rectangle.Empty;
        public Rectangle FieldRect { get; set; } = Rectangle.Empty;
        public bool IsFocused { get; set; }
    }
}
