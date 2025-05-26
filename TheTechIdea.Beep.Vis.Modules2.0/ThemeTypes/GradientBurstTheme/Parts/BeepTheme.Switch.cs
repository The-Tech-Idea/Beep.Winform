using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Switch control Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);
        public TypographyStyle  SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color SwitchForeColor { get; set; } = Color.FromArgb(60, 60, 60);

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(0, 90, 180);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
