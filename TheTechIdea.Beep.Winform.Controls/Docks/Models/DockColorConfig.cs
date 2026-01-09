using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Models
{
    /// <summary>
    /// Color configuration model for dock control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DockColorConfig
    {
        [Category("Colors")]
        [Description("Dock background color")]
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Colors")]
        [Description("Dock foreground/text color")]
        public Color ForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Border color")]
        public Color BorderColor { get; set; } = Color.FromArgb(100, 255, 255, 255);

        [Category("Colors")]
        [Description("Dock item hover color")]
        public Color ItemHoverColor { get; set; } = Color.FromArgb(245, 245, 245);

        [Category("Colors")]
        [Description("Dock item selected color")]
        public Color ItemSelectedColor { get; set; } = Color.FromArgb(0, 122, 255);

        [Category("Colors")]
        [Description("Indicator color for active/running items")]
        public Color IndicatorColor { get; set; } = Color.FromArgb(0, 122, 255);

        [Category("Colors")]
        [Description("Separator color")]
        public Color SeparatorColor { get; set; } = Color.FromArgb(100, 255, 255, 255);

        [Category("Colors")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        public override string ToString() => $"BG: {BackgroundColor}, Text: {ForegroundColor}, Indicator: {IndicatorColor}";
    }
}
