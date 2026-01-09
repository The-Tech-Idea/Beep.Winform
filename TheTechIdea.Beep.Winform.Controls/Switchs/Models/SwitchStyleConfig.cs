using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Models
{
    /// <summary>
    /// Configuration model for switch style properties
    /// Defines visual properties for each control style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SwitchStyleConfig
    {
        [Category("Style")]
        [Description("Track size ratio (width:height)")]
        public float TrackSizeRatio { get; set; } = 1.625f;

        [Category("Style")]
        [Description("Thumb size as percentage of track height")]
        public float ThumbSizeRatio { get; set; } = 0.875f;

        [Category("Style")]
        [Description("Show shadow effect on thumb")]
        public bool ShowThumbShadow { get; set; } = true;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        [Category("Style")]
        [Description("Shadow offset/elevation")]
        public Point ShadowOffset { get; set; } = new Point(0, 2);

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Animation")]
        [Description("Animation duration in milliseconds")]
        public int AnimationDuration { get; set; } = 200;

        [Category("Layout")]
        [Description("Recommended minimum size")]
        public Size RecommendedMinimumSize { get; set; } = new Size(52, 32);

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(8);

        public override string ToString() => $"Ratio: {TrackSizeRatio:F2}, Thumb: {ThumbSizeRatio:F2}";
    }
}
