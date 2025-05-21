using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color NavigationHoverForeColor { get; set; } = Color.White;
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}