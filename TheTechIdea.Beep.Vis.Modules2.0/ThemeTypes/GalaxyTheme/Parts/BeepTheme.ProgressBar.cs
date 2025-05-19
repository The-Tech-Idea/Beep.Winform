using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // ProgressBar properties
<<<<<<< HEAD
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color ProgressBarForeColor { get; set; } = Color.White;
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Sky Blue
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60); // ErrorColor
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // SuccessColor
        public Font ProgressBarFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover shade
        public Color ProgressBarHoverForeColor { get; set; } = Color.White;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Accent highlight
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
