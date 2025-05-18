using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(220, 53, 69);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(40, 167, 69);
        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
