using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Color Palette

        public Color PrimaryColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color SecondaryColor { get; set; } = Color.FromArgb(127, 255, 212);  // Mint
        public Color AccentColor { get; set; } = Color.FromArgb(255, 223, 93);      // Lemon Yellow

        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow (gentle background)
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 224, 235);    // Pastel Pink (for panels/cards/surfaces)

        public Color ErrorColor { get; set; } = Color.FromArgb(255, 99, 132);       // Candy Red (Strawberry)
        public Color WarningColor { get; set; } = Color.FromArgb(255, 205, 86);     // Candy Yellow (bright warning)
        public Color SuccessColor { get; set; } = Color.FromArgb(102, 217, 174);    // Minty Green

        public Color OnPrimaryColor { get; set; } = Color.White;                    // Contrast for buttons, chips, etc.
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(44, 62, 80);  // Navy (readable on all pastels)
    }
}
