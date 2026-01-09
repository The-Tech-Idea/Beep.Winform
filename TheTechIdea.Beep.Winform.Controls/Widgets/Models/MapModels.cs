using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Map point (latitude/longitude)
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapPoint
    {
        [Category("Data")]
        [Description("Latitude coordinate")]
        public double Latitude { get; set; } = 0.0;

        [Category("Data")]
        [Description("Longitude coordinate")]
        public double Longitude { get; set; } = 0.0;

        public override string ToString() => $"({Latitude:F6}, {Longitude:F6})";
    }

    /// <summary>
    /// Map marker
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapMarker
    {
        [Category("Data")]
        [Description("Unique identifier for the marker")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Latitude coordinate")]
        public double Latitude { get; set; } = 0.0;

        [Category("Data")]
        [Description("Longitude coordinate")]
        public double Longitude { get; set; } = 0.0;

        [Category("Data")]
        [Description("Label text for the marker")]
        public string Label { get; set; } = string.Empty;

        [Category("Appearance")]
        [Description("Path to custom icon image")]
        public string IconPath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Optional description or tooltip")]
        public string Description { get; set; } = string.Empty;

        public override string ToString() => Label;
    }

    /// <summary>
    /// Map route
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapRoute
    {
        [Category("Data")]
        [Description("Points along the route")]
        public List<MapPoint> Points { get; set; } = new List<MapPoint>();

        [Category("Appearance")]
        [Description("Color of the route line")]
        public Color Color { get; set; } = Color.Red;

        [Category("Appearance")]
        [Description("Width of the route line in pixels")]
        public int Width { get; set; } = 3;

        [Category("Data")]
        [Description("Route name or description")]
        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
