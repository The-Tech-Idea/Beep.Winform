using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(33, 150, 243);       // Blue 500
        public Color SecondaryColor { get; set; } = Color.FromArgb(156, 39, 176);     // Purple 500
        public Color AccentColor { get; set; } = Color.FromArgb(255, 193, 7);         // Amber 500
        public Color BackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);   // Grey 50
        public Color SurfaceColor { get; set; } = Color.White;
        public Color ErrorColor { get; set; } = Color.FromArgb(211, 47, 47);          // Red 700
        public Color WarningColor { get; set; } = Color.FromArgb(255, 160, 0);        // Orange 600
        public Color SuccessColor { get; set; } = Color.FromArgb(56, 142, 60);        // Green 700
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(33, 33, 33);    // Grey 900
    }
}
