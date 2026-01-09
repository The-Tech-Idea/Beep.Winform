using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Models
{
    /// <summary>
    /// Color configuration model for switch control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SwitchColorConfig
    {
        [Category("Colors")]
        [Description("Switch control background color")]
        public Color SwitchBackgroundColor { get; set; } = Color.Transparent;

        [Category("Colors")]
        [Description("Track background color (ON)")]
        public Color TrackOnColor { get; set; } = Color.FromArgb(76, 175, 80);

        [Category("Colors")]
        [Description("Track background color (OFF)")]
        public Color TrackOffColor { get; set; } = Color.FromArgb(189, 189, 189);

        [Category("Colors")]
        [Description("Track background color (hovered)")]
        public Color TrackHoveredColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Thumb color")]
        public Color ThumbColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Thumb color (hovered)")]
        public Color ThumbHoveredColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Track border color (ON)")]
        public Color TrackBorderOnColor { get; set; } = Color.FromArgb(66, 165, 70);

        [Category("Colors")]
        [Description("Track border color (OFF)")]
        public Color TrackBorderOffColor { get; set; } = Color.FromArgb(224, 224, 224);

        [Category("Colors")]
        [Description("Label text color (ON, active)")]
        public Color LabelOnActiveColor { get; set; } = Color.FromArgb(76, 175, 80);

        [Category("Colors")]
        [Description("Label text color (OFF, active)")]
        public Color LabelOffActiveColor { get; set; } = Color.FromArgb(33, 37, 41);

        [Category("Colors")]
        [Description("Label text color (inactive)")]
        public Color LabelInactiveColor { get; set; } = Color.FromArgb(180, 180, 180);

        [Category("Colors")]
        [Description("Thumb shadow color")]
        public Color ThumbShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        public override string ToString() => $"ON: {TrackOnColor}, OFF: {TrackOffColor}";
    }
}
