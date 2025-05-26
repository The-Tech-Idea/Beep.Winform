using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(76, 175, 80);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}