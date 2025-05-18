using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(63, 81, 181);   // Indigo
        public Color SecondaryColor { get; set; } = Color.FromArgb(255, 87, 34);   // Deep Orange
        public Color AccentColor { get; set; } = Color.FromArgb(0, 188, 212);   // Cyan

        public Color BackgroundColor { get; set; } = Color.FromArgb(250, 250, 250); // Light Gray
        public Color SurfaceColor { get; set; } = Color.White;

        public Color ErrorColor { get; set; } = Color.FromArgb(244, 67, 54);   // Red
        public Color WarningColor { get; set; } = Color.FromArgb(255, 152, 0);   // Orange
        public Color SuccessColor { get; set; } = Color.FromArgb(76, 175, 80);   // Green

        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(33, 33, 33);    // Almost Black
    }
}
