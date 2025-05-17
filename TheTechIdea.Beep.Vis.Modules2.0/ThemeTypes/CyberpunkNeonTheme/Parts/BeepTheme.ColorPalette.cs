using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Color Palette

        public Color PrimaryColor { get; set; } = Color.FromArgb(255, 0, 255);         // Neon Magenta
        public Color SecondaryColor { get; set; } = Color.FromArgb(0, 255, 255);       // Neon Cyan
        public Color AccentColor { get; set; } = Color.FromArgb(255, 255, 0);          // Neon Yellow
        public Color BackgroundColor { get; set; } = Color.FromArgb(18, 18, 32);       // Deep Cyberpunk Black
        public Color SurfaceColor { get; set; } = Color.FromArgb(34, 34, 68);          // Cyberpunk Panel

        public Color ErrorColor { get; set; } = Color.FromArgb(255, 40, 80);           // Neon Red
        public Color WarningColor { get; set; } = Color.FromArgb(255, 128, 0);         // Neon Orange
        public Color SuccessColor { get; set; } = Color.FromArgb(0, 255, 128);         // Neon Green

        public Color OnPrimaryColor { get; set; } = Color.Black;                       // For text/icons on magenta
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan (for text/icons on dark)
    }
}
