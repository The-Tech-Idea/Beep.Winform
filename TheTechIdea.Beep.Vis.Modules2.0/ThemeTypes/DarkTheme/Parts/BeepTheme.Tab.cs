using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color TabForeColor { get; set; } = Color.LightGray;

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(0, 122, 204);  // Accent Blue
        public Color ActiveTabForeColor { get; set; } = Color.White;

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(45, 45, 45);
        public Color InactiveTabForeColor { get; set; } = Color.Gray;

        public Color TabBorderColor { get; set; } = Color.DimGray;
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color TabHoverForeColor { get; set; } = Color.White;

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color TabSelectedForeColor { get; set; } = Color.White;

        public Color TabSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color TabHoverBorderColor { get; set; } = Color.LightBlue;
    }
}
