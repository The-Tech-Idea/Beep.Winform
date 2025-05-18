using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color LoginTitleColor { get; set; } = Color.WhiteSmoke;
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 16F, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.LightSteelBlue;
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.CornflowerBlue;
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.DodgerBlue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color LoginDropdownTextColor { get; set; } = Color.WhiteSmoke;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(80, 80, 80);
    }
}
