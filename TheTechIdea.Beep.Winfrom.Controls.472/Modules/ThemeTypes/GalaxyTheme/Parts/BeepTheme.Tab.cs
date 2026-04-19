using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle  TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public TypographyStyle  TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TabForeColor { get; set; } = Color.White;

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ActiveTabForeColor { get; set; } = Color.White;

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public Color InactiveTabForeColor { get; set; } = Color.LightGray;

        public Color TabBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E);
        public Color TabHoverForeColor { get; set; } = Color.White;

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.White;

        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Highlight border
    }
}
