using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.Transparent;
        public Color LabelForeColor { get; set; } = Color.White;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color LabelHoverForeColor { get; set; } = Color.White;
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(66, 72, 78);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color SubLabelBackColor { get; set; } = Color.Transparent;
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color SubLabelHoverForeColor { get; set; } = Color.White;
    }
}