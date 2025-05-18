using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
        public Color CompanyTitleColor { get; set; } = Color.Black;
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.DodgerBlue;
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Underline);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.RoyalBlue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
