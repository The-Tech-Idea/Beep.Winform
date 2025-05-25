using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(107, 142, 35);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(255, 245, 238);
    }
}