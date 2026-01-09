using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Models
{
    /// <summary>
    /// Configuration model for toggle style properties
    /// Defines visual properties for each toggle style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToggleStyleConfig
    {
        [Category("Style")]
        [Description("Border radius for track and thumb")]
        public int BorderRadius { get; set; } = 14;

        [Category("Style")]
        [Description("Shape of the track")]
        public ToggleTrackShape TrackShape { get; set; } = ToggleTrackShape.Pill;

        [Category("Style")]
        [Description("Shape of the thumb")]
        public ToggleThumbShape ThumbShape { get; set; } = ToggleThumbShape.Circle;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = true;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        [Category("Style")]
        [Description("Shadow offset")]
        public Point ShadowOffset { get; set; } = new Point(0, 2);

        [Category("Style")]
        [Description("Use gradient for track")]
        public bool UseGradient { get; set; } = false;

        [Category("Style")]
        [Description("Gradient type")]
        public LinearGradientMode GradientType { get; set; } = LinearGradientMode.Horizontal;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Border width")]
        public int BorderWidth { get; set; } = 1;

        [Category("Style")]
        [Description("Recommended size for this style")]
        public Size RecommendedSize { get; set; } = new Size(60, 28);

        [Category("Style")]
        [Description("Minimum size for this style")]
        public Size MinimumSize { get; set; } = new Size(40, 20);

        public override string ToString() => $"Style: {TrackShape}, Radius: {BorderRadius}";
    }
}
