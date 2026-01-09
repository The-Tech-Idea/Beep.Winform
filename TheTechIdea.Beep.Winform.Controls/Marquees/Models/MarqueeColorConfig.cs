using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Color configuration model for marquee control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MarqueeColorConfig
    {
        [Category("Colors")]
        [Description("Marquee control background color")]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        [Category("Colors")]
        [Description("Border color")]
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Text color for marquee items")]
        public Color TextColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Shadow color for marquee items")]
        public Color ShadowColor { get; set; } = Color.FromArgb(20, Color.Black);

        public override string ToString() => $"BG: {BackgroundColor}, Text: {TextColor}";
    }
}
