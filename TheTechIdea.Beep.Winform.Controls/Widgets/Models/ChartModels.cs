using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Chart type enum
    /// </summary>
    public enum ChartType
    {
        Bar,
        Line,
        Pie,
        Area,
        Scatter,
        Gauge,
        Heatmap
    }

    /// <summary>
    /// Chart data point
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartDataPoint
    {
        [Category("Data")]
        [Description("Label for the data point")]
        public string Label { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Numeric value for the data point")]
        public double Value { get; set; } = 0.0;

        [Category("Appearance")]
        [Description("Color for this data point")]
        public Color Color { get; set; } = Color.Blue;

        [Category("Data")]
        [Description("Optional category or group")]
        public string Category { get; set; } = string.Empty;

        public override string ToString() => $"{Label}: {Value:F2}";
    }

    /// <summary>
    /// Chart series containing multiple data points
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartSeries
    {
        [Category("Data")]
        [Description("Name of the series")]
        public string Name { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Data points in this series")]
        public List<ChartDataPoint> DataPoints { get; set; } = new List<ChartDataPoint>();

        [Category("Data")]
        [Description("Chart type for this series")]
        public ChartType ChartType { get; set; } = ChartType.Bar;

        [Category("Appearance")]
        [Description("Color for the series")]
        public Color Color { get; set; } = Color.Blue;

        public override string ToString() => Name;
    }
}
