using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.WhiteSmoke;
        public Color SwitchBorderColor { get; set; } = Color.LightGray;
        public Color SwitchForeColor { get; set; } = Color.Black;
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);  // Blue highlight
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color SwitchHoverBorderColor { get; set; } = Color.Gray;
        public Color SwitchHoverForeColor { get; set; } = Color.Black;
    }
}
