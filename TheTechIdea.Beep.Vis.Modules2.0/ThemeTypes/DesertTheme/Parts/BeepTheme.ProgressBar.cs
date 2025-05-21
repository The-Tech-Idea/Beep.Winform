using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 230, 210); // Light sand background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate brown foreground
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Saddle brown border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy brown chunk fill
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick for error state
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(85, 107, 47); // Dark olive green for success state
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(255, 228, 181); // Moccasin hover background
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19); // Saddle brown hover foreground
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Saddle brown hover border
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
