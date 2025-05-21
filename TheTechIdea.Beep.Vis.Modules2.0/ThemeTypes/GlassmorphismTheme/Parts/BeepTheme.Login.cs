using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color LoginTitleColor { get; set; } = Color.Black;
//<<<<<<< HEAD
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);

        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color LoginLinkColor { get; set; } = Color.SteelBlue;
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.SkyBlue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.WhiteSmoke;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;

        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
