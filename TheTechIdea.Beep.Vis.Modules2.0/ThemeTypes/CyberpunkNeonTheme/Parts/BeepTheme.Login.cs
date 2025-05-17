using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Login Popover Colors

        public Color LoginPopoverBackgroundColor { get; set; } = Color.FromArgb(18, 18, 32);         // Cyberpunk Black
        public Color LoginTitleColor { get; set; } = Color.FromArgb(0, 255, 255);                   // Neon Cyan
        public Font LoginTitleFont { get; set; } = new Font("Consolas", 14f, FontStyle.Bold);

        public Color LoginSubtitleColor { get; set; } = Color.FromArgb(0, 255, 128);                // Neon Green
        public Font LoginSubtitleFont { get; set; } = new Font("Consolas", 12f, FontStyle.Italic);

        public Color LoginDescriptionColor { get; set; } = Color.FromArgb(255, 255, 0);             // Neon Yellow
        public Font LoginDescriptionFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Regular);

        public Color LoginLinkColor { get; set; } = Color.FromArgb(255, 0, 255);                    // Neon Magenta
        public Font LoginLinkFont { get; set; } = new Font("Consolas", 11f, FontStyle.Underline);

        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan
        public Color LoginButtonTextColor { get; set; } = Color.Black;
        public Font LoginButtonFont { get; set; } = new Font("Consolas", 11.5f, FontStyle.Bold);

        public Color LoginDropdownBackgroundColor { get; set; } = Color.FromArgb(34, 34, 68);       // Cyberpunk Panel
        public Color LoginDropdownTextColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan

        public Color LoginLogoBackgroundColor { get; set; } = Color.FromArgb(255, 0, 255);          // Neon Magenta
    }
}
