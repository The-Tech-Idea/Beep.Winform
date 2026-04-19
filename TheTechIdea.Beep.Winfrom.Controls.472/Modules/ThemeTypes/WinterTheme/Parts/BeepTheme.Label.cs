using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color LabelForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(200, 220, 240);
    }
}