using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color LabelForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color LabelSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(150, 120, 100);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(180, 160, 140);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(120, 60, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(120, 60, 0);
        public Color SubLabelBackColor { get; set; } =Color.FromArgb(160, 82, 45);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
    }
}