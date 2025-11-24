using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color ProgressBarForeColor { get; set; } = Color.LightGray;
        public Color ProgressBarBorderColor { get; set; } = Color.Gray;
        public Color ProgressBarChunkColor { get; set; } = Color.CornflowerBlue;
        public Color ProgressBarErrorColor { get; set; } = Color.IndianRed;
        public Color ProgressBarSuccessColor { get; set; } = Color.MediumSeaGreen;
        public TypographyStyle  ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ProgressBarHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
