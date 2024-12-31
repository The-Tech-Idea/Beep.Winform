using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Desktop.Controls.Common
{
    [Serializable]
    public class ChartDataSeries : List<ChartDataPoint>
    {
        public Color Color { get; set; } = Color.Blue; // Default color for this series
        public string Name { get; set; } // Name of the data series
        public string XAxis { get; set; } // Name of the X axis
        public string YAxis { get; set; } // Name of the Y axis
        public string Label { get; set; } // Label for the series
        public string ToolTip { get; set; } // Tooltip for the series
        public bool Visible { get; set; } = true; // Whether the series is visible
        public bool ShowInLegend { get; set; } = true; // Whether the series is shown in the legend
        public bool ShowLabel { get; set; } = true; // Whether to show labels for the series
        public bool ShowToolTip { get; set; } = true; // Whether to show tooltips for the series
        public bool ShowLine { get; set; } = true; // Whether to show a line connecting the points
        public bool ShowPoint { get; set; } = true; // Whether to show a point at each data point
        public bool ShowArea { get; set; } = false; // Whether to fill the area under the line
        public ChartType ChartType { get; set; } = ChartType.Line; // Type of chart to display
        public ChartDataPointStyle DataPointStyle { get; set; } = ChartDataPointStyle.Circle; // Legend style for the series
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip, bool showLine, bool showPoint, bool showArea, ChartType chartType)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
            ShowLine = showLine;
            ShowPoint = showPoint;
            ShowArea = showArea;
            ChartType = chartType;
        }


        public ChartDataSeries()
        {
        }
        public ChartDataSeries(string name)
        {
            Name = name;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip, bool showLine)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
            ShowLine = showLine;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip, bool showLine, bool showPoint)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
            ShowLine = showLine;
            ShowPoint = showPoint;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip, bool showLine, bool showPoint, bool showArea)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
            ShowLine = showLine;
            ShowPoint = showPoint;
            ShowArea = showArea;
        }
        public ChartDataSeries(string name, string xAxis, string yAxis, string label, string tooltip, bool visible, bool showInLegend, bool showLabel, bool showToolTip, bool showLine, bool showPoint, bool showArea, IEnumerable<ChartDataPoint> points)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
            Label = label;
            ToolTip = tooltip;
            Visible = visible;
            ShowInLegend = showInLegend;
            ShowLabel = showLabel;
            ShowToolTip = showToolTip;
            ShowLine = showLine;
            ShowPoint = showPoint;
            ShowArea = showArea;
            AddRange(points);
        }
    }
    [Serializable]
    public class ChartDataPoint
    {
        public object X { get; set; }      // Can hold numeric, string, or date values
        public object Y { get; set; }      // Can hold numeric, string, or date values
        public float Size { get; set; }    // For charts that need a size, e.g., bubble charts
        public string Label { get; set; }  // A label for display or identification
        public Action Action { get; set; } // Optional action associated with the data point
        public Color Color { get; set; } = Color.Blue; // Default color for this point

        public string ToolTip { get; set; } // Optional tooltip text for this point

        public ChartDataPoint()
        {
        }
        public ChartDataPoint(object x, object y)
        {
            X = x;
            Y = y;
        }
        public ChartDataPoint(object x, object y, float size)
        {
            X = x;
            Y = y;
            Size = size;
        }
        public ChartDataPoint(object x, object y, float size, string label)
        {
            X = x;
            Y = y;
            Size = size;
            Label = label;
        }
        public ChartDataPoint(object x, object y, float size, string label, Color color)
        {
            X = x;
            Y = y;
            Size = size;
            Label = label;
            Color = color;
        }
        public ChartDataPoint(object x, object y, float size, string label, Color color, string tooltip)
        {
            X = x;
            Y = y;
            Size = size;
            Label = label;
            Color = color;
            ToolTip = tooltip;
        }
        // Method to retrieve X value as double (for numeric calculations)
        public double GetXAsDouble()
        {
            return X is double ? (double)X : 0;
        }

        // Method to retrieve Y value as double (for numeric calculations)
        public double GetYAsDouble()
        {
            return Y is double ? (double)Y : 0;
        }

        // Method to retrieve X value as DateTime (for date-based calculations)
        public DateTime? GetXAsDateTime()
        {
            return X is DateTime date ? date : (DateTime?)null;
        }

        // Method to retrieve Y value as DateTime (for date-based calculations)
        public DateTime? GetYAsDateTime()
        {
            return Y is DateTime date ? date : (DateTime?)null;
        }
        // Helper methods to get numeric values
        public float? GetXAsFloat()
        {
            return X is float x ? x : X is double d ? (float)d : X is int i ? (float)i : (float?)null;
        }

        public float? GetYAsFloat()
        {
            return Y is float y ? y : Y is double d ? (float)d : Y is int i ? (float)i : (float?)null;
        }
        // Method to retrieve X and Y values as strings for text-based axes
        public string GetXAsString() => X?.ToString() ?? string.Empty;
        public string GetYAsString() => Y?.ToString() ?? string.Empty;
        // Get X as a float, handling dates and numbers
        public float? GetXAsFloat(DateTime? minDate = null)
        {
            if (X is float x) return x;
            if (X is double d) return (float)d;
            if (X is int i) return i;
            if (X is DateTime dt && minDate.HasValue)
            {
                // Convert date to float by calculating days difference from a base date
                return (float)(dt - minDate.Value).TotalDays;
            }
            return null;
        }


    }
}
