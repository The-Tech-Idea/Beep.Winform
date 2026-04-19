using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(235, 245, 255);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public TypographyStyle  RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(220, 235, 255);
    }
}
