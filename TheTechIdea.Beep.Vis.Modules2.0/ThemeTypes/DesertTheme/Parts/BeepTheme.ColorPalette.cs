using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Color Palette - Desert Inspired
        public Color PrimaryColor { get; set; } = Color.FromArgb(210, 180, 140);    // Tan (Sandy Beige)
        public Color SecondaryColor { get; set; } = Color.FromArgb(244, 214, 162);  // Light Sand
        public Color AccentColor { get; set; } = Color.FromArgb(201, 144, 66);      // Warm Clay (Orange-Brown)
        public Color BackgroundColor { get; set; } = Color.FromArgb(252, 243, 221); // Very Light Sand
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 250, 240);    // Soft Cream
        public Color ErrorColor { get; set; } = Color.FromArgb(178, 34, 34);        // Firebrick Red
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0);      // Orange
        public Color SuccessColor { get; set; } = Color.FromArgb(34, 139, 34);      // Forest Green
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(56, 44, 21);     // Dark Brown (for text/icons on Primary)
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(101, 67, 33); // Medium Brown (for text/icons on Background)
    }
}
