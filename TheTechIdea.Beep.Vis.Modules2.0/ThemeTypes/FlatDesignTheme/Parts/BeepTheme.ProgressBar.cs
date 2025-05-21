using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(230, 230, 230); // Light gray background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(0, 120, 215);  // Blue foreground
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 120, 215); // Same as ForeColor
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(232, 17, 35); // Red error color
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(16, 124, 16); // Green success
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(210, 210, 210);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(0, 150, 245);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
