using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(0, 80, 120);
        public TypographyStyle CompanyTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120) };
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public TypographyStyle CompanySubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 105, 148) };
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(0, 130, 180);
        public TypographyStyle CompanyDescriptionFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180) };
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(0, 150, 200);
        public TypographyStyle CompanyLinkFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 150, 200), IsUnderlined = true };
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(0, 105, 148);
    }
}