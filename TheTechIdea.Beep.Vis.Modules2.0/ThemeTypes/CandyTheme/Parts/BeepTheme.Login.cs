using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Login Popover Colors

        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 224, 235);   // Pastel Pink
        public Color LoginTitleColor { get; set; } = Color.FromArgb(240, 100, 180);               // Candy Pink
        public TypographyStyle LoginTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 13f, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(127, 255, 212);            // Mint
        public TypographyStyle LoginSubtitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Italic);

        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(44, 62, 80);            // Navy (for maximum readability)
        public TypographyStyle LoginDescriptionFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(54, 162, 235);                 // Soft Blue
        public TypographyStyle LoginLinkFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 253, 194);  // Lemon Yellow
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy

        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(204, 255, 240);      // Mint
    }
}
