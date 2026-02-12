using System;
using System.Drawing;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Strongly typed model for dashboard metrics.
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

        public override string ToString()
        {
            return $"{Title}: {Value}";
        }
    }
}
