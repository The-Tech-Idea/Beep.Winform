using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Strongly-typed model for dashboard metrics
    /// Replaces Dictionary&lt;string, object&gt; for type safety and IntelliSense support
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DashboardMetric
    {
        /// <summary>
        /// Title or label for the metric
        /// </summary>
        [Category("Data")]
        [Description("Title or label for the metric")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The metric value to display (formatted as string)
        /// </summary>
        [Category("Data")]
        [Description("The metric value to display (formatted as string)")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Trend indicator text (e.g., "+12%", "-3.2%")
        /// </summary>
        [Category("Data")]
        [Description("Trend indicator text (e.g., '+12%', '-3.2%')")]
        public string Trend { get; set; } = string.Empty;

        /// <summary>
        /// Color for the metric display
        /// </summary>
        [Category("Appearance")]
        [Description("Color for the metric display")]
        public Color Color { get; set; } = Color.Blue;

        /// <summary>
        /// Path to icon image for the metric
        /// </summary>
        [Category("Appearance")]
        [Description("Path to icon image for the metric")]
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Subtitle or additional information
        /// </summary>
        [Category("Data")]
        [Description("Subtitle or additional information")]
        public string Subtitle { get; set; } = string.Empty;

        /// <summary>
        /// Optional numeric value for calculations
        /// </summary>
        [Category("Data")]
        [Description("Optional numeric value for calculations")]
        public double? NumericValue { get; set; }

        /// <summary>
        /// Converts this metric to a dictionary for backward compatibility
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                ["Title"] = Title,
                ["Value"] = Value,
                ["Trend"] = Trend,
                ["Color"] = Color,
                ["IconPath"] = IconPath,
                ["Subtitle"] = Subtitle,
                ["NumericValue"] = NumericValue ?? 0.0
            };
        }

        /// <summary>
        /// Creates a DashboardMetric from a dictionary (for migration)
        /// </summary>
        public static DashboardMetric FromDictionary(Dictionary<string, object> dict)
        {
            var metric = new DashboardMetric();
            if (dict.ContainsKey("Title")) metric.Title = dict["Title"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Value")) metric.Value = dict["Value"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Trend")) metric.Trend = dict["Trend"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Color") && dict["Color"] is Color color) metric.Color = color;
            if (dict.ContainsKey("IconPath")) metric.IconPath = dict["IconPath"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("Subtitle")) metric.Subtitle = dict["Subtitle"]?.ToString() ?? string.Empty;
            if (dict.ContainsKey("NumericValue") && dict["NumericValue"] is double numValue) metric.NumericValue = numValue;
            return metric;
        }

        public override string ToString()
        {
            return $"{Title}: {Value}";
        }
    }
}
