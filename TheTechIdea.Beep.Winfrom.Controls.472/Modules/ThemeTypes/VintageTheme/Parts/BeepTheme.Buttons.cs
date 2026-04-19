using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
    }
}