using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(33, 37, 41); // dark gray text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 215); // blue when checked
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(0, 102, 204);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 102, 204);
        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
