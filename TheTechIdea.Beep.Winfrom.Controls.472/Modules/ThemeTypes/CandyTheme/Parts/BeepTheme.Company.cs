using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Company Colors

        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 253, 194);   // Lemon Yellow
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(240, 100, 180);               // Candy Pink
        public TypographyStyle CompanyTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(127, 255, 212);            // Mint
        public TypographyStyle CompanySubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);

        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(44, 62, 80);            // Navy (readable)
        public TypographyStyle CompanyDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);

        public Color CompanyLinkColor { get; set; } = Color.FromArgb(54, 162, 235);                 // Soft Blue
        public TypographyStyle CompanyLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 224, 235);  // Pastel Pink
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy

        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(204, 255, 240);      // Mint
    }
}
