using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Switch control Fonts & Colors

        public Font SwitchTitleFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold);
        public Font SwitchSelectedFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Consolas", 12f, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.FromArgb(18, 18, 32);               // Dark background
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon magenta border
        public Color SwitchForeColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon cyan text

        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(0, 255, 128);      // Neon green selected bg
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon cyan selected border
        public Color SwitchSelectedForeColor { get; set; } = Color.White;                      // Selected text

        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(255, 255, 0);         // Neon yellow hover bg
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon magenta hover border
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon cyan hover text
    }
}
