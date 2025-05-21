using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(18, 18, 18);           // Very dark gray/black base
        public Color SecondaryColor { get; set; } = Color.FromArgb(40, 40, 40);         // Dark gray for secondary elements
        public Color AccentColor { get; set; } = Color.Cyan;                            // Neon cyan accent color
        public Color BackgroundColor { get; set; } = Color.FromArgb(10, 10, 10);        // Background dark shade
        public Color SurfaceColor { get; set; } = Color.FromArgb(30, 30, 30);           // Surface panels color
        public Color ErrorColor { get; set; } = Color.FromArgb(220, 20, 60);            // Crimson red for errors
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0);          // Orange for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(50, 205, 50);          // Lime green for success
        public Color OnPrimaryColor { get; set; } = Color.White;                         // Text/icons on primary color
        public Color OnBackgroundColor { get; set; } = Color.LightGray;                  // Text/icons on background
    }
}
