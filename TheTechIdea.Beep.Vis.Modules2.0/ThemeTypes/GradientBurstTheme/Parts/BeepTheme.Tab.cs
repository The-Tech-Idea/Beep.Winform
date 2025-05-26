using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Tab Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle  TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);
        public TypographyStyle  TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color TabForeColor { get; set; } = Color.FromArgb(30, 30, 30);

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ActiveTabForeColor { get; set; } = Color.White;

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(225, 225, 225);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(80, 80, 80);

        public Color TabBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(220, 240, 255);
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(0, 90, 180);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
