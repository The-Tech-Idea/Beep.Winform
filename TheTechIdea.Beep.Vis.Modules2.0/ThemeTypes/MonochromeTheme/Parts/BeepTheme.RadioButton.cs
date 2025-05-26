using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.DimGray;
        public Color RadioButtonForeColor { get; set; } = Color.Black;
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.DimGray;
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBackColor { get; set; } = Color.LightGray;
        public Color RadioButtonHoverForeColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.DarkGray;
        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.Black;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.LightGray;
    }
}
