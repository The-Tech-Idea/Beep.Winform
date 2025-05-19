using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // ProgressBar properties
<<<<<<< HEAD
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ProgressBarForeColor { get; set; } = Color.SteelBlue;
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color ProgressBarChunkColor { get; set; } = Color.DeepSkyBlue;
        public Color ProgressBarErrorColor { get; set; } = Color.Red;
        public Color ProgressBarSuccessColor { get; set; } = Color.Green;

        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;

        public Color ProgressBarHoverBackColor { get; set; } = Color.LightBlue;
        public Color ProgressBarHoverForeColor { get; set; } = Color.Black;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.SteelBlue;
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
