using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Company Colors
//<<<<<<< HEAD
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color CompanyTitleColor { get; set; } = Color.White;
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(0xCC, 0xCC, 0xCC); // Light gray
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);

        public Color CompanyLinkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Sky blue
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor
        public Color CompanyDropdownTextColor { get; set; } = Color.White;

        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Deep galaxy blue
    }
}
