using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Company Colors

        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(24, 24, 48);          // Cyberpunk Black
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(255, 0, 255);                    // Neon Magenta
        public Font CompanyTitleFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(0, 255, 255);                 // Neon Cyan
        public Font CompanySubTitleFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Italic);

        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(0, 255, 128);              // Neon Green
        public Font CompanyDescriptionFont { get; set; } = new Font("Consolas", 10f, FontStyle.Regular);

        public Color CompanyLinkColor { get; set; } = Color.FromArgb(255, 255, 0);                     // Neon Yellow
        public Font CompanyLinkFont { get; set; } = new Font("Consolas", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(0, 102, 255);         // Neon Blue
        public Color CompanyButtonTextColor { get; set; } = Color.FromArgb(255, 255, 0);               // Neon Yellow
        public Font CompanyButtonFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.FromArgb(34, 34, 68);        // Cyberpunk Panel
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(0, 255, 255);             // Neon Cyan

        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(255, 0, 255);           // Neon Magenta (for a "glow" logo effect)
    }
}
