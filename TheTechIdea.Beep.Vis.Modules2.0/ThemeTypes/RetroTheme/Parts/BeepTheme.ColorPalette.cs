using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for primary elements
        public Color SecondaryColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for secondary elements
        public Color AccentColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for accents
        public Color BackgroundColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for main background
        public Color SurfaceColor { get; set; } = Color.FromArgb(0, 64, 64); // Mid-tone teal for surfaces
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red for errors
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0); // Retro orange for warnings
        public Color SuccessColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for success
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(255, 255, 255); // White for contrast on primary
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(255, 255, 255); // White for text on background
    }
}