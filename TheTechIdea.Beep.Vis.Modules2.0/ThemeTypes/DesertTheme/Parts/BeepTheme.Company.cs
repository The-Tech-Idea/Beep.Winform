using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Company Colors - Desert Theme
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 248, 238); // Soft warm cream
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(92, 60, 22); // Dark earthy brown
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(157, 113, 51); // Warm tan
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(135, 108, 78); // Muted brownish gray
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(178, 117, 61); // Rustic orange/brown
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(191, 140, 75); // Soft desert gold
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(255, 248, 238); // same as popover bg
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(92, 60, 22); // dark brown text
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(210, 180, 140); // Light tan desert sand
    }
}
