using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.Black;
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color RadioButtonHoverForeColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 100, 180);
        public TypographyStyle  RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle  RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
