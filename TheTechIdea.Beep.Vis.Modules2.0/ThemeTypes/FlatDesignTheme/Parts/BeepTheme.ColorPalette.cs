using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(33, 150, 243);       // Blue 500
        public Color SecondaryColor { get; set; } = Color.FromArgb(144, 202, 249);    // Blue 200
        public Color AccentColor { get; set; } = Color.FromArgb(255, 193, 7);         // Amber 500
        public Color BackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);   // Light gray background
        public Color SurfaceColor { get; set; } = Color.White;                        // White surface
        public Color ErrorColor { get; set; } = Color.FromArgb(211, 47, 47);          // Red 700
        public Color WarningColor { get; set; } = Color.FromArgb(255, 143, 0);        // Orange 700
        public Color SuccessColor { get; set; } = Color.FromArgb(56, 142, 60);        // Green 700
        public Color OnPrimaryColor { get; set; } = Color.White;                      // Text on primary color
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(33, 33, 33);    // Dark text on background
    }
}
