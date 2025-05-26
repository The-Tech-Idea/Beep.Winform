using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Label Colors and Fonts
        public Color LabelBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color LabelForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color LabelBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color LabelHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color LabelHoverForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color LabelSelectedBorderColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color LabelSelectedForeColor { get; set; } = Color.White;
        public Color LabelDisabledBackColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color LabelDisabledForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color LabelDisabledBorderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public TypographyStyle LabelFont { get; set; } = new TypographyStyle
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
        public TypographyStyle SubLabelFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color SubLabelForColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color SubLabelBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SubLabelHoverBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color SubLabelHoverForeColor { get; set; } = Color.FromArgb(189, 189, 189);
    }
}