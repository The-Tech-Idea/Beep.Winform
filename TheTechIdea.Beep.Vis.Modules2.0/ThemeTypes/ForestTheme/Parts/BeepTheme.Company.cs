using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 255, 245); // Light Mint
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(0, 51, 0); // Dark Green
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(0, 102, 51); // Medium Dark Green
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(85, 107, 47); // Dark Olive Green
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(46, 139, 87); // Sea Green
    }
}
