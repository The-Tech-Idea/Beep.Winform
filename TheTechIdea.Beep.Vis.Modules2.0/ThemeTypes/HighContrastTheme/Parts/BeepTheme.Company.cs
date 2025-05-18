using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.Black;
        public Color CompanyTitleColor { get; set; } = Color.White;
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.Yellow;
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Color CompanyDescriptionColor { get; set; } = Color.LightGray;
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.Cyan;
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.Yellow;
        public Color CompanyButtonTextColor { get; set; } = Color.Black;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.Black;
        public Color CompanyDropdownTextColor { get; set; } = Color.White;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.White;
    }
}
