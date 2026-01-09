using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Models
{
    /// <summary>
    /// Color configuration model for chip control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChipColorConfig
    {
        [Category("Colors")]
        [Description("Chip background color")]
        public Color BackgroundColor { get; set; } = Color.FromArgb(230, 230, 230);

        [Category("Colors")]
        [Description("Chip foreground/text color")]
        public Color ForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Chip border color")]
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Chip hover background color")]
        public Color HoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245);

        [Category("Colors")]
        [Description("Chip selected background color")]
        public Color SelectedBackgroundColor { get; set; } = Color.FromArgb(0, 122, 255);

        [Category("Colors")]
        [Description("Chip selected foreground color")]
        public Color SelectedForegroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Title color")]
        public Color TitleColor { get; set; } = Color.Black;

        [Category("Colors")]
        [Description("Group background color")]
        public Color GroupBackgroundColor { get; set; } = SystemColors.Window;

        public override string ToString() => $"BG: {BackgroundColor}, Text: {ForegroundColor}, Selected: {SelectedBackgroundColor}";
    }
}
