using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Base class for control widget values
    /// </summary>
    public abstract class ControlValue
    {
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }

    /// <summary>
    /// Toggle switch value (on/off)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToggleValue : ControlValue
    {
        [Category("Data")]
        [Description("Toggle state (true = on, false = off)")]
        public bool IsOn { get; set; } = false;

        public override object GetValue() => IsOn;
        public override void SetValue(object value) => IsOn = value is bool b ? b : Convert.ToBoolean(value);
        
        public override string ToString() => IsOn ? "On" : "Off";
    }

    /// <summary>
    /// Slider value with numeric range
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SliderValue : ControlValue
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

        public override object GetValue() => Value;
        public override void SetValue(object value) => Value = Convert.ToDouble(value);
        
        public override string ToString() => $"{Value:F2}";
    }

    /// <summary>
    /// Range selector value (min/max range)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RangeValue : ControlValue
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

        public override object GetValue() => new { Min = MinValue, Max = MaxValue, Current = CurrentValue };
        public override void SetValue(object value)
        {
            if (value is RangeValue rv)
            {
                MinValue = rv.MinValue;
                MaxValue = rv.MaxValue;
                CurrentValue = rv.CurrentValue;
            }
        }
        
        public override string ToString() => $"{MinValue:F2} - {MaxValue:F2} (Current: {CurrentValue:F2})";
    }

    /// <summary>
    /// Date/time picker value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DateValue : ControlValue
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

        public override object GetValue() => Value;
        public override void SetValue(object value) => Value = value is DateTime dt ? dt : Convert.ToDateTime(value);
        
        public override string ToString() => Value.ToString("g");
    }

    /// <summary>
    /// Color picker value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorValue : ControlValue
    {
        [Category("Data")]
        [Description("Selected color")]
        public Color Value { get; set; } = Color.Black;

        public override object GetValue() => Value;
        public override void SetValue(object value) => Value = value is Color c ? c : (Color)value;
        
        public override string ToString() => Value.ToString();
    }

    /// <summary>
    /// Number spinner value
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumberValue : ControlValue
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

        public override object GetValue() => Value;
        public override void SetValue(object value) => Value = Convert.ToDecimal(value);
        
        public override string ToString() => Value.ToString("F2");
    }
}
