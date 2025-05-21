using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color SecondaryColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color AccentColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color SurfaceColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ErrorColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0); // Goldenrod
        public Color SuccessColor { get; set; } = Color.FromArgb(0, 128, 0); // Emerald
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
    }
}