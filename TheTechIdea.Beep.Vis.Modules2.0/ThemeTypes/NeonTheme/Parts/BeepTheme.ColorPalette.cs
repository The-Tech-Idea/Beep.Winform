using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for primary elements
        public Color SecondaryColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for secondary elements
        public Color AccentColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for accents
        public Color BackgroundColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for main background
        public Color SurfaceColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for surfaces
        public Color ErrorColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for errors
        public Color WarningColor { get; set; } = Color.FromArgb(243, 156, 18); // Neon orange for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for success
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for contrast on primary color
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text on background
    }
}