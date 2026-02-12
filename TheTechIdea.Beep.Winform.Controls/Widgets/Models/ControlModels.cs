using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Toggle switch value (on/off)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToggleValue
    {
        [Category("Data")]
        [Description("Toggle state (true = on, false = off)")]
        public bool IsOn { get; set; } = false;

        public override string ToString() => IsOn ? "On" : "Off";
    }

    /// <summary>
    /// Slider value with numeric range
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SliderValue
    {
        [Category("Data")]
        [Description("Current slider value")]
        public double Value { get; set; } = 0.0;

        [Category("Data")]
        [Description("Minimum allowed value")]
        public double MinValue { get; set; } = 0.0;

        [Category("Data")]
        [Description("Maximum allowed value")]
        public double MaxValue { get; set; } = 100.0;

        public override string ToString() => $"{Value:F2}";
    }

    /// <summary>
    /// Range selector value (min/max range)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RangeValue
    {
        [Category("Data")]
        [Description("Minimum value in the range")]
        public double MinValue { get; set; } = 0.0;

        [Category("Data")]
        [Description("Maximum value in the range")]
        public double MaxValue { get; set; } = 100.0;

        [Category("Data")]
        [Description("Current selected value")]
        public double CurrentValue { get; set; } = 50.0;

        public override string ToString() => $"{MinValue:F2} - {MaxValue:F2} (Current: {CurrentValue:F2})";
    }

    /// <summary>
    /// Date/time picker value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DateValue
    {
        [Category("Data")]
        [Description("Selected date and time")]
        public DateTime Value { get; set; } = DateTime.Now;

        [Category("Data")]
        [Description("Minimum allowed date")]
        public DateTime? MinDate { get; set; }

        [Category("Data")]
        [Description("Maximum allowed date")]
        public DateTime? MaxDate { get; set; }

        public override string ToString() => Value.ToString("g");
    }

    /// <summary>
    /// Color picker value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorValue
    {
        [Category("Data")]
        [Description("Selected color")]
        public Color Value { get; set; } = Color.Black;

        public override string ToString() => Value.ToString();
    }

    /// <summary>
    /// Number spinner value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumberValue
    {
        [Category("Data")]
        [Description("Current numeric value")]
        public decimal Value { get; set; } = 0m;

        [Category("Data")]
        [Description("Minimum allowed value")]
        public decimal? MinValue { get; set; }

        [Category("Data")]
        [Description("Maximum allowed value")]
        public decimal? MaxValue { get; set; }

        [Category("Data")]
        [Description("Step increment for spinner")]
        public decimal Step { get; set; } = 1m;

        public override string ToString() => Value.ToString("F2");
    }

    /// <summary>
    /// Checkbox option model for checkbox group controls
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckboxOption
    {
        [Category("Data")]
        [Description("Display label for the option")]
        public string Label { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Checked state")]
        public bool IsChecked { get; set; }

        public override string ToString() => Label;
    }
}
