using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(34, 49, 34); // dark forest green
        public Color ProgressBarForeColor { get; set; } = Color.White;
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(46, 71, 46); // slightly lighter green border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(76, 175, 80); // vibrant green chunk
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(191, 54, 12); // dark red
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(56, 142, 60); // success green
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(46, 71, 46); // hover background
        public Color ProgressBarHoverForeColor { get; set; } = Color.White;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
