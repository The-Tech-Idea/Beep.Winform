using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(34, 139, 34);  // ForestGreen
        public Color SecondaryColor { get; set; } = Color.FromArgb(85, 107, 47); // DarkOliveGreen
        public Color AccentColor { get; set; } = Color.FromArgb(107, 142, 35);   // OliveDrab
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 255, 240); // Honeydew
        public Color SurfaceColor { get; set; } = Color.FromArgb(224, 255, 224);    // LightGreen
        public Color ErrorColor { get; set; } = Color.FromArgb(178, 34, 34);     // FireBrick
        public Color WarningColor { get; set; } = Color.FromArgb(218, 165, 32);  // GoldenRod
        public Color SuccessColor { get; set; } = Color.FromArgb(60, 179, 113);  // MediumSeaGreen
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.Black;
    }
}
