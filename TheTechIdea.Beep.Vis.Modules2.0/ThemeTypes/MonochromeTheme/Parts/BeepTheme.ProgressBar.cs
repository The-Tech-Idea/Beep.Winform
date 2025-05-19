using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.LightGray;
        public Color ProgressBarForeColor { get; set; } = Color.Black;
        public Color ProgressBarBorderColor { get; set; } = Color.Gray;
        public Color ProgressBarChunkColor { get; set; } = Color.DimGray;
        public Color ProgressBarErrorColor { get; set; } = Color.DarkRed;
        public Color ProgressBarSuccessColor { get; set; } = Color.DarkGreen;
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.Gray;
        public Color ProgressBarHoverForeColor { get; set; } = Color.Black;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.DimGray;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
