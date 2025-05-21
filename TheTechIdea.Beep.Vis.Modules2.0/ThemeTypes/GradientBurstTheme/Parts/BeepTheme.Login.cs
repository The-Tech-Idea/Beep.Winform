using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Login Popover Colors
//<<<<<<< HEAD
        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(255, 250, 250, 250); // Soft white
        public Color LoginTitleColor { get; set; } = Color.FromArgb(255, 30, 30, 30); // Deep charcoal
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(255, 70, 130, 180); // SteelBlue
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Italic);

        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(255, 100, 100, 100); // Medium Gray
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(255, 0, 120, 215); // Fluent link blue
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(255, 0, 122, 204); // Vibrant blue
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 13, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(255, 220, 220, 220); // Neutral gray
    }
}
