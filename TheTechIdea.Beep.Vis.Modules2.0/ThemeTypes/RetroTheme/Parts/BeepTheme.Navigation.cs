using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };

        public Color NavigationBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color NavigationForeColor { get; set; } = Color.White;
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color NavigationSelectedForeColor { get; set; } = Color.Black;
    }
}