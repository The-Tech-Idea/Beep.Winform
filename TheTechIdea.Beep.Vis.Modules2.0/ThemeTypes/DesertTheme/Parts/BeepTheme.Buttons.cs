using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Button Colors and Styles - Desert inspired
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font ButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font ButtonSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135); // Burlywood
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(60, 30, 10);    // Dark Brown
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark Goldenrod

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(244, 164, 96);  // Sandy Brown
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(60, 30, 10);    // Dark Brown

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(60, 30, 10);   // Dark Brown
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown

        public Color ButtonBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat
        public Color ButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);    // Brown
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34);  // Firebrick
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0);  // Dark Red

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(101, 67, 33); // Brown
    }
}
