using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(120, 120, 120);
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(150, 150, 150);
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(150, 150, 150) };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(245, 183, 203);
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(245, 183, 203), IsUnderlined = true };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(242, 201, 215);
    }
}
