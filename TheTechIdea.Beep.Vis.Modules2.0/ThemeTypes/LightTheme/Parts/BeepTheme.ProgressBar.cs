using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.LightGray;
        public Color ProgressBarForeColor { get; set; } = Color.DodgerBlue;
        public Color ProgressBarBorderColor { get; set; } = Color.Gray;
        public Color ProgressBarChunkColor { get; set; } = Color.DodgerBlue;
        public Color ProgressBarErrorColor { get; set; } = Color.Red;
        public Color ProgressBarSuccessColor { get; set; } = Color.LimeGreen;
        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.Silver;
        public Color ProgressBarHoverForeColor { get; set; } = Color.RoyalBlue;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.DarkGray;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
