using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(245, 222, 179);  // Wheat
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color SwitchForeColor { get; set; } = Color.FromArgb(101, 67, 33);     // Dark Brown

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(255, 228, 181);  // Moccasin
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(139, 69, 19);    // Saddle Brown

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(255, 218, 185);    // Peach Puff
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(244, 164, 96);   // Sandy Brown
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(160, 82, 45);      // Sienna
    }
}
