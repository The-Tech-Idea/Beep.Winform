using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // ProgressBar properties with defaults
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Light grey background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue primary foreground
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Subtle border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue chunk fill
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(244, 67, 54); // Red error
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(76, 175, 80); // Green success
        public TypographyStyle  ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 10f, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White; // White text inside progress
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(224, 224, 224); // Hover background light grey
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(30, 136, 229); // Hover darker blue foreground
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(150, 150, 150); // Hover border grey
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White; // Hover inside text remains white
    }
}
