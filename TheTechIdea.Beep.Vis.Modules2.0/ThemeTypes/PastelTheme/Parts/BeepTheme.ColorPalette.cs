using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for primary elements
        public Color SecondaryColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for secondary elements
        public Color AccentColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for accents
        public Color BackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for main background
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 255, 255); // White for surfaces
        public Color ErrorColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral for errors
        public Color WarningColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for success
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for contrast on primary
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text on background
    }
}