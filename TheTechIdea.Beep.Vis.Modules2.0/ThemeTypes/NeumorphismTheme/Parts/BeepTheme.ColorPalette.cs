using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for primary elements
        public Color SecondaryColor { get; set; } = Color.FromArgb(80, 150, 200); // Soft blue for secondary elements
        public Color AccentColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for accents
        public Color BackgroundColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for main background
        public Color SurfaceColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for surfaces
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for errors
        public Color WarningColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for success
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for contrast on primary
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for text on background
    }
}