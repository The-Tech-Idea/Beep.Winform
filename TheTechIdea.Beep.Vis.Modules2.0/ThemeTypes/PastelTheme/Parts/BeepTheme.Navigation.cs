using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Navigation & Breadcrumbs Fonts & Colors
        public TypographyStyle NavigationTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle NavigationSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle NavigationUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color NavigationBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color NavigationForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color NavigationHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color NavigationHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color NavigationSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color NavigationSelectedForeColor { get; set; } = Color.White;
    }
}