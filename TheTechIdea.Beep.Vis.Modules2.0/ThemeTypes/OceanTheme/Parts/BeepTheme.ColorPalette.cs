using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for primary elements
        public Color SecondaryColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for secondary elements
        public Color AccentColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for accents
        public Color BackgroundColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for main background
        public Color SurfaceColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for surfaces
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for errors
        public Color WarningColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for success
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for contrast on primary
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text on background
    }
}