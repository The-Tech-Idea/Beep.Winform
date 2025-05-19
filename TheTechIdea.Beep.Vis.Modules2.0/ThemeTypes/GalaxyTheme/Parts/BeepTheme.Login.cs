using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Login Popover Colors
<<<<<<< HEAD
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color LoginTitleColor { get; set; } = Color.White;
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color LoginDescriptionColor { get; set; } = Color.LightGray;
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Sky blue
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public Color LoginDropdownTextColor { get; set; } = Color.White;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Deep galaxy blue
=======
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
        public TypographyStyle LoginTitleFont { get; set; } 
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle LoginSubtitleFont { get; set; } 
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle LoginDescriptionFont { get; set; }
        public Color LoginLinkColor { get; set; } = Color.Blue;
        public TypographyStyle LoginLinkFont { get; set; }
        public Color LoginButtonBackgroundColor { get; set; } = Color.Blue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } 
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
