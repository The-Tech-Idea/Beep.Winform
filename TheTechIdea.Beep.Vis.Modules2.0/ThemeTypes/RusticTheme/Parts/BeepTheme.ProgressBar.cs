using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(160, 82, 45); // fill, not ink
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(107, 142, 35);
        public TypographyStyle ProgressBarFont { get; set; }
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(62, 39, 35);
    }
}
