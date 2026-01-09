using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Models
{
    /// <summary>
    /// Color configuration model for LOV control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LovColorConfig
    {
        [Category("Colors")]
        [Description("LOV control background color")]
        public Color BackgroundColor { get; set; } = SystemColors.Window;

        [Category("Colors")]
        [Description("LOV control foreground/text color")]
        public Color ForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Border color")]
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Border color (hovered)")]
        public Color HoverBorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Border color (focused)")]
        public Color FocusedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);

        [Category("Colors")]
        [Description("Button background color")]
        public Color ButtonBackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);

        [Category("Colors")]
        [Description("Button background color (hovered)")]
        public Color ButtonHoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245);

        [Category("Colors")]
        [Description("Button background color (pressed)")]
        public Color ButtonPressedBackgroundColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Button icon color")]
        public Color ButtonIconColor { get; set; } = Color.FromArgb(100, 100, 100);

        [Category("Colors")]
        [Description("Error/invalid input color")]
        public Color ErrorColor { get; set; } = Color.FromArgb(220, 53, 69);

        public override string ToString() => $"BG: {BackgroundColor}, Text: {ForegroundColor}, Border: {BorderColor}";
    }
}
