using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Configuration model for marquee style properties
    /// Defines visual properties for each control style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MarqueeStyleConfig
    {
        [Category("Style")]
        [Description("Show shadow effect on items")]
        public bool ShowShadow { get; set; } = false;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(20, Color.Black);

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended minimum height")]
        public int RecommendedMinimumHeight { get; set; } = 40;

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(8, 4, 8, 4);

        [Category("Animation")]
        [Description("Recommended component spacing")]
        public int RecommendedComponentSpacing { get; set; } = 20;

        [Category("Animation")]
        [Description("Recommended scroll speed")]
        public float RecommendedScrollSpeed { get; set; } = 2.0f;

        [Category("Animation")]
        [Description("Recommended scroll interval (ms)")]
        public int RecommendedScrollInterval { get; set; } = 30;

        public override string ToString() => $"Spacing: {RecommendedComponentSpacing}, Speed: {RecommendedScrollSpeed:F1}";
    }
}
