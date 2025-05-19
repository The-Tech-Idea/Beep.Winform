using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.Black;
        public Color SwitchBorderColor { get; set; } = Color.Gray;
        public Color SwitchForeColor { get; set; } = Color.WhiteSmoke;

        public Color SwitchSelectedBackColor { get; set; } = Color.WhiteSmoke;
        public Color SwitchSelectedBorderColor { get; set; } = Color.Silver;
        public Color SwitchSelectedForeColor { get; set; } = Color.Black;

        public Color SwitchHoverBackColor { get; set; } = Color.DimGray;
        public Color SwitchHoverBorderColor { get; set; } = Color.LightGray;
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
