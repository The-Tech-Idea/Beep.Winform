using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // ProgressBar properties
//<<<<<<< HEAD
        public Color ProgressBarBackColor { get; set; } = Color.Black;
        public Color ProgressBarForeColor { get; set; } = Color.White;
        public Color ProgressBarBorderColor { get; set; } = Color.White;
        public Color ProgressBarChunkColor { get; set; } = Color.Yellow;
        public Color ProgressBarErrorColor { get; set; } = Color.Red;
        public Color ProgressBarSuccessColor { get; set; } = Color.Lime;
        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.Black;
        public Color ProgressBarHoverBackColor { get; set; } = Color.DarkGray;
        public Color ProgressBarHoverForeColor { get; set; } = Color.White;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.Yellow;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
