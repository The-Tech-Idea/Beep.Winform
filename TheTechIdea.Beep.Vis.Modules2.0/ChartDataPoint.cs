using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class ChartDataSeries 
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
        public List<ChartDataPoint> Points { get; set; } = new List<ChartDataPoint>(); // List of data points
        public ChartDataPointStyle DataPointStyle { get; set; } = ChartDataPointStyle.Circle; // Legend style for the series
       
        public ChartDataSeries()
        {
        }
     
    }
    public class ChartDataPoint
    {
        public string X { get; set; }      // Holds string representation of numeric, string, or date values
        public string Y { get; set; }      // Holds string representation of numeric, string, or date values
        public float Size { get; set; }    // For charts that need a size, e.g., bubble charts
        public string Label { get; set; }  // A label for display or identification
        public Color Color { get; set; } = Color.Blue; // Default color for this point
        public string ToolTip { get; set; } // Optional tooltip text for this point

        public ChartDataPoint() { }
        public ChartDataPoint(string x, string y) { X = x; Y = y; }
        public ChartDataPoint(string x, string y, float size) { X = x; Y = y; Size = size; }
        public ChartDataPoint(string x, string y, float size, string label) { X = x; Y = y; Size = size; Label = label; }
        public ChartDataPoint(string x, string y, float size, string label, Color color) { X = x; Y = y; Size = size; Label = label; Color = color; }
        public ChartDataPoint(string x, string y, float size, string label, Color color, string tooltip) { X = x; Y = y; Size = size; Label = label; Color = color; ToolTip = tooltip; }
    }
}
