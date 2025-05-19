using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // ProgressBar properties
<<<<<<< HEAD
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(230, 230, 250);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 180, 200);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 153, 255);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(220, 53, 69);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(40, 167, 69);
        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(210, 225, 245);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(0, 100, 180);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
=======
        public Color ProgressBarBackColor { get; set; }
        public Color ProgressBarForeColor { get; set; }
        public Color ProgressBarBorderColor { get; set; }
        public Color ProgressBarChunkColor { get; set; }
        public Color ProgressBarErrorColor { get; set; }
        public Color ProgressBarSuccessColor { get; set; }
        public TypographyStyle ProgressBarFont { get; set; }
        public Color ProgressBarInsideTextColor { get; set; }
        public Color ProgressBarHoverBackColor { get; set; }
        public Color ProgressBarHoverForeColor { get; set; }
        public Color ProgressBarHoverBorderColor { get; set; }
        public Color ProgressBarHoverInsideTextColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
