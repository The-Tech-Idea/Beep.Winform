using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Status Bar Colors

        public Color StatusBarBackColor { get; set; } = Color.FromArgb(18, 18, 32);           // Dark cyberpunk black
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon cyan text
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon magenta border

        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon green hover bg
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon yellow hover text
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon cyan hover border
    }
}
