using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.Black;
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.DodgerBlue;
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.DodgerBlue;
        public Color RadioButtonHoverBackColor { get; set; } = Color.LightBlue;
        public Color RadioButtonHoverForeColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.SteelBlue;
        public TypographyStyle  RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.DodgerBlue;
    }
}
