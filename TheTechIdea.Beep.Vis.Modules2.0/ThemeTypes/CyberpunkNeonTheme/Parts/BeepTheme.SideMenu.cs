using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Side Menu Fonts & Colors

        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Italic);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(18, 18, 32);                      // Cyberpunk Black
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);                // Neon Green (hover)
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);             // Neon Magenta (selected)

        public Color SideMenuForeColor { get; set; } = Color.FromArgb(0, 255, 255);                     // Neon Cyan
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;                             // Selected (white for contrast)
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);                // Neon Yellow (hover)

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(255, 0, 255);                   // Neon Magenta border

        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(0, 255, 255);                // Neon Cyan
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(18, 18, 32);                          // Keep it transparent for flexibility

        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.FromArgb(0, 255, 255)
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(0, 255, 128);             // Neon Green
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(18, 18, 32);

        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0, 255, 128)
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon Magenta
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon Cyan
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(0, 255, 128);           // Neon Green
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
