using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color SwitchBorderColor { get; set; } = Color.DimGray;
        public Color SwitchForeColor { get; set; } = Color.LightGray;

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color SwitchSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color SwitchHoverBorderColor { get; set; } = Color.LightBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.White;
    }
}
