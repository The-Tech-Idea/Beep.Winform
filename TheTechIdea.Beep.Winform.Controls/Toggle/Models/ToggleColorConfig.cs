using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Models
{
    /// <summary>
    /// Color configuration model for toggle control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToggleColorConfig
    {
        [Category("Colors")]
        [Description("Color when toggle is ON")]
        public Color OnColor { get; set; } = Color.FromArgb(52, 168, 83);

        [Category("Colors")]
        [Description("Color when toggle is OFF")]
        public Color OffColor { get; set; } = Color.FromArgb(189, 189, 189);

        [Category("Colors")]
        [Description("Thumb color")]
        public Color ThumbColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Thumb color when ON")]
        public Color OnThumbColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Thumb color when OFF")]
        public Color OffThumbColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Text/label color")]
        public Color TextColor { get; set; } = Color.Black;

        [Category("Colors")]
        [Description("Text color when ON")]
        public Color OnTextColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Text color when OFF")]
        public Color OffTextColor { get; set; } = Color.FromArgb(97, 97, 97);

        [Category("Colors")]
        [Description("Border color")]
        public Color BorderColor { get; set; } = Color.Transparent;

        [Category("Colors")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        public override string ToString() => $"ON: {OnColor}, OFF: {OffColor}";
    }
}
