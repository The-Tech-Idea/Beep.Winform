using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 18, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
