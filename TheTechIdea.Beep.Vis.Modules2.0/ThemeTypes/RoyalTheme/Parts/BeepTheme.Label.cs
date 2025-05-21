using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color LabelBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200); // Light gray
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150); // Medium gray
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180); // Darker gray
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
    }
}