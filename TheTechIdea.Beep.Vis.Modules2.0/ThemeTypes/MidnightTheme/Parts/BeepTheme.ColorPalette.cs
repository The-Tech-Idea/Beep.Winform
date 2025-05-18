using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(48, 63, 159);       // Indigo 700
        public Color SecondaryColor { get; set; } = Color.FromArgb(144, 202, 249);    // Light Blue 300
        public Color AccentColor { get; set; } = Color.FromArgb(0, 188, 212);         // Cyan 500
        public Color BackgroundColor { get; set; } = Color.FromArgb(18, 18, 18);      // Almost Black
        public Color SurfaceColor { get; set; } = Color.FromArgb(30, 30, 30);         // Dark Grey
        public Color ErrorColor { get; set; } = Color.FromArgb(244, 67, 54);          // Red 500
        public Color WarningColor { get; set; } = Color.FromArgb(255, 152, 0);        // Orange 500
        public Color SuccessColor { get; set; } = Color.FromArgb(76, 175, 80);        // Green 500
        public Color OnPrimaryColor { get; set; } = Color.White;                       // Text on Primary
        public Color OnBackgroundColor { get; set; } = Color.WhiteSmoke;               // Text on Background
    }
}
