using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Switch control Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);
        public TypographyStyle  SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.Black;
        public Color SwitchBorderColor { get; set; } = Color.White;
        public Color SwitchForeColor { get; set; } = Color.White;
        public Color SwitchSelectedBackColor { get; set; } = Color.Lime;
        public Color SwitchSelectedBorderColor { get; set; } = Color.White;
        public Color SwitchSelectedForeColor { get; set; } = Color.Black;
        public Color SwitchHoverBackColor { get; set; } = Color.Gray;
        public Color SwitchHoverBorderColor { get; set; } = Color.Yellow;
        public Color SwitchHoverForeColor { get; set; } = Color.Yellow;
    }
}
