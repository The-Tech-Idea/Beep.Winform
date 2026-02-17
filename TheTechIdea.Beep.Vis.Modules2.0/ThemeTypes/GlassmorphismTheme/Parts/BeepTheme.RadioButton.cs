using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color RadioButtonForeColor { get; set; } = Color.Black;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(200, 210, 220);

        public Color RadioButtonCheckedBackColor { get; set; } = Color.LightSkyBlue;
        public Color RadioButtonCheckedForeColor { get; set; } = Color.Black;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.SteelBlue;

        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color RadioButtonHoverForeColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.CornflowerBlue;

        public TypographyStyle  RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.DeepSkyBlue;
    }
}
