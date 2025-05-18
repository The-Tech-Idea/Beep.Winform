using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk (soft sandy)
        public Color LoginTitleColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark brown
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 18, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);

        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(222, 184, 135); // BurlyWood
        public Color LoginButtonTextColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark brown
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 250, 240); // FloralWhite
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark brown
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
    }
}
