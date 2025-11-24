using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color RadioButtonForeColor { get; set; } = Color.LightGray;
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.CornflowerBlue;
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.LightBlue;
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color RadioButtonHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public TypographyStyle  RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.CornflowerBlue;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(40, 40, 40);
    }
}
